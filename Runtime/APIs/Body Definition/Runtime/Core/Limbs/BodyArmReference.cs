using System;
using UnityEngine;

namespace FIMSpace.AnimationTools
{
    /// <summary>
    /// Class which holds most important definition for chains of transforms/bones
    /// </summary>
    [System.Serializable]
    public class BodyArmReference : BodyLimbInfo
    {
        [SerializeReference] public IBodyBone Shoulder = new BodyBoneReference();
        [SerializeReference] public IBodyBone UpperArm = new BodyBoneReference();
        [SerializeReference] public IBodyBone LowerArm = new BodyBoneReference();
        [SerializeReference] public IBodyBone Hand = new BodyBoneReference();


        #region Transform Properties

        public Transform ShoulderT { get { if (Shoulder == null) return null; return Shoulder.BodyBone; } }
        public Transform UpperArmT { get { if (UpperArm == null) return null; return UpperArm.BodyBone; } }
        public Transform LowerArmT { get { if (LowerArm == null) return null; return LowerArm.BodyBone; } }
        public Transform HandT { get { if (Hand == null) return null; return Hand.BodyBone; } }

        #endregion


        public override int CountBones()
        {
            int count = 0;
            if (Shoulder.BodyBone != null) count += 1;
            if (UpperArm.BodyBone != null) count += 1;
            if (LowerArm.BodyBone != null) count += 1;
            if (Hand.BodyBone != null) count += 1;
            return count;
        }

        public void IterateBones(Action<IBodyBone> boneAction)
        {
            if (Shoulder.BodyBone != null) boneAction.Invoke(Shoulder);
            if (UpperArm.BodyBone != null) boneAction.Invoke(UpperArm);
            if (LowerArm.BodyBone != null) boneAction.Invoke(LowerArm);
            if (Hand.BodyBone != null) boneAction.Invoke(Hand);
        }

        /// <summary> Upper arm or shoulder transform required in order to perform the search </summary>
        public void TryFindLackingBones()
        {
            if ( ShoulderT == null && UpperArmT == null) return;    

            if (ShoulderT)
            {
                if (UpperArmT == null) UpperArm.BodyBone = BodyBonesFinder.GetContinousChildTransform(ShoulderT);
            }

            if (UpperArmT && LowerArmT == null) LowerArm.BodyBone = BodyBonesFinder.GetContinousChildTransform(UpperArmT);
            if (LowerArmT && HandT == null) Hand.BodyBone = BodyBonesFinder.GetContinousChildTransform(LowerArmT);
        }

        public IBodyBone GetBodyBone(int childIndex)
        {
            childIndex = (int)Mathf.Clamp(childIndex, 0, 3);
            
            if ( ShoulderT != null)
            {
                if (childIndex == 0) return Shoulder;
                else if (childIndex == 1) return UpperArm;
                else if (childIndex == 2) return LowerArm;
                else if (childIndex == 3) return Hand;
            }
            else
            {
                if (childIndex == 3) childIndex = 2;
                if (HandT == null) if (childIndex == 2) childIndex = 1;

                if (childIndex == 0) return UpperArm;
                else if (childIndex == 1) return LowerArm;
                else if (childIndex == 2) return Hand;
            }

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