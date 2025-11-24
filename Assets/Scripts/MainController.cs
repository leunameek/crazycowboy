using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(PlayerMovement))]
[RequireComponent(typeof(CollisionResolver))]
[RequireComponent(typeof(TilemapCollisionDetector))]
public class MainController : MonoBehaviour
{
    [Header("El Mapa")]
    public Tilemap tilemap;
    [Header("Renacer de las cenizas")]
    public Transform spawnPoint;
    public float respawnDelay = 0.4f;
    [Header("Cosas que matan")]
    public Hazard[] hazards;
    [Header("Pa fuera")]
    public LevelExit[] levelExits;
    [Header("Musiquita")]
    public bool playLevelMusicOnStart = true;

    private PlayerMovement movement;
    private CollisionResolver collisionResolver;
    private TilemapCollisionDetector tilemapDetector;
    private PlayerAnimationController animationController;

    private bool isRespawning;
    private bool isExitingLevel;
    private Vector2 spawnPosition;
    private Vector2 position;
    
    void Start()
    {
        InitializeComponents();
        spawnPosition = spawnPoint != null ? (Vector2)spawnPoint.position : (Vector2)transform.position;
        position = spawnPosition;
        transform.position = position;

        if (playLevelMusicOnStart && MusicManager.Instance != null)
        {
            MusicManager.Instance.PlayLevelMusic();
        }
    }
    
    void InitializeComponents()
    {
        movement = GetComponent<PlayerMovement>();
        collisionResolver = GetComponent<CollisionResolver>();
        tilemapDetector = GetComponent<TilemapCollisionDetector>();
        animationController = GetComponent<PlayerAnimationController>();

        if (hazards == null || hazards.Length == 0)
        {
            hazards = FindObjectsByType<Hazard>(FindObjectsSortMode.None);
        }

        if (levelExits == null || levelExits.Length == 0)
        {
            levelExits = FindObjectsByType<LevelExit>(FindObjectsSortMode.None);
        }

        if (tilemapDetector != null)
        {
            tilemapDetector.tilemap = tilemap;
        }
    }
    
    void Update()
    {
        if (isRespawning || isExitingLevel) return;
        movement.HandleInput();
    }
    
    void FixedUpdate()
    {
        if (isRespawning || isExitingLevel) return;
        movement.ApplyGravity();

        if (movement.ShouldJump())
        {
            movement.Jump();
            movement.ConsumeJumpBuffer();
        }

        collisionResolver.ResetCollisions();

        Vector2 velocity = movement.Velocity;
        Vector2 delta = velocity * Time.fixedDeltaTime;

        position = collisionResolver.MoveHorizontal(position, delta.x, ref velocity);

        if (!collisionResolver.IsGroundBelow(position))
        {
            movement.IsGrounded = false;
        }

        bool grounded;
        position = collisionResolver.MoveVertical(position, delta.y, ref velocity, out grounded);
        movement.IsGrounded = grounded;

        movement.SetVelocity(velocity);

        transform.position = position;

        movement.UpdateTimers();

        if (CheckHazardCollision())
        {
            KillPlayer();
        }
        else
        {
            CheckLevelExitCollision();
        }
    }

    public void KillPlayer()
    {
        if (!gameObject.activeInHierarchy || isRespawning || isExitingLevel) return;
        StartCoroutine(RespawnRoutine());
    }

    IEnumerator RespawnRoutine()
    {
        isRespawning = true;

        if (movement != null)
        {
            movement.SetVelocity(Vector2.zero);
            movement.IsGrounded = false;
            movement.ConsumeJumpBuffer();
        }

        if (animationController != null)
        {
            animationController.SetDeathState(true);
        }

        if (MusicManager.Instance != null)
        {
            MusicManager.Instance.PlayLevelMusic(restart: true);
        }

        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlayDeath();
        }

        yield return new WaitForSeconds(respawnDelay);

        position = spawnPosition;
        transform.position = position;

        if (movement != null)
        {
            movement.SetVelocity(Vector2.zero);
            movement.IsGrounded = false;
            movement.ConsumeJumpBuffer();
        }

        if (collisionResolver != null)
        {
            collisionResolver.ResetCollisions();
        }

