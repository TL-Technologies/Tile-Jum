﻿using System;
using UnityEngine;
using UnityEngine.UI;

public class SkinTileUI : MonoBehaviour
{
    public event Action<SkinTileUI> Clicked; 

    [SerializeField] private GameObject _selectedEffectGO;
    [SerializeField] private GameObject _lockedEffectGO;
    [SerializeField] private Image _img;
    [SerializeField] private GameObject _priceGroup;

    private ViewModel _mViewModel;
    

    

    public bool Selected
    {
        get => _selectedEffectGO.activeSelf;
        set => _selectedEffectGO.SetActive(value);
    }


    public void OnClicked()
    {
        Clicked?.Invoke(this);
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public struct ViewModel
    {
        public PlayerSkin Skin{ get; set; }
    }
}


[System.Serializable]
public struct PlayerSkin
{
    public string id;
    public Sprite icon;
    public bool preLocked;
    public LockDetails lockDetails;
    public GameObject prefab;
}


[System.Serializable]
public struct LockDetails
{
    public Type type;
    public int value;

    public enum Type
    {
        Coins
    }
}