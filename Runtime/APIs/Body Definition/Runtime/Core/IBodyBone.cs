using UnityEngine;

namespace FIMSpace.AnimationTools
{
    /// <summary> Reference for body bone which will be serialized with [SerializeReference] </summary>
    public interface IBodyBone
    {
        /// <summary> Bone body transform reference on the scene </summary>
        Transform BodyBone { get; set; }
    }
}