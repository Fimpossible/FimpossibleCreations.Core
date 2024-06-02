using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using UnityEngine;

namespace FIMSpace.AnimationTools
{
    /// <summary>
    /// Component which helps to define any type of character's most important body parts.
    /// BodyDefinitionSetup is used by many Fimpossible Creations plugins, but you can feel free to use it as well.
    /// </summary>
    [System.Serializable]
    public class BodyDefinition : IBodyDefinition
    {
        /// <summary> Just in case reading it as interface type </summary>
        BodyDefinition IBodyDefinition.BodyDefinitionData { get { return this; } }


        [Tooltip("Main Game Object, parent of ehole character.\nPreferred Z-Face Forward orientation.")]
        public Transform BaseTransform;

        [Tooltip("Unity Animator Reference for this Character.\n(Optional)")]
        public Animator Animator;


        [Tooltip("Skeleton Root Bone Reference.\nOptional - will find root bone automatically or will use Animator Transform if not assigned")]
        public Transform SkeletonRoot;

        [Tooltip("Parent transform which contains main character mesh renderers (optional).")]
        public Transform CharacterMeshParent;


        [Tooltip("For some algorithms, hips bone is the most important bone.\nIt needs to be parent of legs and parent of first spine bone.")]
        [SerializeReference] public IBodyBone HipsBone = new BodyBoneReference();

        public BodySpineinfo SpineReferences = new BodySpineinfo();
        public BodyArmsInfo Arms = new BodyArmsInfo();
        public BodyLegsInfo Legs = new BodyLegsInfo();
        public BodyHeadinfo HeadReferences = new BodyHeadinfo();
        public BodyExtraBoneChainsInfo ExtraChains = new BodyExtraBoneChainsInfo();

        public bool Initialized { get; private set; } = false;


        #region Utility Properties

        public Transform AnimatorT { get { if (Animator == null) return null; else return Animator.transform; } }

        /// <summary> Hips transform if assigned </summary>
        public Transform HipsT { get { if (HipsBone == null) return null; else return HipsBone.BodyBone; } }
        /// <summary> THE SAME AS HIPS. Returns its transform if assigned </summary>
        public Transform PelvisT { get { if (HipsBone == null) return null; else return HipsBone.BodyBone; } }
        /// <summary> THE SAME AS HIPS. Returns 'HipsBone' variable. </summary>
        public IBodyBone PelvisBone { get { return HipsBone; } set { HipsBone = value; } }

        public int LeftLegsCount
        {
            get
            {
                if (Legs == null) return 0;
                if (Legs.LeftLegs == null) return 0;
                return Legs.LeftLegs.Count;
            }
        }

        public int RightLegsCount
        {
            get
            {
                if (Legs == null) return 0;
                if (Legs.RightLegs == null) return 0;
                return Legs.RightLegs.Count;
            }
        }

        /// <summary>
        /// Returns average size by calculating bounds
        /// </summary>
        public float GetAverageScale
        {
            get
            {
                return CalculateBounds.size.magnitude;
            }
        }

        /// <summary>
        /// Calculating character bounds basing on all present bone references
        /// </summary>
        public Bounds CalculateBounds
        {
            get
            {
                bool assigned = false;
                Bounds bounds = new Bounds(Vector3.zero, Vector3.zero);

                if (BaseTransform != null) { bounds = new Bounds(BaseTransform.position, Vector2.zero); assigned = true; }
                if (SkeletonRoot != null) { if (!assigned) bounds = new Bounds(SkeletonRoot.position, Vector2.zero); else bounds.Encapsulate(SkeletonRoot.position); assigned = true; }

                IterateAllBones((IBodyBone b) =>
                {
                    if (!assigned) { bounds = new Bounds(b.BodyBone.position, Vector3.zero); assigned = true; }
                    else bounds.Encapsulate(b.BodyBone.position);
                });

                return bounds;
            }
        }

        public void IterateAllBones(Action<IBodyBone> boneAction)
        {
            if (HipsBone.BodyBone) boneAction.Invoke(HipsBone);
            SpineReferences.IterateBones(boneAction);
            Arms.IterateBones(boneAction);
            Legs.IterateBones(boneAction);
            HeadReferences.IterateBones(boneAction);
            ExtraChains.IterateBones(boneAction);
        }

