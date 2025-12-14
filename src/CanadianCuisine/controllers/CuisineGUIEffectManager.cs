using System;
using UnityEngine;

namespace CanadianCuisine.controllers;

public class CuisineGUIEffectManager: MonoBehaviour
{
    public ScreenVFX highJumpSVFX;
    
    public static CuisineGUIEffectManager instance;

    private void Awake()
    {
        instance = this;
    }

    public void StartHighJump()
    {
        highJumpSVFX.StartFX(0.15f);
    }

    public void EndHighJump()
    {
        highJumpSVFX.EndFX();
    }

}