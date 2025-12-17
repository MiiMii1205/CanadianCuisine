using System;
using System.Collections.Generic;
using System.Linq;
using PEAKLib.Core;
using static Peak.Afflictions.Affliction;

namespace CanadianCuisine.Data;

public static class CuisineAfflictionManager
{
    private static SortedList<string, AfflictionDefinition> registered = new();

    public static IList<AfflictionDefinition> Afflictions => registered.Values;

    public static int length => registered.Count;
    
    internal static void RegisterAfflictions(AfflictionDefinition affliction, ModDefinition owner)
    {
        var key = $"{owner.Id}->{affliction.Name}";
        
        if (!registered.TryAdd(key, affliction))
        {
            throw new ArgumentException(
                $"Affliction with name {key} already registered. Choose a unique name."
            );
        }

        ReIndex();
    }

    internal static AfflictionDefinition StatusByType(AfflictionType type)
    {
        return registered.FirstOrDefault(x => x.Value.Type == type).Value;
    }

    internal static AfflictionType TypeByName(string name)
    {
        return registered.FirstOrDefault(x => x.Value.Name == name).Value.Type;
    }

    private static void ReIndex()
    {
        var vanillaStatuses = Enum.GetValues(typeof(AfflictionType)).OfType<AfflictionType>().ToList();
        List<AfflictionDefinition> reg = registered.Values.ToList();
        for (int i = 0, j = 0; i < vanillaStatuses.Count + reg.Count && j < reg.Count; i++)
        {
            // place new status effects into un-defined vanilla enum values
            if (!Enum.IsDefined(typeof(AfflictionType), i))
            {
                reg[j].index = i;
                j++;
            }
        }
    }
    
}