using FIMSpace.FEditor;
using UnityEditor;
using UnityEngine;

namespace FIMSpace.AnimationTools
{
    /// <summary>
    /// Class with methods for displaying different aspects of CharacterDefinitionData
    /// </summary>
    public static partial class BodyDefinitionEditorDraw
    {

        public static void DisplayDefinitionGUI(SerializedProperty sp, BodyDefinition definition, UnityEngine.Object toDirty)
        {
            definition.CheckCorrectness();

            StartChangeCheck();

            GUILayout.Space(3);
            DrawTopReferences(sp, definition);
            DrawTopAutoButtons(sp, definition);

            GUILayout.Space(7);

            if (definition.BaseTransform == null)
            {
                EditorGUILayout.HelpBox("First assign Base Tranform, to display more options.", MessageType.Info);
                return;
            }

            DrawCategoryBar(definition);

            GUILayout.Space(10);
            if ((int)definition._Editor_BodyCategory == -1) EditorGUILayout.LabelField("Select some category to display data", EditorStyles.centeredGreyMiniLabel);
            else DrawCategoryView(sp, definition, definition._Editor_BodyCategory);

            EndChangeCheck(toDirty);
        }


        static void DrawTopAutoButtons(SerializedProperty sp, BodyDefinition definition)
        {
            if (definition.CountAllUsedBones() > 5) return;

            if (definition.Animator)
                if (definition.Animator.isHuman)
                {
                    GUILayout.Space(8);
                    if (GUILayout.Button(new GUIContent("  Auto-Get Humanoid Bones", FGUI_Resources.FindIcon("SPR_BodyDefinition")), FGUI_Resources.ButtonStyle, GUILayout.Height(22))) { definition.AutoSetUsingHumanoidReferences(); EditorUtility.SetDirty(sp.serializedObject.targetObject); }
                    return;
                }

            if (definition.BaseTransform || definition.Animator)
            {
                GUILayout.Space(8);

                if (GUILayout.Button(new GUIContent("  Try Auto-Find Generic Bones", FGUI_Resources.Tex_Bone), FGUI_Resources.ButtonStyle, GUILayout.Height(22))) 
                { 
                    definition.AutoSetUsingvAvailableReferences(); 
                    EditorUtility.SetDirty(sp.serializedObject.targetObject);
                    EditorUtility.DisplayDialog("Warning", "Auto bones setup for generic rig sometimes can be not precise. Check the auto-gathered references and adjust is needed!", "Ok I will check the skeleton");
                }

                EditorGUILayout.LabelField("Adding bones in the tabs below, will make search more precise!", EditorStyles.centeredGreyMiniLabel);
            }
        }


        static void DrawTopReferences(SerializedProperty sp, BodyDefinition defin)
        {
            var spc = sp.Copy();
            spc.Next(true);

            EditorGUILayout.BeginHorizontal();
            EditorGUIUtility.labelWidth = 126;
            EditorGUILayout.PropertyField(spc, new GUIContent(" Base Transform", EditorGUIUtility.IconContent("Transform Icon").image, spc.tooltip), GUILayout.Height(18)); // Base transform

            GUILayout.Space(4);
            EditorGUIUtility.labelWidth = 18;
            spc.Next(false); EditorGUILayout.PropertyField(spc, new GUIContent(" ", EditorGUIUtility.IconContent("Animator Icon").image, spc.tooltip), GUILayout.Height(18), GUILayout.MaxWidth(70)); // Animator
            EditorGUIUtility.labelWidth = 0;

            EditorGUILayout.EndHorizontal();
        }


        static void DrawCategoryBar(BodyDefinition definition)
        {

            GUILayoutOption bWidth = GUILayout.MaxWidth(36);
            GUILayoutOption bHeight = GUILayout.Height(25);
            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button(FGUI_Resources.GetFoldSimbol((int)definition._Editor_BodyCategory != -1, true), EditorStyles.label, bHeight, GUILayout.Width(18)))
            {
                if ((int)definition._Editor_BodyCategory == -1) definition._Editor_BodyCategory = EBodyCategory.Core;
                else definition._Editor_BodyCategory = (EBodyCategory)(-1);
            }

            DrawButton(definition, EBodyCategory.Core, "SPR_BodySpine", bWidth, bHeight);
            DrawButton(definition, EBodyCategory.Arms, "SPR_BodyArm", bWidth, bHeight);
            DrawButton(definition, EBodyCategory.Legs, "SPR_BodyLeg", bWidth, bHeight);
            DrawButton(definition, EBodyCategory.Head, "SPR_BodyHead", bWidth, bHeight);
            DrawButton(definition, EBodyCategory.Extra, "SPR_BodyExtra", bWidth, bHeight);

            GUILayout.FlexibleSpace();
            EditorGUILayout.LabelField(new GUIContent(definition.CountAllUsedBones().ToString(), "Count of all transforms references in the body definition"), EditorStyles.centeredGreyMiniLabel, GUILayout.MaxWidth(26), bHeight);

            if (GUILayout.Button(new GUIContent(definition._Editor_DrawGizmos ? FGUI_Resources.Tex_Gizmos : FGUI_Resources.Tex_GizmosOff, "Click to switch gizmos draw on the scene"), EditorStyles.label, GUILayout.Width(18), bHeight))
            {
                definition._Editor_DrawGizmos = !definition._Editor_DrawGizmos;
            }

            EditorGUILayout.EndHorizontal();
        }


        static void DrawCategoryView(SerializedProperty sp, BodyDefinition definition, EBodyCategory category)
        {
            if ((int)category < 0)
            {
                EditorGUILayout.LabelField("No Category Selected", EditorStyles.centeredGreyMiniLabel);
                return;
            }

            if (category == EBodyCategory.Core)
            {
                DrawCategoryHeader(category);
                GUILayout.Space(6);

                DrawCoreTab(sp, definition);
            }
            else if (category == EBodyCategory.Arms)
            {
                DrawArmsTab(sp, definition);
            }
            else if (category == EBodyCategory.Legs)
            {
                DrawLegsTab(sp, definition);
            }
            else if (category == EBodyCategory.Head)
            {
                DrawHeadTab(sp, definition);
            }
            else if (category == EBodyCategory.Extra)
            {
                DrawChainsTab(sp, definition);
            }
        }


        static void DrawCategoryHeader(EBodyCategory category)
        {
            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.LabelField(category.ToString() + " References", FGUI_Resources.HeaderStyle);

            EditorGUILayout.EndHorizontal();

            GUILayout.Space(4);
        }


    }
}
