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
using System.Text;
using System.Collections.Generic;
using System.IO;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Co1umbine.SHYMotionRecorder
{
    [Serializable]
    public class MotionDataSettings
    {
        /// <summary>
        /// Muscle mapping for Humanoid
        /// </summary>
        public static Dictionary<string, string> TraitPropMap = new Dictionary<string, string>
        {
            {"Left Thumb 1 Stretched", "LeftHand.Thumb.1 Stretched"},
            {"Left Thumb Spread", "LeftHand.Thumb.Spread"},
            {"Left Thumb 2 Stretched", "LeftHand.Thumb.2 Stretched"},
            {"Left Thumb 3 Stretched", "LeftHand.Thumb.3 Stretched"},
            {"Left Index 1 Stretched", "LeftHand.Index.1 Stretched"},
            {"Left Index Spread", "LeftHand.Index.Spread"},
            {"Left Index 2 Stretched", "LeftHand.Index.2 Stretched"},
            {"Left Index 3 Stretched", "LeftHand.Index.3 Stretched"},
            {"Left Middle 1 Stretched", "LeftHand.Middle.1 Stretched"},
            {"Left Middle Spread", "LeftHand.Middle.Spread"},
            {"Left Middle 2 Stretched", "LeftHand.Middle.2 Stretched"},
            {"Left Middle 3 Stretched", "LeftHand.Middle.3 Stretched"},
            {"Left Ring 1 Stretched", "LeftHand.Ring.1 Stretched"},
            {"Left Ring Spread", "LeftHand.Ring.Spread"},
            {"Left Ring 2 Stretched", "LeftHand.Ring.2 Stretched"},
            {"Left Ring 3 Stretched", "LeftHand.Ring.3 Stretched"},
            {"Left Little 1 Stretched", "LeftHand.Little.1 Stretched"},
            {"Left Little Spread", "LeftHand.Little.Spread"},
            {"Left Little 2 Stretched", "LeftHand.Little.2 Stretched"},
            {"Left Little 3 Stretched", "LeftHand.Little.3 Stretched"},
            {"Right Thumb 1 Stretched", "RightHand.Thumb.1 Stretched"},
            {"Right Thumb Spread", "RightHand.Thumb.Spread"},
            {"Right Thumb 2 Stretched", "RightHand.Thumb.2 Stretched"},
            {"Right Thumb 3 Stretched", "RightHand.Thumb.3 Stretched"},
            {"Right Index 1 Stretched", "RightHand.Index.1 Stretched"},
            {"Right Index Spread", "RightHand.Index.Spread"},
            {"Right Index 2 Stretched", "RightHand.Index.2 Stretched"},
            {"Right Index 3 Stretched", "RightHand.Index.3 Stretched"},
            {"Right Middle 1 Stretched", "RightHand.Middle.1 Stretched"},
            {"Right Middle Spread", "RightHand.Middle.Spread"},
            {"Right Middle 2 Stretched", "RightHand.Middle.2 Stretched"},
            {"Right Middle 3 Stretched", "RightHand.Middle.3 Stretched"},
            {"Right Ring 1 Stretched", "RightHand.Ring.1 Stretched"},
            {"Right Ring Spread", "RightHand.Ring.Spread"},
            {"Right Ring 2 Stretched", "RightHand.Ring.2 Stretched"},
            {"Right Ring 3 Stretched", "RightHand.Ring.3 Stretched"},
            {"Right Little 1 Stretched", "RightHand.Little.1 Stretched"},
            {"Right Little Spread", "RightHand.Little.Spread"},
            {"Right Little 2 Stretched", "RightHand.Little.2 Stretched"},
            {"Right Little 3 Stretched", "RightHand.Little.3 Stretched"},
        };
    }

    /// <summary>
    /// Motion Data ScriptableObject
    /// </summary>
    public class HumanoidPoses : ScriptableObject
    {
#if UNITY_EDITOR

        // Output as a Humanoid anim file.
        [ContextMenu("Export as Humanoid animation clips")]
        public void ExportHumanoidAnim()
        {
            var clip = new AnimationClip { frameRate = 30 };
            AnimationUtility.SetAnimationClipSettings(clip, new AnimationClipSettings { loopTime = false });


            // body position
            {
                var curveX = new AnimationCurve();
                var curveY = new AnimationCurve();
                var curveZ = new AnimationCurve();
                foreach (var item in Poses)
                {
                    curveX.AddKey(item.Time, item.BodyPosition.x);
                    curveY.AddKey(item.Time, item.BodyPosition.y);
                    curveZ.AddKey(item.Time, item.BodyPosition.z);
                }

                const string muscleX = "RootT.x";
                clip.SetCurve("", typeof(Animator), muscleX, curveX);
                const string muscleY = "RootT.y";
                clip.SetCurve("", typeof(Animator), muscleY, curveY);
                const string muscleZ = "RootT.z";
                clip.SetCurve("", typeof(Animator), muscleZ, curveZ);
            }
            // Leftfoot position
            {
                var curveX = new AnimationCurve();
                var curveY = new AnimationCurve();
                var curveZ = new AnimationCurve();
                foreach (var item in Poses)
                {
                    curveX.AddKey(item.Time, item.LeftfootIK_Pos.x);
                    curveY.AddKey(item.Time, item.LeftfootIK_Pos.y);
                    curveZ.AddKey(item.Time, item.LeftfootIK_Pos.z);
                }

                const string muscleX = "LeftFootT.x";
                clip.SetCurve("", typeof(Animator), muscleX, curveX);
                const string muscleY = "LeftFootT.y";
                clip.SetCurve("", typeof(Animator), muscleY, curveY);
                const string muscleZ = "LeftFootT.z";
                clip.SetCurve("", typeof(Animator), muscleZ, curveZ);
            }
            // Rightfoot position
            {
                var curveX = new AnimationCurve();
                var curveY = new AnimationCurve();
                var curveZ = new AnimationCurve();
                foreach (var item in Poses)
                {
                    curveX.AddKey(item.Time, item.RightfootIK_Pos.x);
                    curveY.AddKey(item.Time, item.RightfootIK_Pos.y);
                    curveZ.AddKey(item.Time, item.RightfootIK_Pos.z);
                }

                const string muscleX = "RightFootT.x";
                clip.SetCurve("", typeof(Animator), muscleX, curveX);
                const string muscleY = "RightFootT.y";
                clip.SetCurve("", typeof(Animator), muscleY, curveY);
                const string muscleZ = "RightFootT.z";
                clip.SetCurve("", typeof(Animator), muscleZ, curveZ);
            }
            // body rotation
            {
                var curveX = new AnimationCurve();
                var curveY = new AnimationCurve();
                var curveZ = new AnimationCurve();
                var curveW = new AnimationCurve();
                foreach (var item in Poses)
                {
                    curveX.AddKey(item.Time, item.BodyRotation.x);
                    curveY.AddKey(item.Time, item.BodyRotation.y);
                    curveZ.AddKey(item.Time, item.BodyRotation.z);
                    curveW.AddKey(item.Time, item.BodyRotation.w);
                }

                const string muscleX = "RootQ.x";
                clip.SetCurve("", typeof(Animator), muscleX, curveX);
                const string muscleY = "RootQ.y";
                clip.SetCurve("", typeof(Animator), muscleY, curveY);
                const string muscleZ = "RootQ.z";
                clip.SetCurve("", typeof(Animator), muscleZ, curveZ);
                const string muscleW = "RootQ.w";
                clip.SetCurve("", typeof(Animator), muscleW, curveW);
            }
            // Leftfoot rotation
            {
                var curveX = new AnimationCurve();
                var curveY = new AnimationCurve();
                var curveZ = new AnimationCurve();
                var curveW = new AnimationCurve();
                foreach (var item in Poses)
                {
                    curveX.AddKey(item.Time, item.LeftfootIK_Rot.x);
                    curveY.AddKey(item.Time, item.LeftfootIK_Rot.y);
                    curveZ.AddKey(item.Time, item.LeftfootIK_Rot.z);
                    curveW.AddKey(item.Time, item.LeftfootIK_Rot.w);
                }

                const string muscleX = "LeftFootQ.x";
                clip.SetCurve("", typeof(Animator), muscleX, curveX);
                const string muscleY = "LeftFootQ.y";
                clip.SetCurve("", typeof(Animator), muscleY, curveY);
                const string muscleZ = "LeftFootQ.z";
                clip.SetCurve("", typeof(Animator), muscleZ, curveZ);
                const string muscleW = "LeftFootQ.w";
                clip.SetCurve("", typeof(Animator), muscleW, curveW);
            }
            // Rightfoot rotation
            {
                var curveX = new AnimationCurve();
                var curveY = new AnimationCurve();
                var curveZ = new AnimationCurve();
                var curveW = new AnimationCurve();
                foreach (var item in Poses)
                {
                    curveX.AddKey(item.Time, item.RightfootIK_Rot.x);
                    curveY.AddKey(item.Time, item.RightfootIK_Rot.y);
                    curveZ.AddKey(item.Time, item.RightfootIK_Rot.z);
                    curveW.AddKey(item.Time, item.RightfootIK_Rot.w);
                }

                const string muscleX = "RightFootQ.x";
                clip.SetCurve("", typeof(Animator), muscleX, curveX);
                const string muscleY = "RightFootQ.y";
                clip.SetCurve("", typeof(Animator), muscleY, curveY);
                const string muscleZ = "RightFootQ.z";
                clip.SetCurve("", typeof(Animator), muscleZ, curveZ);
                const string muscleW = "RightFootQ.w";
                clip.SetCurve("", typeof(Animator), muscleW, curveW);
            }

            // muscles
            for (int i = 0; i < HumanTrait.MuscleCount; i++)
            {
                var curve = new AnimationCurve();
                foreach (var item in Poses)
                {
                    curve.AddKey(item.Time, item.Muscles[i]);
                }

                var muscle = HumanTrait.MuscleName[i];
                if (MotionDataSettings.TraitPropMap.ContainsKey(muscle))
                {
                    muscle = MotionDataSettings.TraitPropMap[muscle];
                }

                clip.SetCurve("", typeof(Animator), muscle, curve);
            }

            clip.EnsureQuaternionContinuity();

            var path = string.Format("Assets/Resources/RecordMotion_{0:yyyy_MM_dd_HH_mm_ss}_Humanoid.anim", DateTime.Now);
            var uniqueAssetPath = AssetDatabase.GenerateUniqueAssetPath(path);

            AssetDatabase.CreateAsset(clip, uniqueAssetPath);
            AssetDatabase.SaveAssets();
        }

        [ContextMenu("Print CSV Data Column")]
        public void ShowCSVDataColumn()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("BodyPosition.x");    
            sb.Append(",");
            sb.Append("BodyPosition.y");    
            sb.Append(",");
            sb.Append("BodyPosition.z");    
            sb.Append(",");
            sb.Append("BodyRotation.x");    
            sb.Append(",");
            sb.Append("BodyRotation.y");    
            sb.Append(",");
            sb.Append("BodyRotation.z");    
            sb.Append(",");
            sb.Append("BodyRotation.w");
            sb.Append(",");
            foreach (var muscle in HumanTrait.MuscleName)
            {
                if (muscle.Contains("Jaw") || muscle.Contains("Eye"))
                {
                    continue;
                }
                sb.Append("Muscle." + muscle);
                sb.Append(",");
            }
            foreach (var humanoidBone in Poses[0].HumanoidBones)
            {
                if (humanoidBone.bone == HumanBodyBones.Jaw || humanoidBone.bone == HumanBodyBones.LeftEye || humanoidBone.bone == HumanBodyBones.RightEye || humanoidBone.bone == HumanBodyBones.LastBone)
                {
                    continue;
                }

                sb.Append($"{humanoidBone.bone}.Position.x");
                sb.Append(",");
                sb.Append($"{humanoidBone.bone}.Position.y");
                sb.Append(",");
                sb.Append($"{humanoidBone.bone}.Position.z");
                sb.Append(",");
                sb.Append($"{humanoidBone.bone}.Rotation.x");
                sb.Append(",");
                sb.Append($"{humanoidBone.bone}.Rotation.y");
                sb.Append(",");
                sb.Append($"{humanoidBone.bone}.Rotation.z");
                sb.Append(",");
                sb.Append($"{humanoidBone.bone}.Rotation.w");
                sb.Append(",");
            }
            sb.Append("Left Foot is Grounded");
            sb.Append(",");
            sb.Append("Left Toe is Grounded");
            sb.Append(",");
            sb.Append("Right Foot is Grounded");
            sb.Append(",");
            sb.Append("Right Toe is Grounded");
            sb.Append(",");
            sb.Append("Velocity.x");
            sb.Append(",");
            sb.Append("Velocity.z");
            sb.Append(",");
            sb.Append("Yaw angular velocity");
            Debug.Log(sb.ToString());
        }

        public string savePath;

        [ContextMenu("Export as CSV data SHY format")]
        public void ExportCSV()
        {

            //ファイルオープン
            string directoryStr = savePath;
            if (directoryStr == "")
            {
                //自動設定ディレクトリ
                directoryStr = Application.streamingAssetsPath + "/";
            }
            if (!Directory.Exists(directoryStr))
            {
                Directory.CreateDirectory(directoryStr);
            }

            FileStream fs = new FileStream(directoryStr + this.name + ".csv", FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);

            for (int i = 0; i < Poses.Count; i++)
            {
                var pose = Poses[i];
                var prev = i != 0 ? Poses[i - 1] : pose;
                string seriStr = pose.SerializeCSV(prev);
                sw.WriteLine(seriStr);
            }

            //ファイルクローズ
            try
            {
                sw.Close();
                fs.Close();
                sw = null;
                fs = null;
            }
            catch (Exception e)
            {
                Debug.LogError("ファイル書き出し失敗！" + e.Message + e.StackTrace);
            }

            if (sw != null)
            {
                sw.Close();
            }

            if (fs != null)
            {
                fs.Close();
            }

            UnityEditor.AssetDatabase.Refresh();
        }
#endif

        public static readonly int poseSize = 467;

        [Serializable]
        public class SerializeHumanoidPose
        {
            private static readonly float groundFootHeightThreshold = 0.2f;
            private static readonly float groundToeHeightThreshold = 0.1f;
            private static readonly float groundFootVelocityThreshold = 0.05f;
            private static readonly float groundToeVelocityThreshold = 0.05f;

            public Vector3 BodyPosition;
            public Quaternion BodyRotation;
            public Vector3 LeftfootIK_Pos;
            public Quaternion LeftfootIK_Rot;
            public Vector3 RightfootIK_Pos;
            public Quaternion RightfootIK_Rot;

            public float[] Muscles;

            public float Time;

            [Serializable]
            public class HumanoidBone
            {
                public string Name;
                public Vector3 Position;
                public Quaternion Rotation;
                public Vector3 LocalPosition;
                public Quaternion LocalRotation;
                public HumanBodyBones bone;

                private static Dictionary<Transform, string> _pathCache = new Dictionary<Transform, string>();

                public HumanoidBone() { }
                public HumanoidBone(HumanoidBone other)
                {
                    Name = other.Name;
                    Position = other.Position;
                    Rotation = other.Rotation;
                    LocalPosition = other.LocalPosition;
                    LocalRotation = other.LocalRotation;
                    bone = other.bone;
                }

                private static string BuildRelativePath(Transform root, Transform target)
                {
                    var path = "";
                    _pathCache.TryGetValue(target, out path);
                    if (path != null) return path;

                    var current = target;
                    while (true)
                    {
                        if (current == null) throw new Exception(target.name + "は" + root.name + "の子ではありません");
                        if (current == root) break;

                        path = (path == "") ? current.name : current.name + "/" + path;

                        current = current.parent;
                    }

                    _pathCache.Add(target, path);

                    return path;
                }

                public void Set(Transform root, Transform t, HumanBodyBones bone)
                {
                    if (t == null)
                    {
                        Name = "";
                        Position = Vector3.zero;
                        Rotation = Quaternion.identity;
                        LocalPosition = Vector3.zero;
                        LocalRotation = Quaternion.identity;
                        this.bone = bone;
                        return;
                    }

                    Name = BuildRelativePath(root, t);

                    Position = t.position;
                    Rotation = t.rotation;
                    LocalPosition = t.localPosition;
                    LocalRotation = t.localRotation;
                    this.bone = bone;
                }
            }

            public List<HumanoidBone> HumanoidBones = new List<HumanoidBone>();

            public SerializeHumanoidPose() { }

            public SerializeHumanoidPose(SerializeHumanoidPose other)
            {
                BodyPosition = other.BodyPosition;
                BodyRotation = other.BodyRotation;

                LeftfootIK_Pos = other.LeftfootIK_Pos;
                LeftfootIK_Rot = other.LeftfootIK_Rot;
                RightfootIK_Pos = other.RightfootIK_Pos;
                RightfootIK_Rot = other.RightfootIK_Rot;

                Muscles = other.Muscles.DeepClone();

                Time = other.Time;

                HumanoidBones = new List<HumanoidBone>();
                foreach (var bone in other.HumanoidBones)
                {
                    HumanoidBones.Add(new HumanoidBone(bone));
                }
            }

            /// <summary>
            /// Converts to SHY data format.
            /// </summary>
            /// <param name="nextTo">Required to calculate Velocity.</param>
            /// <returns></returns>
            public string SerializeCSV(SerializeHumanoidPose nextTo)
            {
                StringBuilder sb = new StringBuilder();
                SerializeVector3(sb, BodyPosition);
                SerializeQuaternion(sb, BodyRotation);

                // forloop iterate over Muscles
                for (int i = 0; i < Muscles.Length; i++)
                {

                    // if MuscleName[i] include "Jaw" and "Eye" then continue
                    if (HumanTrait.MuscleName[i].Contains("Jaw") || HumanTrait.MuscleName[i].Contains("Eye"))
                    {
                        continue;
                    }

                    sb.Append(Muscles[i]);
                    sb.Append(",");
                }

                int leftFootIsGround = 0;
                int leftToeIsGround = 0;
                int rightFootIsGround = 0;
                int rightToeIsGround = 0;

                HumanoidBone hip = null;

                foreach (var humanoidBone in HumanoidBones)
                {
                    // if humanoidBone.bone is "Jaw", "Eye", "LastBone" then continue
                    if (humanoidBone.bone == HumanBodyBones.Jaw || humanoidBone.bone == HumanBodyBones.LeftEye || humanoidBone.bone == HumanBodyBones.RightEye || humanoidBone.bone == HumanBodyBones.LastBone)
                    {
                        continue;
                    }

                    // Calucurate whether feet are grounded if bone is Foot or Toe
                    if (humanoidBone.bone == HumanBodyBones.LeftFoot)
                    {
                        if (humanoidBone.Position.y < groundFootHeightThreshold &&
                            (humanoidBone.Position - nextTo.HumanoidBones.Find(b => b.bone == HumanBodyBones.LeftFoot).Position).magnitude < groundFootVelocityThreshold)
                            leftFootIsGround = 1;
                    }
                    else if (humanoidBone.bone == HumanBodyBones.LeftToes)
                    {
                        if (humanoidBone.Position.y < groundToeHeightThreshold &&
                            (humanoidBone.Position - nextTo.HumanoidBones.Find(b => b.bone == HumanBodyBones.LeftToes).Position).magnitude < groundToeVelocityThreshold)
                            leftToeIsGround = 1;
                    }
                    else if (humanoidBone.bone == HumanBodyBones.RightFoot)
                    {
                        if (humanoidBone.Position.y < groundFootHeightThreshold &&
                            (humanoidBone.Position - nextTo.HumanoidBones.Find(b => b.bone == HumanBodyBones.RightFoot).Position).magnitude < groundFootVelocityThreshold)
                            rightFootIsGround = 1;
                    }
                    else if (humanoidBone.bone == HumanBodyBones.RightToes)
                    {
                        if (humanoidBone.Position.y < groundToeHeightThreshold &&
                            (humanoidBone.Position - nextTo.HumanoidBones.Find(b => b.bone == HumanBodyBones.RightToes).Position).magnitude < groundToeVelocityThreshold)
                            rightToeIsGround = 1;
                    }

                    if (humanoidBone.bone == HumanBodyBones.Hips)
                    {
                        hip = humanoidBone;
                    }

                    SerializeVector3(sb, humanoidBone.Position);
                    SerializeQuaternion(sb, humanoidBone.Rotation);
                }

                // Whether feet are grounded.
                sb.Append(leftFootIsGround);
                sb.Append(",");
                sb.Append(leftToeIsGround);
                sb.Append(",");
                sb.Append(rightFootIsGround);
                sb.Append(",");
                sb.Append(rightToeIsGround);
                sb.Append(",");

                // Root velocity
                var rootVelocity = (BodyPosition - nextTo.BodyPosition);

                sb.Append(rootVelocity.x);
                sb.Append(",");
                sb.Append(rootVelocity.z);
                sb.Append(",");

                // Yaw axis angular velocity
                var rotation = hip.Rotation;
                var prevRotation = nextTo.HumanoidBones.Find(b => b.bone == HumanBodyBones.Hips).Rotation;

                var angularVelocity = Quaternion.Angle(rotation, prevRotation);
                angularVelocity = angularVelocity > 180 ? 360 - angularVelocity : angularVelocity;

                sb.Append(angularVelocity);

                return sb.ToString();
            }

            private static void SerializeVector3(StringBuilder sb, Vector3 vec)
            {
                sb.Append(vec.x);
                sb.Append(",");
                sb.Append(vec.y);
                sb.Append(",");
                sb.Append(vec.z);
                sb.Append(",");
            }

            private static void SerializeQuaternion(StringBuilder sb, Quaternion q)
            {
                sb.Append(q.x);
                sb.Append(",");
                sb.Append(q.y);
                sb.Append(",");
                sb.Append(q.z);
                sb.Append(",");
                sb.Append(q.w);
                sb.Append(",");
            }

            public void DeserializeCSV(string str)
            {
                string[] dataString = str.Split(',');
                DeserializeCSV(dataString);
            }

            public void DeserializeCSV(string[] dataString)
            {
                int bodyTransformCount = 7;
                int muscleCountActual = 89;
                int boneTransformCount = 7;

                BodyPosition = DeserializeVector3(dataString, 0);
                BodyRotation = DeserializeQuaternion(dataString, 3);

                Muscles = new float[HumanTrait.MuscleCount];
                int actualIndex = 0;
                for (int i = 0; i < HumanTrait.MuscleCount; i++)
                {
                    if (HumanTrait.MuscleName[i].Contains("Jaw") || HumanTrait.MuscleName[i].Contains("Eye"))
                    {
                        continue;
                    }
                    Muscles[i] = float.Parse(dataString[actualIndex + bodyTransformCount]);
                    actualIndex++;
                }

                HumanoidBones = new List<HumanoidBone>();
                var boneValues = Enum.GetValues(typeof(HumanBodyBones)) as HumanBodyBones[];
                actualIndex = 0;
                for (int i = 0; i < boneValues.Length; i++)
                {
                    if (boneValues[i] == HumanBodyBones.Jaw || boneValues[i] == HumanBodyBones.LeftEye || boneValues[i] == HumanBodyBones.RightEye || boneValues[i] == HumanBodyBones.LastBone)
                    {
                        continue;
                    }
                    int startIndex = bodyTransformCount + muscleCountActual + (actualIndex * boneTransformCount);
                    if (dataString.Length <= startIndex)
                    {
                        break;
                    }

                    HumanoidBone bone = new HumanoidBone();
                    bone.Name = boneValues[i].ToString();
                    bone.Position = DeserializeVector3(dataString, startIndex + 0);
                    bone.Rotation = DeserializeQuaternion(dataString, startIndex + 3);
                    bone.bone = boneValues[i];
                    HumanoidBones.Add(bone);
                    actualIndex++;
                }
            }

            public void Deserialize(float[] data)
            {
                int bodyTransformCount = 7;
                int muscleCountActual = 89;
                int boneTransformCount = 7;

                BodyPosition = new Vector3(data[0], data[1], data[2]);
                BodyRotation = new Quaternion(data[3], data[4], data[5], data[6]);

                Muscles = new float[HumanTrait.MuscleCount];
                int actualIndex = 0;
                for (int i = 0; i < HumanTrait.MuscleCount; i++)
                {
                    if (HumanTrait.MuscleName[i].Contains("Jaw") || HumanTrait.MuscleName[i].Contains("Eye"))
                    {
                        continue;
                    }
                    Muscles[i] = data[actualIndex + bodyTransformCount];
                    actualIndex++;
                }

                HumanoidBones = new List<HumanoidBone>();
                var boneValues = Enum.GetValues(typeof(HumanBodyBones)) as HumanBodyBones[];
                actualIndex = 0;
                for (int i = 0; i < boneValues.Length; i++)
                {
                    if (boneValues[i] == HumanBodyBones.Jaw || boneValues[i] == HumanBodyBones.LeftEye || boneValues[i] == HumanBodyBones.RightEye || boneValues[i] == HumanBodyBones.LastBone)
                    {
                        continue;
                    }
                    int startIndex = bodyTransformCount + muscleCountActual + (actualIndex * boneTransformCount);
                    if (data.Length <= startIndex)
                    {
                        break;
                    }

                    HumanoidBone bone = new HumanoidBone();
                    bone.Name = boneValues[i].ToString();
                    bone.Position = new Vector3(data[startIndex + 0], data[startIndex + 1], data[startIndex + 2]);
                    bone.Rotation = new Quaternion(data[startIndex + 3], data[startIndex + 4], data[startIndex + 5], data[startIndex + 6]);
                    bone.bone = boneValues[i];
                    HumanoidBones.Add(bone);
                    actualIndex++;
                }
            }

            private static Vector3 DeserializeVector3(IList<string> str, int startIndex)
            {
                return new Vector3(float.Parse(str[startIndex]), float.Parse(str[startIndex + 1]), float.Parse(str[startIndex + 2]));
            }

            private static Quaternion DeserializeQuaternion(IList<string> str, int startIndex)
            {
                return new Quaternion(float.Parse(str[startIndex]), float.Parse(str[startIndex + 1]), float.Parse(str[startIndex + 2]), float.Parse(str[startIndex + 3]));
            }

        }

        public List<SerializeHumanoidPose> Poses = new List<SerializeHumanoidPose>();
    }
}
