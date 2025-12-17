using CanadianCuisine.data;
using Peak.Afflictions;
using Photon.Pun;
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace CanadianCuisine.controllers;

public class CuisineAfflictionCharacter : MonoBehaviourPunCallbacks
{
    private Character character;
    private bool hasHighJump = false;
    private float _originalJumpImpulse;

    public bool HasHighJump => hasHighJump;

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

        if (character.refs.afflictions.HasAfflictionType(
                CuisineAfflictionManager.TypeByName(CuisineAfflictionValues.HIGH_JUMP_NAME), out var affliction) &&
            affliction is AfflictionHighJump affliction_HighJump)
        {
            hasHighJump = true;
            highJumpMultiplier = affliction_HighJump.highJumpMultiplier;
        }

        m_characterMovement.jumpImpulse =
            hasHighJump ? _originalJumpImpulse * highJumpMultiplier : _originalJumpImpulse;
    }

    [PunRPC]
    public void HighJumpRpc(bool isPalJump, float jumpImpulse)
    {
        float staminaCostMult = 1f;
        float jumpMult = 1f;
        Vector3 jumpDir = Vector3.up;
        if (isPalJump)
        {
            staminaCostMult = 0f;
            jumpMult = 2f;
            character.data.sincePalJump = 0f;
            jumpDir += character.data.lookDirection_Flat * 0.25f;
            foreach (var sfxInstance in m_characterMovement.boostPlayer)
            {
                sfxInstance.Play(character.Center);
            }
        }

        character.data.jumpsRemaining--;
        character.data.isCrouching = false;
        character.data.chargingJump = true;
        character.OnStartJump();
        StartCoroutine(IDoJump());

        IEnumerator IDoJump()
        {
            _ = jumpImpulse;
            if (!character.OutOfStamina())
            {
                character.refs.afflictions.SubtractStatus(CharacterAfflictions.STATUSTYPE.Web, 0.05f, fromRPC: true);
            }

            float c = 0f;
            while (c < 0.1f)
            {
                character.data.sinceGrounded = 0f;
                character.data.sinceJump = 0f;
                c += Time.deltaTime;
                yield return null;
            }

            character.OnJump();
            character.data.chargingJump = false;
            character.data.isJumping = true;
            bool flag = character.GetTotalStamina() > m_characterMovement.jumpStaminaUsageSprinting &&
                        character.input.sprintIsPressed;

            character.data.sprintJump = flag;
            character.UseStamina(
                (flag ? m_characterMovement.jumpStaminaUsageSprinting : m_characterMovement.jumpStaminaUsage) *
                staminaCostMult, flag);
            foreach (var part in character.refs.ragdoll.partList)
            {
                // We override the jump impulse to use the boosting impulse. This way, boosting another player while in high jump gives an even bigger boost
                part.AddForce(jumpDir * (jumpImpulse * jumpMult * m_characterMovement.balloonJumpMultiplier),
                    ForceMode.Acceleration);
            }
        }
    }
}