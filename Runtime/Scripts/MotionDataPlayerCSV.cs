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

namespace Co1umbine.SHYMotionRecorder
{
    /// <summary>
    /// Play motion data exported to CSV
    /// </summary>
    public class MotionDataPlayerCSV : MotionDataPlayer, IMotionLoader
    {
        [SerializeField, Tooltip("In a form ending in a slash. For example: 'Assets/Resources/'")]
        protected string _recordedDirectory;

        [SerializeField, Tooltip("WITH EXTENSION.")]
        protected string _recordedFileName;

        // Use this for initialization
        [ContextMenu("Reload motion")]
        private void Start()
        {
            Initiate();
        }

        private void Initiate()
        {
            if (string.IsNullOrEmpty(_recordedDirectory))
            {
                _recordedDirectory = Application.streamingAssetsPath + "/";
            }

            string motionCSVPath = _recordedDirectory + _recordedFileName;
            LoadCSVData(motionCSVPath);
        }

        // Create _recordedMotionData from CSV
        private void LoadCSVData(string motionDataPath)
        {
            // Exit if file does not exist
            if (!File.Exists(motionDataPath))
            {
                return;
            }


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

        public virtual void LoadMotion(string directoryName, string fileName)
        {
            _recordedDirectory = directoryName;
            _recordedFileName = fileName;
            Initiate();
        }
    }
}