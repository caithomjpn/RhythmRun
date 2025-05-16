using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif
using System.Collections;
using System;
using System.Linq;
using UnityEngine.SceneManagement;

namespace StarterAssets
{
    [RequireComponent(typeof(CharacterController))]
#if ENABLE_INPUT_SYSTEM
    [RequireComponent(typeof(PlayerInput))]
#endif
    public class ThirdPersonController : MonoBehaviour
    {
        [Header("Player")]
        public float MoveSpeed = 2.0f;
        public float SprintSpeed = 5.335f;
        [Range(0.0f, 0.3f)] public float RotationSmoothTime = 0.12f;
        public float SpeedChangeRate = 10.0f;

        public AudioClip LandingAudioClip;
        public AudioClip[] FootstepAudioClips;
        [Range(0, 1)] public float FootstepAudioVolume = 0.5f;

        [Space(10)]
        public float JumpHeight = 1.2f;
        public float Gravity = -25.0f;

        [Space(10)]
        public float JumpTimeout = 0.01f;
        public float FallTimeout = 0.15f;

        [Header("Player Grounded")]
        public bool Grounded = true;
        public float GroundedOffset = -0.14f;
        public float GroundedRadius = 0.28f;
        public LayerMask GroundLayers;

        [Header("Cinemachine")]
        public GameObject CinemachineCameraTarget;
        public float TopClamp = 70.0f;
        public float BottomClamp = -30.0f;
        public float CameraAngleOverride = 0.0f;
        public bool LockCameraPosition = false;

        private float _cinemachineTargetYaw;
        private float _cinemachineTargetPitch;

        private float _speed;
        private float _animationBlend;
        private float _targetRotation = 0.0f;
        private float _rotationVelocity;
        private float _verticalVelocity;
        private float _terminalVelocity = 53.0f;

        private float _jumpTimeoutDelta;
        private float _fallTimeoutDelta;

        private int _animIDSpeed;
        private int _animIDGrounded;
        private int _animIDJump;
        private int _animIDFreeFall;
        private int _animIDMotionSpeed;

#if ENABLE_INPUT_SYSTEM
        private PlayerInput _playerInput;
#endif
        private Animator _animator;
        private StarterAssetsInputs _input;
        private GameObject _mainCamera;

        private const float _threshold = 0.01f;

        private bool _hasAnimator;
        private bool autoRun = true;

        private bool IsCurrentDeviceMouse
        {
            get
            {
#if ENABLE_INPUT_SYSTEM
                return _playerInput.currentControlScheme == "KeyboardMouse";
#else
                return false;
#endif
            }
        }

        private CharacterController _controller;
        private float _originalHeight;
        public float slideSpeed = 10f;
        public float slideDuration = 1f;
        public float longJumpSpeed = 10f;

        [Header("Lane Movement")]
        public float laneOffset = 1.2f;
        private int currentLane = 1;
        private bool isShiftingLane = false;
        public float laneShiftDuration = 0.01f;

        private void Awake()
        {
            if (_mainCamera == null)
            {
                _mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
            }
        }

        private void Start()
        {
            _cinemachineTargetYaw = CinemachineCameraTarget.transform.rotation.eulerAngles.y;
            _hasAnimator = TryGetComponent(out _animator);
            _controller = GetComponent<CharacterController>();
            _input = GetComponent<StarterAssetsInputs>();
#if ENABLE_INPUT_SYSTEM
            _playerInput = GetComponent<PlayerInput>();
#else
            Debug.LogError("Starter Assets package is missing dependencies.");
#endif
            AssignAnimationIDs();
            _jumpTimeoutDelta = JumpTimeout;
            _fallTimeoutDelta = FallTimeout;
            _originalHeight = _controller.height;
            float beatDuration = 60f / 115f; // 115 BPM
            SprintSpeed = 4f / beatDuration; 
            Metronome.OnBeat += LogPlayerXZPosition;


        }

        private void Update()
        {
            if (!Metronome.GameHasStarted)
            {
                _input.move = Vector2.zero;
                _input.jump = false;
                return;
            }
            _hasAnimator = TryGetComponent(out _animator);

            JumpAndGravity();
            GroundedCheck();
            Move();

            // ❌ BLOCK all input on wrong beats
            if (!Metronome.IsActionBeat || !Metronome.BeatWindowOpen)
            {
                _input.move.x = 0;
                _input.move.y = 0;
                _input.jump = false;

                if (Input.anyKeyDown)
                {
                    FindObjectOfType<HitorMiss>().ShowMiss();
                    Debug.Log("❌ Input ignored — not an action beat.");
                }

                return;
            }


            // Slide
            if (Input.GetKeyDown(KeyCode.LeftControl) && _controller.isGrounded && Metronome.BeatWindowOpen)
            {
                _animator.SetTrigger("Slide");
                FindObjectOfType<HitorMiss>().ShowHit(); 
                StartCoroutine(Slide());
            }

            // Long jump
            if (Input.GetKey(KeyCode.LeftShift) && _controller.isGrounded && Metronome.BeatWindowOpen)
            {
                FindObjectOfType<HitorMiss>().ShowHit(); 
                StartCoroutine(LongJump());
            }

            // Jump
            if (Input.GetKeyDown(KeyCode.Space) && _controller.isGrounded && Metronome.BeatWindowOpen)
            {
                FindObjectOfType<HitorMiss>().ShowHit(); 

                _verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);
                if (_hasAnimator)
                {
                    _animator.SetBool(_animIDJump, true);
                }
                _jumpTimeoutDelta = JumpTimeout;
            }

