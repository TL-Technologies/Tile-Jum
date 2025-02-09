// /*
// Created by Darsan
// */

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class LevelCreator : MonoBehaviour
{

    [SerializeField] private Tile _tilePrefab;
    [SerializeField] private FinishBar _finishBarPrefab;
    [SerializeField] private Transform _startPoint;
    [SerializeField] private Vector2 _minAndMaxTileX;

    public Vector2Int MinAndMaxTilesBetweenColor { get; set; } = new Vector2Int(3,6);
    public int ApproximateColorTileCount { get; set; } = 10;
    public float StepDistance { get; set; }

    private readonly List<Tile> _tiles= new List<Tile>();

    public IEnumerable<Tile> Tiles => _tiles;

    public void CreateOrRefresh()
    {
        _tiles.ForEach(tileGroup => Destroy(tileGroup.gameObject));
        _tiles.Clear();

        
        while (ApproximateColorTileCount-1>Tiles.Count())
        {
            var group = Instantiate(_tilePrefab,transform);

            group.transform.position = group.transform.position.WithX(_minAndMaxTileX.RandomWithIn()).WithZ(_tiles.Count == 0
                ? _startPoint.position.z
                : _tiles.Last().transform.position.z + StepDistance);
            _tiles.Add(group);
            
        }


        var finishBar = Instantiate(_finishBarPrefab,transform);
        finishBar.transform.position = finishBar.transform.position.WithZ( _tiles.Last().transform.position.z + StepDistance);
    }

 

}

public enum ColorType
{
    None,Color1,Color2,Color3,Color4
}