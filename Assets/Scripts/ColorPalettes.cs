// /*
// Created by Darsan
// */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorPalettes : ScriptableObject,IEnumerable<ColorPalette>
{
    public const string DEFAULT_NAME = nameof(ColorPalettes);
    public static ColorPalettes Default => Resources.Load<ColorPalettes>(nameof(ColorPalettes));

    [SerializeField]private List<ColorPalette> _colorPalettes = new List<ColorPalette>();
    private ColorPalette? _currentColorPalette;

    public ColorPalette CurrentColorPalette
    {
        get => (ColorPalette) (_currentColorPalette ?? (_currentColorPalette = _colorPalettes.GetRandom()));
        set => _currentColorPalette = value;
    }

    public IEnumerator<ColorPalette> GetEnumerator()
    {
        return _colorPalettes.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void RandColorPalette()
    {
        CurrentColorPalette = _colorPalettes.GetRandom();
    }

#if UNITY_EDITOR
    [UnityEditor.MenuItem("MyGames/Color Palettes")]
    public static void OpenPlayerSkin()
    {
        GamePlayEditorManager.OpenScriptableAtDefault<ColorPalettes>();
    }
#endif
}

[Serializable]
public struct ColorPalette
{
    [SerializeField]private List<Color> _colors;

    public Color GetColor(ColorType color)
    {
        if (_colors==null)
        {
            return Color.white;
        }

        var index = (int)color;

        return _colors.Count < index + 1 ? Color.white : _colors[index];
    }
}