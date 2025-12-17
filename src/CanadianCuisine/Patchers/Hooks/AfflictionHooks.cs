using CanadianCuisine.Behaviours.Afflictions;
using CanadianCuisine.Data;
using Md.Peak.Afflictions.Affliction;
using MonoDetour;
using MonoDetour.HookGen;
using Peak.Afflictions;

namespace CanadianCuisine.Patchers.Hooks;

[MonoDetourTargets(typeof(Affliction))]
internal static class AfflictionHooks
{
    [MonoDetourHookInitialize]
    static void Init()
    {
        CreateBlankAffliction.Postfix(Postfix_BlankAffliction);
    }

    private static void Postfix_BlankAffliction(ref Affliction.AfflictionType afflictionType,
        ref Affliction returnValue)
    {
        if (((Affliction?) returnValue) == null)
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
                returnValue = new AfflictionParalyzed();
            }
        }
    }
}