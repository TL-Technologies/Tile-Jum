// /*
// Created by Darsan
// */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TileGroup:MonoBehaviour 
{
    [SerializeField]private List<Tile> _tiles = new List<Tile>();

    public int ActiveIndex
    {
        set
        {
            for (var i = 0; i < _tiles.Count; i++)
            {
                _tiles[i].gameObject.SetActive(i==value);
            }
        }
    }

    public void Hit(Tile tile, Vector3 hitPoint)
    {
        tile.Hit(hitPoint);
        StartCoroutine(HitEnumerator(tile, hitPoint));
    }

    private IEnumerator HitEnumerator(Tile tile,Vector3 hitPoint)
    {
        var startPositionY = tile.transform.position.y;
        yield return SimpleCoroutine.MoveTowardsEnumerator(onCallOnFrame: n
            => tile.transform.position =
                tile.transform.position.WithY(Mathf.Lerp(startPositionY, startPositionY - 0.1f, n)), speed: 8f);
    }
}