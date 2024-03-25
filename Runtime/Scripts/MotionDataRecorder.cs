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
using System;
using System.IO;
using System.Reflection;
using UnityEngine.Serialization;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Co1umbine.SHYMotionRecorder
{
    /// <summary>
    /// Motion data recording class
    /// The order of script execution is to get the posture after the VRIK process is finished, so
    /// Maximum value = 32000 is specified
    /// </summary>
    [DefaultExecutionOrder(32000)]
    public class MotionDataRecorder : MonoBehaviour
    {
        [SerializeField]
        private KeyCode _recordStartKey = KeyCode.R;
        [SerializeField]
        private KeyCode _recordStopKey = KeyCode.X;

        [SerializeField]
        private Animator _animator;

        [SerializeField]
        private bool _recording;
        [SerializeField]
        protected int FrameIndex;

        [SerializeField]
        private HumanBodyBones IK_LeftFootBone = HumanBodyBones.LeftFoot;
        [SerializeField]
        private HumanBodyBones IK_RightFootBone = HumanBodyBones.RightFoot;

        protected HumanoidPoses Poses;
        protected float RecordedTime;
        protected float StartTime;

        private HumanPose _currentPose;
        private HumanPoseHandler _poseHandler;
        public Action OnRecordStart;
        public Action OnRecordEnd;

        [Tooltip("FPS to record, not limited by 0. The FPS of Update cannot be exceeded.")]
        public float TargetFPS = 30.0f;

        [SerializeField, Tooltip("In a form ending in a slash. For example: 'Assets/Resources/'")]
        protected string _outputDirectory;

        [SerializeField, Tooltip("WITHOUT EXTENSION.\nWild card: <Take> <GameObject> <Date> <Time>")]
        protected string _outputFileName = "RecordMotion_<GameObject>_<Date>_<Time>";

        [SerializeField]
        private bool saveAsScriptableObject = true;

        [SerializeField]
        private bool saveAsCSV;

        public Animator Animator { get => _animator; set => _animator = value; }

        private int take;

        // Use this for initialization
        private void Awake()
        {
            if (_animator == null)
            {
                Debug.LogError("No animator is set in MotionDataRecorder.\r\nDelete MotionDataRecorder.");
                Destroy(this);
                return;
            }

            _poseHandler = new HumanPoseHandler(_animator.avatar, _animator.transform);
        }

        private void Update()
        {
            if (Input.GetKeyDown(_recordStartKey))
            {
                RecordStart();
            }

            if (Input.GetKeyDown(_recordStopKey))
            {
                RecordEnd();
            }
        }

        // Update is called once per frame
        private void LateUpdate()
        {
            if (!_recording)
            {
                return;
            }


            RecordedTime = Time.time - StartTime;

            if (TargetFPS != 0.0f)
            {
                var nextTime = (1.0f * (FrameIndex + 1)) / TargetFPS;
                if (nextTime > RecordedTime)
                {
                    return;
                }
                if (FrameIndex % TargetFPS == 0)
                {
                    print("Motion_FPS=" + 1 / (RecordedTime / FrameIndex));
                }
            }
            else
            {
                if (Time.frameCount % Application.targetFrameRate == 0)
                {
                    print("Motion_FPS=" + 1 / Time.deltaTime);
                }
            }


            // Get current humanoid pose
            _poseHandler.GetHumanPose(ref _currentPose);
            // write the pose acquired to poses
            var serializedPose = new HumanoidPoses.SerializeHumanoidPose();

            var bodyTQ = new TQ(_currentPose.bodyPosition, _currentPose.bodyRotation);
            var LeftFootTQ = new TQ(_animator.GetBoneTransform(IK_LeftFootBone).position, _animator.GetBoneTransform(IK_LeftFootBone).rotation);
            var RightFootTQ = new TQ(_animator.GetBoneTransform(IK_RightFootBone).position, _animator.GetBoneTransform(IK_RightFootBone).rotation);
            LeftFootTQ = AvatarUtility.GetIKGoalTQ(_animator.avatar, _animator.humanScale, AvatarIKGoal.LeftFoot, bodyTQ, LeftFootTQ);
            RightFootTQ = AvatarUtility.GetIKGoalTQ(_animator.avatar, _animator.humanScale, AvatarIKGoal.RightFoot, bodyTQ, RightFootTQ);

            serializedPose.BodyPosition = bodyTQ.t;
            serializedPose.BodyRotation = bodyTQ.q;
            serializedPose.LeftfootIK_Pos = LeftFootTQ.t;
            serializedPose.LeftfootIK_Rot = LeftFootTQ.q;
            serializedPose.RightfootIK_Pos = RightFootTQ.t;
            serializedPose.RightfootIK_Rot = RightFootTQ.q;



            serializedPose.Muscles = new float[_currentPose.muscles.Length];
            serializedPose.Time = RecordedTime;
            for (int i = 0; i < serializedPose.Muscles.Length; i++)
            {
                serializedPose.Muscles[i] = _currentPose.muscles[i];
            }

            SetHumanBoneTransformToHumanoidPoses(_animator, ref serializedPose);

            Poses.Poses.Add(serializedPose);
            FrameIndex++;
        }

        /// <summary>
        /// Start Recording
        /// </summary>
        private void RecordStart()
        {
            if (_recording)
            {
                return;
            }

            Poses = ScriptableObject.CreateInstance<HumanoidPoses>();

            if (OnRecordStart != null)
            {
                OnRecordStart();
            }

            OnRecordEnd += WriteAnimationFile;
            _recording = true;
            RecordedTime = 0f;
            StartTime = Time.time;
            FrameIndex = 0;
        }

        /// <summary>
        /// Finish Recording
        /// </summary>
        private void RecordEnd()
        {
            if (!_recording)
            {
                return;
            }


            if (OnRecordEnd != null)
            {
                OnRecordEnd();
            }

            OnRecordEnd -= WriteAnimationFile;
            _recording = false;
        }

        public static void SetHumanBoneTransformToHumanoidPoses(Animator animator, ref HumanoidPoses.SerializeHumanoidPose pose)
        {
            HumanBodyBones[] values = Enum.GetValues(typeof(HumanBodyBones)) as HumanBodyBones[];
            foreach (HumanBodyBones b in values)
            {
                if (b < 0 || b >= HumanBodyBones.LastBone)
                {
                    continue;
                }

                Transform t = animator.GetBoneTransform(b);
                if (t != null)
                {
                    var bone = new HumanoidPoses.SerializeHumanoidPose.HumanoidBone();
                    bone.Set(animator.transform, t, b);
                    pose.HumanoidBones.Add(bone);
                }
                else
                {
                    var bone = new HumanoidPoses.SerializeHumanoidPose.HumanoidBone();
                    bone.Set(animator.transform, null, b);
                    pose.HumanoidBones.Add(bone);
                }
            }
        }

        protected virtual void WriteAnimationFile()
        {
            if (!saveAsCSV && !saveAsScriptableObject)
            {
                Debug.LogWarning($"[MotionDataRecorder {Animator.gameObject.name}] While no save flag set, you're going to save.");
                return;
            }

            string fileNameStr = _outputFileName;

            // Replace wild card
            fileNameStr = fileNameStr.Replace("<Take>", take.ToString());
            fileNameStr = fileNameStr.Replace("<GameObject>", Animator.gameObject.name);
            fileNameStr = fileNameStr.Replace("<Date>", DateTime.Now.ToString("yyyy-MM-dd"));
            fileNameStr = fileNameStr.Replace("<Time>", DateTime.Now.ToString("HH-mm-ss"));

            if (fileNameStr == "")
            {
                // Automatic setting file name
                fileNameStr = string.Format("RecordMotion_{0:yyyy-MM-dd_HH-mm-ss}", DateTime.Now);
            }

            string directoryStr = _outputDirectory;
            if (directoryStr == "")
            {
                // Automatic setting directory
                directoryStr = "Assets/Resources/";
            }

            if (!Directory.Exists(directoryStr))
            {
                Directory.CreateDirectory(directoryStr);
            }

            // Save as CSV
            if (saveAsCSV)
                SaveAsCSV(directoryStr, fileNameStr);

            // Save as ScriptableObject
#if UNITY_EDITOR
            if (saveAsScriptableObject)
                SaveAsScriptableObject(directoryStr, fileNameStr);

#endif

            StartTime = Time.time;
            RecordedTime = 0f;
            FrameIndex = 0;
            take++;
        }

        private void SaveAsCSV(string directoryName, string fileName)
        {
            FileStream fs = new FileStream(directoryName + fileName + ".csv", FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);

            for (int i = 0; i < Poses.Poses.Count; i++)
            {
                var pose = Poses.Poses[i];
                var prev = i != 0 ? Poses.Poses[i - 1] : pose;
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
                Debug.LogError("[MotionDataRecorder] Failed to save.\n" + e.Message + e.StackTrace);
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

            AssetDatabase.CreateAsset(Poses, uniqueAssetPath);
            AssetDatabase.Refresh();
        }
#endif

        public class TQ
        {
            public TQ(Vector3 translation, Quaternion rotation)
            {
                t = translation;
                q = rotation;
            }
            public Vector3 t;
            public Quaternion q;
            // Scale should always be 1,1,1
        }
        public class AvatarUtility
        {
            static public TQ GetIKGoalTQ(Avatar avatar, float humanScale, AvatarIKGoal avatarIKGoal, TQ animatorBodyPositionRotation, TQ skeletonTQ)
            {
                int humanId = (int)HumanIDFromAvatarIKGoal(avatarIKGoal);
                if (humanId == (int)HumanBodyBones.LastBone)
                    throw new InvalidOperationException("Invalid human id.");
                MethodInfo methodGetAxisLength = typeof(Avatar).GetMethod("GetAxisLength", BindingFlags.Instance | BindingFlags.NonPublic);
                if (methodGetAxisLength == null)
                    throw new InvalidOperationException("Cannot find GetAxisLength method.");
                MethodInfo methodGetPostRotation = typeof(Avatar).GetMethod("GetPostRotation", BindingFlags.Instance | BindingFlags.NonPublic);
                if (methodGetPostRotation == null)
                    throw new InvalidOperationException("Cannot find GetPostRotation method.");
                Quaternion postRotation = (Quaternion)methodGetPostRotation.Invoke(avatar, new object[] { humanId });
                var goalTQ = new TQ(skeletonTQ.t, skeletonTQ.q * postRotation);
                if (avatarIKGoal == AvatarIKGoal.LeftFoot || avatarIKGoal == AvatarIKGoal.RightFoot)
                {
                    // Here you could use animator.leftFeetBottomHeight or animator.rightFeetBottomHeight rather than GetAxisLenght
                    // Both are equivalent but GetAxisLength is the generic way and work for all human bone
                    float axislength = (float)methodGetAxisLength.Invoke(avatar, new object[] { humanId });
                    Vector3 footBottom = new Vector3(axislength, 0, 0);
                    goalTQ.t += (goalTQ.q * footBottom);
                }
                // IK goal are in avatar body local space
                Quaternion invRootQ = Quaternion.Inverse(animatorBodyPositionRotation.q);
                goalTQ.t = invRootQ * (goalTQ.t - animatorBodyPositionRotation.t);
                goalTQ.q = invRootQ * goalTQ.q;
                goalTQ.t /= humanScale;

                return goalTQ;
            }
            static public HumanBodyBones HumanIDFromAvatarIKGoal(AvatarIKGoal avatarIKGoal)
            {
                HumanBodyBones humanId = HumanBodyBones.LastBone;
                switch (avatarIKGoal)
                {
                    case AvatarIKGoal.LeftFoot: humanId = HumanBodyBones.LeftFoot; break;
                    case AvatarIKGoal.RightFoot: humanId = HumanBodyBones.RightFoot; break;
                    case AvatarIKGoal.LeftHand: humanId = HumanBodyBones.LeftHand; break;
                    case AvatarIKGoal.RightHand: humanId = HumanBodyBones.RightHand; break;
                }
                return humanId;
            }
        }
    }
}
