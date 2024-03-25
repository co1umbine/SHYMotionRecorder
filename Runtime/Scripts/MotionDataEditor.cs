/**
[SHYMotionRecorder]

Copyright (c) 2023 co1umbine

This software is released under the MIT License.
http://opensource.org/licenses/mit-license.php


[EasyMotionRecorder]

MIT License

Copyright (c) 2018 Duo.inc 2020 @neon-izm
http://opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Co1umbine.SHYMotionRecorder
{
    /// <summary>
    /// Motion data editor.
    /// Attach to the same game object as the Player that plays the edited motion.
    /// You can delete or save from the beginning to the current frame.
    /// </summary>
    [DefaultExecutionOrder(11001), RequireComponent(typeof(IMotionPlayer))]
    public class MotionDataEditor : MonoBehaviour
    {
        [SerializeField, Tooltip("In a form ending in a slash. For example: 'Assets/Resources/'")]
        private string outputDirectory;

        [SerializeField, Tooltip("WITHOUT EXTENSION.\nWild card: <Take> <GameObject> <Date> <Time>")]
        private string outputFileName = "Motion_<GameObject>_<Take>";

        [SerializeField]
        private int initTake = 0;

        [SerializeField]
        private bool saveAsScriptableObject = true;

        [SerializeField]
        private bool saveAsCSV;

        [SerializeField]
        private int maxLogSize = 0;

        private List<HumanoidPoses.SerializeHumanoidPose> originalMotionCache;

        private IMotionPlayer player;

        private Stack<(List<HumanoidPoses.SerializeHumanoidPose> motion, int frame)> operationBackLog = new Stack<(List<HumanoidPoses.SerializeHumanoidPose> motion, int frame)>();
        private Stack<(List<HumanoidPoses.SerializeHumanoidPose> motion, int frame)> operationForwardLog = new Stack<(List<HumanoidPoses.SerializeHumanoidPose> motion, int frame)>();

        private int take;

        public int BackLogCount { get { return operationBackLog.Count; } }
        public int ForwardLogCount { get { return operationForwardLog.Count; } }

        void Start()
        {
            player = GetComponent<IMotionPlayer>();
            if (player == null)
            {
                Debug.LogError("[MotionDataEditor] MotionDataPlayer is not attached.");
                return;
            }
            CacheOriginalMotion();
            take = initTake;
        }

        private void CacheOriginalMotion()
        {
            if (player?.MotionData == null) return;
            originalMotionCache = CopySerializeHumanoidPose(player.MotionData);
        }

        public void DeleteMotion()
        {
            ReduceMotion();

            Debug.Log($"[MotionDataEditor {player.Animator.gameObject.name}] Splited motion data has been deleted.");
        }

        public void SaveMotion()
        {
            if (!saveAsCSV && !saveAsScriptableObject)
            {
                Debug.LogWarning($"[MotionDataEditor {player.Animator.gameObject.name}] While no save flag set, you're going to save.");
                return;
            }

            string fileNameStr = outputFileName;

            // Replace wild card
            fileNameStr = fileNameStr.Replace("<Take>", take.ToString());
            fileNameStr = fileNameStr.Replace("<GameObject>", player.Animator.gameObject.name);
            fileNameStr = fileNameStr.Replace("<Date>", DateTime.Now.ToString("yyyy-MM-dd"));
            fileNameStr = fileNameStr.Replace("<Time>", DateTime.Now.ToString("HH-mm-ss"));

            if (fileNameStr == "")
            {
                // Automatic setting file name
                fileNameStr = string.Format("motion_{0:yyyy_MM_dd_HH_mm_ss}", DateTime.Now);
            }

            string directoryStr = outputDirectory;
            if (directoryStr == "")
            {
                // Automatic setting directory
                directoryStr = "Assets/Resources/";
            }

            if (!Directory.Exists(outputDirectory))
            {
                Directory.CreateDirectory(outputDirectory);
            }

            // Save as CSV
            if (saveAsCSV)
            {
                SaveAsCSV(directoryStr, fileNameStr);
            }

#if UNITY_EDITOR

            if (saveAsScriptableObject)
            {
                SaveAsScriptableObject(directoryStr, fileNameStr);
            }
#endif

            ReduceMotion();

            take++;

            Debug.Log($"[MotionDataEditor {player.Animator.gameObject.name}] Splited motion data has been saved.");
        }

        private void SaveAsCSV(string directoryName, string fileName)
        {
            FileStream fs = new FileStream(directoryName + fileName + ".csv", FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);

            for (int i = 0; i < player.CurrentFrame + 1; i++)
            {
                var pose = player.MotionData[i];
                var prev = i != 0 ? player.MotionData[i - 1] : pose;
                string seriStr = pose.SerializeCSV(prev);
                sw.WriteLine(seriStr);
            }

            // Close file
            try
            {
                sw.Close();
                fs.Close();
                sw = null;
                fs = null;
            }
            catch (Exception e)
            {
                Debug.LogError("[MotionDataEditor] Failed to save.\n" + e.Message + e.StackTrace);
            }

            if (sw != null)
            {
                sw.Close();
            }

            if (fs != null)
            {
                fs.Close();
            }

#if UNITY_EDITOR
            UnityEditor.AssetDatabase.Refresh();
#endif
        }

#if UNITY_EDITOR
        private void SaveAsScriptableObject(string directoryName, string fileName)
        {
            var path = directoryName + fileName + ".asset";
            var uniqueAssetPath = AssetDatabase.GenerateUniqueAssetPath(path);

            var Poses = ScriptableObject.CreateInstance<HumanoidPoses>();

            Poses.Poses = CopySerializeHumanoidPose(player.MotionData.GetRange(0, player.CurrentFrame + 1));

            AssetDatabase.CreateAsset(Poses, uniqueAssetPath);
            AssetDatabase.Refresh();
        }
#endif

        private void ReduceMotion()
        {
            var poses = player.MotionData;

            OperationProgress();

            if (player.CurrentFrame < poses.Count - 1)
            {
                poses = poses.GetRange(player.CurrentFrame + 1, poses.Count - (player.CurrentFrame + 1));
                var firstTime = poses[0].Time;
                foreach (var p in poses)
                {
                    p.Time -= firstTime;
                }
            }
            else
            {
                poses = new List<HumanoidPoses.SerializeHumanoidPose>();
            }

            player.SetMotion(poses);
            player.SetFrame(0);
            player.StopMotion();
        }

        public void ResetMotion()
        {
            if (originalMotionCache == null) return;

            OperationProgress();
            player?.SetMotion(CopySerializeHumanoidPose(originalMotionCache));
            Debug.Log($"[MotionDataEditor {player?.Animator?.gameObject.name}] Motion data has been reset to original data.");
        }

        private void OperationProgress()
        {
            operationBackLog.Push((CopySerializeHumanoidPose(player.MotionData), player.CurrentFrame));
            StackTrim(ref operationBackLog, maxLogSize);
            if (operationForwardLog.Count > 0) operationForwardLog.Clear();
        }
        public void Undo()
        {
            (List<HumanoidPoses.SerializeHumanoidPose> motion, int frame) log;
            if(operationBackLog.TryPop(out log))
            {
                operationForwardLog.Push((CopySerializeHumanoidPose(player.MotionData), player.CurrentFrame));
                StackTrim(ref operationForwardLog, maxLogSize);
                player.SetMotion(CopySerializeHumanoidPose(log.motion));
                player.SetFrame(log.frame);
                Debug.Log($"[MotionDataEditor {player.Animator.gameObject.name}] The motion data status has returned to the previous state.");
            }
        }
        public void Redo()
        {
            (List<HumanoidPoses.SerializeHumanoidPose> motion, int frame) log;
            if(operationForwardLog.TryPop(out log))
            {
                operationBackLog.Push((CopySerializeHumanoidPose(player.MotionData), player.CurrentFrame));
                StackTrim(ref operationBackLog, maxLogSize);
                player.SetMotion(CopySerializeHumanoidPose(log.motion));
                player.SetFrame(log.frame);
                Debug.Log($"[MotionDataEditor {player.Animator.gameObject.name}] The motion data status has advanced to the next state.");
            }
        }

        private void StackTrim<T>(ref Stack<T> stack, int size)
        {
            if (stack.Count <= size) return;
            var array = stack.ToArray();
            array = array[0..^1];
            stack = new Stack<T>(array.Reverse());
        }

        private void OnDisable()
        {
            ResetMotion();
        }

        private List<HumanoidPoses.SerializeHumanoidPose> CopySerializeHumanoidPose(List<HumanoidPoses.SerializeHumanoidPose> shps)
        {
            var result = new List<HumanoidPoses.SerializeHumanoidPose>();
            foreach(var shp in shps)
            {
                result.Add(new HumanoidPoses.SerializeHumanoidPose(shp));
            }
            return result;
        }
    }
}