using System.Collections;
using Sound;
using UnityEngine;

namespace Player
{
    [RequireComponent(typeof(PlayerEntity))]
    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(WallRun))]
    [RequireComponent(typeof(PlayerInputs))]
    public class PlayerController : MonoBehaviour
    {
        [Header("Player")]
        [Tooltip("Sprint speed factor of the character")]
        public float SprintFactor = 1.5f;
        [Tooltip("Crouch speed factor of the character")]
        public float CrouchFactor = 0.5f;
        [Tooltip("Slide speed factor of the character")]
        public float SlideFactor = 2.0f;
        [Tooltip("Max air speed of the character")]
        public float MaxAirSpeedFactor = 4.0f;
        [Tooltip("Acceleration and deceleration")]
        public float SpeedChangeRateFactor = 1.0f;
        [Tooltip("Acceleration and deceleration in the air")]
        public float AirSpeedChangeRate = 0.75f;
        [Tooltip("Rotation speed of the character")]
        public float RotationSpeed = 1.0f;

        [Header("Jump & Gravity")]
        [Tooltip("The height the player can jump")]
        public float JumpHeight = 1.2f;
        [Tooltip("Time required to pass before being able to jump again. Set to 0f to instantly jump again")]
        public float JumpTimeout = 0.1f;
        [Tooltip("Time used to buffer the jump")]
        public float JumpBufferingTime = 0.3f;
        [Tooltip("The character uses its own gravity value")]
        public float Gravity = -15.0f;
        [Tooltip("Delay to jump again when grounded before we apply the ground speed")]
        public float BunnyHopDelay = 0.15f;

        [Header("Grounded & Ceilinged")]
        [Tooltip("If the character is grounded or not. Not part of the CharacterController built in grounded check")]
        public bool Grounded = true;
        [Tooltip("If the character is ceilinged or not. Not part of the CharacterController built in ceilinged check")]
        public bool Ceilinged = true;
        [Tooltip("Useful for rough ground")]
        public float GroundedOffset = -1.0f;
        [Tooltip("Useful for rough ceiling")]
        public float CeilingedOffset = 1.0f;
        [Tooltip("The radius of the grounded and ceilinged check. Should match the radius of the CharacterController")]
        public float CollisionRadius = 0.5f;
        [Tooltip("What layers the character uses as ground or ceiling")]
        public LayerMask CollisionLayers;

        [Header("Crouch & Slide")]
        [Tooltip("Player height when crouched")]
        public float CrouchHeight = 1.0f;
        [Tooltip("Character controller y center when the player is crouched")]
        public float CrouchYCenter = -0.5f;
        [Tooltip("Speed from stand to crouch")]
        public float CrouchChangeRate = 10.0f;

        [Header("Camera")]
        [Tooltip("The follow target set in the Cinemachine Virtual Camera that the camera will follow")]
        public GameObject CinemachineCameraTarget;
        [Tooltip("How far in degrees can you move the camera up")]
        public float TopClamp = 90.0f;
        [Tooltip("How far in degrees can you move the camera down")]
        public float BottomClamp = -90.0f;

        // cinemachine
        private float _cinemachineTargetPitch;

        // player
        private float _speed;
        private Vector3 _velocity;
        private float _verticalVelocity;
        private float _rotationVelocity;
        private float _terminalVelocity = 53.0f;
        private int _jumpUsed = 0;

        // BunnyHop
        private float _timeSinceGrounded = 0;

        // Slope
        private bool _onSlope;
        private bool _descendingSlope;
        private RaycastHit _slopeInfo;

        // Crouch
        private float _standingHeight;
        private float _standingCeilingedOffset;
        private float _standingHeadYPosition;

        // timeout deltatime
        private float _jumpTimeoutDelta;
        private float _jumpBufferingDelta;

        // components
        private CharacterController _controller;
        private PlayerInputs _input;
        private WallRun _wallRun;
        private GameObject _mainCamera;
        private GameObject _head;

        private const float _threshold = 0.01f;
        private const float _slopeRaycastRange = 1.6f;

        [Header("Pause")]
        [Tooltip("The display that will appear when the player presses the pause input")]
        public InterruptingDisplay pauseDisplay;
        float savedTimeScale;

        private void Awake()
        {
            // get a reference to our main camera
            if (_mainCamera == null)
            {
                _mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
            }
        }

        private void Start()
        {
            _controller = GetComponent<CharacterController>();
            _input = GetComponent<PlayerInputs>();
            _wallRun = GetComponent<WallRun>();

            // reset our timeouts on start
            _jumpTimeoutDelta = JumpTimeout;

            // save the height and the grounded offset of the standing player
            _standingHeight = _controller.height;
            _standingCeilingedOffset = CeilingedOffset;

            // Retrieve head gameobject
            _head = transform.Find("Head").gameObject;
            _standingHeadYPosition = _head.transform.localPosition.y;
            StartCoroutine(Steps());
        }

        private void Update()
        {
            if (GameManager.I.Player.GetCurrentHealth() <= 0) {
                return ;
            }
            Collisions();
            JumpAndGravity();
            Crouch();
            Move();
        }

        private void LateUpdate()
        {
            CameraRotation();
        }

        private void Collisions()
        {
            GroundedCheck();
            Ceilinged = CollisionCheck(transform.position, CeilingedOffset, CollisionRadius, CollisionLayers);
        }

        private void GroundedCheck()
        {
            var oldGrounded = Grounded;
            Grounded = CollisionCheck(transform.position, GroundedOffset, CollisionRadius, CollisionLayers);
            
            if (!Grounded)
            {
                _timeSinceGrounded = 0;
            }
            else
            {
                if (oldGrounded != Grounded)
                    AudioManager.PlayLandingSound();
                _timeSinceGrounded += Time.deltaTime;
            }
        }

        private void CameraRotation()
        {
            if (GameManager.I.Player.GetCurrentHealth() <= 0) {
                return ;
            }
            // if there is an input
            if (_input.look.sqrMagnitude >= _threshold)
            {
                var sensitivity = PlayerPrefs.GetFloat("Sensitivity", 1.0f);

                _cinemachineTargetPitch += _input.look.y * RotationSpeed * Time.deltaTime * sensitivity;
                _rotationVelocity = _input.look.x * RotationSpeed * Time.deltaTime * sensitivity;

                // clamp our pitch rotation
                _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);

                // rotate the player left and right
                transform.Rotate(Vector3.up * _rotationVelocity);
            }

            // Update Cinemachine camera target pitch
            if (_cinemachineTargetPitch != TopClamp && _cinemachineTargetPitch != BottomClamp)
            {
                CinemachineCameraTarget.transform.localRotation = Quaternion.Euler(_cinemachineTargetPitch, 0.0f, _wallRun.GetCameraRoll());
            }
        }

        private void Crouch()
        {
            if (_input.crouch && !_input.sprint)
            {
                _controller.height = Mathf.Lerp(_controller.height, CrouchHeight, Time.deltaTime * CrouchChangeRate);
                _controller.center = new Vector3(0.0f, Mathf.Lerp(_controller.center.y, CrouchYCenter, Time.deltaTime * CrouchChangeRate), 0.0f);
                _head.transform.localPosition = new Vector3(
                    _head.transform.localPosition.x,
                    Mathf.Lerp(_head.transform.localPosition.y, _standingHeadYPosition + CrouchYCenter, Time.deltaTime * CrouchChangeRate),
                    _head.transform.localPosition.z
                );
                CeilingedOffset = Mathf.Lerp(CeilingedOffset, _standingCeilingedOffset - (_standingHeight - _controller.height), Time.deltaTime * CrouchChangeRate);
            }
            else if (!Ceilinged)
            {
                _controller.height = Mathf.Lerp(_controller.height, _standingHeight, Time.deltaTime * CrouchChangeRate);
                _controller.center = new Vector3(0.0f, Mathf.Lerp(_controller.center.y, 0.0f, Time.deltaTime * CrouchChangeRate), 0.0f);
                _head.transform.localPosition = new Vector3(
                    _head.transform.localPosition.x,
                    Mathf.Lerp(_head.transform.localPosition.y, _standingHeadYPosition, Time.deltaTime * CrouchChangeRate),
                    _head.transform.localPosition.z
                );
                CeilingedOffset = Mathf.Lerp(CeilingedOffset, _standingCeilingedOffset, Time.deltaTime * CrouchChangeRate);
            }
        }

        private IEnumerator Steps()
        {
            while (true)
            {
                float currentHorizontalSpeed = GetHorizontalSpeed();
                if ((Grounded || _wallRun.isWallRunning) && currentHorizontalSpeed != 0)
                    AudioManager.PlayStepSound();
                yield return new WaitForSecondsRealtime(0.25f);
            }
        }
        
        private void Move()
        {
            if (!_wallRun.isWallRunning)
            {
                // detect if the player is on a slope
                DetectSlope();

                // set target speed based on move speed, sprint speed and if sprint is pressed
                float targetSpeed = GetTargetSpeed();

                // a simplistic acceleration and deceleration designed to be easy to remove, replace, or iterate upon

                // note: Vector2's == operator uses approximation so is not floating point error prone, and is cheaper than magnitude
                // if there is no input, set the target speed to 0
                if (_input.move == Vector2.zero) targetSpeed = 0.0f;

                // a reference to the players current horizontal velocity
                float currentHorizontalSpeed = GetHorizontalSpeed();

                float speedOffset = 0.1f;
                float inputMagnitude = _input.analogMovement ? _input.move.magnitude : 1f;

                // accelerate or decelerate to target
                if (currentHorizontalSpeed < targetSpeed - speedOffset || currentHorizontalSpeed > targetSpeed + speedOffset)
                {
                    // creates curved result rather than a linear one giving a more organic speed change
                    // note T in Lerp is clamped, so we don't need to clamp our speed
                    _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude, Time.deltaTime * (GameManager.I.Player.moveSpeed * SpeedChangeRateFactor));

                    // round speed to 3 decimal places
                    _speed = Mathf.Round(_speed * 1000f) / 1000f;
                }
                else
                {
                    _speed = targetSpeed;
                }

                // normalise input direction
                Vector3 inputDirection = new Vector3(_input.move.x, 0.0f, _input.move.y).normalized;

                // note: Vector2's != operator uses approximation so is not floating point error prone, and is cheaper than magnitude
                // if there is a move input rotate player when the player is moving
                if (_input.move != Vector2.zero)
                {
                    // move
                    inputDirection = transform.right * _input.move.x + transform.forward * _input.move.y;
                }
                
                // compute velocity
                _velocity = inputDirection.normalized * _speed;
                if (_verticalVelocity < 0) _velocity = AdjustVelocityToSlope(_velocity);
            }
            else
            {
                // get the velocity from the wall run script
                _velocity = _wallRun.wallRunVelocity;
                _verticalVelocity = _wallRun.wallRunVerticalVelocity;
            }
            _velocity.y += _verticalVelocity;

            // move the player
            _controller.Move(_velocity * Time.deltaTime);
        }

        private void JumpAndGravity()
        {
            if (_input.jump)
            {
                _jumpBufferingDelta = JumpBufferingTime;
            }
            else if (_jumpBufferingDelta >= 0)
            {
                _jumpBufferingDelta -= Time.deltaTime;
                _input.jump = true;
            }

            if (Grounded)
            {
                // stop our velocity dropping infinitely when grounded
                if (_verticalVelocity < 0.0f)
                {
                    _verticalVelocity = -2f;
                }

                // Reset the jump counter
                if (_jumpTimeoutDelta <= 0.0f)
                {
                    _jumpUsed = 0;
                }
            }

            // Jump
            if (_input.jump && _wallRun.isWallRunning)
            {
                // call the wallrun jump
                _wallRun.WallRunJump(GetJumpForce());

                // reset the jump timeout timer
                _jumpTimeoutDelta = JumpTimeout;
                if (Random.Range(0, 3) == 0)
                    AudioManager.PlayJumpSound();
            }
            else if (_input.jump && _jumpTimeoutDelta <= 0.0f && _jumpUsed < GameManager.I.Player.nbJumps)
            {
                // the square root of H * -2 * G = how much velocity needed to reach desired height
                _verticalVelocity = GetJumpForce();

                // reset the jump timeout timer
                _jumpTimeoutDelta = JumpTimeout;

                // Count the number of jump used
                _jumpUsed++;
                if (Random.Range(0, 3) == 0)
                    AudioManager.PlayJumpSound();
            }
            else
            {
                // don't jump
                _input.jump = false;
            }

            // jump timeout
            if (_jumpTimeoutDelta >= 0.0f)
            {
                _jumpTimeoutDelta -= Time.deltaTime;
            }

            // apply gravity over time if under terminal (multiply by delta time twice to linearly speed up over time)
            if (_verticalVelocity < _terminalVelocity)
            {
                _verticalVelocity += Gravity * Time.deltaTime;
            }
        }

        public float GetSavedTimeScale()
        {
            return (savedTimeScale);
        }

        private float GetTargetSpeed()
        {
            if (!Grounded || _timeSinceGrounded <= BunnyHopDelay)
                return GetAirTargetSpeed();
            if (_input.sprint)
                return GameManager.I.Player.moveSpeed * SprintFactor;
            if (_input.crouch)
                return GameManager.I.Player.moveSpeed * (_descendingSlope ? SlideFactor : CrouchFactor);
            return GameManager.I.Player.moveSpeed;
        }

        private float GetAirTargetSpeed()
        {
            if (_speed < GameManager.I.Player.moveSpeed)
                return GameManager.I.Player.moveSpeed;
            float maxAirSpeed = GameManager.I.Player.moveSpeed * MaxAirSpeedFactor;
            return _speed < maxAirSpeed ? _speed + AirSpeedChangeRate : maxAirSpeed;
        }

        public float GetHorizontalSpeed()
        {
            float yControllerVelocity = _descendingSlope ? _controller.velocity.y : 0.0f;
            return new Vector3(_controller.velocity.x, yControllerVelocity, _controller.velocity.z).magnitude;
        }

        private bool CollisionCheck(Vector3 position, float offset, float radius, LayerMask layers)
        {
            // set sphere position, with offset
            Vector3 spherePosition = new Vector3(position.x, position.y + offset, position.z);
            return Physics.CheckSphere(spherePosition, radius, layers, QueryTriggerInteraction.Ignore);
        }

        private void DetectSlope()
        {
            _onSlope = false;
            _descendingSlope = false;

            if (!Grounded) return;

            Ray ray = new Ray(transform.position + _controller.center, Vector3.down);

            if (Physics.Raycast(ray, out _slopeInfo, _slopeRaycastRange) && _slopeInfo.normal != Vector3.up)
                _onSlope = true;
            _descendingSlope = _controller.velocity.y < 0.0f && _onSlope;
        }

        private Vector3 AdjustVelocityToSlope(Vector3 velocity)
        {
            if (_onSlope)
            {
                Quaternion slopeRotation = Quaternion.FromToRotation(Vector3.up, _slopeInfo.normal);
                Vector3 adjustedVelocity = slopeRotation * velocity;

                if (adjustedVelocity.y < 0)
                {
                    return adjustedVelocity;
                }
            }

            return velocity;
        }

        private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
        {
            if (lfAngle < -360f) lfAngle += 360f;
            if (lfAngle > 360f) lfAngle -= 360f;
            return Mathf.Clamp(lfAngle, lfMin, lfMax);
        }

        private float GetJumpForce()
        {
            return Mathf.Sqrt(JumpHeight * -2f * Gravity);
        }

        private void OnDrawGizmosSelected()
        {
            Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
            Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);

            if (Grounded) Gizmos.color = transparentGreen;
            else Gizmos.color = transparentRed;

            // when selected, draw a gizmo in the position of, and matching radius of, the grounded collider
            Gizmos.DrawSphere(new Vector3(transform.position.x, transform.position.y + GroundedOffset, transform.position.z), CollisionRadius);

            if (Ceilinged) Gizmos.color = transparentGreen;
            else Gizmos.color = transparentRed;

            // when selected, draw a gizmo in the position of, and matching radius of, the ceilinged collider
            Gizmos.DrawSphere(new Vector3(transform.position.x, transform.position.y + CeilingedOffset, transform.position.z), CollisionRadius);

            // When selected, draw a gizmo representing the slope raycast
            Gizmos.color = Color.magenta;
            Gizmos.DrawRay(transform.position, new Vector3(0.0f, -_slopeRaycastRange, 0.0f));

            // when selected, draw a gizmo representing the velocity
            Gizmos.color = Color.red;
            Gizmos.DrawRay(transform.position, _velocity);
        }
    }
}
