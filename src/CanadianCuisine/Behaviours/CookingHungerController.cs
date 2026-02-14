using System;
using PEAKLib.Core;
using UnityEngine;

namespace CanadianCuisine.Behaviours;

public class CookingHungerController: MonoBehaviour
{
    public Item item = null!;
    public Action_RestoreHunger restoreHunger = null!;
    private void Awake()
    {
        item = ThrowHelper.ThrowIfArgumentNull(GetComponent<Item>());
        restoreHunger = ThrowHelper.ThrowIfArgumentNull(GetComponent<Action_RestoreHunger>());
    }

    public void UpdateCookingHunger(int totalCooked)
    {
        if (restoreHunger)
        {
            if (totalCooked < 2)
            {
                restoreHunger.restorationAmount *= 2f;
            } else if (totalCooked > 2)
            {
                restoreHunger.restorationAmount = Mathf.Max(restoreHunger.restorationAmount - 0.05f, 0f);
            }
        }
        
    }
}