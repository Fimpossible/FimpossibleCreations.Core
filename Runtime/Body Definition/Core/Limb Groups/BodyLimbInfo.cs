using System.Collections.Generic;
using UnityEngine;

namespace FIMSpace.AnimationTools
{
    /// <summary>
    /// Class which holds most important definition for chains of transforms/bones
    /// </summary>
    [System.Serializable]
    public abstract class BodyLimbInfo
    {
        /// <summary> Count of used transforms </summary>
        public abstract int CountBones();

        protected int CountBonesList(List<IBodyBone> bones)
        {
            int count = 0;

            for (int b = 0; b < bones.Count; b++)
            {
                if (bones[b] != null) if ( bones[b].BodyBone != null) count += 1;
            }

            return count;
        }

    }
}