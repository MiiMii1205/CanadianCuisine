using CanadianCuisine.controllers;
using System;
using CanadianCuisine.data;
using MonoDetour;
using MonoDetour.HookGen;
using Md.Peak.Afflictions.Affliction;
using Peak.Afflictions;
using UnityEngine;

namespace CanadianCuisine.hooks;

[MonoDetourTargets(typeof(Affliction))]
internal static class AfflictionsHooks
{
    [MonoDetourHookInitialize]
    static void Init()
    {
        CreateBlankAffliction.Postfix(Postfix_BlankAffliction);
    }

    private static void Postfix_BlankAffliction(ref Affliction.AfflictionType afflictionType,
        ref Affliction returnValue)
    {
        if (returnValue == null)
        {
            var defs = CuisineAfflictionManager.StatusByType(afflictionType);

            Plugin.Log.LogInfo($"Found affliction type {defs.Name}");
            
            if (defs.Name == CuisineAfflictionValues.HIGH_JUMP_NAME)
            {
                returnValue = new AfflictionHighJump();
            }            
            else if (defs.Name == CuisineAfflictionValues.WITH_CONSEQUENCE)
            {
                returnValue = new AfflictionWithConsequence();
            }           
            else if (defs.Name == CuisineAfflictionValues.PARALYZED)
            {
                returnValue = new AfflictionWithConsequence();
            }
            else
            {
                returnValue = returnValue;
            }
        }
    }
}