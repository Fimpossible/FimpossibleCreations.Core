
using System;
using UnityEngine;

namespace FIMSpace.AnimationTools
{
    /// <summary>
    /// Class which holds most important definition for chains of transforms/bones
    /// </summary>
    [System.Serializable]
    public class BodyLegReference : BodyLimbInfo
    {
        [SerializeReference] public IBodyBone UpperLeg = new BodyBoneReference();
        [SerializeReference] public IBodyBone LowerLeg = new BodyBoneReference();
        /// <summary> The same as Foot but with different name. </summary>
        [SerializeReference] public IBodyBone Ankle = new BodyBoneReference();
         
        /// <summary> Its 'Ankle' but if you find 'Ankle' naming confusing, then use 'Foot'. </summary>
        public IBodyBone Foot { get { return Ankle; } set { Ankle = value; } }

        /// <summary> Feet and Foot are different bones. Feet are finders of foot. </summary>
        [SerializeReference] public IBodyBone Feet = new BodyBoneReference();

        #region Transform Properties

        public Transform UpperLegT { get { if (UpperLeg == null) return null; return UpperLeg.BodyBone; } }
        public Transform LowerLegT { get { if (LowerLeg == null) return null; return LowerLeg.BodyBone; } }
        /// <summary> The same as Foot but with different name. </summary>
        public Transform AnkleT { get { if (Ankle == null) return null; return Ankle.BodyBone; } }
        /// <summary> The same as Ankle but with different name (just returns Ankle) </summary>
        public Transform FootT { get { if (Ankle == null) return null; return Ankle.BodyBone; } }
        public Transform FeetT { get { if (Feet == null) return null; return Feet.BodyBone; } }

        #endregion


        public override int CountBones()
        {
            int count = 0;
            if (UpperLeg.BodyBone != null) count += 1;
            if (LowerLeg.BodyBone != null) count += 1;
            if (Ankle.BodyBone != null) count += 1;
            if (Feet.BodyBone != null) count += 1;
            return count;
        }

        public void IterateBones(Action<IBodyBone> boneAction)
        {
            if (UpperLeg.BodyBone != null) boneAction.Invoke(UpperLeg);
            if (LowerLeg.BodyBone != null) boneAction.Invoke(LowerLeg);
            if (Ankle.BodyBone != null) boneAction.Invoke(Ankle);
            if (Feet.BodyBone != null) boneAction.Invoke(Feet);
        }

        /// <summary> Upper leg transform required in order to perform the search </summary>
        public void TryFindLackingBones()
        {
            if (UpperLegT == null) return;
            if (LowerLegT == null) LowerLeg.BodyBone = BodyBonesFinder.GetContinousChildTransform(UpperLegT);
            if (LowerLegT && FootT == null) Foot.BodyBone = BodyBonesFinder.GetContinousChildTransform(LowerLegT);
            if (FootT && FeetT == null) Feet.BodyBone = BodyBonesFinder.GetContinousChildTransform(FootT);
        }


        public IBodyBone GetBodyBone(int childIndex)
        {
            childIndex = (int)Mathf.Clamp(childIndex, 0, 4);

            if (FeetT == null) if (childIndex == 4) childIndex = 3;
            if (AnkleT == null) if (childIndex == 3) childIndex = 2;

            if (childIndex == 0) return UpperLeg;
            else if (childIndex == 1) return LowerLeg;
            else if (childIndex == 2) return Ankle;
            else if (childIndex == 3) return Feet;

            return null;
        }

        public Transform GetBone(int childIndex)
        {
            var bBone = GetBodyBone(childIndex);
            if (bBone != null) return bBone.BodyBone;
            return null;
        }
    }
}