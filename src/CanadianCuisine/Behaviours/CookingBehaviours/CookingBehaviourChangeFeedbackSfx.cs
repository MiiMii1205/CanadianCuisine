namespace CanadianCuisine.Behaviours.CookingBehaviours;

public class CookingBehaviourChangeFeedbackSfx : AdditionalCookingBehavior
{
    public string soundEffectNameToChangeTo = "";

    public CookingBehaviourChangeFeedbackSfx()
    {
        if (soundEffectNameToChangeTo == "")
        {
            Plugin.Log.LogWarning("Cooking Behaviour will try to change to an empty sound effect name.");
        }
    }

    public override void TriggerBehaviour(int cookedAmount)
    {
        var uses = itemCooking.item.gameObject.GetComponents<ItemUseFeedback>();

        ItemUseFeedback? mainFeedback = itemCooking.item._useFeedback;
        ItemUseFeedback? nextFeedback = null;

        foreach (var itemUseFeedback in uses)
        {
            if (itemUseFeedback != mainFeedback && itemUseFeedback.sfxUsed.name == soundEffectNameToChangeTo)
            {
                nextFeedback = itemUseFeedback;
            }
        }

        if (nextFeedback != null && mainFeedback != null)
        {
            mainFeedback.sfxUsed = nextFeedback.sfxUsed;
            Plugin.Log.LogDebug($"Changed Feedback SFX to \"{soundEffectNameToChangeTo}\"");
        }
    }
}