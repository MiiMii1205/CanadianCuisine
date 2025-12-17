using System.Collections;
using CanadianCuisine.Behaviours.Afflictions;
using CanadianCuisine.Data;
using Photon.Pun;
using UnityEngine;

namespace CanadianCuisine.Behaviours;

public class CuisineCharacterAfflictions : MonoBehaviourPunCallbacks
{
    private Character m_character = null!;
    private bool m_hasHighJump = false;
    private float m_originalJumpImpulse;

    public bool HasHighJump => m_hasHighJump;

    public float highJumpMultiplier = 2f;
    private CharacterMovement m_characterMovement = null!;

    private void Awake()
    {
        m_character = GetComponent<Character>();
        m_characterMovement = m_character.GetComponent<CharacterMovement>();
        m_originalJumpImpulse = m_characterMovement.jumpImpulse;
    }

    public void RecalculateHighJump()
    {
        m_hasHighJump = false;

        if (m_character.refs.afflictions.HasAfflictionType(
                CuisineAfflictionManager.TypeByName(CuisineAfflictionValues.HIGH_JUMP_NAME), out var affliction) &&
            affliction is AfflictionHighJump afflictionHighJump)
        {
            m_hasHighJump = true;
            highJumpMultiplier = afflictionHighJump.highJumpMultiplier;
        }

        m_characterMovement.jumpImpulse =
            m_hasHighJump ? m_originalJumpImpulse * highJumpMultiplier : m_originalJumpImpulse;
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
            m_character.data.sincePalJump = 0f;
            jumpDir += m_character.data.lookDirection_Flat * 0.25f;
            foreach (var sfxInstance in m_characterMovement.boostPlayer)
            {
                sfxInstance.Play(m_character.Center);
            }
        }

        m_character.data.jumpsRemaining--;
        m_character.data.isCrouching = false;
        m_character.data.chargingJump = true;
        m_character.OnStartJump();
        StartCoroutine(IDoJump());

        // ReSharper disable once InconsistentNaming
        IEnumerator IDoJump()
        {
            _ = jumpImpulse;
            if (!m_character.OutOfStamina())
            {
                m_character.refs.afflictions.SubtractStatus(CharacterAfflictions.STATUSTYPE.Web, 0.05f, fromRPC: true);
            }

            float c = 0f;
            while (c < 0.1f)
            {
                m_character.data.sinceGrounded = 0f;
                m_character.data.sinceJump = 0f;
                c += Time.deltaTime;
                yield return null;
            }

            m_character.OnJump();
            m_character.data.chargingJump = false;
            m_character.data.isJumping = true;
            bool flag = m_character.GetTotalStamina() > m_characterMovement.jumpStaminaUsageSprinting &&
                        m_character.input.sprintIsPressed;

            m_character.data.sprintJump = flag;
            m_character.UseStamina(
                (flag ? m_characterMovement.jumpStaminaUsageSprinting : m_characterMovement.jumpStaminaUsage) *
                staminaCostMult, flag);
            foreach (var part in m_character.refs.ragdoll.partList)
            {
                // We override the jump impulse to use the boosting impulse. This way, boosting another player while in high jump gives an even bigger boost
                part.AddForce(jumpDir * (jumpImpulse * jumpMult * m_characterMovement.balloonJumpMultiplier),
                    ForceMode.Acceleration);
            }
        }
    }
}