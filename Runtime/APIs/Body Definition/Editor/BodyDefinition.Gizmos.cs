using FIMSpace.FEditor;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace FIMSpace.AnimationTools
{
    public static partial class BodyDefinitionEditorDraw
    {
        static Bounds _averageBounds = new Bounds();
        static float _averageScale = 1.5f;

        public static void DrawSceneHandles(BodyDefinition definition)
        {
            if (definition._Editor_DrawGizmos == false) return;

            _averageBounds = definition.CalculateBounds;
            _averageScale = _averageBounds.size.magnitude;
            Gizmos_DrawBaseIndication(definition);

            if (definition._Editor_BodyCategory < 0)
            {
                Gizmos_DrawSkeletonSpine(definition, new Color(0.4f, 1f, 0.3f, 0.5f));
                Gizmos_DrawSphereCap(definition.HipsBone, _averageScale * 0.03f);
                Gizmos_DrawSkeletonArms(definition, new Color(0.4f, 1f, 0.3f, 0.35f), null);
                Gizmos_DrawSkeletonLegs(definition, new Color(0.4f, 1f, 0.3f, 0.35f), null);
                Gizmos_DrawSkeletonHeadBones(definition, new Color(0.4f, 1f, 0.3f, 0.35f));
                return;
            }

            var displayedCategory = definition._Editor_BodyCategory;

            if (displayedCategory == EBodyCategory.Core)
            {
                Gizmos_DrawSkeletonSpine(definition, new Color(0.4f, 1f, 0.3f, 0.7f));
                Gizmos_DrawSphereCap(definition.HipsBone, _averageScale * 0.02f);

                Gizmos_DrawSkeletonArms(definition, new Color(0.4f, 1f, 0.3f, 0.15f), null);
                Gizmos_DrawSkeletonLegs(definition, new Color(0.4f, 1f, 0.3f, 0.15f), null);
                Gizmos_DrawSkeletonHeadBones(definition, new Color(0.4f, 1f, 0.3f, 0.15f));
            }
            else if (displayedCategory == EBodyCategory.Arms)
            {
                BodyArmReference selectedArm = null;
                if (leftArmSelected > -1) selectedArm = definition.Arms.LeftArms[leftArmSelected];
                else if (rightArmSelected > -1) selectedArm = definition.Arms.RightArms[rightArmSelected];
                Gizmos_DrawSkeletonArms(definition, new Color(1f, 0.4f, 0.3f, 0.4f), selectedArm);

                Gizmos_DrawSkeletonSpine(definition, new Color(1f, 0.4f, 0.3f, 0.15f));
            }
            else if (displayedCategory == EBodyCategory.Legs)
            {
                BodyLegReference selectedLeg = null;
                if (leftLegSelected > -1) selectedLeg = definition.Legs.LeftLegs[leftLegSelected];
                else if (rightLeglected > -1) selectedLeg = definition.Legs.RightLegs[rightLeglected];
                Gizmos_DrawSkeletonLegs(definition, new Color(0.3f, 0.4f, 1f, 0.4f), selectedLeg);

                Gizmos_DrawSkeletonSpine(definition, new Color(0.3f, 0.4f, 1f, 0.15f));
            }
            else if (displayedCategory == EBodyCategory.Head)
            {
                Gizmos_DrawSkeletonSpine(definition, new Color(0.7f, 0.7f, .3f, 0.15f));
                Gizmos_DrawSkeletonHeadBones(definition, new Color(0.7f, 0.7f, .3f, 0.7f));
            }
            else if (displayedCategory == EBodyCategory.Extra)
            {
                Gizmos_DrawSkeletonSpine(definition, new Color(0.3f, 0.7f, .7f, 0.15f));
                Gizmos_DrawSkeletonExtraChains(definition, new Color(0.3f, 0.7f, .7f, 0.5f), selectedChain);
            }
        }



        #region Draw Single Gizmos Elements


        static void Gizmos_DrawSphereCap(IBodyBone bone, float radius)
        {
            if (bone == null) return;
            if (bone.BodyBone) Handles.SphereHandleCap(0, bone.BodyBone.position, Quaternion.identity, radius, EventType.Repaint);
        }


        private static void Gizmos_DrawCircle(Transform bone, Color col, Quaternion? rot = null)
        {
            if (bone == null) return;

            Handles.color = col;

            float radius = _averageBounds.size.magnitude * 0.15f;
            if (radius < 0.0001f) radius = 0.5f;
            Handles.CircleHandleCap(0, bone.position, rot != null ? rot.Value : (Quaternion.Euler(90f, 0f, 0f) * bone.rotation), radius, EventType.Repaint);
        }

        private static void Gizmos_DrawAALine(Vector3 a, Vector3 b, Color col, float fatness = 2f)
        {
            Handles.color = col;
            Handles.DrawAAPolyLine(fatness, a, b);
        }

        private static void Gizmos_DrawDottedLine(Vector3 a, Vector3 b, Color col, float dotSize = 2f)
        {
            Handles.color = col;
            Handles.DrawDottedLine(a, b, dotSize);
        }


        static void DrawBoneHandle(Vector3 from, Vector3 to, float fatness = 1f, float width = 1f, float arrowOffset = 1f)
        {
            FGizmosHandles.DrawBoneHandle(from, to, fatness, false, width, arrowOffset);
        }




        static void Gizmos_DrawBaseIndication(BodyDefinition definition)
        {
            if (definition.BaseTransform != null)
            {
                Gizmos_DrawCircle(definition.BaseTransform, new Color(0.25f, 0.65f, 1f, 0.6f));
                if (definition.HipsT) Gizmos_DrawDottedLine(definition.BaseTransform.position, definition.HipsT.position, new Color(0.25f, 0.65f, 1f, 0.6f));
            }
        }

        static void Gizmos_DrawSingleBone(IBodyBone bone, IBodyBone nextBone, float fatness = 3f, float width = 1f, float arrowOffset = 1f)
        {
            if (bone == null) return;
            if (bone.BodyBone == null) return;
            if (nextBone == null) return;
            if (nextBone.BodyBone == null) return;
            DrawBoneHandle(bone.BodyBone.position, nextBone.BodyBone.position, fatness, width, arrowOffset);
        }

        static void Gizmos_DrawSingleBone(IBodyBone bone, float fatness = 3f)
        {
            if (bone == null) return;
            if (bone.BodyBone == null) return;

            if (bone.BodyBone.childCount == 0)
            {
                Vector3 nextPos = bone.BodyBone.position;
                if (bone.BodyBone.parent != null) nextPos += (bone.BodyBone.position - bone.BodyBone.parent.position) * 0.5f;
                else nextPos += bone.BodyBone.forward * 0.2f;

                DrawBoneHandle(bone.BodyBone.position, nextPos, fatness);
                return;
            }

            if (bone.BodyBone.parent != null)
            {
                DrawBoneHandle(bone.BodyBone.position, bone.BodyBone.position + (bone.BodyBone.position - bone.BodyBone.parent.position) * 0.5f, fatness);
                return;
            }

            DrawBoneHandle(bone.BodyBone.position, bone.BodyBone.GetChild(0).position, fatness);
        }


        #endregion



        #region Draw Chains



        static void Gizmos_DrawSkeletonSpine(BodyDefinition definition, Color color)
        {
            Handles.color = color;
            Gizmos_DrawChain(definition.SpineReferences.Bones, 2f, definition.HipsBone, definition.HeadReferences.HeadStart);
        }

        static void Gizmos_DrawSkeletonArms(BodyDefinition definition, Color color, BodyArmReference selected)
        {
            Handles.color = color;

            for (int i = 0; i < definition.Arms.LeftArms.Count; i++)
            {
                var arm = definition.Arms.LeftArms[i];
                if (selected == arm) { Handles.color = new Color(color.r, color.g, color.b, 1f); }
                Gizmos_DrawArm(definition, arm);
                if (selected == arm) { Handles.color = color; }
            }

            for (int i = 0; i < definition.Arms.RightArms.Count; i++)
            {
                var arm = definition.Arms.RightArms[i];
                if (selected == arm) { Handles.color = new Color(color.r, color.g, color.b, 1f); }
                Gizmos_DrawArm(definition, arm);
                if (selected == arm) { Handles.color = color; }
            }

        }

        static void Gizmos_DrawArm(BodyDefinition definition, BodyArmReference arm)
        {
            if (arm.Shoulder.BodyBone)
            {
                if (Handles.color.a > 0.2f) Gizmos_DrawSphereCap(arm.Shoulder, _averageScale * 0.021f);
                Gizmos_DrawSingleBone(arm.Shoulder, arm.UpperArm, 3f, 1.25f, 1f);

                if (definition.SpineReferences.SpineEndT)
                    Gizmos_DrawDottedLine(arm.Shoulder.BodyBone.position, definition.SpineReferences.SpineEndT.position, Handles.color);
            }
            else
            {
                if (arm.UpperArmT && definition.SpineReferences.SpineEndT)
                    Gizmos_DrawDottedLine(arm.UpperArmT.position, definition.SpineReferences.SpineEndT.position, Handles.color);
            }

            Gizmos_DrawSingleBone(arm.UpperArm, arm.LowerArm, 3f, 0.5f, 1.5f);
            Gizmos_DrawSingleBone(arm.LowerArm, arm.Hand, 3f, 0.5f, 1.5f);

            if (Handles.color.a > 0.2f) if (arm.Hand.BodyBone) Gizmos_DrawSphereCap(arm.Hand, _averageScale * 0.02f);
        }


        static void Gizmos_DrawSkeletonLegs(BodyDefinition definition, Color color, BodyLegReference selected)
        {
            Handles.color = color;

            for (int i = 0; i < definition.Legs.LeftLegs.Count; i++)
            {
                var leg = definition.Legs.LeftLegs[i];
                if (selected == leg) { Handles.color = new Color(color.r, color.g, color.b, 1f); }
                Gizmos_DrawLeg(leg, definition);
                if (selected == leg) { Handles.color = color; }
            }

            for (int i = 0; i < definition.Legs.RightLegs.Count; i++)
            {
                var leg = definition.Legs.RightLegs[i];
                if (selected == leg) { Handles.color = new Color(color.r, color.g, color.b, 1f); }
                Gizmos_DrawLeg(leg, definition);
                if (selected == leg) { Handles.color = color; }
            }
        }

        static void Gizmos_DrawLeg(BodyLegReference leg, BodyDefinition definition)
        {
            Gizmos_DrawSingleBone(leg.UpperLeg, leg.LowerLeg, 3f, 0.35f, 1f);
            Gizmos_DrawSingleBone(leg.LowerLeg, leg.Foot, 3f, 0.35f, 1.35f);

            if (leg.Feet.BodyBone)
            {
                Gizmos_DrawSingleBone(leg.Foot, leg.Feet, 3f, .5f, 1f);
            }
            else
            {
                if (definition.BaseTransform && leg.Foot.BodyBone)
                {
                    Vector3 groundPos = definition.BaseTransform.InverseTransformPoint(leg.Foot.BodyBone.position);
                    groundPos.y = 0f;
                    groundPos.z += 0.05f;
                    DrawBoneHandle(leg.Foot.BodyBone.position, definition.BaseTransform.TransformPoint(groundPos));
                }
            }

            if (Handles.color.a > 0.2f) if (leg.Foot.BodyBone) Gizmos_DrawSphereCap(leg.Foot, _averageScale * 0.02f);

            if (definition.HipsT) if (leg.UpperLegT) Gizmos_DrawDottedLine(leg.UpperLeg.BodyBone.position, definition.HipsT.position, Handles.color, 3f);
        }

        static void Gizmos_DrawSkeletonHeadBones(BodyDefinition definition, Color color)
        {
            Handles.color = color;
            var headRefs = definition.HeadReferences;

            Gizmos_DrawSingleBone(headRefs.Head);

            Gizmos_DrawChain(headRefs.NeckBones, 2f, null, headRefs.Head);
            Gizmos_DrawSphereCap(headRefs.Jaw, _averageScale * 0.004f);

            if (headRefs.HeadT && headRefs.JawT) Gizmos_DrawAALine(headRefs.JawT.position, headRefs.HeadT.position, color, 4f);

            if (definition.HipsT && headRefs.HeadT)
            {
                if (definition.SpineReferences.SpineEnd.BodyBone)
                {
                    float scale = Vector3.Distance(definition.SpineReferences.SpineEnd.BodyBone.position, headRefs.HeadT.position) * 0.05f;
                    Gizmos_DrawSphereCap(headRefs.LeftEye, scale);
                    Gizmos_DrawSphereCap(headRefs.RightEye, scale);
                }

                if (headRefs.LeftEyeT) Gizmos_DrawDottedLine(headRefs.LeftEyeT.position, headRefs.HeadT.position, color);
                if (headRefs.RightEyeT) Gizmos_DrawDottedLine(headRefs.RightEyeT.position, headRefs.HeadT.position, color);
            }
        }
        

        static void Gizmos_DrawSkeletonExtraChains(BodyDefinition definition, Color color, int selected)
        {
            if (definition.ExtraChains == null || definition.ExtraChains.ExtraChains == null) return;

            Handles.color = color;
            for (int i = 0; i < definition.ExtraChains.ExtraChains.Count; i++)
            {
                var extra = definition.ExtraChains.ExtraChains[i];
                if (extra == null) continue;

                if (selected == i) { Handles.color = new Color(color.r, color.g, color.b, 1f); }
                Gizmos_DrawChain(extra.Bones, 2f);
                if (selected == i) { Handles.color = color; }
            }
        }


        static void Gizmos_DrawChain(List<IBodyBone> bones, float fatness = 2, IBodyBone alternativeChainStart = null, IBodyBone alternativeChainEnd = null)
        {
            // Extra Chain Start
            if (alternativeChainStart != null && alternativeChainStart.BodyBone && bones.Count > 0 && bones[0] != null && bones[0].BodyBone)
            {
                DrawBoneHandle(alternativeChainStart.BodyBone.position, bones[0].BodyBone.position, fatness);
            }

            for (int b = 0; b < bones.Count - 1; b++)
            {
                if (bones[b] == null) continue;
                if (bones[b].BodyBone == null) continue;

                if (bones[b + 1].BodyBone == null)
                {
                    DrawBoneHandle(bones[b].BodyBone.position, bones[b].BodyBone.position + bones[b].BodyBone.forward * 0.1f, fatness, 1f, 1.5f);
                    continue;
                }

                DrawBoneHandle(bones[b].BodyBone.position, bones[b + 1].BodyBone.position, fatness);
            }

            // Extra Chain End
            if (alternativeChainEnd != null && alternativeChainEnd.BodyBone && bones.Count > 0 && bones[bones.Count - 1] != null && bones[bones.Count - 1].BodyBone)
            {
                DrawBoneHandle(bones[bones.Count - 1].BodyBone.position, alternativeChainEnd.BodyBone.position, fatness);
            }
        }



        #endregion


    }
}
