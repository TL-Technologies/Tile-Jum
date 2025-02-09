using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class RewardCoinsButton : MonoBehaviour
{

    [SerializeField] private int _rewardCoins;
    [SerializeField] private Text _coinsTxt;
    private Button _button;

    private void Awake()
    {
        _coinsTxt.text = $"+{_rewardCoins}";
        _button = GetComponent<Button>();

        _button.onClick.AddListener(OnClickButton);
    }

    private void OnClickButton()
    {
        if (!AdsManager.IsVideoAvailable())
        {
            return;
        }

        AdsManager.ShowVideoAds(true, success =>
        {
            if(!success)
                return;
            ResourceManager.Coins += _rewardCoins;
        });
    }

    private void Update()
    {
        _button.interactable = AdsManager.IsVideoAvailable();
    }


}
