using UnityEngine;

[DefaultExecutionOrder(100)]
public class CameraFollower : MonoBehaviour
{
    [SerializeField] private float _offset;
    [SerializeField] private Transform _target;
    [SerializeField] private float _lerbSpeed = 5;
    [SerializeField] private Vector2 _minAndMaxWidthAtTarget;
    [SerializeField] private Vector3 _targetBasePoint;
    [SerializeField] private Vector3 _targetAngle;

    private bool _active;
    private Camera _camera;

    public Transform Target
    {
        get => _target;
        set => _target = value;
    }

    public Camera Camera => _camera == null ? _camera = Camera.main : _camera;

    public bool Active
    {
        get => _active;
        set
        {
            _active = value;
            if (value)
            {
                Position = _target.transform.position.z;
            }
        }
    }

    private float Position
    {
        get => transform.position.z + _offset;
        set
        {
            var position = transform.position;
            position.z = value - _offset;
            transform.position = position;
        }
    }


    // Use this for initialization
    void Start()
    {
//        var screenPoint = Camera.WorldToScreenPoint(_targetBasePoint);
//        screenPoint.x = 0;
//        screenPoint.z = Vector3.Dot(_targetBasePoint - transform.position, Camera.transform.forward);
//        var worldPoint = Camera.ScreenToWorldPoint(screenPoint);
//        var currentWidth = Mathf.Abs(worldPoint.x * 2);
//
//        var targetWidth = currentWidth;
//
//        if (currentWidth > _minAndMaxWidthAtTarget.y)
//        {
//            targetWidth = _minAndMaxWidthAtTarget.y;
//        }
//        else if (currentWidth < _minAndMaxWidthAtTarget.x)
//        {
//            targetWidth = _minAndMaxWidthAtTarget.x;
//        }
//        Debug.Log($"Target Width:{targetWidth} Current Width:{currentWidth}");
//
//        var targetClappingPlan = targetWidth* screenPoint.z / currentWidth;
//
//        var moveDelta = (targetClappingPlan - screenPoint.z);
//        var angle = Vector3.Angle((Camera.transform.position - _targetBasePoint).normalized, -Camera.transform.forward);
//        var actualMoveDelta = moveDelta * (1 / Mathf.Cos(angle * Mathf.Deg2Rad));
//        var actualMoveVec = (Camera.transform.position - _targetBasePoint).normalized * actualMoveDelta;
//        Camera.transform.position += actualMoveVec;
//        _offset -= actualMoveVec.z;
    }


    // Update is called once per frame
    void FixedUpdate()
    {
        if (!Active)
        {
            return;
        }
        transform.rotation = Quaternion.Lerp(transform.rotation,Quaternion.Euler(_targetAngle),7*Time.fixedDeltaTime);
        Position = Mathf.Lerp(Position, _target.transform.position.z, _lerbSpeed * Time.fixedDeltaTime);
        //        Debug.Log($"Target Position z :{_target.position.z} Position:{Position}");

    }

}