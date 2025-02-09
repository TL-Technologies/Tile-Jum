using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public partial class ResourceManager : Singleton<ResourceManager>
{
    public static event Action<int> CoinsChanged; 
#if IN_APP

    public static string NO_ADS_PRODUCT_ID => GameSettings.Default.InAppSetting.removeAdsId;

    public static event Action<string> ProductPurchased;
    public static event Action<bool> ProductRestored;
#endif

    public static bool EnableAds
    {
        get => PrefManager.GetBool(nameof(EnableAds), true);
        set => PrefManager.SetBool(nameof(EnableAds), value);
    }

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
#if IN_APP

    public static bool AbleToRestore => EnableAds;

    public Purchaser Purchaser { get; private set; }

    protected override void OnInit()
    {
        base.OnInit();
        Purchaser = new Purchaser(new List<string>(), new[] { NO_ADS_PRODUCT_ID });
        Purchaser.RestorePurchased += PurchaserOnRestorePurchased;
    }

    private void PurchaserOnRestorePurchased(bool success)
    {
        if (EnableAds && Purchaser.ItemAlreadyPurchased(NO_ADS_PRODUCT_ID))
        {
            EnableAds = false;
            ProductPurchased?.Invoke(NO_ADS_PRODUCT_ID);
        }
        ProductRestored?.Invoke(success);
    }


    public static void RestorePurchase()
    {
        Instance.Purchaser.Restore();
    }

    public static void PurchaseNoAds(Action<bool> completed = null)
    {
        if (!EnableAds)
        {
            return;
        }

        Instance.Purchaser.BuyProduct(NO_ADS_PRODUCT_ID, success =>
        {
            if (success)
            {
                EnableAds = false;
            }
            completed?.Invoke(success);
            if (success)
                ProductPurchased?.Invoke(NO_ADS_PRODUCT_ID);
        });
    }
#endif
}


//Skin Related
public partial class ResourceManager
{
    public static event Action<PlayerSkin> PlayerSkinSelectionChanged;
    public const string SKIN_LOCK_PREFIX = "skin_locked";
    private const string SELECTED_SKIN_KEY = "SelectedSkin";

    [SerializeField] private PlayerSkins _playerSkins;

    public static PlayerSkins PlayerSkins => Instance != null? Instance._playerSkins : Resources.Load<PlayerSkins>(nameof(PlayerSkins));
    

    public static PlayerSkin GetSkinById(string id) => PlayerSkins.FirstOrDefault(skin => skin.id == id);

    public static void SetSelectedSkin(string id)
    {
        PrefManager.SetString(SELECTED_SKIN_KEY, id);
        PlayerSkinSelectionChanged?.Invoke(GetSkinById(id));
    }

    public static string GetSelectedSkin()
    {
        return PrefManager.GetString(SELECTED_SKIN_KEY, PlayerSkins.FirstOrDefault().id);
    }

    public static bool IsSkinLocked(string skinId)
    {
        var skin = GetSkinById(skinId);
        if (!skin.preLocked)
        {
            return false;
        }

        return PrefManager.GetBool($"SkinLock_{skinId}", true);
    }

    public static void SetSkinLock(string skinId, bool locked = false)
    {
        PrefManager.SetBool($"SkinLock_{skinId}", false);
    }

}
