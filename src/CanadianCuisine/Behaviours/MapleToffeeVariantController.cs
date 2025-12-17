using UnityEngine;
using Random = UnityEngine.Random;

namespace CanadianCuisine.Behaviours;

public class MapleToffeeVariantController: MonoBehaviour
{
    private void Start()
    {
        var t = transform;
        var chosenVariantIndex = Random.Range(0, t.childCount);

        var r = GetComponentsInChildren<MeshRenderer>();
        
        foreach (var meshRenderer in r)
        {
            meshRenderer.gameObject.SetActive(false);
        }
        
        t.GetChild(chosenVariantIndex).gameObject.SetActive(true);

    }
}