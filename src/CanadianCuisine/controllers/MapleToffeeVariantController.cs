using UnityEngine;
using Random = UnityEngine.Random;

namespace CanadianCuisine.controllers;

public class MapleToffeeVariantController: MonoBehaviour
{
    private void Start()
    {
        var t = transform;
        var choosenVariantIndex = Random.Range(0, t.childCount);

        var r = GetComponentsInChildren<MeshRenderer>();
        
        foreach (var meshRenderer in r)
        {
            meshRenderer.gameObject.SetActive(false);
        }
        
        t.GetChild(choosenVariantIndex).gameObject.SetActive(true);

    }
}