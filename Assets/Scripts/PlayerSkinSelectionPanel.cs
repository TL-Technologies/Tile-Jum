using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Game;
using UnityEngine;
using UnityEngine.UI;

public class PlayerSkinSelectionPanel : ShowHidable
{
    [SerializeField] private SkinTileUI _skinTileUIPrefab;
    [SerializeField] private RectTransform _content;

    private readonly List<SkinTileUI> _tiles = new List<SkinTileUI>();
    private SkinTileUI _selectedTile;

    public SkinTileUI SelectedTile
    {
        get { return _selectedTile; }
        set
        {
            if (_selectedTile != null)
            {
                _selectedTile.Selected = false;
            }

            _selectedTile = value;
            _selectedTile.Selected = true;
            ResourceManager.SetSelectedSkin(_selectedTile.MViewModel.Skin.id);
        }
    }

    private void Awake()
    {
        foreach (var playerSkin in ResourceManager.PlayerSkins)
        {
            var skinTileUI = Instantiate(_skinTileUIPrefab, _content);
            skinTileUI.MViewModel = new SkinTileUI.ViewModel
            {
                Skin = playerSkin,
                
            };
            skinTileUI.Clicked += SkinTileUIOnClicked;
            _tiles.Add(skinTileUI);
        }

        var skin = ResourceManager.GetSelectedSkin();
        SelectedTile = _tiles.First(ui => ui.MViewModel.Skin.id == skin);
    }

    private void Start()
    {
        var gridLayout = _content.GetComponent<GridLayoutGroup>();
        if (gridLayout != null)
        {
            if (gridLayout.constraint != GridLayoutGroup.Constraint.FixedColumnCount)
                return;
            var rectTransform = (RectTransform) _content.transform;
            var sizeX = rectTransform.rect.size.x;
            var totalSize = gridLayout.cellSize.x * gridLayout.constraintCount + gridLayout.padding.left + gridLayout.padding.right;
            var calSpace = (sizeX - totalSize)/(gridLayout.constraintCount+1);
            if(gridLayout.spacing.x*2 < calSpace )
                gridLayout.spacing = new Vector2(calSpace*0.8f,gridLayout.spacing.y);
        }
    }

    // ReSharper disable once MethodTooLong
    private void SkinTileUIOnClicked(SkinTileUI tileUI)
    {
        if (ResourceManager.IsSkinLocked(tileUI.MViewModel.Skin.id))
        {
            var playerSkin = tileUI.MViewModel.Skin;
            if (playerSkin.lockDetails.value <= ResourceManager.Coins)
            {
                ResourceManager.Coins -= playerSkin.lockDetails.value;
                ResourceManager.SetSkinLock(playerSkin.id);
                tileUI.MViewModel = tileUI.MViewModel;
            }
            return;
        }

        SelectedTile = tileUI;
    }

    public void OnClickBack()
    {
        Hide();
    }
}