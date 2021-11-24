using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UniRx;

using Scripts.Core.Inputs;

namespace Scripts.Core
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(AudioSource))]
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private float _speed;
        [SerializeField] private float _rotationSpeed;
        [SerializeField] private bool _lockCursor = true;
        [SerializeField] private AudioClip[] _footStepSounds;

        [SerializeField] private float _stepInterval;

        private CompositeDisposable _disposables;
        private Rigidbody _rg;
        private Camera _camera;
        private CharacterController _characterController;
        private Vector3 _moveDir;
        private bool _cursorIsLocked = true;
        private AudioSource _audioSource;
        private float _stepCycle;
        private float _NextStep;

        private float _angleX;
        private float AngleX
        {
            get
            {
                return _angleX;
            }
            set
            {
                if (value < -85) _angleX = -85;
                else if (value > 85) _angleX = 85;
                else _angleX = value;
            }
        }
        private float _angleY;
        private float AngleY
        {
            get
            {
                return _angleY;
            }
            set
            {
                _angleY = value;
            }
        }

        public void Awake()
        {
            _moveDir = Vector3.zero;
            _rg = GetComponent<Rigidbody>();
            _disposables = new CompositeDisposable();
            _characterController = GetComponent<CharacterController>();
            _audioSource = GetComponent<AudioSource>();
            _audioSource.clip = _footStepSounds[0];
            _camera = GetComponentInChildren<Camera>();
            if (_camera == null)
            {
                throw new ArgumentNullException("Add camera to child gameobject");
            }
        }

        public void OnEnable()
        {
            GeneralInput.Instance.MoveObservable
                .Where(v => { return v != Vector2.zero; })
                .Subscribe(v => MoveLogic(v))
                .AddTo(_disposables);
            GeneralInput.Instance.RotationObservable
                .Where(v => { return v != Vector2.zero; })
                .Subscribe(v => RotateLogic(v))
                .AddTo(_disposables);
            GeneralInput.Instance.LockCursorObservable
                .Subscribe(b => UpdateCursorLock(b))
                .AddTo(_disposables);
        }
        public void OnDisable()
        {
            _disposables.Clear();
        }
        public void OnDestroy()
        {
            _disposables.Dispose();
        }

        private void MoveLogic(Vector2 moveVector)
        {
            Vector3 desiredMove = _camera.transform.forward * moveVector.y + _camera.transform.right * moveVector.x;

            RaycastHit hitInfo;
            Physics.SphereCast(transform.position, _characterController.radius, Vector3.down, out hitInfo,
                               _characterController.height / 2f, Physics.AllLayers, QueryTriggerInteraction.Ignore);
            desiredMove = Vector3.ProjectOnPlane(desiredMove, hitInfo.normal).normalized;

            _moveDir.x = desiredMove.x * _speed;
            _moveDir.z = desiredMove.z * _speed;

            _characterController.Move(_moveDir * Time.fixedDeltaTime);

            ProgressStepCycle(_speed, moveVector);
        }
        private void RotateLogic(Vector2 inputVector)
        {
            AngleY += _rotationSpeed * inputVector.y;
            AngleX -= _rotationSpeed * inputVector.x;
            Quaternion target = Quaternion.Euler(AngleX, AngleY, 0);
            _camera.transform.rotation = Quaternion.Slerp(_camera.transform.rotation, target, 1);
        }
        private void UpdateCursorLock(bool isLockCursor)
        {
            if (_lockCursor)
            {
                if (isLockCursor)
                {
                    Cursor.lockState = CursorLockMode.Locked;
                    Cursor.visible = false;
                }
                else
                {
                    Cursor.lockState = CursorLockMode.None;
                    Cursor.visible = true;
                }
            }
        }
        
        private void ProgressStepCycle(float speed, Vector2 moveVector)
        {
            if (_characterController.velocity.sqrMagnitude > 0)
            {
                _stepCycle += (_characterController.velocity.magnitude + (speed)) *
                             Time.fixedDeltaTime;
            }

            if (!(_stepCycle > _NextStep))
            {
                return;
            }

            _NextStep = _stepCycle + _stepInterval;
            
            _audioSource.PlayOneShot(_audioSource.clip);
            int n = UnityEngine.Random.Range(1, _footStepSounds.Length);
            _audioSource.clip = _footStepSounds[n];
            _footStepSounds[n] = _footStepSounds[0];
            _footStepSounds[0] = _audioSource.clip;
        }
    }

}