            if (Input.GetKeyDown(KeyCode.R))
            {
                autoRun = !autoRun;
            }

        
            if (Metronome.BeatWindowOpen)
            {
                if (Input.GetKeyDown(KeyCode.A))
                {
                    ShiftInstantly(-1);
                    FindObjectOfType<HitorMiss>().ShowHit();
                }
                else if (Input.GetKeyDown(KeyCode.D))
                {
                    ShiftInstantly(+1);
                    FindObjectOfType<HitorMiss>().ShowHit();
                }
            }
            if (Input.GetKeyDown(KeyCode.Q))
            {
                SceneManager.LoadScene("SelectStage"); 
            }


        }

        private void ShiftInstantly(int direction) 
        {
            int nextLane = Mathf.Clamp(currentLane + direction, 0, 2);

            if (nextLane == currentLane)
            {
                Debug.Log(" Already at this lane, skipping shift.");
                return;
            }

            currentLane = nextLane;
            Debug.Log($" currentLane = {currentLane}, direction = {direction}");

            float[] lanePositions = { -laneOffset, 0f, laneOffset };
            Vector3 pos = transform.position;
            pos.x = lanePositions[currentLane];
            transform.position = pos;

            Debug.Log($" Shifted to lane {currentLane}, X = {pos.x}");
        }




        private IEnumerator Slide()
        {
            float originalHeight = _controller.height;
            _controller.height = originalHeight / 2;
            float originalSpeed = MoveSpeed;
            MoveSpeed = slideSpeed;

            yield return new WaitForSeconds(slideDuration);

            _controller.height = originalHeight;
            MoveSpeed = originalSpeed;
        }

