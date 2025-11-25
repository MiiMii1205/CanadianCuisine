using System.Collections.Generic;
using System.IO;
using BepInEx;
using BepInEx.Logging;
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

    private void Awake()
    {
        Log = Logger;
        
        AddLocalizedTextCsv();
        
        Log.LogInfo($"Plugin {Name} is loading...");
        
        this.LoadBundleAndContentsWithName("canadiansnacks.peakbundle");
        Log.LogInfo("Snacks items are loaded!");
        
        this.LoadBundleAndContentsWithName("canadianfruits.peakbundle");
        Log.LogInfo("Fruits items are loaded!");
        
        Log.LogInfo($"Plugin {Name} is loaded!");
        

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