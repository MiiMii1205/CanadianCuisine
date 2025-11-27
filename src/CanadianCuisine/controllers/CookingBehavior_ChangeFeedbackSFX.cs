namespace CanadianCuisine.controllers;

public class CookingBehavior_ChangeFeedbackSFX : AdditionalCookingBehavior
{
    public override void TriggerBehaviour(int cookedAmount)
    {
        Plugin.Log.LogInfo("WEO");
  
        var uses = itemCooking.item.gameObject.GetComponents<ItemUseFeedback>();
        
        ItemUseFeedback? mainFeedback = itemCooking.item._useFeedback;
        ItemUseFeedback? nextFeedback = null;

        foreach (var itemUseFeedback in uses)
        {
            if (itemUseFeedback != mainFeedback)
            {
                nextFeedback = itemUseFeedback;
            }
        }

        if (nextFeedback != null && mainFeedback != null)
        {
            mainFeedback.sfxUsed = nextFeedback.sfxUsed;
            
        }
        
    }
}