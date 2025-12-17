using CanadianCuisine.Behaviours;
using HarmonyLib;
using Photon.Pun;
using pworld.Scripts.Extensions;
using Object = UnityEngine.Object;

namespace CanadianCuisine.Patchers;

public class CuisineCharacterPatcher
{
    [HarmonyPatch(typeof(CharacterAfflictions), "Awake")]
    [HarmonyPostfix]
    public static void AfflictionAwakePostfix(CharacterAfflictions __instance)
    {
        var affliction = __instance.character.gameObject.GetOrAddComponent<CuisineCharacterAfflictions>();

        if (affliction == null)
        {
            Plugin.Log.LogError($"Can't add CuisineCharacterAfflictions to {__instance.character.characterName}");
        }
    }

    [HarmonyPatch(typeof(GUIManager), "Awake")]
    [HarmonyPostfix]
    public static void GUIAwakePostfix(GUIManager __instance)
    {
        var man = __instance.gameObject.GetOrAddComponent<CuisineGUIManager>();

        var effectTrs = __instance.poisonSVFX.transform;

        if (man == null)
        {
            Plugin.Log.LogError($"Can't add CuisineGUIManager to {__instance.character.characterName}'s GUI");
        }
        else
        {
            var effects = Object.Instantiate(Plugin.HighJumpEffectPrefab, effectTrs.parent);
            effects.transform.transform.localPosition = effectTrs.localPosition;
            effects.transform.transform.localRotation = effectTrs.localRotation;
            effects.transform.transform.localScale = effectTrs.localScale;
            man.highJumpSvfx = effects.GetComponent<ScreenVFX>();
            effects.SetActive(false);
        }
    }

    [HarmonyPatch(typeof(CharacterMovement), "CheckForPalJump")]
    [HarmonyPrefix]
    public static void CheckForPalJumpPrefix(CharacterMovement __instance, Character c, ref bool __runOriginal)
    {
        if (c.GetComponent<CuisineCharacterAfflictions>() is not {HasHighJump: true})
        {
            __runOriginal = true;
        }
        else
        {
            __runOriginal = false;
            Plugin.Log.LogInfo($"High Jump boost given to {__instance.character.characterName} by {c.characterName}");
            if (__instance.character.data.sinceStandOnPlayer < 0.3f && c.data.sinceJump < 0.3f)
            {
                __instance.character.data.lastStoodOnPlayer = null;
                if (__instance.character.refs.view.IsMine)
                {
                    // This way we can call our custom High Jump RPC function that uses the character impulse as a base 
                    __instance.character.refs.view.RPC("HighJumpRpc", RpcTarget.All, true, c.refs.movement.jumpImpulse);
                }
            }
        }
    }
    
    [HarmonyPatch(typeof(CharacterClimbing), "RPCA_ClimbJump")]
    [HarmonyPrefix]
    public static void RPCA_ClimbJumpPrefix(CharacterClimbing __instance, ref bool __runOriginal)
    {
        if (__instance.GetComponent<CuisineCharacterAfflictions>() is not {HasHighJump: true} cac)
        {
            __runOriginal = true;
        }
        else
        {
            __runOriginal = false;
            Plugin.Log.LogInfo($"{__instance.character.characterName} lunged while in high jump.");
            // OG Code
            __instance.character.data.sinceClimbJump = 0f;
            __instance.character.UseStamina(0.2f);
            
            // Multiply by a fraction of the high jump multiplier so the lung stays proportional to the jump height
            __instance.playerSlide += __instance.character.input.movementInput.normalized * (8f *
                (cac.highJumpMultiplier / 4f));
            if (__instance.view.IsMine && !__instance.character.isBot)
            {
                GamefeelHandler.instance.AddPerlinShake(10f, 0.5f, 10f);
                GUIManager.instance.ClimbJump();
            }
        }
    }
}