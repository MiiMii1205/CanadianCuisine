using CanadianCuisine.Data;
using Zorro.Core.Serizalization;

namespace CanadianCuisine.Behaviours.Afflictions;

public class AfflictionParalyzed: Peak.Afflictions.Affliction
{

    public override AfflictionType GetAfflictionType()
    {
        return CuisineAfflictionManager.TypeByName(CuisineAfflictionValues.PARALYZED);
    }

    public override void Stack(Peak.Afflictions.Affliction incomingAffliction)
    {
    }

    public override void Serialize(BinarySerializer serializer)
    {
        serializer.WriteFloat(totalTime);
    }

    public override void Deserialize(BinaryDeserializer serializer)
    {
        totalTime = serializer.ReadFloat();
    }

    public override void OnApplied()
    {
        character.Fall(totalTime);
    }

}