        #endregion


        /// <summary>
        /// Checking correctness, finding not assigned bones if detected and removing null references.
        /// </summary>
        public void Initialize()
        {
            Initialized = true;
        }


        public void OnReset(Transform t)
        {
            if (BaseTransform == null)
            {
                if (t.parent != null) if (t.parent.childCount < 5) BaseTransform = t.parent;
                if (BaseTransform == null) BaseTransform = t;
            }

            if (Animator == null)
            {
                Animator = BaseTransform.GetComponentInChildren<Animator>();
                if (Animator == null) Animator = t.GetComponentInChildren<Animator>();
                if (Animator == null) Animator = BodyBonesFinder.FindComponentInAllChildren<Animator>(t);
            }

            TryFindHips();
            TryFindHead();
        }


        public int CountAllUsedBones()
        {
            int count = 0;
            if (HipsBone.BodyBone != null) count += 1;
            count += SpineReferences.CountBones();
            count += Arms.CountBones();
            count += Legs.CountBones();
            count += HeadReferences.CountBones();
            count += ExtraChains.CountBones();
            return count;
        }


        #region Helper Get Limbs Methods

        public int GetArmsCount() => Arms.LeftArms.Count + Arms.RightArms.Count;
        public BodyArmReference GetArm(int v)
        {
            if (v < 0) return null;
            if (v >= GetArmsCount()) return null;
            if (v >= Arms.LeftArms.Count) return Arms.RightArms[v - Arms.LeftArms.Count];
            return Arms.LeftArms[v];
        }

        public BodyArmReference GetLeftArm(int v)
        {
            if (v < 0) return null;
            if (v >= Arms.LeftArms.Count) return null;
            return Arms.LeftArms[v];
        }

        public BodyArmReference GetRightArm(int v)
        {
            if (v < 0) return null;
            if (v >= Arms.RightArms.Count) return null;
            return Arms.RightArms[v];
        }

        public int GetLegsCount() => Legs.LeftLegs.Count + Legs.RightLegs.Count;
        public BodyLegReference GetLeg(int v)
        {
            if (v < 0) return null;
            if (v >= GetLegsCount()) return null;
            if (v >= Legs.LeftLegs.Count) return Legs.RightLegs[v - Legs.LeftLegs.Count];
            return Legs.LeftLegs[v];
        }

        #endregion



        #region Auto Get Methods


