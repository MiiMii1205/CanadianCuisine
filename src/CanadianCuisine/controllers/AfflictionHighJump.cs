using CanadianCuisine.data;
using Peak.Afflictions;
using UnityEngine;
using Zorro.Core.Serizalization;

namespace CanadianCuisine.controllers;

public class AfflictionHighJump : Affliction
{
    public float highJumpMultiplier = 2f;
    private CuisineAfflictionCharacter? m_cuisineAfflictionCharacter;
    private Color m_jumpy = new(0.48599997f, 0.73300004f, 0.59599996f);


    public override void UpdateEffectNetworked()
    {
        character.refs.customization.PulseStatus(m_jumpy);
    }

    public AfflictionHighJump(float totalTime, float highJumpMultiplier)
        : base(totalTime)
    {
        this.highJumpMultiplier = highJumpMultiplier;
    }
    
    public AfflictionHighJump(float totalTime)
        : base(totalTime)
    {}

    public AfflictionHighJump()
    {
        
    }
    
    public override AfflictionType GetAfflictionType()
    {
        return CuisineAfflictionManager.TypeByName(CuisineAfflictionValues.HIGH_JUMP_NAME);
    }

    public override void Stack(Affliction incomingAffliction)
    {
        totalTime = incomingAffliction.totalTime;

        if (incomingAffliction is AfflictionHighJump highJump)
        {
            highJumpMultiplier = Mathf.Max(highJumpMultiplier, highJump.highJumpMultiplier);
        }
        
        timeElapsed = 0f;
        m_cuisineAfflictionCharacter ??= character.GetComponent<CuisineAfflictionCharacter>();
        m_cuisineAfflictionCharacter.RecalculateHighJump();
    }

    public override void Serialize(BinarySerializer serializer)
    {
        serializer.WriteFloat(totalTime);
        serializer.WriteFloat(highJumpMultiplier);
    }

    public override void Deserialize(BinaryDeserializer serializer)
    {
        totalTime = serializer.ReadFloat();
        highJumpMultiplier = serializer.ReadFloat();
    }

    public override void OnApplied()
    {
        m_cuisineAfflictionCharacter ??= character.GetComponent<CuisineAfflictionCharacter>();
        m_cuisineAfflictionCharacter.RecalculateHighJump();
        if (character.IsLocal)
        {
            CuisineGUIEffectManager.instance.StartHighJump();
        }
        
    }

    public override void OnRemoved()
    {
        m_cuisineAfflictionCharacter?.RecalculateHighJump();
        if (character.IsLocal)
        {
            CuisineGUIEffectManager.instance.EndHighJump();
        }
    }
}