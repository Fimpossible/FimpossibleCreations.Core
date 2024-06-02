using UnityEngine;

namespace FIMSpace.AnimationTools
{
    /// <summary>
    /// Utility Component.
    /// Component which helps to define any type of character's most important body parts.
    /// BodyDefinitionSetup is used by many Fimpossible Creations plugins, but you can feel free to use it as well.
    /// </summary>
    [AddComponentMenu("FImpossible Creations/Body Definition")]
    public class BodyDefinitionSetup : FimpossibleComponent, IBodyDefinition
    {
        [SerializeField, HideInInspector] private BodyDefinition bodyDefinition = new BodyDefinition();
        public BodyDefinition BodyDefinitionData { get { return bodyDefinition; } }

        /// <summary>
        /// Checking correctness, finding not assigned bones if detected and removing null references.
        /// </summary>
        public void Initialize()
        {
            if (bodyDefinition.Initialized) return;
            bodyDefinition.Initialize();
        }

        protected virtual void Reset()
        {
            bodyDefinition.OnReset(transform);
        }


#if UNITY_EDITOR

        [HideInInspector] public bool _Editor_DisplayTopInfo = true;

#endif

    }
}
