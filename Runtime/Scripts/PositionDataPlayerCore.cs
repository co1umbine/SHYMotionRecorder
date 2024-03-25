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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Co1umbine.SHYMotionRecorder
{
    public class PositionDataPlayerCore : MotionDataPlayerCore
    {
        private Transform boneIndicatorsParent;

        private Dictionary<HumanBodyBones, Transform> boneIndicators;

        // Use this for initialization
        private void Start()
        {
            if(boneIndicatorsParent == null)
            {
                boneIndicatorsParent = new GameObject("BoneIndicators").transform;
                boneIndicatorsParent.SetParent(Animator.transform, false);
            }

            if(boneIndicators == null)
                boneIndicators = new Dictionary<HumanBodyBones, Transform>();
        }

        protected override void SetHumanPose()
        {
            var bones = MotionData[_frameIndex].HumanoidBones;
            foreach (var bone in bones)
            {
                IndicateBonePosition(bone);
            }

            // Adjusting the playback speed of motion data that has been degraded.
            if (_playingTime > _frameIndex / TargetFPS && !_paused)
            {
                _frameIndex++;
            }

            if (_frameIndex == MotionData.Count - 1)
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
                boneIndicators[bone.bone].localPosition = bone.Position;
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
            boneIndicators[bone.bone].localPosition = bone.Position;
        }

        protected override void DirectSetHumanPose(HumanoidPoses.SerializeHumanoidPose motion)
        {
            var bones = motion.HumanoidBones;
            foreach (var bone in bones)
            {
                IndicateBonePosition(bone);
            }
        }

    }
}