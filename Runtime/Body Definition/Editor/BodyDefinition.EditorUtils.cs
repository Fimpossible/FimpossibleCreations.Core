using FIMSpace.FEditor;
using UnityEditor;
using UnityEngine;

namespace FIMSpace.AnimationTools
{
    /// <summary>
    /// BodyDefinition EditorDraw GUI Utility methods
    /// </summary>
    public static partial class BodyDefinitionEditorDraw
    {

        static void StartChangeCheck()
        {
            EditorGUI.BeginChangeCheck();
        }


        static void EndChangeCheck(SerializedProperty sp)
        {
            if (!EditorGUI.EndChangeCheck()) return;
            Changed(sp);
        }

        static void EndChangeCheck(UnityEngine.Object toDirty)
        {
            if (!EditorGUI.EndChangeCheck()) return;
            if (toDirty == null) return;
            EditorUtility.SetDirty(toDirty);
        }

        static void Changed(SerializedProperty sp, bool applySerializedObject = false)
        {
            if (sp == null) return;
            if (sp.serializedObject == null) return;
            if (sp.serializedObject.targetObject == null) return;
            EditorUtility.SetDirty(sp.serializedObject.targetObject);
            if (applySerializedObject) if (sp.serializedObject != null) sp.serializedObject.ApplyModifiedProperties();
        }

        static void DrawButton(BodyDefinition d, EBodyCategory category, string icon, GUILayoutOption width, GUILayoutOption height)
        {
            if (category == d._Editor_BodyCategory) GUI.backgroundColor = Color.green;

            if (GUILayout.Button(new GUIContent(FGUI_Resources.FindIcon(icon)), FGUI_Resources.ButtonStyle, width, height))
            {
                if (d._Editor_BodyCategory == category) d._Editor_BodyCategory = (EBodyCategory)(-1);
                else
                    d._Editor_BodyCategory = category;
            }

            GUI.backgroundColor = Color.white;
        }


        static Transform TransformField(string title, Texture icon, string tooltip, Transform defaultValue, int height = 18, int labelWidth = 0, int minWidth = 30)
        {
            EditorGUIUtility.labelWidth = labelWidth;
            Transform get = (Transform)EditorGUILayout.ObjectField(new GUIContent(title, icon, tooltip), defaultValue, typeof(Transform), true, GUILayout.Height(height), GUILayout.MinWidth(minWidth));
            EditorGUIUtility.labelWidth = 0;
            return get;
        }

        static void TransformField(SerializedProperty property, string title, Texture icon, int height = 18)
        {
            EditorGUILayout.PropertyField(property, new GUIContent(title, icon, property.tooltip), true, GUILayout.Height(height));
        }

        static void TransformFieldB(SerializedProperty property, IBodyBone bone, string title, Texture icon, int height = 18)
        {
            SerializedProperty spc = null;
            string tooltip = "";

            if (property != null)
            {
                spc = property.Copy();
                tooltip = spc.tooltip;
                spc.Next(true);
            }

            if (spc != null && spc.propertyType == SerializedPropertyType.ObjectReference)
                EditorGUILayout.PropertyField(spc, new GUIContent(title, icon, tooltip), true, GUILayout.Height(height));
            else
                bone.BodyBone = (Transform)EditorGUILayout.ObjectField(new GUIContent(title, icon, tooltip), bone.BodyBone, typeof(Transform), true, GUILayout.Height(height));
        }

    }
}
