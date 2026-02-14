using CanadianCuisine.Behaviours;
using Md.ItemCooking;
using MonoDetour;
using MonoDetour.HookGen;

namespace CanadianCuisine.Patchers.Hooks;

[MonoDetourTargets(typeof(ItemCooking))]
public class CookingHooks
{
    [MonoDetourHookInitialize]
    static void Init()
    {
        UpdateCookedBehavior.Prefix(Prefix_UpdateCookedBehavior);
    }

    private static void Prefix_UpdateCookedBehavior(ItemCooking self)
    {
        if (self.item.TryGetComponent<CookingHungerController>(out var cookH))
        {
            IntItemData data = self.item.GetData<IntItemData>(DataEntryKey.CookedAmount);

            if (data.Value == 0)
            {
                data.Value += self.preCooked;
            }

            var cookedTimesDelta = data.Value - self.timesCookedLocal;
            
            if (cookedTimesDelta > 0)
            {
                for (int j = 1 + self.timesCookedLocal; j <= data.Value; j++)
                {
                    cookH.UpdateCookingHunger(j);
                }
            }
        }
    }
}