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
using UnityEditor;
using UnityEngine;

namespace Co1umbine.SHYMotionRecorder
{
    [CustomEditor(typeof(MotionDataEditor))]
    public class MotionDataEditorEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUILayout.Space();

            GUILayout.Label("Edit", EditorStyles.boldLabel);

            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("Delete Motion"))
            {
                ((MotionDataEditor)target).DeleteMotion();
            }

            if (GUILayout.Button("Save Motion"))
            {
                ((MotionDataEditor)target).SaveMotion();
            }

            EditorGUILayout.EndHorizontal();


            if (GUILayout.Button("Reset Motion"))
            {
                ((MotionDataEditor)target).ResetMotion();
            }

            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal();

            EditorGUI.BeginDisabledGroup(((MotionDataEditor)target).BackLogCount == 0);
            if (GUILayout.Button("Å©Undo " + ((MotionDataEditor)target).BackLogCount))
            {
                ((MotionDataEditor)target).Undo();
            }
            EditorGUI.EndDisabledGroup();

            EditorGUI.BeginDisabledGroup(((MotionDataEditor)target).ForwardLogCount == 0);
            if (GUILayout.Button(((MotionDataEditor)target).ForwardLogCount + " RedoÅ®"))
            {
                ((MotionDataEditor)target).Redo();
            }
            EditorGUI.EndDisabledGroup();

            EditorGUILayout.EndHorizontal();
        }
    }
}
