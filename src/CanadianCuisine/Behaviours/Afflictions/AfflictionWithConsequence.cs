using CanadianCuisine.Data;
using UnityEngine;
using Zorro.Core.Serizalization;

namespace CanadianCuisine.Behaviours.Afflictions;

public class AfflictionWithConsequence : Peak.Afflictions.Affliction
{
    [SerializeReference] public Peak.Afflictions.Affliction? mainAffliction;

    [SerializeReference] public Peak.Afflictions.Affliction? consequentAffliction;

    public float delay = 0f;

    public AfflictionWithConsequence(float totalTime)
        : base(totalTime)
    {
    }

    public AfflictionWithConsequence()
    {
    }

    public override AfflictionType GetAfflictionType()
    {
        return CuisineAfflictionManager.TypeByName(CuisineAfflictionValues.WITH_CONSEQUENCE);
    }

    public override void Stack(Peak.Afflictions.Affliction incomingAffliction)
    {
        if (incomingAffliction is AfflictionWithConsequence consc)
        {
            if (consc.mainAffliction != null && mainAffliction != null &&
                consc.mainAffliction.GetAfflictionType() == mainAffliction.GetAfflictionType())
            {
                totalTime = consc.totalTime;
                timeElapsed = 0f;
                
                if (consc.consequentAffliction != null && consequentAffliction != null &&
                    consc.consequentAffliction.GetAfflictionType() == consequentAffliction.GetAfflictionType())
                {
                    consequentAffliction.totalTime += consc.consequentAffliction.totalTime;
                }
            }
        }
    }

    public override void Serialize(BinarySerializer serializer)
    {
        serializer.WriteFloat(delay);
        serializer.WriteBool(mainAffliction != null);
        serializer.WriteInt((int) (mainAffliction?.GetAfflictionType() ?? (AfflictionType) (-1)));
        serializer.WriteBool(consequentAffliction != null);
        serializer.WriteInt((int) (consequentAffliction?.GetAfflictionType() ?? (AfflictionType) (-1)));

        mainAffliction?.Serialize(serializer);
        consequentAffliction?.Serialize(serializer);
    }

    public override bool Tick()
    {
        if (mainAffliction != null && mainAffliction.character != null)
        {
            timeElapsed = Mathf.Max(mainAffliction.timeElapsed, timeElapsed);
            return mainAffliction.timeElapsed >= (double) mainAffliction.totalTime;
        }

        return base.Tick();

    }

    public override void Deserialize(BinaryDeserializer serializer)
    {
        delay = serializer.ReadFloat();

        var isMainAfflictionNotNull = serializer.ReadBool();
        
        var mainAfflictionInt = serializer.ReadInt(); 
        
        var isConsequentAfflictionNotNull = serializer.ReadBool();
        
        var consequentAfflictionInt = serializer.ReadInt();

        if (isMainAfflictionNotNull)
        {
            mainAffliction = CreateBlankAffliction((AfflictionType) mainAfflictionInt);
            mainAffliction.Deserialize(serializer);
        }

        if (isConsequentAfflictionNotNull)
        {
            consequentAffliction = CreateBlankAffliction((AfflictionType) consequentAfflictionInt);
            consequentAffliction.Deserialize(serializer);
        }
        
        totalTime = delay + (mainAffliction?.totalTime ?? 0);
    }

    public override void OnApplied()
    {
        if (character.IsLocal)
        {
            character.refs.afflictions.AddAffliction(mainAffliction);
        }
    }

    public override void OnRemoved()
    {
        if (character.IsLocal)
        {
            character.refs.afflictions.AddAffliction(consequentAffliction);
        }
    }
}