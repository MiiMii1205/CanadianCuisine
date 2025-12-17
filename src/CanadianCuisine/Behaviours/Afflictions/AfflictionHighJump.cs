using CanadianCuisine.Data;
using UnityEngine;
using Zorro.Core.Serizalization;

namespace CanadianCuisine.Behaviours.Afflictions;

public class AfflictionHighJump : Peak.Afflictions.Affliction
{
    public float highJumpMultiplier = 2f;
    private CuisineCharacterAfflictions? m_cuisineAfflictionCharacter;
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

    public override void Stack(Peak.Afflictions.Affliction incomingAffliction)
    {
        totalTime = incomingAffliction.totalTime;

        if (incomingAffliction is AfflictionHighJump highJump)
        {
            highJumpMultiplier = Mathf.Max(highJumpMultiplier, highJump.highJumpMultiplier);
        }
        
        timeElapsed = 0f;
        m_cuisineAfflictionCharacter ??= character.GetComponent<CuisineCharacterAfflictions>();
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
        m_cuisineAfflictionCharacter ??= character.GetComponent<CuisineCharacterAfflictions>();
        m_cuisineAfflictionCharacter.RecalculateHighJump();
        if (character.IsLocal)
        {
            CuisineGUIManager.instance.StartHighJump();
        }
        
    }

    public override void OnRemoved()
    {
        m_cuisineAfflictionCharacter?.RecalculateHighJump();
        if (character.IsLocal)
        {
            CuisineGUIManager.instance.EndHighJump();
        }
    }
}