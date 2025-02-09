using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public partial class ResourceManager : Singleton<ResourceManager>
{
    public static event Action<int> CoinsChanged; 
    
    public static int Coins
    {
        get => PrefManager.GetInt(nameof(Coins));
        set
        {
            if(value == Coins)
                return;

            PrefManager.SetInt(nameof(Coins), value);
            CoinsChanged?.Invoke(value);
        }
    }
}