        /// <summary> Assign all found references using Unity humanoid animator rig features </summary>
        public void AutoSetUsingHumanoidReferences()
        {
            if (Animator == null) return;

            #region Get Core References

            if (Animator.GetBoneTransform(HumanBodyBones.Hips))
                HipsBone = GetNewBoneReference(Animator.GetBoneTransform(HumanBodyBones.Hips));

            SpineReferences.Bones.Clear();

            if (Animator.GetBoneTransform(HumanBodyBones.Spine))
                SpineReferences.Bones.Add(GetNewBoneReference(Animator.GetBoneTransform(HumanBodyBones.Spine)));

            if (Animator.GetBoneTransform(HumanBodyBones.Chest))
                SpineReferences.Bones.Add(GetNewBoneReference(Animator.GetBoneTransform(HumanBodyBones.Chest)));

            if (Animator.GetBoneTransform(HumanBodyBones.UpperChest))
                SpineReferences.Bones.Add(GetNewBoneReference(Animator.GetBoneTransform(HumanBodyBones.UpperChest)));

            #endregion

            #region Get Arms References

            if (Animator.GetBoneTransform(HumanBodyBones.LeftShoulder))
                Arms.LeftArms[0].Shoulder = GetNewBoneReference(Animator.GetBoneTransform(HumanBodyBones.LeftShoulder));

            Arms.LeftArms[0].UpperArm = GetNewBoneReference(Animator.GetBoneTransform(HumanBodyBones.LeftUpperArm));
            Arms.LeftArms[0].LowerArm = GetNewBoneReference(Animator.GetBoneTransform(HumanBodyBones.LeftLowerArm));
            Arms.LeftArms[0].Hand = GetNewBoneReference(Animator.GetBoneTransform(HumanBodyBones.LeftHand));

            // ------------------------------------------------------------------------------------------

            if (Animator.GetBoneTransform(HumanBodyBones.RightShoulder))
                Arms.RightArms[0].Shoulder = GetNewBoneReference(Animator.GetBoneTransform(HumanBodyBones.RightShoulder));

            Arms.RightArms[0].UpperArm = GetNewBoneReference(Animator.GetBoneTransform(HumanBodyBones.RightUpperArm));
            Arms.RightArms[0].LowerArm = GetNewBoneReference(Animator.GetBoneTransform(HumanBodyBones.RightLowerArm));
            Arms.RightArms[0].Hand = GetNewBoneReference(Animator.GetBoneTransform(HumanBodyBones.RightHand));

            #endregion

            #region Get Leg References

            Legs.LeftLegs[0].UpperLeg = GetNewBoneReference(Animator.GetBoneTransform(HumanBodyBones.LeftUpperLeg));
            Legs.LeftLegs[0].LowerLeg = GetNewBoneReference(Animator.GetBoneTransform(HumanBodyBones.LeftLowerLeg));
            Legs.LeftLegs[0].Ankle = GetNewBoneReference(Animator.GetBoneTransform(HumanBodyBones.LeftFoot));

            // ------------------------------------------------------------------------------------------

            Legs.RightLegs[0].UpperLeg = GetNewBoneReference(Animator.GetBoneTransform(HumanBodyBones.RightUpperLeg));
            Legs.RightLegs[0].LowerLeg = GetNewBoneReference(Animator.GetBoneTransform(HumanBodyBones.RightLowerLeg));
            Legs.RightLegs[0].Ankle = GetNewBoneReference(Animator.GetBoneTransform(HumanBodyBones.RightFoot));

            #endregion

            #region Get Head References

            if (Animator.GetBoneTransform(HumanBodyBones.Head))
                HeadReferences.Head = GetNewBoneReference(Animator.GetBoneTransform(HumanBodyBones.Head));

            if (Animator.GetBoneTransform(HumanBodyBones.Jaw))
                HeadReferences.Jaw = GetNewBoneReference(Animator.GetBoneTransform(HumanBodyBones.Jaw));

            if (Animator.GetBoneTransform(HumanBodyBones.Neck))
            {
                if (HeadReferences.NeckBones.Count == 0) HeadReferences.NeckBones.Add(GetNewBoneReference(Animator.GetBoneTransform(HumanBodyBones.Neck)));
                else HeadReferences.NeckBones[0] = GetNewBoneReference(Animator.GetBoneTransform(HumanBodyBones.Neck));
            }

            if (Animator.GetBoneTransform(HumanBodyBones.LeftEye))
                HeadReferences.LeftEye = GetNewBoneReference(Animator.GetBoneTransform(HumanBodyBones.LeftEye));

            if (Animator.GetBoneTransform(HumanBodyBones.RightEye))
                HeadReferences.RightEye = GetNewBoneReference(Animator.GetBoneTransform(HumanBodyBones.RightEye));

            #endregion

            AutoGetRootBoneUsingAvailableReferences(true);
            AutoGetMainRenderersParent(true);
        }


        /// <summary> Auto get skeleton root using hips transform, animator reference and base transform </summary>
        public Transform AutoGetRootBoneUsingAvailableReferences(bool assign = true)
        {
            Transform rootBone = HipsT;

            if (rootBone && rootBone.parent)
            {
                while (rootBone.parent != null && rootBone.parent != AnimatorT && rootBone.parent != BaseTransform)
                {
                    rootBone = rootBone.parent;
                }
            }
            else { if (AnimatorT) rootBone = AnimatorT; }

            if (rootBone == null) rootBone = BaseTransform;

            if (assign) SkeletonRoot = rootBone;
            return rootBone;
        }


