using System;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class CoinsText:MonoBehaviour
{

    [SerializeField] private int _maxInstanceUpdateCoinsChange = 1;
    [SerializeField] private float _coinChangeTime = 0.3f;

    private Text _text;
    private float _coins;
    private float _coinsChangeSpeed;

    private float Coins
    {
        get => _coins;
        set
        {
            _text.text = Mathf.RoundToInt(value).ToString();
            _coins = value;
        }
    }

    private void Awake()
    {
        _text = GetComponent<Text>();
    }

    private void OnEnable()
    {
        //_text = GetComponent<Text>();
        Coins = ResourceManager.Coins;
        ResourceManager.CoinsChanged += ResourceManagerOnCoinChanged;
    }

    private void ResourceManagerOnCoinChanged(int i)
    {
        var change = Mathf.Abs(ResourceManager.Coins - Coins);
        if (change <= _maxInstanceUpdateCoinsChange)
        {
            _coinsChangeSpeed = 100000;
        }
        else
        {
            _coinsChangeSpeed = change/_coinChangeTime;
        }
    }

    private void OnDestroy()
    {
        ResourceManager.CoinsChanged -= ResourceManagerOnCoinChanged;
    }

    private void Update()
    {
        if (ResourceManager.Coins != Coins)
        {
            Coins = Mathf.MoveTowards(Coins, ResourceManager.Coins, _coinsChangeSpeed * Time.deltaTime);
        }
    }
}