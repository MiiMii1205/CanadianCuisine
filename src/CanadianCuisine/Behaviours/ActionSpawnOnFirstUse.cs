namespace CanadianCuisine.Behaviours;

public class ActionSpawnOnFirstUse : Action_Spawn
{
    public override void RunAction()
    {
        var uses = item.GetData<OptionableIntItemData>(DataEntryKey.ItemUses);

        if (!uses.HasData || uses.Value == (item.totalUses - 1))
        {
            base.RunAction();
        }
    }
}