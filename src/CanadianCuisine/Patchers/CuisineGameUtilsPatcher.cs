using HarmonyLib;
using Photon.Pun;
using UnityEngine;

namespace CanadianCuisine.Patchers;

public class CuisineGameUtilsPatcher
{
    [HarmonyPatch(typeof(GameUtils), nameof(GameUtils.InstantiateAndGrabRPC))]
    [HarmonyPrefix]
    public static void InstGPrefix(
        GameUtils __instance,
        ref bool __runOriginal,
        string itemPrefabName,
        PhotonView characterView,
        int cookedAmount)
    {
        if (cookedAmount != 0 && PhotonNetwork.IsMasterClient && characterView.TryGetComponent<Character>(out var character))
        {
            __runOriginal = false;
            
            character.refs.items.lastEquippedSlotTime = 0.0f;
            var bodypart = character.GetBodypart(BodypartType.Hip);
                
            var it = PhotonNetwork
                .InstantiateItemRoom(itemPrefabName,
                    bodypart.transform.position + bodypart.transform.forward * 0.5f,
                    Quaternion.identity).GetComponent<Item>();
                
            it.photonView.RPC(nameof(Item.SetCookedAmountRPC), RpcTarget.All, cookedAmount);
            it.Interact(character);
        }
    }
}