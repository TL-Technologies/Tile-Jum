using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game
{
    [DefaultExecutionOrder(50)]
    public class Player : MonoBehaviour
    {
        public event Action PositionChanged;
        public event Action<Tile, bool, Vector3> JumpedOnTile;
        public event Action HitEnd;
        public event Action Died;

        [SerializeField] private Vector2 _minAndMaxX;
        [SerializeField] private float _gravityMultiplexer = 1f;
        [SerializeField] private float _jumpSpeed = 8f;
        [SerializeField] private float _forwardSpeed = 5f;
        [SerializeField] private float _radius = 0.15f;
        [SerializeField] private MeshRenderer _renderer;
        [SerializeField] private BallParticleEffect  _breakEffect;

        [Header("Sounds")] [SerializeField] private AudioClip _correctTileJumpClip;
        [SerializeField] private AudioClip _breakClip, _colorChangeClip, _levelFinishClip,_perfectClip;

        public Sprite[] _sprites;
        
        
        public float ForwardSpeed
        {
            get => _forwardSpeed;
            set => _forwardSpeed = value;
        }
        

        public PlayerSkin PlayerSkin
        {
            get => _playerSkin;
            set
            {
                _playerSkin = value;
                var position = _renderer.transform.position;
                var rotation = _renderer.transform.rotation;
                var lastColor = _renderer.material.color;
                Destroy(_renderer.gameObject);
                var skin = Instantiate(value.prefab, position, rotation);
                skin.transform.parent = transform;
                _renderer = skin.GetComponent<MeshRenderer>();
                _renderer.material.color = lastColor;
            }
        }

        public bool Active { get; set; }

        private Vector2 _lastCameraWorldPoint;
        private ColorType _color;
        private PlayerSkin _playerSkin;

        public Quaternion RendererRotation
        {
            get => _renderer.transform.rotation;
            set => _renderer.transform.rotation = value;
        }

        public ColorType Color
        {
            get => _color;
            set
            {
                _color = value;
                _renderer.material.color = value.ToColor();
            }
        }

        public Vector3 Velocity { get; set; }
        public Vector3 RendererAngularVelocity { get; set; }

        public float JumpSpeed
        {
            get => _jumpSpeed;
            set => _jumpSpeed = value;
        }
        public float ForwardStepDistance =>
            CalculateForwardDistance(_jumpSpeed, Physics.gravity.y * _gravityMultiplexer, _forwardSpeed);

        public Vector3 Position
        {
            get => transform.position;
            set
            {
                transform.position = value;
                PositionChanged?.Invoke();
            }
        }

        private void Awake()
        {
            RendererRotation = Quaternion.AngleAxis(-30, Vector3.right) * Quaternion.AngleAxis(-30, Vector3.up);
        }

        private void Update()
        {
            if (!Active)
                return;

            if (Input.GetKeyDown(KeyCode.P))
            {
                gameObject.SetActive(false);
            }

            if (Input.GetMouseButtonDown(0))
            {
                _lastCameraWorldPoint = Input.mousePosition;
            }

            if (Input.GetMouseButton(0))
            {
                var delta = (Vector2) Input.mousePosition - _lastCameraWorldPoint;
                var deltaX = 0.005f * (1024f * delta.x / Screen.width);
                Position = Position.WithX(_minAndMaxX.Clamp(Position.x + deltaX));
                _lastCameraWorldPoint = Input.mousePosition;
            }
        }

        private void FixedUpdate()
        {
            if (!Active)
                return;

            Velocity = Velocity.WithZ(_forwardSpeed);
            Position += Velocity * Time.fixedDeltaTime;
            Velocity += _gravityMultiplexer * Time.fixedDeltaTime * Physics.gravity;
            RendererRotation = Quaternion.Euler(RendererAngularVelocity * Time.fixedDeltaTime) * RendererRotation;
            UpdateCollision();
        }

        

        public void Jump()
        {
            Velocity = Velocity.WithY(_jumpSpeed);
            RendererAngularVelocity = Random.onUnitSphere * 360f;
        }


        public void JumpTo(Vector3 point)
        {
            var hDis = point.z - transform.position.z;
            var vDis = (point.y - transform.position.y);
            var time = Mathf.Abs(hDis / _forwardSpeed);

            var u = (vDis - (0.5f * Physics.gravity.y * _gravityMultiplexer * time * time)) / time;
            Velocity = Velocity.WithY(u);
            RendererAngularVelocity = Random.onUnitSphere * 360f;
        }

        public void Jump(Tile bar, Vector3 point)
        {

            Jump();
           
        }

        private void UpdateCollision()
        {
            if (Physics.Raycast(Position, Vector3.down, out var centerHit, 0.001f + _radius, ~(1 << 8)))
            {
                OnHit(centerHit);
                return;
            }


//            var sideDis = _radius * 0.5f;
//            var depth = _radius * Mathf.Sin(Mathf.Acos(sideDis / _radius));
//            if (Physics.Raycast(Position - sideDis * Vector3.right, Vector3.down, out var leftHit, 0.001f //+ depth
//                ,
//                ~(1 << 8)))
//            {
//                OnHit(leftHit);
//                Debug.Break();
//            }
//
//            if (Physics.Raycast(Position + sideDis * Vector3.right, Vector3.down, out var rightHit, 0.001f //+ depth
//                ,
//                ~(1 << 8)))
//            {
//                OnHit(rightHit);
//            }
        }

        private void OnHit(RaycastHit hit)
        {
            Debug.Log(hit.transform.gameObject);
            if (hit.transform.CompareTag("Tiles"))
            {

                var tile = hit.transform.GetComponent<Tile>();
                var perfect = tile.IsPerfect(hit.point.ToXZ());
                Position = Position.WithYZ(hit.point.y + _radius, hit.transform.position.z);

                if (perfect)
                {
                    Position = Position.WithX(hit.transform.position.x);
                   
                }

                PlayClipIfCan(_correctTileJumpClip);
                if(perfect)
                    PlayClipIfCan(_perfectClip);
                tile.Hit(Position.WithY(Position.y - _radius));
                JumpedOnTile?.Invoke(tile,perfect,hit.point);
                Jump(tile,Position.WithY(Position.y-_radius));
            }
            else if (hit.transform.CompareTag("FinishBar"))
            {
                HitEnd?.Invoke();
            }
            else if (hit.transform.CompareTag("Road"))
            {
                Die();
            }
        }

        public void Die()
        {
            var effect = Instantiate(_breakEffect, transform.position, Quaternion.identity);
            effect.Color = Color.ToColor();
            Handheld.Vibrate();
            PlayClipIfCan(_breakClip);
            Active = false;
            gameObject.SetActive(false);
            Died?.Invoke();
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawWireSphere(Position, _radius);
        }

        public float CalculateForwardDistance(float jumpSpeed, float gravity, float forwardSpeed)
        {
            return 2 * (jumpSpeed / Mathf.Abs(gravity)) * forwardSpeed;
        }

        private void PlayClipIfCan(AudioClip clip, float volume = 0.35f, float pitch = 1f)
        {
            if (!AudioManager.IsSoundEnable || clip == null)
                return;

            var audioSource = AudioManager.PlayClipAtPoint(clip, Camera.main.transform.position);
            audioSource.pitch = pitch;
            audioSource.volume = volume;
        }
        
       
    }
}