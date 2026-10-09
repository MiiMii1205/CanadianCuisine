using System.Collections;
using UnityEngine;

namespace CanadianCuisine.Behaviours;

public class ActionSpawnAndGrab : ItemAction
{
    public Item itemToSpawn;

    public override void RunAction()
    {
        if (character)
        {
            character.StartCoroutine(SpawnItemDelayed(
                item.GetData<FloatItemData>( DataEntryKey.UseRemainingPercentage ).Value <= 0,
                item.GetData<IntItemData>( DataEntryKey.CookedAmount )?.Value ?? 0));
        }
    }

    private IEnumerator SpawnItemDelayed(bool shouldBeConsumed, int cookedAmount = 0)
    {
        var c = character;
        var it = itemToSpawn;

        if (shouldBeConsumed)
        {
            float timeout = 2f;
            while (this != null)
            {
                timeout -= Time.deltaTime;
                if (timeout <= 0f)
                {
                    yield break;
                }

                yield return null;
            }
        }
        else
        {
            yield return new WaitForEndOfFrame();
        }

        GameUtils.instance.InstantiateAndGrab(it, c, cookedAmount);
    }
}