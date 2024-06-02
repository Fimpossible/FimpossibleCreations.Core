using System;
using System.Collections.Generic;

namespace FIMSpace.AnimationTools
{
    /// <summary>
    /// Class which holds most important definition for chains of transforms/bones
    /// </summary>
    [System.Serializable]
    public class BodyArmsInfo : BodyLimbInfo
    {
        public List<BodyArmReference> LeftArms = new List<BodyArmReference>();
        public List<BodyArmReference> RightArms = new List<BodyArmReference>();

        public override int CountBones()
        {
            int count = 0;
            for (int i = 0; i < LeftArms.Count; i++) count += LeftArms[i].CountBones();
            for (int i = 0; i < RightArms.Count; i++) count += RightArms[i].CountBones();
            return count;
        }

        public void IterateBones(Action<IBodyBone> boneAction)
        {
            for (int i = 0; i < LeftArms.Count; i++) LeftArms[i].IterateBones(boneAction);
            for (int i = 0; i < RightArms.Count; i++) RightArms[i].IterateBones(boneAction);
        }

        public BodyArmReference GetLeftArm(int limbIndex)
        {
            return BodyDefinition.GetWithClampedIndex( LeftArms, limbIndex);
        }

        public BodyArmReference GetRightArm(int limbIndex)
        {
            return BodyDefinition.GetWithClampedIndex(RightArms, limbIndex);
        }
    }
}