using CanadianCuisine.controllers;
using System;
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

    private static void Postfix_BlankAffliction(ref Affliction.AfflictionType afflictionType, ref Affliction returnValue)
    {
        if (returnValue == null)
        {
            var defs = CuisineAfflictionManager.StatusByType(afflictionType);

            Plugin.Log.LogInfo($"Found affliction type {defs.Name}");

            returnValue = defs.Name switch
            {
                "HighJump" => new AfflictionHighJump(),
                _ => returnValue
            };
        }
    }

}