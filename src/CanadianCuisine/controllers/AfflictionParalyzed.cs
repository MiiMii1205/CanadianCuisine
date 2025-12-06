using CanadianCuisine.data;
using Peak.Afflictions;
using Zorro.Core.Serizalization;

namespace CanadianCuisine.controllers;

public class AfflictionParalyzed: Affliction
{

    public override AfflictionType GetAfflictionType()
    {
        return CuisineAfflictionManager.TypeByName(CuisineAfflictionValues.PARALYZED);
    }

    public override void Stack(Affliction incomingAffliction)
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