        private IEnumerator LongJump()
        {
            float originalSpeed = SprintSpeed;
            SprintSpeed = longJumpSpeed;
            _verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity) * 1.5f;
            yield return new WaitForSeconds(0.5f);
            SprintSpeed = originalSpeed;
        }

        private void LateUpdate()

        {
        if (!Metronome.IsActionBeat)
            {

                _input.move.x = 0;
                _input.move.y = 0;
                _input.jump = false;
                if (Input.GetKeyDown(KeyCode.Space) ||
                    Input.GetKeyDown(KeyCode.LeftControl) ||
                    Input.GetKey(KeyCode.LeftShift) ||
                    Input.GetKeyDown(KeyCode.A) || 
                    Input.GetKeyDown(KeyCode.D)     
                )
                {
                    Debug.Log("❌ Input ignored — not an action beat.");
                }

                return; 
            }

            CameraRotation();
        }

        private void AssignAnimationIDs()
        {
            _animIDSpeed = Animator.StringToHash("Speed");
            _animIDGrounded = Animator.StringToHash("Grounded");
            _animIDJump = Animator.StringToHash("Jump");
            _animIDFreeFall = Animator.StringToHash("FreeFall");
            _animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
        }

        private void GroundedCheck()
        {
            Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z);
            Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers, QueryTriggerInteraction.Ignore);

            if (_hasAnimator)
            {
                _animator.SetBool(_animIDGrounded, Grounded);
            }
        }

        private void Move()
        {
            float targetSpeed = SprintSpeed;

            float forwardValue = (!Metronome.GameHasStarted) ? 0f : (autoRun ? 1.0f : _input.move.y);
            Vector3 inputDirection = new Vector3(_input.move.x, 0.0f, forwardValue).normalized;

            float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;
            float speedOffset = 0.1f;
            float inputMagnitude = _input.analogMovement ? inputDirection.magnitude : 1f;

            if (currentHorizontalSpeed < targetSpeed - speedOffset || currentHorizontalSpeed > targetSpeed + speedOffset)
            {
                _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude, Time.deltaTime * SpeedChangeRate);
                _speed = Mathf.Round(_speed * 1000f) / 1000f;
            }
            else
            {
                _speed = targetSpeed;
            }

            _animationBlend = Mathf.Lerp(_animationBlend, targetSpeed, Time.deltaTime * SpeedChangeRate);
            if (_animationBlend < 0.01f) _animationBlend = 0f;

            if (inputDirection != Vector3.zero)
            {
                _targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg + _mainCamera.transform.eulerAngles.y;
                float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, ref _rotationVelocity, RotationSmoothTime);
                transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
            }

            Vector3 moveDirection = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward;

            _controller.Move(moveDirection.normalized * (_speed * Time.deltaTime) +
                             new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);

            if (_hasAnimator)
            {
                _animator.SetFloat(_animIDSpeed, _animationBlend);
                _animator.SetFloat(_animIDMotionSpeed, inputMagnitude);
            }
            // Lock the X position to 0 after movement
            Vector3 correctedPosition = transform.position;

            // Define the allowed lanes
            float[] allowedLanes = { -laneOffset, 0f, laneOffset };

            // Snap to the nearest one
            float nearestLane = allowedLanes.OrderBy(lane => Mathf.Abs(correctedPosition.x - lane)).First();
            correctedPosition.x = allowedLanes[currentLane];
            transform.position = correctedPosition;


        }

        private void JumpAndGravity()
        {
            if (Grounded)
            {
                _fallTimeoutDelta = FallTimeout;

                if (_hasAnimator)
                {
                    _animator.SetBool(_animIDJump, false);
                    _animator.SetBool(_animIDFreeFall, false);
                }

                if (_verticalVelocity < 0.0f)
                {
                    _verticalVelocity = -2f;
                }

                //if (_input.jump && _jumpTimeoutDelta <= 0.0f && Metronome.BeatWindowOpen)
                //{
                //    _verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);

                //    if (_hasAnimator)
                //    {
                //        _animator.SetBool(_animIDJump, true);
                //    }
                //    _input.jump = false;

                //}

                if (_jumpTimeoutDelta >= 0.0f)
                {
                    _jumpTimeoutDelta -= Time.deltaTime;
                }
            }
            else
            {
                _jumpTimeoutDelta = JumpTimeout;

                if (_fallTimeoutDelta >= 0.0f)
                {
                    _fallTimeoutDelta -= Time.deltaTime;
                }
                else
                {
                    if (_hasAnimator)
                    {
                        _animator.SetBool(_animIDFreeFall, true);
                    }
                }

                //_input.jump = false;
            }

            if (_verticalVelocity < _terminalVelocity)
            {
                _verticalVelocity += Gravity * Time.deltaTime;
            }
        }

        private void CameraRotation()
        {
            if (_input.look.sqrMagnitude >= _threshold && !LockCameraPosition)
            {
                float deltaTimeMultiplier = IsCurrentDeviceMouse ? 1.0f : Time.deltaTime;

                _cinemachineTargetYaw += _input.look.x * deltaTimeMultiplier;
                _cinemachineTargetPitch += _input.look.y * deltaTimeMultiplier;
            }

            _cinemachineTargetYaw = ClampAngle(_cinemachineTargetYaw, float.MinValue, float.MaxValue);
            _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);

            CinemachineCameraTarget.transform.rotation = Quaternion.Euler(_cinemachineTargetPitch + CameraAngleOverride, _cinemachineTargetYaw, 0.0f);
        }

        private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
        {
            if (lfAngle < -360f) lfAngle += 360f;
            if (lfAngle > 360f) lfAngle -= 360f;
            return Mathf.Clamp(lfAngle, lfMin, lfMax);
        }

        private void OnDrawGizmosSelected()
        {
            Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
            Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);

            Gizmos.color = Grounded ? transparentGreen : transparentRed;

            Gizmos.DrawSphere(new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z), GroundedRadius);
        }

        private void OnFootstep(AnimationEvent animationEvent)
        {
            if (animationEvent.animatorClipInfo.weight > 0.5f)
            {
                if (FootstepAudioClips.Length > 0)
                {
                    var index = UnityEngine.Random.Range(0, FootstepAudioClips.Length);
                    AudioSource.PlayClipAtPoint(FootstepAudioClips[index], transform.TransformPoint(_controller.center), FootstepAudioVolume);
                }
            }
        }

        private void OnLand(AnimationEvent animationEvent)
        {
            if (animationEvent.animatorClipInfo.weight > 0.5f)
            {
                AudioSource.PlayClipAtPoint(LandingAudioClip, transform.TransformPoint(_controller.center), FootstepAudioVolume);
            }
        }
        private void LogPlayerXZPosition()
        {
            Vector3 pos = transform.position;
            Debug.Log($"[Player Position] X: {pos.x:F2}, Z: {pos.z:F2}");
        }
        private void OnDestroy()
        {
            Metronome.OnBeat -= LogPlayerXZPosition;
        }






    }
}
