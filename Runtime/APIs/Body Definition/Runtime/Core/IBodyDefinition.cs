namespace FIMSpace.AnimationTools
{
    /// <summary>
    /// Interface of component which contains character definition data to access it from any component
    /// </summary>
    public interface IBodyDefinition
    {
        BodyDefinition BodyDefinitionData { get; }
    }
}