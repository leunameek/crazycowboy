using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(PlayerMovement))]
[RequireComponent(typeof(SpriteRenderer))]
public class PlayerAnimationController : MonoBehaviour
{
    [Header("Cosas de Animacion (Bailes)")]
    [Tooltip("Velocidad minima para que mueva las patas")]
    public float walkSpeedThreshold = 0.1f;
    
    [Tooltip("rotar dependiendo direccion")]
    public bool flipSpriteOnDirection = true;

    private Animator animator;
    private PlayerMovement movement;
    private SpriteRenderer spriteRenderer;
    
    private int speedHash;
    private int isGroundedHash;
    private int velocityYHash;
    private int deathBoolHash;
    
    [Header("muerre anim")]
    [Tooltip("si muere usamos el bool para animar")]
    public string deathBoolParam = "IsDead";
    
    void Awake()
    {
        animator = GetComponent<Animator>();
        movement = GetComponent<PlayerMovement>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        speedHash = Animator.StringToHash("Speed");
        isGroundedHash = Animator.StringToHash("IsGrounded");
        velocityYHash = Animator.StringToHash("VelocityY");
        deathBoolHash = Animator.StringToHash(deathBoolParam);
    }
    
    void Update()
    {
        UpdateAnimationParameters();
        UpdateSpriteDirection();
    }

    void UpdateAnimationParameters()
    {
        if (animator == null || movement == null) return;
        
        Vector2 velocity = movement.Velocity;

        float speed = Mathf.Abs(velocity.x);
        animator.SetFloat(speedHash, speed);

        animator.SetBool(isGroundedHash, movement.IsGrounded);

        animator.SetFloat(velocityYHash, velocity.y);
    }
    void UpdateSpriteDirection()
    {
        if (!flipSpriteOnDirection || spriteRenderer == null || movement == null) return;
        
        float velocityX = movement.Velocity.x;

        if (Mathf.Abs(velocityX) > walkSpeedThreshold)
        {
            spriteRenderer.flipX = velocityX < 0;
        }
    }

    public void PlayAnimation(string stateName)
    {
        if (animator != null)
        {
            animator.Play(stateName);
        }
    }

    public void SetAnimatorBool(string paramName, bool value)
    {
        if (animator != null)
        {
            animator.SetBool(paramName, value);
        }
    }

    public void SetAnimatorFloat(string paramName, float value)
    {
        if (animator != null)
        {
            animator.SetFloat(paramName, value);
        }
    }

    public void TriggerAnimation(string triggerName)
    {
        if (animator != null)
        {
            animator.SetTrigger(triggerName);
        }
    }

    public void SetDeathState(bool isDead)
    {
        if (animator == null) return;

        if (!string.IsNullOrEmpty(deathBoolParam))
        {
            animator.SetBool(deathBoolHash, isDead);
        }
    }
}
