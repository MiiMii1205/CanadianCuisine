using CanadianCuisine.data;
using Peak.Afflictions;
using Photon.Pun;

namespace CanadianCuisine.controllers;

public class CuisineAfflictionCharacter : MonoBehaviourPunCallbacks
{
    private Character character;
    private bool hasHighJump = false;
    private float _originalJumpImpulse;

    public float highJumpMultiplier = 2f;
    private CharacterMovement m_characterMovement;

    private void Awake()
    {
        character = GetComponent<Character>();
        m_characterMovement = character.GetComponent<CharacterMovement>();
        _originalJumpImpulse = m_characterMovement.jumpImpulse;
    }

    public void RecalculateHighJump()
    {
        hasHighJump = false;

        if (character.refs.afflictions.HasAfflictionType(CuisineAfflictionManager.TypeByName(CuisineAfflictionValues.HIGH_JUMP_NAME), out var affliction) &&
            affliction is AfflictionHighJump affliction_HighJump)
        {
            hasHighJump = true;
            highJumpMultiplier = affliction_HighJump.highJumpMultiplier;
        }

        m_characterMovement.jumpImpulse =
            hasHighJump ? _originalJumpImpulse * highJumpMultiplier : _originalJumpImpulse;
    }
  
}