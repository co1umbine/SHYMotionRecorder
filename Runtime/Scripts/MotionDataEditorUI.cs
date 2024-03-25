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
    public class MotionDataEditorUI : MonoBehaviour
    {
        public MotionDataEditor editor;

        public MotionDataPlayer player;

        [SerializeField]
        protected TMP_Text maxFrame;

        [SerializeField]
        protected TMP_Text currentFrame;

        [SerializeField]
        protected Slider frameSlider;

        [SerializeField]
        protected Button saveButton;

        [SerializeField]
        protected Button deleteButton;

        [SerializeField]
        protected Button resetButton;

        [SerializeField]
        protected Button undoButton;

        [SerializeField]
        protected Button redoButton;

        [SerializeField]
        protected Button playButton;

        [SerializeField]
        protected Button pauseButton;


        void Start()
        {
            Initiate();
        }

        protected virtual void Initiate()
        {
            editor = editor ?? GetComponent<MotionDataEditor>();
            player = player ?? GetComponent<MotionDataPlayer>();

            if (frameSlider != null)
            {
                frameSlider.minValue = 0;
                frameSlider.wholeNumbers = true;
                frameSlider.onValueChanged.AddListener(OnFrameChange);
            }

            if (playButton != null)
            {
                playButton.onClick.AddListener(OnPlayButton);
            }
            if (pauseButton != null)
            {
                pauseButton.onClick.AddListener(OnPauseButton);
            }

            if (saveButton != null)
            {
                saveButton.onClick.AddListener(OnSaveButton);
            }

            if (deleteButton != null)
            {
                deleteButton.onClick.AddListener(OnDeleteButton);
            }

            if (resetButton != null)
            {
                resetButton.onClick.AddListener(OnResetButton);
            }

            if (undoButton != null)
            {
                undoButton.onClick.AddListener(OnUndoButton);
            }

            if (redoButton != null)
            {
                redoButton.onClick.AddListener(OnRedoButton);
            }
        }

        void Update()
        {
            FrameUpdate();
        }

        protected virtual void FrameUpdate()
        {
            if (maxFrame != null && player != null)
            {
                maxFrame.text = player.MaxFrame.ToString();
            }
            if (currentFrame != null && player != null)
            {
                currentFrame.text = player.CurrentFrame.ToString();
            }
            if (frameSlider != null && player != null)
            {
                frameSlider.maxValue = player.MaxFrame;
                frameSlider.value = player.CurrentFrame;
            }
            if (playButton != null && player != null)
            {
                if (player.Status == PlayerStatus.Stop || player.Status == PlayerStatus.Pause)
                {
                    playButton.gameObject.SetActive(true);
                }
                else if (player.Status == PlayerStatus.Playing)
                {
                    playButton.gameObject.SetActive(false);
                }
            }
            if (pauseButton != null && player != null)
            {
                if (player.Status == PlayerStatus.Playing)
                {
                    pauseButton.gameObject.SetActive(true);
                }
                else if (player.Status == PlayerStatus.Stop || player.Status == PlayerStatus.Pause)
                {
                    pauseButton.gameObject.SetActive(false);
                }
            }
            if (undoButton != null && editor != null)
            {
                if(editor.BackLogCount == 0)
                {
                    undoButton.interactable = false;
                }
                else
                {
                    undoButton.interactable = true;
                }
            }
            if (redoButton != null && editor != null)
            {
                if(editor.ForwardLogCount == 0)
                {
                    redoButton.interactable = false;
                }
                else
                {
                    redoButton.interactable = true;
                }
            }
        }

        protected virtual void OnFrameChange(float frame)
        {
            if (player == null) return;

            if (player.Status == PlayerStatus.Pause)
            {
                player.SetFrame((int)frame);
                return;
            }
            else if (player.Status == PlayerStatus.Stop)
            {
                player.PlayMotion();
                player.PauseMotion();
            }
        }

        protected virtual void OnPlayButton()
        {
            if (player == null) return;

            if (player.Status == PlayerStatus.Pause)
            {
                player.PauseMotion();
            }
            if (player.Status == PlayerStatus.Stop)
            {
                player.PlayMotion();
            }
        }

        protected virtual void OnPauseButton()
        {
            if (player == null) return;

            if (player.Status == PlayerStatus.Playing)
            {
                player.PauseMotion();
            }
        }

        protected virtual void OnSaveButton()
        {
            if(editor == null) return;
            editor.SaveMotion();
        }
        protected virtual void OnDeleteButton()
        {
            if(editor == null) return;
            editor.DeleteMotion();
        }
        protected virtual void OnResetButton()
        {
            if(editor == null) return;
            editor.ResetMotion();
        }
        protected virtual void OnUndoButton()
        {
            if (editor == null) return;
            editor.Undo();
        }
        protected virtual void OnRedoButton()
        {
            if (editor == null) return;
            editor.Redo();
        }
    }
}