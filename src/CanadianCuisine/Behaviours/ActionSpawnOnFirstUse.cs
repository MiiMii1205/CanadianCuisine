namespace CanadianCuisine.Behaviours;

public class ActionSpawnOnFirstUse : Action_Spawn
{
    public override void RunAction()
    {
        if (!item.data.TryGetDataEntry<OptionableIntItemData>(DataEntryKey.ItemUses, out var uses) || uses.Value == (item.totalUses - 1))
        {
            base.RunAction();
        }
    }
}