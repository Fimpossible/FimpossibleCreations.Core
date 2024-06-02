using System;
using System.Collections.Generic;
using UnityEngine;

namespace FIMSpace.AnimationTools
{
    /// <summary>
    /// Class which holds most important definition for chains of transforms/bones
    /// </summary>
    [System.Serializable]
    public class BodyExtraChainReference : BodyLimbInfo
    {
        [SerializeReference] public List<IBodyBone> Bones = new List<IBodyBone>();

        /// <summary> If bone of such index exists in the bones list, returning its transform, otherwise returning null </summary>
        public IBodyBone GetBone(int index)
        {
            return BodyDefinition.GetWithClampedIndex(Bones, index);
        }

        /// <summary> If bone of such index exists in the bones list, returning its transform, otherwise returning null </summary>
        public Transform GetTransform(int index)
        {
            var bBone = GetBone(index);
            if (bBone != null) return bBone.BodyBone;
            return null;
        }

        public bool ContainsTransform(Transform transform)
        {
            for (int i = 0; i < Bones.Count; i++) if (Bones[i].BodyBone == transform) return true;
            return false;
        }

        public override int CountBones() { return CountBonesList(Bones);  }

        [HideInInspector] public string ChainNameID = "Chain";

        public void IterateBones(Action<IBodyBone> boneAction)
        {
            for (int i = 0; i < Bones.Count; i++) if (Bones[i].BodyBone) boneAction.Invoke(Bones[i]);
        }

        public void TryAssignAllChildBones(BodyDefinition definition)
        {
            Transform childTr = GetTransform(0);
            if ( childTr == null ) return;

            while( childTr != null )
            {
                childTr = BodyBonesFinder.GetContinousChildTransform(childTr);
                if (childTr == null) return;

                Bones.Add( definition.GetNewBoneReference(childTr));
            }
        }
    }
}