using System.Linq;
using Fastblast;
using UnityEngine;

namespace Player
{
    [RequireComponent(typeof(PlayerController))]
    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(PlayerInputs))]
    public class WallRun : MonoBehaviour
    {
        [Header("Wallrun")]
        [Tooltip("If the character is wall running or not")]
        public bool isWallRunning = false;
        [Tooltip("Max range between the player and the wall to wallrun")]
        public float WallMaxDistance = 1.0f;
        [Tooltip("Speed multiplier when wallrunning")]
        public float WallSpeedMultiplier = 1.2f;
        [Tooltip("Minimum height between the player and the ground to wallrun")]
        public float minimumHeight = 1.2f;
        [Tooltip("Camera angle when the player is wallrunning")]
        public float maxAngleRoll = 20.0f;
        [Tooltip("Camera angle threshold when the player is running")]
        [Range(0.0f, 1.0f)]
        public float normalizedAngleTreshold = 0.1f;
        [Tooltip("Gravity applied when wallrunning")]
        public float wallGravityDownForce = 20f;
        [Tooltip("What layers the character uses as wall")]
        public LayerMask WallLayers;

        [Header("Wallrun Jump")]
        [Tooltip("If the character is wall running or not")]
        public float jumpDuration = 1.0f;
        public float wallBouncing = 3.0f;
        public float cameraTransitionDuration = 1.0f;

        public Vector3 wallRunVelocity { get; private set; }
        public float wallRunVerticalVelocity { get; private set; } = 0;

        private PlayerController _playerController;
        private CharacterController _characterController;
        private PlayerInputs _input;

        private Vector3[] directions;
        private RaycastHit[] hits;

        private Vector3 lastWallPosition;
        private Vector3 lastWallNormal;
        private float elapsedTimeSinceJump = 0.0f;
        private float elapsedTimeSinceWallAttach = 0.0f;
        private float elapsedTimeSinceWallDetatch = 0.0f;
        private bool jumping;

        private void Start()
        {
            _playerController = GetComponent<PlayerController>();
            _characterController = GetComponent<CharacterController>();
            _input = GetComponent<PlayerInputs>();

            directions = new Vector3[]
            {
                Vector3.right,
                Vector3.right + Vector3.forward,
                Vector3.forward,
                Vector3.left + Vector3.forward,
                Vector3.left
            };
        }

        public void LateUpdate()
        {
            isWallRunning = false;
            
            if (_input.jump)
            {
                jumping = true;
            }

            if (CanAttach())
            {
                hits = new RaycastHit[directions.Length];

                for (int i = 0; i < directions.Length; i++)
                {
                    Vector3 dir = transform.TransformDirection(directions[i]);
                    Physics.Raycast(transform.position, dir, out hits[i], WallMaxDistance, WallLayers);
                }

                if (CanWallRun())
                {
                    hits = hits.ToList().Where(h => h.collider != null).OrderBy(h => h.distance).ToArray();
                    if (hits.Length > 0)
                    {
                        OnWall(hits[0]);
                        lastWallPosition = hits[0].point;
                        lastWallNormal = hits[0].normal;
                    }
                }
            }

            if (isWallRunning)
            {
                elapsedTimeSinceWallDetatch = 0;
                elapsedTimeSinceWallAttach += Time.deltaTime;
                wallRunVerticalVelocity += wallGravityDownForce * Time.deltaTime;
            }
            else
            {
                elapsedTimeSinceWallAttach = 0;
                elapsedTimeSinceWallDetatch += Time.deltaTime;
                wallRunVerticalVelocity = 0;
            }
        }

        private void OnWall(RaycastHit hit)
        {
            float d = Vector3.Dot(hit.normal, Vector3.up);
            if (d >= -normalizedAngleTreshold && d <= normalizedAngleTreshold)
            {
                float vertical = _input.move.y;
                Vector3 alongWall = transform.TransformDirection(Vector3.forward);

                wallRunVelocity = alongWall * vertical * WallSpeedMultiplier;
                isWallRunning = true;
            }
        }

        private bool CanWallRun()
        {
            return !_playerController.Grounded && _input.move.y > 0 && VerticalCheck();
        }

        private bool VerticalCheck()
        {
            return !Physics.Raycast(transform.position, Vector3.down, minimumHeight);
        }

        private bool CanAttach()
        {
            if (jumping)
            {
                elapsedTimeSinceJump += Time.deltaTime;
                if (elapsedTimeSinceJump > jumpDuration)
                {
                    elapsedTimeSinceJump = 0.0f;
                    jumping = false;
                }
                return false;
            }
            return true;
        }

        float CalculateSide()
        {
            if (isWallRunning)
            {
                Vector3 heading = lastWallPosition - transform.position;
                Vector3 perp = Vector3.Cross(transform.forward, heading);
                float dir = Vector3.Dot(perp, transform.up);
                return dir;
            }
            return 0;
        }

        public float GetCameraRoll()
        {
            float dir = CalculateSide();
            float cameraAngle = _playerController.CinemachineCameraTarget.transform.eulerAngles.z;
            float targetAngle = 0;
            if (dir != 0)
            {
                targetAngle = Mathf.Sign(dir) * maxAngleRoll;
            }
            return Mathf.LerpAngle(cameraAngle, targetAngle, Mathf.Max(elapsedTimeSinceWallAttach, elapsedTimeSinceWallDetatch) / cameraTransitionDuration);
        }

        public void WallRunJump(float jumpForce)
        {
            Vector3 jump = GetWallJumpDirection() * jumpForce;
            wallRunVerticalVelocity = jump.y;
            wallRunVelocity += new Vector3(jump.x, 0.0f, jump.z);
        }

        private Vector3 GetWallJumpDirection()
        {
            if (isWallRunning)
            {
                return lastWallNormal * wallBouncing + Vector3.up;
            }
            return Vector3.zero;
        }
    }
}