        /// <summary> Trying to find parent transform of mesh renderers for the character </summary>
        public Transform AutoGetMainRenderersParent(bool assign = true)
        {
            if (BaseTransform == null) return null;

            Renderer firstSkin = null;
            for (int i = 0; i < BaseTransform.childCount; i += 1)
            {
                firstSkin = BaseTransform.GetChild(i).GetComponentInChildren<SkinnedMeshRenderer>();
                if (firstSkin != null) break;
            }

            if (firstSkin == null) // Not found in the first depth check - check all child transforms then
            {
                var allChild = BaseTransform.GetComponentsInChildren<Transform>();
                for (int i = 0; i < allChild.Length; i++) { firstSkin = allChild[i].GetComponent<SkinnedMeshRenderer>(); if (firstSkin) break; }
                if (firstSkin == null) for (int i = 0; i < allChild.Length; i++) { firstSkin = allChild[i].GetComponent<MeshRenderer>(); if (firstSkin) break; }
            }

            if (firstSkin == null) return null;

            Transform parent = firstSkin.transform;

            while (parent.parent != null && parent.parent != BaseTransform && parent != AnimatorT)
            {
                parent = parent.parent;
            }

            if (assign) CharacterMeshParent = parent;
            return parent;
        }


        /// <summary> Trying to find hips bone using objects naming </summary>
        public void TryFindHips()
        {
            if (BaseTransform == null) return;
            Transform probablyHips = BodySkeletonFinder.TryFind(BaseTransform, BodySkeletonFinder.PelvisNames, Animator, true);
            if (probablyHips) HipsBone.BodyBone = probablyHips;
        }


        /// <summary> Trying to find head bone using objects naming </summary>
        public void TryFindHead()
        {
            if (BaseTransform == null) return;
            Transform probablyHead = BodySkeletonFinder.TryFind(BaseTransform, BodySkeletonFinder.HeadNames, Animator, true);
            if (probablyHead) HeadReferences.Head.BodyBone = probablyHead;
        }


        /// <summary> Using BodySkeletonFinder.SkeletonInfo to find creature bones </summary>
        public void AutoSetUsingvAvailableReferences()
        {
            if (BaseTransform == null) return;

            if (HipsT == null) TryFindHips();
            if (HeadReferences.HeadT == null) TryFindHead();
            //
            BodySkeletonFinder.SkeletonInfo skeleInfo = new BodySkeletonFinder.SkeletonInfo(BaseTransform, null, PelvisT);
            ApplySkeletonInfo(skeleInfo);

            AutoGetRootBoneUsingAvailableReferences(true);
            AutoGetMainRenderersParent(true);
        }


        /// <summary> Apply BodySkeletonFinder.SkeletonInfo to define creature bones </summary>
        public void ApplySkeletonInfo(BodySkeletonFinder.SkeletonInfo skeletonInfo)
        {
            if (HipsT == null) HipsBone.BodyBone = skeletonInfo.ProbablyHips;
            if (HeadReferences.HeadT == null) HeadReferences.Head.BodyBone = skeletonInfo.ProbablyHead;

            #region Core and Head

            if (skeletonInfo.SpineChainLength > 0)
            {
                if (skeletonInfo.SpineChainLength > 2)
                    while (SpineReferences.Bones.Count < skeletonInfo.ProbablySpineChainShort.Count)
                    {
                        SpineReferences.Bones.Add(GetNewEmptyBoneReference());
                    }

                for (int i = 0; i < skeletonInfo.ProbablySpineChainShort.Count; i++)
                {
                    SpineReferences.Bones[i].BodyBone = skeletonInfo.ProbablySpineChainShort[i];
                }

                HeadReferences.NeckBones.Clear();

                Transform headCheck = HeadReferences.HeadT;
                if (headCheck && headCheck.parent && headCheck.parent != SpineReferences.SpineEndT)
                {
                    for (int i = 0; i < 4; i++)
                    {
                        headCheck = headCheck.parent;
                        if (headCheck == null) break;
                        if (SpineReferences.ContainsTransform(headCheck)) break;
                        HeadReferences.NeckBones.Add(GetNewBoneReference(headCheck));
                    }

                    HeadReferences.NeckBones.Reverse();
                }
            }

            #endregion

            #region Arms

            BodySkeletonFinder.AdjustCount(Arms.LeftArms, skeletonInfo.LeftArms);
            for (int i = 0; i < skeletonInfo.ProbablyLeftArms.Count; i++)
            {
                if (skeletonInfo.ProbablyLeftArms[i].Count == 0) continue;
                SkeletonInfo_ApplyArm(Arms.LeftArms[i], skeletonInfo.ProbablyLeftArms[i]);
            }

            BodySkeletonFinder.AdjustCount(Arms.RightArms, skeletonInfo.RightArms);
            for (int i = 0; i < skeletonInfo.ProbablyRightArms.Count; i++)
            {
                if (skeletonInfo.ProbablyRightArms[i].Count == 0) continue;
                SkeletonInfo_ApplyArm(Arms.RightArms[i], skeletonInfo.ProbablyRightArms[i]);
            }

            #endregion

            #region Legs

            BodySkeletonFinder.AdjustCount(Legs.LeftLegs, skeletonInfo.LeftLegs);
            for (int i = 0; i < skeletonInfo.ProbablyLeftLegs.Count; i++)
            {
                if (skeletonInfo.ProbablyLeftLegs[i].Count == 0) continue;
                SkeletonInfo_ApplyLeg(Legs.LeftLegs[i], skeletonInfo.ProbablyLeftLegs[i]);
            }

            BodySkeletonFinder.AdjustCount(Legs.RightLegs, skeletonInfo.RightLegs);
            for (int i = 0; i < skeletonInfo.ProbablyRightLegs.Count; i++)
            {
                if (skeletonInfo.ProbablyRightLegs[i].Count == 0) continue;
                SkeletonInfo_ApplyLeg(Legs.RightLegs[i], skeletonInfo.ProbablyRightLegs[i]);
            }

            #endregion

        }

