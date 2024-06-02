using FIMSpace.FEditor;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace FIMSpace.AnimationTools
{
    /// <summary>
    /// Class with methods for displaying different aspects of CharacterDefinitionData
    /// </summary>
    public static partial class BodyDefinitionEditorDraw
    {

        public static void DrawCoreTab(SerializedProperty sp, BodyDefinition definition)
        {
            // --------------------------------------

            EditorGUILayout.BeginHorizontal();

            var spc = sp.FindPropertyRelative("SkeletonRoot");

            if (definition.SkeletonRoot != null)
            {
                EditorGUIUtility.labelWidth = 60;
                TransformField(spc, " Root", FGUIResources.Tex_Bone);
            }
            else
            {
                if (GUILayout.Button(new GUIContent(FGUIResources.Tex_Bone, "Click to try finding root bone automatically"), FGUIResources.ButtonStyle, GUILayout.Width(23), GUILayout.Height(18)))
                {
                    definition.AutoGetRootBoneUsingAvailableReferences(true);
                }

                EditorGUIUtility.labelWidth = 96;
                EditorGUILayout.PropertyField(spc);
            }


            GUILayout.Space(6);
            EditorGUIUtility.labelWidth = 18;
            spc.Next(false); EditorGUILayout.PropertyField(spc, new GUIContent(" ", EditorGUIUtility.IconContent("MeshRenderer Icon").image, spc.tooltip), GUILayout.Height(18), GUILayout.MaxWidth(60)); // Mesh Parent
            EditorGUIUtility.labelWidth = 0;

            EditorGUILayout.EndHorizontal();


            GUILayout.Space(12);
            EditorGUILayout.BeginHorizontal();
            EditorGUIUtility.labelWidth = 60;
            //TransformField(sp.FindPropertyRelative("SkeletonRoot"), " Root", FGUI_Resources.Tex_Bone);

            TransformFieldB(sp.FindPropertyRelative("HipsBone"), definition.HipsBone, "  Hips", FGUIResources.FindIcon("SPR_SPelvis"));
            //definition.HipsBone.BodyBone = TransformField(" Hips", FGUI_Resources.FindIcon("SPR_SPelvis"), "For some algorithms, hips bone is the most important bone.\nIt needs to be parent of legs and first spine bone.", definition.HipsBone.BodyBone);
            EditorGUILayout.EndHorizontal();


            GUILayout.Space(12);


            EditorGUILayout.BeginHorizontal();

            // Left Side Spine Image
            EditorGUILayout.BeginVertical();
            EditorGUILayout.LabelField("", EditorStyles.centeredGreyMiniLabel, GUILayout.Width(14), GUILayout.ExpandHeight(true));
            EditorGUILayout.EndVertical();

            var imgRect = GUILayoutUtility.GetLastRect();
            imgRect.width = 64;
            imgRect.x -= 14;
            GUI.Label(imgRect, new GUIContent(FGUIResources.FindIcon("SPR_BodySpine")));
            //GUI.Box(imgRect, GUIContent.none, FGUI_Resources.BGInBoxStyleH);

            EditorGUILayout.BeginVertical(GUILayout.Width(28));

            if (GUILayout.Button(new GUIContent("+", "Add more spine bones to the chain"), FGUIResources.ButtonStyle, GUILayout.Width(22), GUILayout.Height(20)))
            {
                if (definition.SpineReferences.Bones.Count < 2)
                {
                    Transform nBone = null;
                    if (definition.SpineReferences.Bones.Count == 0) if (definition.HipsT) nBone = BodyBonesFinder.GetContinousChildTransform(definition.HipsT);
                    if (definition.SpineReferences.Bones.Count == 1) if (definition.SpineReferences.GetSpineBone(0)) nBone = BodyBonesFinder.GetContinousChildTransform(definition.SpineReferences.GetSpineBone(0));
                    definition.SpineReferences.Bones.Add(definition.GetNewBoneReference(nBone));
                    Changed(sp);
                }
                else
                {
                    Transform nBone = null;
                    if (definition.SpineReferences.GetSpineBone(definition.SpineReferences.Bones.Count - 1)) nBone = BodyBonesFinder.GetContinousChildTransform(definition.SpineReferences.GetSpineBone(definition.SpineReferences.Bones.Count - 1));

                    if (nBone)
                        definition.SpineReferences.Bones.Add(definition.GetNewBoneReference(nBone));
                    else
                        definition.SpineReferences.Bones.Insert(1, definition.GetNewBoneReference(nBone));

                    Changed(sp);
                }


            }

            EditorGUILayout.EndVertical();

            // Spine References
            EditorGUILayout.BeginVertical();
            EditorGUIUtility.labelWidth = 90;

            var spBList = sp.FindPropertyRelative("SpineReferences").FindPropertyRelative("Bones");
            if (spBList == null)
            {
                EditorGUILayout.LabelField("Not Found Spine References", EditorStyles.centeredGreyMiniLabel);
            }
            else
            {
                int bCount = spBList.arraySize;
                int toRemove = -1;
                for (int i = bCount - 1; i >= 0; i--)
                {
                    string boneName;
                    if (i == 0) boneName = "Spine Start";
                    else if (i == bCount - 1) boneName = "Spine End";
                    else if (i == bCount - 2) boneName = "Upper Chest";
                    else if (i == 1) boneName = "Chest";
                    else if (i == bCount / 2) boneName = "Spine Middle";
                    else boneName = "Spine Bone";

                    EditorGUILayout.BeginHorizontal();
                    TransformFieldB(spBList.GetArrayElementAtIndex(i), definition.SpineReferences.Bones[i], boneName, null);

                    if (i != 0)
                    {
                        FGUIInspector.RedGUIBackground();
                        if (GUILayout.Button(FGUIResources.GUIC_Remove, FGUIResources.ButtonStyle, GUILayout.Width(26), GUILayout.Height(18))) { toRemove = i; }
                        FGUIInspector.RestoreGUIBackground();
                    }

                    EditorGUILayout.EndHorizontal();
                }

                if (toRemove > -1) { definition.SpineReferences.Bones.RemoveAt(toRemove); Changed(sp); }
            }

            EditorGUIUtility.labelWidth = 0;
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();
        }


        static int leftArmSelected = 0;
        static int rightArmSelected = 0;
        public static void DrawArmsTab(SerializedProperty sp, BodyDefinition definition)
        {
            EditorGUILayout.BeginHorizontal(); // Display Left - Right arm lists

            GUI.backgroundColor = new Color(.4f, 1f, .4f, 0.25f);
            EditorGUILayout.BeginVertical(FGUIResources.BGInBoxStyleH);
            GUI.backgroundColor = Color.white;

            DrawArmsList(definition.Arms.LeftArms, ref leftArmSelected, EBodySide.Left);

            EditorGUILayout.EndVertical();

            if (leftArmSelected > -1) { rightArmSelected = -1; }
            if (leftArmSelected >= definition.Arms.LeftArms.Count) leftArmSelected = -1;

            GUI.backgroundColor = new Color(.4f, .4f, 1f, 0.25f);
            EditorGUILayout.BeginVertical(FGUIResources.BGInBoxStyleH);
            GUI.backgroundColor = Color.white;

            DrawArmsList(definition.Arms.RightArms, ref rightArmSelected, EBodySide.Right);

            EditorGUILayout.EndVertical();
            if (rightArmSelected > -1) { leftArmSelected = -1; }
            if (rightArmSelected >= definition.Arms.RightArms.Count) rightArmSelected = -1;

            EditorGUILayout.EndHorizontal();


            GUILayout.Space(8);

            if (leftArmSelected < 0 && rightArmSelected < 0)
            {
                EditorGUILayout.LabelField("No Arm Selected", EditorStyles.centeredGreyMiniLabel);
            }
            else
            {
                // Display Selecte Arm Data
                bool remove;

                if (leftArmSelected > -1)
                    remove = DisplayArmSetup(sp, definition.Arms.LeftArms[leftArmSelected]);
                else
                    remove = DisplayArmSetup(sp, definition.Arms.RightArms[rightArmSelected]);

                if (remove)
                {
                    if (leftArmSelected > -1)
                    {
                        definition.Arms.LeftArms.RemoveAt(leftArmSelected);
                        Changed(sp, true);
                        leftArmSelected = definition.Arms.LeftArms.Count - 1;
                    }
                    else
                    {
                        definition.Arms.RightArms.RemoveAt(rightArmSelected);
                        Changed(sp, true);
                        rightArmSelected = definition.Arms.RightArms.Count - 1;
                    }
                }
            }

        }


        static void DrawArmsList(List<BodyArmReference> arms, ref int selected, EBodySide side)
        {
            EditorGUILayout.BeginHorizontal();

            for (int i = 0; i < arms.Count; i++) // Selection Buttons
            {
                if (arms[i] == null) continue;
                if (selected == i) GUI.backgroundColor = Color.green;


                if (arms.Count == 1)
                {
                    if (GUILayout.Button(side == EBodySide.Left ? "Left Arm" : " Right Arm")) { if (selected == i) selected = -1; else selected = i; }
                }
                else
                if (GUILayout.Button((i + 1).ToString(), GUILayout.Width(20))) { if (selected == i) selected = -1; else selected = i; }

                GUI.backgroundColor = Color.white;
            }

            GUILayout.FlexibleSpace();

            // Add new arm field button
            if (GUILayout.Button("+", FGUIResources.ButtonStyle, GUILayout.Width(22), GUILayout.Height(18))) { arms.Add(new BodyArmReference()); }

            EditorGUILayout.EndHorizontal();

            if (arms.Count != 1)
            {
                // Ghosting arm side text
                var rect = GUILayoutUtility.GetLastRect();
                rect.x -= 11;
                GUI.color = new Color(1f, 1f, 1f, 0.5f);
                GUI.Label(rect, new GUIContent(side.ToString() + " Arms"), EditorStyles.centeredGreyMiniLabel);
                GUI.color = Color.white;
            }
        }

        static bool DisplayArmSetup(SerializedProperty sp, BodyArmReference arm)
        {
            bool remove = false;

            EditorGUILayout.BeginHorizontal();
            arm.Shoulder.BodyBone = TransformField(" Shoulder (Optional)", FGUIResources.FindIcon("SPR_SShoulder"), "", arm.Shoulder.BodyBone, 18, 140);

            // Auto get arm button display
            if (arm.HandT == null || arm.LowerArmT == null)
                if (arm.ShoulderT || arm.UpperArmT)
                {
                    if (GUILayout.Button(new GUIContent(FGUIResources.Tex_Connections, "Click to find lacking bone references automatically."), GUILayout.Width(24), GUILayout.Height(18)))
                    {
                        arm.TryFindLackingBones();
                        Changed(sp);
                    }
                }

            if (GUILayout.Button(FGUIResources.GUIC_Remove, FGUIResources.ButtonStyle, GUILayout.Width(24), GUILayout.Height(18))) remove = true;
            EditorGUILayout.EndHorizontal();

            arm.UpperArm.BodyBone = TransformField(" Upper Arm", FGUIResources.FindIcon("SPR_SUpperArm"), "", arm.UpperArm.BodyBone);
            arm.LowerArm.BodyBone = TransformField(" Lower Arm", FGUIResources.FindIcon("SPR_SLowerArm"), "", arm.LowerArm.BodyBone);
            arm.Hand.BodyBone = TransformField(" Hand", FGUIResources.FindIcon("SPR_SHand"), "", arm.Hand.BodyBone);

            return remove;
        }



        static int leftLegSelected = 0;
        static int rightLeglected = 0;
        public static void DrawLegsTab(SerializedProperty sp, BodyDefinition definition)
        {
            EditorGUILayout.BeginHorizontal(); // Display Left - Right arm lists

            GUI.backgroundColor = new Color(.4f, 1f, .4f, 0.25f);
            EditorGUILayout.BeginVertical(FGUIResources.BGInBoxStyleH);
            GUI.backgroundColor = Color.white;

            DrawLegsList(definition.Legs.LeftLegs, ref leftLegSelected, EBodySide.Left);

            EditorGUILayout.EndVertical();

            if (leftLegSelected > -1) { rightLeglected = -1; }
            if (leftLegSelected >= definition.Legs.LeftLegs.Count) leftLegSelected = -1;

            GUI.backgroundColor = new Color(.4f, .4f, 1f, 0.25f);
            EditorGUILayout.BeginVertical(FGUIResources.BGInBoxStyleH);
            GUI.backgroundColor = Color.white;

            DrawLegsList(definition.Legs.RightLegs, ref rightLeglected, EBodySide.Right);

            EditorGUILayout.EndVertical();
            if (rightLeglected > -1) { leftLegSelected = -1; }
            if (rightLeglected >= definition.Legs.RightLegs.Count) rightLeglected = -1;

            EditorGUILayout.EndHorizontal();


            GUILayout.Space(8);

            if (leftLegSelected < 0 && rightLeglected < 0)
            {
                EditorGUILayout.LabelField("No Leg Selected", EditorStyles.centeredGreyMiniLabel);
            }
            else
            {
                // Display Selecte Arm Data
                bool remove;

                if (leftLegSelected > -1)
                    remove = DisplayLegSetup(sp, definition.Legs.LeftLegs[leftLegSelected]);
                else
                    remove = DisplayLegSetup(sp, definition.Legs.RightLegs[rightLeglected]);

                if (remove)
                {
                    if (leftLegSelected > -1)
                    {
                        definition.Legs.LeftLegs.RemoveAt(leftLegSelected);
                        Changed(sp, true);
                        leftLegSelected = definition.Legs.RightLegs.Count - 1;
                    }
                    else
                    {
                        definition.Legs.RightLegs.RemoveAt(rightLeglected);
                        Changed(sp, true);
                        rightLeglected = definition.Legs.RightLegs.Count - 1;
                    }
                }
            }

        }


        static void DrawLegsList(List<BodyLegReference> legs, ref int selected, EBodySide side)
        {
            EditorGUILayout.BeginHorizontal();

            for (int i = 0; i < legs.Count; i++) // Selection Buttons
            {
                if (legs[i] == null) continue;
                if (selected == i) GUI.backgroundColor = Color.green;

                if (legs.Count == 1)
                {
                    if (GUILayout.Button(side == EBodySide.Left ? "Left Leg" : " Right Leg")) { if (selected == i) selected = -1; else selected = i; }
                }
                else
                if (GUILayout.Button((i + 1).ToString(), GUILayout.Width(20))) { if (selected == i) selected = -1; else selected = i; }

                GUI.backgroundColor = Color.white;
            }

            GUILayout.FlexibleSpace();

            // Add new arm field button
            if (GUILayout.Button("+", FGUIResources.ButtonStyle, GUILayout.Width(22), GUILayout.Height(18))) { legs.Add(new BodyLegReference()); }

            EditorGUILayout.EndHorizontal();

            if (legs.Count != 1)
            {
                // Ghosting arm side text
                var rect = GUILayoutUtility.GetLastRect();
                rect.x -= 11;
                GUI.color = new Color(1f, 1f, 1f, 0.5f);
                GUI.Label(rect, new GUIContent(side.ToString() + " Legs"), EditorStyles.centeredGreyMiniLabel);
                GUI.color = Color.white;
            }
        }

        static bool DisplayLegSetup(SerializedProperty sp, BodyLegReference leg)
        {
            bool remove = false;

            EditorGUILayout.BeginHorizontal();
            leg.UpperLeg.BodyBone = TransformField(" Upper Leg", FGUIResources.FindIcon("SPR_SUpperLeg"), "", leg.UpperLeg.BodyBone);

            // Auto get arm button display
            if (leg.FootT == null || leg.LowerLegT == null)
                if (leg.UpperLegT)
                {
                    if (GUILayout.Button(new GUIContent(FGUIResources.Tex_Connections, "Click to find lacking bone references automatically."), GUILayout.Width(24), GUILayout.Height(18)))
                    {
                        leg.TryFindLackingBones();
                        Changed(sp);
                    }
                }


            FGUIInspector.RedGUIBackground();
            if (GUILayout.Button(FGUIResources.GUIC_Remove, FGUIResources.ButtonStyle, GUILayout.Width(28), GUILayout.Height(18))) remove = true;
            FGUIInspector.RestoreGUIBackground();

            EditorGUILayout.EndHorizontal();

            leg.LowerLeg.BodyBone = TransformField(" Lower Leg", FGUIResources.FindIcon("SPR_SLowerLeg"), "", leg.LowerLeg.BodyBone);
            leg.Ankle.BodyBone = TransformField(" Ankle/Foot", FGUIResources.FindIcon("SPR_SAnkle"), "", leg.Ankle.BodyBone);
            leg.Feet.BodyBone = TransformField(" Feet (Optional)", FGUIResources.FindIcon("SPR_SFeet"), "", leg.Feet.BodyBone, 18, 130);

            return remove;
        }


        public static void DrawHeadTab(SerializedProperty sp, BodyDefinition definition)
        {
            sp = sp.FindPropertyRelative("HeadReferences");

            //EditorGUILayout.BeginHorizontal();
            //EditorGUIUtility.labelWidth = 60;
            TransformFieldB(sp.FindPropertyRelative("Head"), definition.HeadReferences.Head, " Head", FGUIResources.FindIcon("SPR_SHead"));

            GUILayout.Space(2);
            TransformFieldB(sp.FindPropertyRelative("Jaw"), definition.HeadReferences.Jaw, " Jaw (Optional)", FGUIResources.FindIcon("SPR_SJaw"));
            //EditorGUILayout.EndHorizontal();

            GUILayout.Space(10);


            EditorGUILayout.BeginHorizontal();

            // Left Side Neck Image
            EditorGUILayout.BeginVertical();
            EditorGUILayout.LabelField("", EditorStyles.centeredGreyMiniLabel, GUILayout.Width(14), GUILayout.ExpandHeight(true));
            EditorGUILayout.EndVertical();

            var imgRect = GUILayoutUtility.GetLastRect();
            imgRect.width = 64;
            imgRect.x -= 1;
            GUI.Label(imgRect, new GUIContent(FGUIResources.FindIcon("SPR_Neck")));
            //GUI.Box(imgRect, GUIContent.none, FGUI_Resources.BGInBoxStyleH);

            EditorGUILayout.BeginVertical(GUILayout.Width(28));

            if (GUILayout.Button(new GUIContent("+", "Add more neck bones to the chain"), FGUIResources.ButtonStyle, GUILayout.Width(22), GUILayout.Height(20)))
            {
                if (definition.HeadReferences.NeckBones.Count < 2)
                {
                    Transform nBone = null;
                    if (definition.HeadReferences.NeckBones.Count == 0) if (definition.SpineReferences.SpineEndT) nBone = BodyBonesFinder.GetContinousChildTransform(definition.SpineReferences.SpineEndT);
                    if (definition.HeadReferences.NeckBones.Count == 1) if (definition.HeadReferences.GetNeckBone(0)) nBone = BodyBonesFinder.GetContinousChildTransform(definition.HeadReferences.GetNeckBone(0));

                    definition.HeadReferences.NeckBones.Add(definition.GetNewBoneReference(nBone));
                    Changed(sp);
                }
                else
                {
                    Transform nBone = null;
                    if (definition.HeadReferences.GetNeckBone(definition.HeadReferences.NeckBones.Count - 1)) nBone = BodyBonesFinder.GetContinousChildTransform(definition.HeadReferences.GetNeckBone(definition.HeadReferences.NeckBones.Count - 1));

                    if (nBone)
                        definition.HeadReferences.NeckBones.Add(definition.GetNewBoneReference(nBone));
                    else
                        definition.HeadReferences.NeckBones.Insert(1, definition.GetNewBoneReference(nBone));
                    
                    Changed(sp);
                }
            }

            EditorGUILayout.EndVertical();

            // Neck Bones References
            EditorGUILayout.BeginVertical();
            EditorGUIUtility.labelWidth = 90;

            var spBList = sp.FindPropertyRelative("NeckBones");
            if (spBList == null)
            {
                EditorGUILayout.LabelField("Not Found Neck References", EditorStyles.centeredGreyMiniLabel);
            }
            else
            {
                int bCount = spBList.arraySize;

                if (bCount == 0)
                {
                    EditorGUILayout.LabelField("No Neck Bones (Optional)", EditorStyles.centeredGreyMiniLabel);
                }
                else
                {
                    int toRemove = -1;
                    for (int i = bCount - 1; i >= 0; i--)
                    {
                        string boneName = "Neck Bone " + (i + 1);

                        EditorGUILayout.BeginHorizontal();
                        TransformFieldB(spBList.GetArrayElementAtIndex(i), definition.HeadReferences.NeckBones[i], boneName, null);

                        FGUIInspector.RedGUIBackground();
                        if (GUILayout.Button(FGUIResources.GUIC_Remove, FGUIResources.ButtonStyle, GUILayout.Width(26), GUILayout.Height(18))) { toRemove = i; }
                        FGUIInspector.RestoreGUIBackground();

                        EditorGUILayout.EndHorizontal();
                    }

                    if (toRemove > -1) { definition.HeadReferences.NeckBones.RemoveAt(toRemove); Changed(sp); }
                }
            }

            EditorGUIUtility.labelWidth = 0;
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();


            GUILayout.Space(10);


            EditorGUILayout.BeginHorizontal();

            bool wide = EditorGUIUtility.currentViewWidth > 386;


            EditorGUIUtility.labelWidth = wide ? 120 : 68;
            string title = wide ? " Left Eye (Optional)" : " (Left Eye)";
            TransformFieldB(sp.FindPropertyRelative("LeftEye"), definition.HeadReferences.LeftEye, title, null);

            GUILayout.Space(6);
            title = wide ? " Right Eye (Optional)" : " (Right Eye)";
            TransformFieldB(sp.FindPropertyRelative("RightEye"), definition.HeadReferences.RightEye, title, null);
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(3);
            TransformFieldB(sp.FindPropertyRelative("BreathePoint"), definition.HeadReferences.BreathePoint, " Breathe Point (Optional)", FGUIResources.FindIcon("SPR_SJaw"));

            EditorGUIUtility.labelWidth = 0;
        }

        static int selectedChain = 0;
        public static void DrawChainsTab(SerializedProperty sp, BodyDefinition definition)
        {
            float viewWidth = EditorGUIUtility.currentViewWidth - 40;
            float currentWidth = 24;

            EditorGUILayout.BeginHorizontal();
            bool wasButton = false;

            if (selectedChain >= definition.ExtraChains.ExtraChains.Count) { selectedChain = -1; }

            if (definition.ExtraChains.ExtraChains.Count == 0)
            {
                EditorGUILayout.LabelField("No Extra Chain Added Yet", EditorStyles.centeredGreyMiniLabel);
            }
            else
            {
                for (int i = 0; i < definition.ExtraChains.ExtraChains.Count; i++)
                {
                    GUIContent draw = new GUIContent(definition.ExtraChains.ExtraChains[i].ChainNameID);
                    float wdth = EditorStyles.miniButton.CalcSize(draw).x + 4;

                    if (currentWidth + wdth > viewWidth)
                    {
                        GUILayout.FlexibleSpace();

                        if (wasButton == false)
                            if (GUILayout.Button("+", FGUIResources.ButtonStyle, GUILayout.Width(22), GUILayout.Height(18)))
                            {
                                definition.ExtraChains.ExtraChains.Add(new BodyExtraChainReference());
                                Changed(sp);
                                sp.serializedObject.ApplyModifiedProperties();
                                selectedChain = definition.ExtraChains.ExtraChains.Count - 1;
                            }

                        EditorGUILayout.EndHorizontal();

                        currentWidth = 114;
                        if (!wasButton) wasButton = true;

                        EditorGUILayout.BeginHorizontal();
                    }
                    else
                    {
                        currentWidth += wdth;
                    }

                    if (selectedChain == i) GUI.backgroundColor = Color.green;

                    if (GUILayout.Button(draw, GUILayout.Width(wdth)))
                    {
                        if (selectedChain == i) selectedChain = -1;
                        else
                            selectedChain = i;
                    }

                    GUI.backgroundColor = Color.white;
                }
            }

            if (!wasButton)
            {
                GUILayout.FlexibleSpace();

                if (GUILayout.Button("+", FGUIResources.ButtonStyle, GUILayout.Width(22), GUILayout.Height(18)))
                {
                    definition.ExtraChains.ExtraChains.Add(new BodyExtraChainReference());
                    Changed(sp);
                    sp.serializedObject.ApplyModifiedProperties();
                    selectedChain = definition.ExtraChains.ExtraChains.Count - 1;
                }
            }

            EditorGUILayout.EndHorizontal();

            #region Drag and drop for new chains

            var dropEvent = UnityEngine.Event.current;
            var dropRect = GUILayoutUtility.GetLastRect();

            switch (dropEvent.type)
            {
                case EventType.DragUpdated:
                case EventType.DragPerform:
                    if (!dropRect.Contains(dropEvent.mousePosition)) break;

                    DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

                    if (dropEvent.type == UnityEngine.EventType.DragPerform)
                    {
                        DragAndDrop.AcceptDrag();

                        foreach (var dragged in DragAndDrop.objectReferences)
                        {
                            if (dragged is GameObject)
                            {
                                GameObject go = dragged as GameObject;
                                if (go != null)
                                {
                                    var chain = new BodyExtraChainReference();
                                    chain.Bones = new List<IBodyBone>();
                                    chain.Bones.Add(definition.GetNewBoneReference(go.transform));
                                    chain.ChainNameID = go.name;
                                    definition.ExtraChains.ExtraChains.Add(chain);
                                    Changed(sp, true);
                                    selectedChain = chain.Bones.Count - 1;
                                }
                            }
                        }
                    }

                    UnityEngine.Event.current.Use();
                    break;
            }

            #endregion


            if (definition.ExtraChains.ExtraChains.Count == 0) return;

            GUILayout.Space(8);

            if (selectedChain == -1)
            {
                EditorGUILayout.LabelField("No Selected Chain", EditorStyles.centeredGreyMiniLabel);
            }
            else
            {
                GUI.backgroundColor = Color.green;
                EditorGUILayout.BeginVertical(FGUIResources.BGInBoxStyle);
                GUI.backgroundColor = Color.white;

                var chain = definition.ExtraChains.ExtraChains[selectedChain];
                EditorGUILayout.BeginHorizontal();

                EditorGUIUtility.labelWidth = 62;
                chain.ChainNameID = EditorGUILayout.TextField("Name ID:", chain.ChainNameID, GUILayout.MinWidth(200));
                EditorGUIUtility.labelWidth = 0;

                GUILayout.FlexibleSpace();
                if (GUILayout.Button(new GUIContent(" +", FGUIResources.Tex_Bone), EditorStyles.miniButton, GUILayout.MaxWidth(38), GUILayout.Height(19)))
                {
                    chain.Bones.Add(definition.GetNewEmptyBoneReference());
                    Changed(sp);
                }

                bool remove = false;
                FGUIInspector.RedGUIBackground();
                if (GUILayout.Button(FGUIResources.GUIC_Remove, FGUIResources.ButtonStyle, GUILayout.MaxWidth(24), GUILayout.Height(18))) remove = true;

                FGUIInspector.RestoreGUIBackground();

                EditorGUILayout.EndHorizontal();


                GUILayout.Space(4);
                if (chain.Bones.Count == 0)
                {
                    EditorGUILayout.LabelField("No bones in chain yet", EditorStyles.centeredGreyMiniLabel);
                }
                else
                {
                    int bToRemove = -1;
                    for (int b = 0; b < chain.Bones.Count; b++)
                    {
                        EditorGUILayout.BeginHorizontal();
                        chain.Bones[b].BodyBone = TransformField((b + 1).ToString(), null, "", chain.Bones[b].BodyBone, 18, 16, 160);
                        GUILayout.FlexibleSpace();

                        // Assign child bones button
                        if (b == 0)
                            if (chain.Bones[b].BodyBone)
                                if (chain.Bones.Count > 0 && chain.Bones[0].BodyBone)
                                {
                                    if (chain.Bones[chain.Bones.Count - 1].BodyBone == null || chain.Bones[chain.Bones.Count - 1].BodyBone.childCount != 0)
                                    {
                                        if (GUILayout.Button(new GUIContent(FGUIResources.Tex_Connections, "Try assign all child bones in the chain, starting from this bone"), GUILayout.Width(26), GUILayout.Height(18)))
                                        {
                                            chain.TryAssignAllChildBones(definition);
                                            Changed(sp);
                                        }
                                    }
                                }

                        FGUIInspector.RedGUIBackground();
                        if (GUILayout.Button("X", GUILayout.Width(22))) bToRemove = b;
                        FGUIInspector.RestoreGUIBackground();
                        EditorGUILayout.EndHorizontal();
                    }

                    if (bToRemove > -1) chain.Bones.RemoveAt(bToRemove);
                }


                if (remove)
                {
                    definition.ExtraChains.ExtraChains.RemoveAt(selectedChain);
                    selectedChain -= 1;
                }

                EditorGUILayout.EndVertical();


                #region Drag and drop for new bone

                dropEvent = UnityEngine.Event.current;
                dropRect = GUILayoutUtility.GetLastRect();

                switch (dropEvent.type)
                {
                    case EventType.DragUpdated:
                    case EventType.DragPerform:
                        if (!dropRect.Contains(dropEvent.mousePosition)) break;

                        DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

                        if (dropEvent.type == UnityEngine.EventType.DragPerform)
                        {
                            DragAndDrop.AcceptDrag();

                            foreach (var dragged in DragAndDrop.objectReferences)
                            {
                                if (dragged is GameObject)
                                {
                                    GameObject go = dragged as GameObject;
                                    if (go != null)
                                    {
                                        var newBone = definition.GetNewBoneReference(go.transform);
                                        chain.Bones.Add(newBone);
                                        Changed(sp);
                                    }
                                }
                            }
                        }

                        UnityEngine.Event.current.Use();
                        break;
                }

                #endregion


            }
        }


    }
}
