/**
[SHYMotionRecorder]

Copyright (c) 2023 co1umbine

This software is released under the MIT License.
http://opensource.org/licenses/mit-license.php


[EasyMotionRecorder]

MIT License

Copyright (c) 2018 Duo.inc 2020 @neon-izm 2020 @neon-izm
http://opensource.org/licenses/mit-license.php
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Co1umbine.SHYMotionRecorder
{
    public enum PlayerStatus
    {
        Stop,
        Playing,
        Pause,
    }


    public interface IMotionPlayer
    {
        public int MaxFrame { get; }
        public int CurrentFrame { get; }
        public PlayerStatus Status { get; }

        public List<HumanoidPoses.SerializeHumanoidPose> MotionData { get; }

        public Animator Animator { get; }

        public void PlayMotion();

        public void StopMotion();

        public void PauseMotion();

        public void BackMotion();

        public void NextMotion();

        public void SetFrame(int frame);

        public void SetMotion(List<HumanoidPoses.SerializeHumanoidPose> motionData);
    }
}