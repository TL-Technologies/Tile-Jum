using System;
using System.Collections.Generic;
using UnityEngine;

public class DynamicEnvCreator : MonoBehaviour
{
    public event Action<ICycleTile> CreatedTile;
    public event Action<ICycleTile> RemovedTile;


    private const float WORLD_WIDTH = Constants.WORLD_SIZE;
    private readonly List<ICycleTile> _tileList = new List<ICycleTile>();
    private readonly List<int> _levelMapList = new List<int>();

    [SerializeField] private float _coverage;
    [SerializeField] private Transform _startPointTrans;
    [SerializeField] private LevelCreatorPrefabSelector _prefabSelector;
    [SerializeField] private Transform _targetTransform;
    [SerializeField] private bool _autoStart;

    public bool IsCreating { get; private set; }

    public IEnumerable<ICycleTile> Tiles => _tileList;

    public IEnumerable<int> LevelMap => _levelMapList;


    public Transform TargetTransform
    {
        get => _targetTransform;
        set => _targetTransform = value;
    }

    public float Coverage
    {
        get => _coverage;
        set => _coverage = value;
    }

    private void Start()
    {
        if(_autoStart)
            StartCreating();
    }

    public void StartCreating()
    {
        IsCreating = true;
        Update();
    }

    public void StopCreating()
    {
        IsCreating = false;
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(_startPointTrans.position - Vector3.right * WORLD_WIDTH / 2,
            _startPointTrans.position + Vector3.right * WORLD_WIDTH / 2);
    }

    // ReSharper disable once MethodTooLong
    private void Update()
    {
        if (!IsCreating)
            return;
        while ((_tileList.Count == 0 ||
               ((_tileList[_tileList.Count - 1].Position.y + _tileList[_tileList.Count - 1].Size -
                 TargetTransform.position.z) <
                Coverage))&&_tileList.Count<20f)
        {
            var targetPos = _tileList.Count > 0
                ? _tileList[_tileList.Count - 1].Position.ToXZtoXYZ() + Vector3.forward * (_tileList[_tileList.Count - 1].Size)
                : _startPointTrans.position;

            var selectedPrefab = _prefabSelector.GetSelectedPrefab();
            if (selectedPrefab == null)
            {
                return;
            }

            var cycleTile = (ICycleTile) Instantiate((MonoBehaviour) selectedPrefab, transform);
            (cycleTile as MonoBehaviour)?.gameObject.SetActive(true);
            cycleTile.SetPosition(targetPos.ToXZ());
            AddTile(cycleTile);
        }


        if (_tileList.Count > 0 &&
            TargetTransform.position.z - (_tileList[0].Position.y + _tileList[0].Size) > Coverage)
        {
            var cycleTile = _tileList[0];
            RemoveTile(cycleTile);
        }
    }

    private void RemoveTile(ICycleTile cycleTile)
    {
        _tileList.RemoveAt(0);
        RemovedTile?.Invoke(cycleTile);
        var monoBehavior = cycleTile as MonoBehaviour;
        if (monoBehavior != null)
        {
            Destroy(monoBehavior.gameObject);
        }
    }

    private void AddTile(ICycleTile cycleTile)
    {
        _tileList.Add(cycleTile);
        _levelMapList.Add(cycleTile.Id);
        CreatedTile?.Invoke(cycleTile);
    }

    public void ClearLevel()
    {
        _tileList.ForEach(tile => Destroy(((MonoBehaviour) tile).gameObject));
        _tileList.Clear();
    }
}