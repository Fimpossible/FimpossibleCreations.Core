using UnityEngine;

namespace FIMSpace.AnimationTools
{
    /// <summary>
    /// Default information about character's transform/bone with IBodyBone implementation for [SerializeReference] lists
    /// </summary>
    [System.Serializable]
    public class BodyBoneReference : IBodyBone
    {
        public Transform transform;
        public Transform BodyBone { get { return transform; } set { transform = value; } }

        /// <summary> Optional data about bone for runtime use </summary>
        public object BodyCustomData { get; set; } = null;
    }
}