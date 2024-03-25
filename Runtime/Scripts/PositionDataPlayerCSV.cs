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

using UnityEngine;
using System.IO;
using System.Collections.Generic;

namespace Co1umbine.SHYMotionRecorder
{
    /// <summary>
    /// Play motion data exported to CSV
    /// </summary>
    public class PositionDataPlayerCSV : MotionDataPlayerCSV
    {
        private Transform boneIndicatorsParent;

        private Dictionary<HumanBodyBones, Transform> boneIndicators;

        // Use this for initialization
        private void Start()
        {
            Initiate();
        }

        [ContextMenu("Reload motion")]
        private void Initiate()
        {
            if(!string.IsNullOrEmpty(_recordedDirectory))
            {
                string motionCSVPath = _recordedDirectory + _recordedFileName;
                LoadCSVData(motionCSVPath);

                if (boneIndicatorsParent == null)
                {
                    boneIndicatorsParent = new GameObject("BoneIndicators").transform;
                    boneIndicatorsParent.SetParent(Animator.transform, false);
                }

                if (boneIndicators == null)
                    boneIndicators = new Dictionary<HumanBodyBones, Transform>();
            }
        }

        protected override void SetHumanPose()
        {
            if (RecordedMotionData == null)
            {
                return;
            }

            if (CurrentFrame >= RecordedMotionData.Poses.Count)
            {
                return;
            }

            var bones = RecordedMotionData.Poses[_frameIndex].HumanoidBones;
            foreach(var bone in bones)
            {
                IndicateBonePosition(bone);
            }

            // Adjusting the playback speed of motion data that has been degraded.
            if (_playingTime > _frameIndex / TargetFPS && !_paused)
            {
                _frameIndex++;
            }

            if (_frameIndex == RecordedMotionData.Poses.Count - 1)
            {
                if (_onPlayFinish != null)
                {
                    _onPlayFinish();
                }
            }
        }

        private void IndicateBonePosition(HumanoidPoses.SerializeHumanoidPose.HumanoidBone bone)
        {
            // Find bone indicator in cache
            if (boneIndicators.ContainsKey(bone.bone))
            {
                boneIndicators[bone.bone].position = bone.Position;
                return;
            }

            // Create bone indicator if it doesn't exist under boneIndicatorsParent
            var boneIndicator = boneIndicatorsParent.Find(bone.bone.ToString());
            if (boneIndicator == null)
            {
                boneIndicator = GameObject.CreatePrimitive(PrimitiveType.Sphere).transform;
                boneIndicator.name = bone.bone.ToString();
                boneIndicator.localScale = Vector3.one * 0.05f;
                boneIndicator.SetParent(boneIndicatorsParent);
            }

            boneIndicators.Add(bone.bone, boneIndicator);
            boneIndicators[bone.bone].position = bone.Position;
        }

        // Create _recordedMotionData from CSV
        private void LoadCSVData(string motionDataPath)
        {
            // Exit if file does not exist
            if (!File.Exists(motionDataPath))
            {
                return;
            }
            Debug.Log("[PositionDataPlayerCSV] Loading motion data from " + motionDataPath);
            RecordedMotionData = ScriptableObject.CreateInstance<HumanoidPoses>();

            FileStream fs = null;
            StreamReader sr = null;

            // File loading
            try
            {
                fs = new FileStream(motionDataPath, FileMode.Open);
                sr = new StreamReader(fs);

                while (sr.Peek() > -1)
                {
                    string line = sr.ReadLine();
                    var seriHumanPose = new HumanoidPoses.SerializeHumanoidPose();
                    if (line != "")
                    {
                        seriHumanPose.DeserializeCSV(line);
                        RecordedMotionData.Poses.Add(seriHumanPose);
                    }
                }
                sr.Close();
                fs.Close();
                sr = null;
                fs = null;
            }
            catch (System.Exception e)
            {
                Debug.LogError("[MotionDataEditor] Failed to save.\n" + e.Message + e.StackTrace);
            }

            if (sr != null)
            {
                sr.Close();
            }

            if (fs != null)
            {
                fs.Close();
            }
        }

        public override void LoadMotion(string directoryName, string fileName)
        {
            base._recordedDirectory = directoryName;
            base._recordedFileName = fileName;
            Initiate();
        }
    }
}