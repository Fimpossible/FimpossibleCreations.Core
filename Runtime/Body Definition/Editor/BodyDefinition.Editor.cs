using UnityEditor;
using UnityEngine;

namespace FIMSpace.AnimationTools
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(BodyDefinitionSetup))]
    public class BodyDefinitionEditor : Editor
    {
        public BodyDefinitionSetup Get { get { if (_get == null) _get = (BodyDefinitionSetup)target; return _get; } }
        private BodyDefinitionSetup _get;

        public override bool UseDefaultMargins() => false;

        GUIStyle _bgStyle = null;
        SerializedProperty sp_definition;

        private void OnEnable()
        {
            sp_definition = serializedObject.FindProperty("bodyDefinition");

            //if ( Get.BodyDefinitionData.Animator == null || Get.BodyDefinitionData.Animator.isHuman == false)
            //{
            //    Get.BodyDefinitionData._Editor_BodyCategory = EBodyCategory.Core;
            //}
        }

        bool wasGui = false;

        public override void OnInspectorGUI()
        {
            if (_bgStyle == null)
            {
                _bgStyle = new GUIStyle();
                _bgStyle.padding = new RectOffset(12, 10, 2, 4);

                FSceneIcons.SetGizmoIconEnabled(Get, false);
            }

            EditorGUILayout.BeginVertical(_bgStyle);

            serializedObject.Update();

            GUILayout.Space(2f);
            EditorGUILayout.BeginVertical();

            if (Get._Editor_DisplayTopInfo)
            {
                EditorGUILayout.HelpBox("This component is displaying CharacterDefinitionData, which can be used as shared info about one character. CharacterDefinitionData can also be used individually on unique components, with use of ICharacterDefinition interface.", UnityEditor.MessageType.Info);
            
                var bRect = GUILayoutUtility.GetLastRect();
                if (GUI.Button(bRect, "", EditorStyles.label)) Get._Editor_DisplayTopInfo = !Get._Editor_DisplayTopInfo;
            }

            if (Get._Editor_DisplayTopInfo) GUI.backgroundColor = Color.white; else GUI.backgroundColor = new Color(1f, 1f, 1f, 0.6f);
            if (GUI.Button(new Rect(6, 0, EditorGUIUtility.currentViewWidth - 10, 5), "", EditorStyles.helpBox)) { Get._Editor_DisplayTopInfo = !Get._Editor_DisplayTopInfo; }
            
            GUI.backgroundColor = Color.white;
            GUILayout.Space(6f);

            DrawPropertiesExcluding(serializedObject, "m_Script");
            GUI_DrawSetup();

            EditorGUILayout.EndVertical();

            serializedObject.ApplyModifiedProperties();
         
            EditorGUILayout.EndVertical();

            wasGui = true;
        }

        void GUI_DrawSetup()
        {
            BodyDefinitionEditorDraw.DisplayDefinitionGUI(sp_definition, Get.BodyDefinitionData, Get);
        }

        private void OnSceneGUI()
        {
            if (!wasGui) return;
            BodyDefinitionEditorDraw.DrawSceneHandles(Get.BodyDefinitionData);
        }

    }

}
