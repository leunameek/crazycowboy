using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movimiento")]
    public float moveSpeed = 6f;
    public float acceleration = 25f;
    public float friction = 15f;
    
    [Header("Saltoo")]
    public float jumpForce = 12f;
    public float gravity = 30f;
    public float maxFallSpeed = 20f;
    public float coyoteTime = 0.12f;
    public float jumpBufferTime = 0.08f;
    
    private InputAction moveAction;
    private InputAction jumpAction;
    
    private Vector2 velocity;
    private bool isGrounded;
    private float groundedTimer;
    private float jumpBufferTimer;

    public Vector2 Velocity => velocity;
    public bool IsGrounded 
    { 
        get => isGrounded;
        set => isGrounded = value;
    }
    
    void Start()
    {
        InitializeInput();
    }
    
    void InitializeInput()
    {
        var inputAsset = InputSystem.actions;
        moveAction = inputAsset.FindAction("Player/Move");
        jumpAction = inputAsset.FindAction("Player/Jump");
        
        if (moveAction != null) moveAction.Enable();
        if (jumpAction != null) jumpAction.Enable();
    }

    public void HandleInput()
    {
        float horizontal = moveAction != null ? moveAction.ReadValue<Vector2>().x : 0f;

        if (Mathf.Abs(horizontal) > 0.01f)
        {
            float targetSpeed = horizontal * moveSpeed;
            velocity.x = Mathf.MoveTowards(velocity.x, targetSpeed, acceleration * Time.deltaTime);
        }
        else if (isGrounded)
        {
            velocity.x = Mathf.MoveTowards(velocity.x, 0f, friction * Time.deltaTime);
        }

        if (jumpAction != null && jumpAction.WasPressedThisFrame())
            jumpBufferTimer = jumpBufferTime;
        
        jumpBufferTimer -= Time.deltaTime;
    }

    public void ApplyGravity()
    {
        if (!isGrounded)
        {
            velocity.y -= gravity * Time.fixedDeltaTime;
            velocity.y = Mathf.Max(velocity.y, -maxFallSpeed);
        }
        else if (velocity.y < 0)
        {
            velocity.y = 0;
        }
    }

    public void Jump()
    {
        velocity.y = jumpForce;
        isGrounded = false;
        groundedTimer = 0;

        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlayJump();
        }
    }

    public void UpdateTimers()
    {
        if (isGrounded) 
            groundedTimer = coyoteTime;
        else 
            groundedTimer -= Time.fixedDeltaTime;
    }

    public bool ShouldJump()
    {
        return jumpBufferTimer > 0 && (isGrounded || groundedTimer > 0);
    }

    public void ConsumeJumpBuffer()
    {
        jumpBufferTimer = 0;
    }

    public void SetVelocity(Vector2 newVelocity)
    {
        velocity = newVelocity;
    }

    public void SetVelocityX(float x)
    {
        velocity.x = x;
    }

    public void SetVelocityY(float y)
    {
        velocity.y = y;
    }
}
