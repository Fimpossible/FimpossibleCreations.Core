using System;
using System.Collections.Generic;

namespace FIMSpace.AnimationTools
{
    /// <summary>
    /// Class which holds most important definition for chains of transforms/bones
    /// </summary>
    [System.Serializable]
    public class BodyLegsInfo : BodyLimbInfo
    {
        public List<BodyLegReference> LeftLegs = new List<BodyLegReference>();
        public List<BodyLegReference> RightLegs = new List<BodyLegReference>();

        public override int CountBones()
        {
            int count = 0;
            for (int i = 0; i < LeftLegs.Count; i++) count += LeftLegs[i].CountBones();
            for (int i = 0; i < RightLegs.Count; i++) count += RightLegs[i].CountBones();
            return count;
        }

        public void IterateBones(Action<IBodyBone> boneAction)
        {
            for (int i = 0; i < LeftLegs.Count; i++) LeftLegs[i].IterateBones(boneAction);
            for (int i = 0; i < RightLegs.Count; i++) RightLegs[i].IterateBones(boneAction);
        }

        public BodyLegReference GetLeftLeg(int limbIndex)
        {
            return BodyDefinition.GetWithClampedIndex(LeftLegs, limbIndex);
        }

        public BodyLegReference GetRightLeg(int limbIndex)
        {
            return BodyDefinition.GetWithClampedIndex(RightLegs, limbIndex);
        }

    }
}