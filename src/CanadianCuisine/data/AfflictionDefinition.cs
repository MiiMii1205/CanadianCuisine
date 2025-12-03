using Peak.Afflictions;

namespace CanadianCuisine.data;

public class AfflictionDefinition
{
    public string Name { get; set; } = "";
    internal int Index;

    /// <summary>
    /// This leads to invalid enums, but that's the point.
    /// </summary>
    public Affliction.AfflictionType Type => (Affliction.AfflictionType) Index;
    
}