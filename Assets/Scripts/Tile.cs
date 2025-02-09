// /*
// Created by Darsan
// */

using System;
using System.Collections;
using UnityEngine;

public class Tile : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _renderer;
    [SerializeField] private float _perfectRadius;
    [SerializeField] private SpriteRenderer _perfectSpRender;
    [SerializeField] private GameObject _hitEffect;
    [SerializeField] private GameObject _perfectEffect;
    [SerializeField] private Color _hitColor;

    private ColorType _color;

    public ColorType Color
    {
        get => _color;
        set
        {
            _color = value;
            _renderer.material.color = value.ToColor();
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position,_perfectRadius);
    }

    public bool IsPerfect(Vector2 vec)
    {
        return (vec.ToXZtoXYZ(transform.position.y) - transform.position).magnitude < _perfectRadius;
    }


    public void Hit(Vector3 hitPoint)
    {
        if (IsPerfect(hitPoint.ToXZ()))
        {
            StartCoroutine(PerfectEffect());
        }

        StartCoroutine(HitEffect());
    }

    private IEnumerator PerfectEffect()
    {
        Instantiate(_perfectEffect, transform.position, Quaternion.identity);
        Debug.Log(nameof(PerfectEffect));
        var startScale = _perfectSpRender.transform.localScale;
        var endScale = _perfectSpRender.transform.localScale * 1.5f;
        yield return SimpleCoroutine.MoveTowardsEnumerator(onCallOnFrame: n =>
        {
            _perfectSpRender.transform.localScale = Vector3.Lerp(startScale, endScale, n);
            _perfectSpRender.color = new Color(_perfectSpRender.color.r, _perfectSpRender.color.g,
                _perfectSpRender.color.b, 1 - n);
        }, speed: 2);
    }

    private IEnumerator HitEffect()
    {
        Instantiate(_hitEffect, transform.position, Quaternion.identity);

//        var startScale = _renderer.transform.localScale;
//        var endScale = _renderer.transform.localScale * 1.5f;

        var startColor = _renderer.color;
        yield return SimpleCoroutine.MoveTowardsEnumerator(onCallOnFrame: n =>
        {
//            _renderer.transform.localScale = Vector3.Lerp(startScale, endScale, n);
            _renderer.color = n<=0.5? UnityEngine.Color.Lerp(startColor,_hitColor,n/0.5f): UnityEngine.Color.Lerp(_hitColor, startColor, (n-0.5f) / 0.5f);
        }, speed: 2);
    }
}

public static class ColorTypeExtensions
{
    public static Color ToColor(this ColorType color)
    {
        return ColorPalettes.Default.CurrentColorPalette.GetColor(color);
//        switch (color)
//        {
//            case ColorType.Color1:
//                return Color.green;
//            case ColorType.Color2:
//                return Color.magenta;
//            case ColorType.Color3:
//                return Color.yellow;
//            case ColorType.Color4:
//                return Color.cyan;
//            case ColorType.None:
//                return Color.white;
//            default:
//                throw new ArgumentOutOfRangeException(nameof(color), color, null);
//        }
    }
}