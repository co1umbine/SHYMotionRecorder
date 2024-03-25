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
using UnityEngine.UI;
using TMPro;

namespace Co1umbine.SHYMotionRecorder
{
    public class FixedFrameTrimmerUI : MotionDataEditorUI
    {
        public int frameSize = 240;

        [SerializeField] private Button setRangeButton;
        [SerializeField] private Toggle playInRangeToggle;
        [SerializeField] private RectTransform frameStartIndicater;
        [SerializeField] private RectTransform frameEndIndicater;
        [SerializeField] private TMP_Text rangeStartFrame;
        [SerializeField] private TMP_Text rangeEndFrame;

        private int startRange = 0;
        private bool playInRange = false;

        protected override void Initiate()
        {
            base.Initiate();

            if (setRangeButton != null)
            {
                setRangeButton.onClick.AddListener(OnSetRange);
            }
            if (playInRangeToggle != null)
            {
                playInRangeToggle.onValueChanged.AddListener(OnPlayInRangeToggle);
                playInRange = playInRangeToggle.isOn;
            }
        }

        private void OnPlayInRangeToggle(bool active)
        {
            playInRange = active;
        }

        private void OnSetRange()
        {
            startRange = player.CurrentFrame;
        }

        protected override void FrameUpdate()
        {
            base.FrameUpdate();

            if (frameEndIndicater != null && frameStartIndicater != null && player != null && frameStartIndicater != null)
            {
                var sMax = frameStartIndicater.anchorMax;
                var sMin = frameStartIndicater.anchorMin;
                frameStartIndicater.anchorMax = new Vector2((float)startRange / player.MaxFrame, sMax.y);
                frameStartIndicater.anchorMin = new Vector2((float)startRange / player.MaxFrame, sMin.y);

                var eMax = frameEndIndicater.anchorMax;
                var eMin = frameEndIndicater.anchorMin;
                frameEndIndicater.anchorMax = new Vector2((float)(startRange + frameSize) / player.MaxFrame, eMax.y);
                frameEndIndicater.anchorMin = new Vector2((float)(startRange + frameSize) / player.MaxFrame, eMin.y);
            }

            if(player != null && playInRange && player.Status == PlayerStatus.Playing && player.CurrentFrame >= startRange + frameSize)
            {
                player.PauseMotion();
                player.SetFrame(startRange);
            }

            if(rangeStartFrame != null)
            {
                rangeStartFrame.text = startRange.ToString();
            }

            if(rangeEndFrame != null)
            {
                rangeEndFrame.text = (startRange + frameSize - 1).ToString();
            }
        }

        protected override void OnSaveButton()
        {
            if (editor == null || player == null) return;
            player.SetFrame(startRange);
            editor.DeleteMotion();
            player.SetFrame(frameSize - 1);
            editor.SaveMotion();
        }
    }
}