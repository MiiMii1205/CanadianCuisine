using UnityEngine;

namespace CanadianCuisine.Behaviours;

public class CuisineGUIManager: MonoBehaviour
{
    public ScreenVFX highJumpSvfx = null!;
    
    public static CuisineGUIManager instance = null!;

    private void Awake()
    {
        instance = this;
    }

    public void StartHighJump()
    {
        highJumpSvfx.StartFX(0.15f);
    }

    public void EndHighJump()
    {
        highJumpSvfx.EndFX();
    }

}