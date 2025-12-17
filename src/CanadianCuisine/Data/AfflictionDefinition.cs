using Peak.Afflictions;

namespace CanadianCuisine.Data;

public class AfflictionDefinition
{
    public string Name { get; set; } = "";
    internal int index;

    /// <summary>
    /// This leads to invalid enums, but that's the point.
    /// </summary>
    public Affliction.AfflictionType Type => (Affliction.AfflictionType) index;
    
}