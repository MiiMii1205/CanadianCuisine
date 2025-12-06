using System;
using System.Collections.Generic;
using System.IO;
using BepInEx;
using BepInEx.Logging;
using CanadianCuisine.controllers;
using CanadianCuisine.data;
using HarmonyLib;
using MonoDetour;
using MonoDetour.HookGen;
using Peak.Afflictions;
using PEAKLib.Core;
using PEAKLib.Items;
using PEAKLib.Items.UnityEditor;
using pworld.Scripts.Extensions;
using UnityEngine;

namespace CanadianCuisine;

[BepInAutoPlugin]
[BepInDependency(ItemsPlugin.Id)]
[BepInDependency(CorePlugin.Id)]
public partial class Plugin : BaseUnityPlugin
{
    internal static ManualLogSource Log { get; private set; } = null!;

    private void FixColorblindMaterials(GameObject go)
    {
        var col = go.GetComponentInChildren<ColorblindVariant>();

        var probableShader = Shader.Find(col.colorblindMaterial.shader.name);


        if (probableShader == null)
        {
            Log.LogWarning(
                $": Shader {col.colorblindMaterial.shader.name} was not found."
            );
        }
        else
        {
            col.colorblindMaterial.shader = probableShader;
        }
    }

    private void OnDestroy()
    {
        DefaultMonoDetourManager.Instance.Dispose();

        Harmony.UnpatchID(Id);

        Log.LogInfo($"Plugin {Name} unloaded!");
    }

    private void Awake()
    {
        Log = Logger;

        AddLocalizedTextCsv();

        Log.LogInfo($"Plugin {Name} is loading...");

        this.LoadBundleWithName("canadiansnacks.peakbundle", peakBundle =>
        {
            var prefab = peakBundle.LoadAsset<GameObject>("HostCuttings.prefab");

            var itCook = prefab.AddComponent<ItemCooking>();

            itCook.ignoreDefaultCookBehavior = true;

            itCook.additionalCookingBehaviors =
            [
                new CookingBehaviorChangeFeedbackSfx()
                {
                    cookedAmountToTrigger = 1,
                    soundEffectNameToChangeTo = "SFXI Heal Hunger Stamina"
                },
                new CookingBehavior_EnableScripts()
                {
                    cookedAmountToTrigger = 1,
                    scriptsToEnable = [prefab.GetComponent<Action_GiveExtraStamina>()]
                },
                new CookingBehaviorChangeFeedbackSfx()
                {
                    cookedAmountToTrigger = 3,
                    soundEffectNameToChangeTo = "SFXI Heal Fortified Milk"
                },
                new CookingBehavior_EnableScripts()
                {
                    cookedAmountToTrigger = 3,
                    scriptsToEnable = [prefab.GetComponent<Action_ApplyAffliction>()]
                },
                new CookingBehavior_EnableScripts()
                {
                    cookedAmountToTrigger = 3,
                    scriptsToEnable = [prefab.GetComponent<Action_GiveExtraStamina>()]
                },
                new CookingBehaviorChangeFeedbackSfx()
                {
                    cookedAmountToTrigger = 4,
                    soundEffectNameToChangeTo = "SFXI Heal Hunger Normal"
                },
                new CookingBehavior_DisableScripts()
                {
                    cookedAmountToTrigger = 4,
                    scriptsToDisable =
                    [
                        prefab.GetComponent<Action_GiveExtraStamina>(),
                        prefab.GetComponent<Action_ApplyAffliction>()
                    ]
                }
            ];

            var papinoCookie = peakBundle.LoadAsset<GameObject>("PapinoCookies.prefab");

            var afflic = papinoCookie.AddComponent<Action_ApplyAffliction>();

            afflic.OnCastFinished = true;

            var mainEffect = new AfflictionWithConsequence()
            {
                mainAffliction = new Affliction_LowGravity()
                {
                    totalTime = 12,
                    warning = true,
                    lowGravAmount = 3
                },
                delay = 1,
                consequentAffliction = new AfflictionParalyzed()
                {
                    totalTime = 4f
                }
            };

            afflic.affliction = mainEffect;

            var papCook = papinoCookie.GetOrAddComponent<ItemCooking>();

            papCook.additionalCookingBehaviors = papCook.additionalCookingBehaviors.AddToArray(
                new CookingBehavior_DisableScripts()
                {
                    scriptsToDisable = [afflic],
                    cookedAmountToTrigger = 1
                });


            /*

            var toffee = peakBundle.LoadAsset<GameObject>("MapleToffee.prefab");

            toffee.transform.GetChild(3).GetChild(0).GetChild(0).gameObject.AddComponent<MapleToffeeVariantController>();*/

            peakBundle.Mod.RegisterContent();
        });

        Log.LogInfo("Snacks items are loaded!");

        this.LoadBundleWithName("canadianfruits.peakbundle", peakBundle =>
        {
            // Fix bubberries leaf shaders. GD/FoliageGD is left behind by PEAKLib, so we'll do it manually for now

            var leafShade = Shader.Find("GD/FoliageGD");

            if (leafShade == null)
            {
                Log.LogWarning(
                    $": Shader GD/FoliageGD was not found."
                );
            }
            else
            {
                var allBubP = new List<GameObject>
                {
                    peakBundle.LoadAsset<GameObject>("Bub Berry Blue.prefab"),
                    peakBundle.LoadAsset<GameObject>("Bub Berry Purple.prefab"),
                    peakBundle.LoadAsset<GameObject>("Bub Berry White.prefab")
                };

                foreach (var bub in allBubP)
                {
                    // Also fix colorblinds material while we're at it...
                    FixColorblindMaterials(bub);

                    foreach (Renderer renderer in bub.GetComponentsInChildren<Renderer>())
                    {
                        foreach (Material mat in renderer.sharedMaterials)
                        {
                            if (mat.shader.name != leafShade.name)
                            {
                                continue;
                            }

                            // Replace dummy shader
                            mat.shader = leafShade;

                            mat.EnableKeyword("_ALPHATEST_ON");
                            mat.EnableKeyword("_TRIPLANAR1_TOPDOWN");
                            mat.EnableKeyword("_TRIPLANAR2_TRIPLANAR");
                            mat.EnableKeyword("_TRIPLANAR3_UV");
                            mat.EnableKeyword("_USESIMPLEMASK_ON");
                        }

                        foreach (Material mat in renderer.materials)
                        {
                            if (mat.shader.name != leafShade.name)
                            {
                                continue;
                            }

                            // Replace dummy shader
                            mat.shader = leafShade;

                            mat.EnableKeyword("_ALPHATEST_ON");
                            mat.EnableKeyword("_TRIPLANAR1_TOPDOWN");
                            mat.EnableKeyword("_TRIPLANAR2_TRIPLANAR");
                            mat.EnableKeyword("_TRIPLANAR3_UV");
                            mat.EnableKeyword("_USESIMPLEMASK_ON");
                        }
                    }
                }
            }

            peakBundle.Mod.RegisterContent();
        });
        Log.LogInfo("Fruits items are loaded!");

        LoadAllCustomAfflictions();


        MonoDetourManager.InvokeHookInitializers(typeof(Plugin).Assembly);
        new Harmony(Id).PatchAll(typeof(CuisineCharacterPatcher));

        Log.LogInfo($"Plugin {Name} is loaded!");
    }