        /// <summary> Assign BodySkeletonFinder.SkeletonInfo data for arm </summary>
        void SkeletonInfo_ApplyArm(BodyArmReference arm, List<Transform> gotArm)
        {
            if (gotArm.Count == 0) return;

            if (gotArm.Count <= 3)
            {
                arm.UpperArm.BodyBone = gotArm[0];
                if (gotArm.Count > 1) arm.LowerArm.BodyBone = gotArm[1];
                if (gotArm.Count > 2) arm.Hand.BodyBone = gotArm[2];
            }
            else
            {
                arm.Shoulder.BodyBone = gotArm[0];
                arm.UpperArm.BodyBone = gotArm[1];
                arm.LowerArm.BodyBone = gotArm[2];
                arm.Hand.BodyBone = gotArm[gotArm.Count - 1];
            }
        }

        /// <summary> Assign BodySkeletonFinder.SkeletonInfo data for leg </summary>
        void SkeletonInfo_ApplyLeg(BodyLegReference leg, List<Transform> gotLeg)
        {
            if (gotLeg.Count == 0) return;

            if (gotLeg.Count <= 3)
            {
                leg.UpperLeg.BodyBone = gotLeg[0];
                if (gotLeg.Count > 1) leg.LowerLeg.BodyBone = gotLeg[1];
                if (gotLeg.Count > 2) leg.Foot.BodyBone = gotLeg[2];
            }
            else
            {
                leg.UpperLeg.BodyBone = gotLeg[0];
                leg.LowerLeg.BodyBone = gotLeg[1];
                leg.Foot.BodyBone = gotLeg[2];
                leg.Feet.BodyBone = gotLeg[gotLeg.Count - 1];
            }
        }


        #endregion



        /// <summary>
        /// Ensuring all classes, data structures and lists existance.
        /// </summary>
        public void CheckCorrectness()
        {
            if (SpineReferences == null) SpineReferences = new BodySpineinfo();
            if (SpineReferences.Bones == null) SpineReferences.Bones = new List<IBodyBone>();
            while (SpineReferences.Bones.Count < 2) { SpineReferences.Bones.Add(GetNewEmptyBoneReference()); }

            if (Arms == null) Arms = new BodyArmsInfo();
            if (Arms.LeftArms == null) Arms.LeftArms = new List<BodyArmReference>();
            if (Arms.LeftArms.Count == 0) Arms.LeftArms.Add(new BodyArmReference());

            if (Arms.RightArms == null) Arms.RightArms = new List<BodyArmReference>();
            if (Arms.RightArms.Count == 0) Arms.RightArms.Add(new BodyArmReference());

            if (Legs == null) Legs = new BodyLegsInfo();
            if (Legs.LeftLegs == null) Legs.LeftLegs = new List<BodyLegReference>();
            if (Legs.LeftLegs.Count == 0) Legs.LeftLegs.Add(new BodyLegReference());

            if (Legs.RightLegs == null) Legs.RightLegs = new List<BodyLegReference>();
            if (Legs.RightLegs.Count == 0) Legs.RightLegs.Add(new BodyLegReference());

            if (HeadReferences == null) HeadReferences = new BodyHeadinfo();
            if (HeadReferences.NeckBones == null) HeadReferences.NeckBones = new List<IBodyBone>();

            if (ExtraChains == null) ExtraChains = new BodyExtraBoneChainsInfo();
            if (ExtraChains.ExtraChains == null) ExtraChains.ExtraChains = new List<BodyExtraChainReference>();
        }


