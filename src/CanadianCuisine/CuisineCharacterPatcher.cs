using System;
using CanadianCuisine.controllers;
using HarmonyLib;
using pworld.Scripts.Extensions;

namespace CanadianCuisine;

public class CuisineCharacterPatcher
{
    [HarmonyPatch(typeof(CharacterAfflictions), "Awake")]
    [HarmonyPostfix]
    public static void AfflictionAwakePostfix(CharacterAfflictions __instance)
    {
        var affliction = __instance.character.gameObject.GetOrAddComponent<CuisineAfflictionCharacter>();
        
        if (affliction != null)
        {
            Plugin.Log.LogError("Can't add custom affliction manager");
        }
    }
    
}