        if (animationController != null)
        {
            animationController.SetDeathState(false);
        }

        isRespawning = false;
    }

    bool CheckHazardCollision()
    {
        if (hazards == null || hazards.Length == 0 || collisionResolver == null) return false;

        Vector2 halfSize = collisionResolver.size * 0.5f;

        for (int i = 0; i < hazards.Length; i++)
        {
            var hazard = hazards[i];
            if (hazard == null || !hazard.isActiveAndEnabled) continue;

            if (hazard.Overlaps(position, halfSize))
            {
                return true;
            }
        }

        return false;
    }

    void CheckLevelExitCollision()
    {
        if (levelExits == null || levelExits.Length == 0 || collisionResolver == null) return;

        Vector2 halfSize = collisionResolver.size * 0.5f;

        for (int i = 0; i < levelExits.Length; i++)
        {
            var exit = levelExits[i];
            if (exit == null || !exit.isActiveAndEnabled) continue;

            if (exit.Overlaps(position, halfSize))
            {
                LoadNextLevel(exit);
                return;
            }
        }
    }

    void LoadNextLevel(LevelExit exit)
    {
        isExitingLevel = true;
        
        if (!string.IsNullOrEmpty(exit.nextLevelName))
        {
            SceneManager.LoadScene(exit.nextLevelName);
        }
        else
        {
            int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
            if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
            {
                SceneManager.LoadScene(nextSceneIndex);
            }
            else
            {
                Debug.Log("Se acabaron los niveles! A casa!");
                SceneManager.LoadScene(0); 
            }
        }
    }
    
    void OnDrawGizmos()
    {
        Vector2 currentPos = Application.isPlaying ? position : (Vector2)transform.position;

        if (!Application.isPlaying)
        {
            collisionResolver = GetComponent<CollisionResolver>();
            tilemapDetector = GetComponent<TilemapCollisionDetector>();
            movement = GetComponent<PlayerMovement>();
        }
        
        if (collisionResolver != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireCube(currentPos, collisionResolver.size);
        }
        
        if (Application.isPlaying && movement != null && collisionResolver != null)
        {
            Gizmos.color = movement.IsGrounded ? Color.green : Color.red;
            Vector2 groundCheckPos = currentPos + new Vector2(0, -collisionResolver.size.y * 0.5f - 0.05f);
            Gizmos.DrawWireCube(groundCheckPos, new Vector2(collisionResolver.size.x * 0.8f, 0.1f));
        }
        
        if (Application.isPlaying && movement != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(currentPos, currentPos + movement.Velocity * Time.fixedDeltaTime * 10f);
        }

        if (Application.isPlaying && collisionResolver != null)
        {
            float pointSize = 0.08f;
            var collisions = collisionResolver.GetCollisionState();
            
            Gizmos.color = collisions.left ? Color.red : Color.green;
            Gizmos.DrawSphere(currentPos + new Vector2(-collisionResolver.size.x * 0.5f, 0), pointSize);
            
            Gizmos.color = collisions.right ? Color.red : Color.green;
            Gizmos.DrawSphere(currentPos + new Vector2(collisionResolver.size.x * 0.5f, 0), pointSize);
            
            Gizmos.color = collisions.top ? Color.red : Color.green;
            Gizmos.DrawSphere(currentPos + new Vector2(0, collisionResolver.size.y * 0.5f), pointSize);
            
            Gizmos.color = collisions.bottom ? Color.red : Color.green;
            Gizmos.DrawSphere(currentPos + new Vector2(0, -collisionResolver.size.y * 0.5f), pointSize);
        }

        if (tilemapDetector != null && tilemapDetector.tilemap != null && collisionResolver != null)
        {
            Gizmos.color = new Color(1, 0, 0, 0.3f);
            Vector2 halfSize = collisionResolver.size * 0.5f - Vector2.one * tilemapDetector.skinWidth;
            var tiles = tilemapDetector.GetOverlappingTiles(currentPos, halfSize);
            
            foreach (var cellPos in tiles)
            {
                Vector3 cellCenter = tilemapDetector.GetCellCenterWorld(cellPos);
                Gizmos.DrawCube(cellCenter, tilemapDetector.GetCellSize() * 0.9f);
            }
        }
    }
}