        /// <summary> Overridable for custom IBodyBone implementing class with custom extra data </summary>
        public virtual IBodyBone GetNewEmptyBoneReference() { return new BodyBoneReference(); }

        /// <summary> Overridable for custom IBodyBone implementing class with custom extra data using known transform (but it still can be null!) to add initialization for new IBodyBone </summary>
        public virtual IBodyBone GetNewBoneReference(Transform transform) { var bone = GetNewEmptyBoneReference(); bone.BodyBone = transform; return bone; }

        /// <summary> Returning first body part meeting the criteria. Limb index will work if there is more than two left legs, three right arms etc. </summary>
        public IBodyBone GetBodyPart(EBodyCategory bodyType, EBodySide side, int childIndex = 0, int limbIndex = 0)
        {
            if (bodyType == EBodyCategory.Core)
            {
                if (childIndex == 0) return PelvisBone;
                else
                {
                    if (childIndex < SpineReferences.Bones.Count)
                    {
                        return SpineReferences.Bones[childIndex];
                    }
                    else
                    {
                        int headIdx = childIndex - SpineReferences.Bones.Count;
                        return HeadReferences.GetBodyBone(headIdx);
                    }
                }
            }
            else if (bodyType == EBodyCategory.Arms)
            {
                if (side == EBodySide.Left)
                {
                    BodyArmReference arm = Arms.GetLeftArm(limbIndex);
                    if (arm != null) return arm.GetBodyBone(childIndex);
                }
                else
                {
                    BodyArmReference arm = Arms.GetRightArm(limbIndex);
                    if (arm != null) return arm.GetBodyBone(childIndex);
                }
            }
            else if (bodyType == EBodyCategory.Legs)
            {
                if (side == EBodySide.Left)
                {
                    BodyLegReference leg = Legs.GetLeftLeg(limbIndex);
                    if (leg != null) return leg.GetBodyBone(childIndex);
                }
                else
                {
                    BodyLegReference leg = Legs.GetRightLeg(limbIndex);
                    if (leg != null) return leg.GetBodyBone(childIndex);
                }
            }
            else if (bodyType == EBodyCategory.Extra)
            {
                var chain = ExtraChains.GetChain(limbIndex);
                if (chain == null) return null;
                return chain.GetBone(childIndex);
            }

            return null;
        }

        /// <summary> Returning first body part meeting the criteria. Limb index will work if there is more than two left legs, three right arms etc. </summary>
        public Transform GetBodyPartT(EBodyCategory bodyType, EBodySide side, int childIndex = 0, int limbIndex = 0)
        {
            var bodyBone = GetBodyPart(bodyType, side, childIndex, limbIndex);
            if (bodyBone != null) return bodyBone.BodyBone;
            return null;
        }

#if UNITY_EDITOR

        [HideInInspector] public EBodyCategory _Editor_BodyCategory = (EBodyCategory)(-1);
        [HideInInspector] public bool _Editor_DrawGizmos = true;

#endif

        #region Utilities

        public static T GetWithClampedIndex<T>(List<T> list, int i) 
        {
            if (list == null) return default;
            if (list.Count == 0) return default;
            i = ClampIndex(i, list);
            return list[i];
        }

        public static int ClampIndex<T>(int i, List<T> count)
        {
            if (count.Count == 0) return 0;
            if (i < 0) i = 0;
            if (i >= count.Count) i = count.Count - 1;
            return i;
        }

        #endregion

    }
}