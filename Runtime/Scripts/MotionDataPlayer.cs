﻿/**
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
using System.Collections.Generic;

namespace Co1umbine.SHYMotionRecorder
{
    /// <summary>
    /// Motion data playback class.
    /// Change the Script Execution Order of swinging assets such as SpringBone, DynamicBone, BulletPhysicsImpl to 20000, etc.
    /// Make it a large value.
    /// DefaultExecutionOrder(11000) is intended to make the processing order slower than the VRIK system.
    /// </summary>
    [DefaultExecutionOrder(11000)]
    public class MotionDataPlayer : MonoBehaviour, IMotionPlayer
    {
        [SerializeField]
        private KeyCode _playStartKey = KeyCode.S;
        [SerializeField]
        private KeyCode _playStopKey = KeyCode.T;
        [SerializeField]
        private KeyCode _playPauseKey = KeyCode.P;
        [SerializeField]
        private KeyCode _playBackKey = KeyCode.LeftArrow;
        [SerializeField]
        private KeyCode _playNextKey = KeyCode.RightArrow;

        [SerializeField]
        protected HumanoidPoses RecordedMotionData;
        [SerializeField]
        private Animator _animator;

        [SerializeField, Tooltip("Specify the playback start frame. If it is 0, it will start from the beginning of the file.")]
        private int _startFrame;
        [SerializeField]
        protected bool _playing;
        protected bool _paused = false;
        [SerializeField]
        private PlayerStatus _status = PlayerStatus.Stop;
        [SerializeField]
        protected int _frameIndex;

        [Tooltip("FPS to record. The FPS of Update cannot be exceeded.")]
        public float TargetFPS = 30.0f;

        private HumanPoseHandler _poseHandler;
        protected Action _onPlayFinish;
        protected float _playingTime;

        public int MaxFrame { get { return RecordedMotionData?.Poses?.Count ?? 0; } }
        public int CurrentFrame { get { return _frameIndex; } }

        public PlayerStatus Status { get => _status; private set => _status = value; }

        public List<HumanoidPoses.SerializeHumanoidPose> MotionData { get => RecordedMotionData?.Poses; }
        
        public Animator Animator { get => _animator; }

        private void Awake()
        {
            if (_animator == null)
            {
                Debug.LogError("[MotionDataPlayer] Animator is not set in MotionDataPlayer. Delete MotionDataPlayer.");
                Destroy(this);
                return;
            }


            _poseHandler = new HumanPoseHandler(_animator.avatar, _animator.transform);
            _onPlayFinish += StopMotion;
        }

        // Update is called once per frame
        private void Update()
        {
            if (Input.GetKeyDown(_playStartKey))
            {
                PlayMotion();
            }

            if (Input.GetKeyDown(_playStopKey))
            {
                StopMotion();
            }

            if (Input.GetKeyDown(_playPauseKey))
            {
                PauseMotion();
            }

            if (Input.GetKeyDown(_playBackKey))
            {
                BackMotion();
            }

            if (Input.GetKeyDown(_playNextKey))
            {
                NextMotion();
            }
        }

        private void LateUpdate()
        {
            if (!_playing)
            {
                return;
            }

            if (_paused)
            {
                return;
            }

            _playingTime += Time.deltaTime;
            SetHumanPose();
        }

        /// <summary>
        /// Start playing motion data.
        /// </summary>
        public void PlayMotion()
        {
            if (_playing)
            {
                return;
            }

            if (RecordedMotionData == null)
            {
                Debug.LogError("No recorded motion data has been specified. No playback.");
                return;
            }


            _playingTime = _startFrame * (1f / TargetFPS);
            _frameIndex = _startFrame;
            _playing = true;
            Status = PlayerStatus.Playing;
        }

        /// <summary>
        /// Motion data playback complete. Called automatically even if the number of frames reaches the end.
        /// </summary>
        public void StopMotion()
        {
            if (!_playing)
            {
                return;
            }

            _playingTime = 0f;
            _frameIndex = _startFrame;
            _playing = false;
            Status = PlayerStatus.Stop;
        }

        /// <summary>
        /// Motion data playback pause/stop switch.
        /// </summary>
        public void PauseMotion()
        {
            if (!_playing)
            {
                return;
            }
            _paused = !_paused;
            if (_paused)
            {
                Status = PlayerStatus.Pause;
            }
            else
            {
                Status = PlayerStatus.Playing;
            }
        }

        /// <summary>
        /// Return frame.
        /// </summary>
        public void BackMotion()
        {
            if (!_playing)
            {
                return;
            }

            _frameIndex--;
            if (_frameIndex < 0) _frameIndex = 0;

            _playingTime = RecordedMotionData.Poses[_frameIndex].Time;
            SetHumanPose();
        }

        /// <summary>
        /// Frame forward.
        /// </summary>
        public void NextMotion()
        {
            if (!_playing)
            {
                return;
            }

            _frameIndex++;
            if (_frameIndex >= MaxFrame) _frameIndex = MaxFrame - 1;

            _playingTime = RecordedMotionData.Poses[_frameIndex].Time;
            SetHumanPose();
        }

        /// <summary>
        /// Move to specified frame.
        /// </summary>
        /// <param name="frame">frame count</param>
        public void SetFrame(int frame)
        {
            if (frame < 0 || MaxFrame <= frame) return;

            PlayMotion();
            if (!_paused) PauseMotion();

            _frameIndex = frame;
            _playingTime = RecordedMotionData.Poses[_frameIndex].Time;
            SetHumanPose();
        }

        protected virtual void SetHumanPose()
        {
            var pose = new HumanPose();
            pose.muscles = RecordedMotionData.Poses[_frameIndex].Muscles;
            pose.bodyPosition = RecordedMotionData.Poses[_frameIndex].BodyPosition;
            pose.bodyRotation = RecordedMotionData.Poses[_frameIndex].BodyRotation;

            _poseHandler.SetHumanPose(ref pose);

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

        public void SetMotion(List<HumanoidPoses.SerializeHumanoidPose> motionData)
        {
            if (Status != PlayerStatus.Stop)
                StopMotion();
            RecordedMotionData.Poses = motionData;
        }
    }
}