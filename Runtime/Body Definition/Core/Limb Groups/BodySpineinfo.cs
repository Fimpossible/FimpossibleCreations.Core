using System;
using System.Collections.Generic;
using UnityEngine;

namespace FIMSpace.AnimationTools
{
    /// <summary>
    /// Class which holds most important definition for chains of transforms/bones
    /// </summary>
    [System.Serializable]
    public class BodySpineinfo : BodyLimbInfo
    {
        [SerializeReference] public List<IBodyBone> Bones = new List<IBodyBone>();

        public IBodyBone SpineEnd { get { if (Bones.Count < 1) return null; return Bones[Bones.Count - 1]; } }
        public IBodyBone SpineStart { get { if (Bones.Count < 1) return null; return Bones[0]; } }


        #region Transform Properties

        public Transform SpineEndT { get { if (Bones.Count < 1) return null; if (Bones[Bones.Count - 1] == null) return null; return Bones[Bones.Count - 1].BodyBone; } }
        public Transform SpineStartT { get { if (Bones.Count < 1) return null; if (Bones[0] == null) return null; return Bones[0].BodyBone; } }

        public bool ContainsTransform(Transform transform)
        {
            for (int i = 0; i < Bones.Count; i++) if (Bones[i].BodyBone == transform) return true;
            return false;
        }

        /// <summary> If bone of such index exists in the bones list, returning its transform, otherwise returning null </summary>
        public Transform GetSpineBone(int index)
        {
            if (index < 0) return null;
            if (index >= Bones.Count) return null;
            if (Bones[index] == null) return null;
            return Bones[index].BodyBone;
        }

        #endregion

        public override int CountBones() { return CountBonesList(Bones); }

        public void IterateBones(Action<IBodyBone> boneAction)
        {
            for (int i = 0; i < Bones.Count; i++)
            {
                if (Bones[i].BodyBone != null) boneAction.Invoke(Bones[i]);
            }
        }

    }
}