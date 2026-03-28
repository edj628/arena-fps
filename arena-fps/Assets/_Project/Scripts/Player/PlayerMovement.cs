using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float walkSpeed = 10f;
    [SerializeField] private float airAcceleration = 5f;
    [SerializeField] private float friction = 8f;

    [Header("Jumping")]
    [SerializeField] private float jumpForce = 6f;
    [SerializeField] private int maxJumps = 2;

    [Header("Wall Running")]
    [SerializeField] private float wallRunSpeed = 12f;
    [SerializeField] private float wallRunDuration = 1.2f;
    [SerializeField] private float wallRunGravity = 2f;
    [SerializeField] private LayerMask wallLayer;

    [Header("Rocket Jump")]
    [SerializeField] private float rocketJumpForce = 18f;

    [Header("Look")]
    [SerializeField] private Transform cameraHolder;
    [SerializeField] private float mouseSensitivity = 0.15f;
    [SerializeField] private float maxLookAngle = 89f;

    private CharacterController _controller;
    private Vector3 _velocity;
    private int _jumpsRemaining;
    private float _pitch;

    // Wall run state
    private bool _isWallRunning;
    private float _wallRunTimer;
    private Vector3 _wallNormal;

    // Input
    private Vector2 _moveInput;
    private Vector2 _lookInput;
    private bool _jumpPressed;

    private void Awake()
    {
        _controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // --- Input callbacks (hooked up via PlayerInput component) ---

    public void OnMove(InputValue value) => _moveInput = value.Get<Vector2>();
    public void OnLook(InputValue value) => _lookInput = value.Get<Vector2>();
    public void OnJump(InputValue value) { if (value.isPressed) _jumpPressed = true; }

    private void Update()
    {
        HandleLook();
        HandleWallRun();
        HandleMovement();
        _jumpPressed = false;
    }

    // ---------------------------------------------------------------
    // Look
    // ---------------------------------------------------------------

    private void HandleLook()
    {
        _pitch -= _lookInput.y * mouseSensitivity;
        _pitch = Mathf.Clamp(_pitch, -maxLookAngle, maxLookAngle);
        cameraHolder.localEulerAngles = new Vector3(_pitch, 0f, 0f);
        transform.Rotate(Vector3.up, _lookInput.x * mouseSensitivity);
    }

    // ---------------------------------------------------------------
    // Movement (Quake-style strafe jumping)
    // ---------------------------------------------------------------

    private void HandleMovement()
    {
        bool grounded = _controller.isGrounded;

        if (grounded)
        {
            _jumpsRemaining = maxJumps;

            // Apply friction on ground
            float speed = _velocity.magnitude;
            if (speed > 0)
            {
                float drop = speed * friction * Time.deltaTime;
                _velocity *= Mathf.Max(speed - drop, 0) / speed;
            }

            // Zero out downward velocity when grounded
            if (_velocity.y < 0) _velocity.y = -1f;
        }

        Vector3 wishDir = transform.right * _moveInput.x + transform.forward * _moveInput.y;
        wishDir.y = 0f;
        if (wishDir.sqrMagnitude > 1f) wishDir.Normalize();

        if (grounded)
            Accelerate(wishDir, walkSpeed, walkSpeed * 10f);
        else if (!_isWallRunning)
            Accelerate(wishDir, walkSpeed, airAcceleration); // strafe-jump air control

        // Jump
        if (_jumpPressed && _jumpsRemaining > 0 && !_isWallRunning)
        {
            _velocity.y = jumpForce;
            _jumpsRemaining--;
        }

        // Wall run jump-off
        if (_jumpPressed && _isWallRunning)
            WallJump();

        // Gravity
        if (!_isWallRunning)
            _velocity.y -= 20f * Time.deltaTime;

        _controller.Move(_velocity * Time.deltaTime);
    }

    // Quake-style acceleration
    private void Accelerate(Vector3 wishDir, float wishSpeed, float accel)
    {
        float currentSpeed = Vector3.Dot(_velocity, wishDir);
        float addSpeed = wishSpeed - currentSpeed;
        if (addSpeed <= 0) return;

        float accelSpeed = Mathf.Min(accel * Time.deltaTime * wishSpeed, addSpeed);
        _velocity += accelSpeed * wishDir;
    }

    // ---------------------------------------------------------------
    // Wall Running
    // ---------------------------------------------------------------

    private void HandleWallRun()
    {
        if (_controller.isGrounded)
        {
            StopWallRun();
            return;
        }

        // Detect wall to the left or right
        bool wallLeft  = Physics.Raycast(transform.position, -transform.right, out RaycastHit hitLeft,  1.1f, wallLayer);
        bool wallRight = Physics.Raycast(transform.position,  transform.right, out RaycastHit hitRight, 1.1f, wallLayer);

        if ((wallLeft || wallRight) && _moveInput.y > 0.1f)
        {
            _wallNormal = wallLeft ? hitLeft.normal : hitRight.normal;

            if (!_isWallRunning)
            {
                _isWallRunning = true;
                _wallRunTimer = wallRunDuration;
                _jumpsRemaining = 1; // allow one wall jump
            }

            _wallRunTimer -= Time.deltaTime;
            if (_wallRunTimer <= 0)
            {
                StopWallRun();
                return;
            }

            // Run along the wall
            Vector3 wallForward = Vector3.Cross(_wallNormal, Vector3.up);
            if (Vector3.Dot(wallForward, transform.forward) < 0) wallForward = -wallForward;

            _velocity = wallForward * wallRunSpeed;
            _velocity.y = -wallRunGravity; // slight downward slide
        }
        else
        {
            StopWallRun();
        }
    }

    private void StopWallRun() => _isWallRunning = false;

    private void WallJump()
    {
        _isWallRunning = false;
        _velocity = _wallNormal * (jumpForce * 1.2f);
        _velocity.y = jumpForce;
        _jumpsRemaining = 0;
    }

    // ---------------------------------------------------------------
    // Rocket Jump (called by explosive weapon on impact near player)
    // ---------------------------------------------------------------

    /// <summary>
    /// Apply an explosive impulse to the player. Call this from the rocket/grenade weapon.
    /// </summary>
    /// <param name="explosionOrigin">World position of the explosion.</param>
    /// <param name="radius">Blast radius.</param>
    public void ApplyExplosiveForce(Vector3 explosionOrigin, float radius)
    {
        float distance = Vector3.Distance(transform.position, explosionOrigin);
        if (distance > radius) return;

        float falloff = 1f - (distance / radius);
        Vector3 dir = (transform.position - explosionOrigin).normalized;
        _velocity += dir * rocketJumpForce * falloff;
    }
}