    private void LoadAllCustomAfflictions()
    {
        CuisineAfflictionManager.RegisterAfflictions(new AfflictionDefinition()
        {
            Name = CuisineAfflictionValues.HIGH_JUMP_NAME
        }, ModDefinition.GetOrCreate(Info));

        CuisineAfflictionManager.RegisterAfflictions(new AfflictionDefinition()
        {
            Name = CuisineAfflictionValues.WITH_CONSEQUENCE
        }, ModDefinition.GetOrCreate(Info));

        CuisineAfflictionManager.RegisterAfflictions(new AfflictionDefinition()
        {
            Name = CuisineAfflictionValues.PARALYZED
        }, ModDefinition.GetOrCreate(Info));
    }

    private void AddLocalizedTextCsv()
    {
        using var reader = new StreamReader(Path.Join(Path.GetDirectoryName(Info.Location),
            "CanadianCuisineLocalizedText.csv"));

        var currentLine = 0;

        while (!reader.EndOfStream)
        {
            var line = reader.ReadLine();

            if (line == null)
            {
                break;
            }

            currentLine++;

            List<string> valList = new List<string>(line.Split(','));

            var locName = valList.Deque();

            var endline = valList.Pop();

            if (endline != "ENDLINE")
            {
                Log.LogError($"Invalid localization at line {currentLine}");
            }

            if (locName != "CURRENT_LANGUAGE")
            {
                LocalizedText.mainTable[locName] = valList;
                Log.LogDebug($"Added localization of {locName}");
            }
        }

        Log.LogDebug($"Added {currentLine - 1} localizations");
    }
}