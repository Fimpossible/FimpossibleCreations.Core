using System;
using System.Collections.Generic;
using UnityEngine;

namespace FIMSpace.AnimationTools
{
    /// <summary>
    /// Class which holds most important definition for chains of transforms/bones
    /// </summary>
    [System.Serializable]
    public class BodyHeadinfo : BodyLimbInfo
    {
        [SerializeReference] public IBodyBone Head = new BodyBoneReference();
        [SerializeReference] public IBodyBone Jaw = new BodyBoneReference();

        [SerializeReference] public List<IBodyBone> NeckBones = new List<IBodyBone>();

        [SerializeReference] public IBodyBone LeftEye = new BodyBoneReference();
        [SerializeReference] public IBodyBone RightEye = new BodyBoneReference();

        [SerializeReference] public IBodyBone BreathePoint = new BodyBoneReference();

        #region Transform properties

        public Transform HeadT { get { if (Head == null) return null; return Head.BodyBone; } }
        public Transform JawT { get { if (Jaw == null) return null; return Jaw.BodyBone; } }
        public Transform LeftEyeT { get { if (LeftEye == null) return null; return LeftEye.BodyBone; } }
        public Transform RightEyeT { get { if (RightEye == null) return null; return RightEye.BodyBone; } }

        public IBodyBone GetNeckBodyBone(int index)
        {
            return BodyDefinition.GetWithClampedIndex(NeckBones, index);
        }

        /// <summary> If bone of such index exists in the bones list, returning its transform, otherwise returning null </summary>
        public Transform GetNeckBone(int index)
        {
            var bBone = GetNeckBodyBone(index);
            if (bBone != null) return bBone.BodyBone;
            return null;
        }

        /// <summary> Returning neck bone, if count exceeds then returning head </summary>
        public IBodyBone GetBodyBone(int index)
        {
            if (index >= NeckBones.Count) return Head;
            if (index < 0) index = 0;
            return GetNeckBodyBone(index);
        }

        /// <summary> Returning neck bone, if count exceeds then returning head </summary>
        public Transform GetBone(int index)
        {
            var bBone = GetBodyBone(index);
            if (bBone != null) return bBone.BodyBone;
            return null;
        }

        #endregion


        /// <summary> If there is neck it will return neck, if not, it will return head reference </summary>
        public IBodyBone HeadStart
        {
            get
            {
                if (NeckBones.Count > 0) { if (NeckBones[0].BodyBone) return NeckBones[0]; } else { return Head; }
                return null;
            }
        }

        public override int CountBones()
        {
            int count = 0;
            if (Head.BodyBone != null) count += 1;
            if (Jaw.BodyBone != null) count += 1;
            if (LeftEye.BodyBone != null) count += 1;
            if (RightEye.BodyBone != null) count += 1;
            if (BreathePoint.BodyBone != null) count += 1;
            count += CountBonesList(NeckBones);

            return count;
        }

        public void IterateBones(Action<IBodyBone> boneAction)
        {
            if (Head.BodyBone != null) boneAction.Invoke(Head);
            for (int i = 0; i < NeckBones.Count; i++) if (NeckBones[i].BodyBone != null) boneAction.Invoke(NeckBones[i]);
            if (Jaw.BodyBone != null) boneAction.Invoke(Jaw);
            if (LeftEye.BodyBone != null) boneAction.Invoke(LeftEye);
            if (RightEye.BodyBone != null) boneAction.Invoke(RightEye);
            if (BreathePoint.BodyBone != null) boneAction.Invoke(BreathePoint);
        }
    }
}