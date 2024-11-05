using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LootTier", menuName = "Loot System/LootTier", order = int.MaxValue)]
public class LootTiers : GameSettings
{
    #region Singleton Access

    private static LootTiers _instance;

    public static bool Initialized { get { return _instance != null; } }

    public static void Init()
    {
        _instance = GameSettings.GetGameSettings<LootTiers>();
    }
  
    public static LootTiers Settings
    {
        get
        {
            return _instance;
        }
    }

    #endregion
  
    #region Fields
    
    public Dictionary<string, float> Tiers = new();
  
    #endregion
  
    #region CONSTRUCTOR
  
    protected override void OnInitialized()
    {
        //do any initializing you may want for the game
    }
  
    #endregion
}
