using System;
using CanadianCuisine.controllers;
using HarmonyLib;
using pworld.Scripts.Extensions;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CanadianCuisine;

public class CuisineCharacterPatcher
{
    [HarmonyPatch(typeof(CharacterAfflictions), "Awake")]
    [HarmonyPostfix]
    public static void AfflictionAwakePostfix(CharacterAfflictions __instance)
    {
        var affliction = __instance.character.gameObject.GetOrAddComponent<CuisineAfflictionCharacter>();
        
        if (affliction == null)
        {
            Plugin.Log.LogError("Can't add custom affliction manager");
        }
    }
        
    [HarmonyPatch(typeof(GUIManager), "Awake")]
    [HarmonyPostfix]
    public static void GUIAwakePostfix(GUIManager __instance)
    {
        var man = __instance.gameObject.GetOrAddComponent<CuisineGUIEffectManager>();

        var effectTrs = __instance.poisonSVFX.transform;
        
        if (man == null)
        {
            Plugin.Log.LogError("Can't add custom GUI manager");
        }
        else
        {
            var effects = Object.Instantiate(Plugin.HighJumpEffectPrefab, effectTrs.parent);
            effects.transform.transform.localPosition = effectTrs.localPosition;
            effects.transform.transform.localRotation = effectTrs.localRotation;
            effects.transform.transform.localScale = effectTrs.localScale;
            man.highJumpSVFX = effects.GetComponent<ScreenVFX>();
            effects.SetActive(false);
        }
    }
    
}