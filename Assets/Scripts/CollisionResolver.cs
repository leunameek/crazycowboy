using UnityEngine;

[RequireComponent(typeof(TilemapCollisionDetector))]
public class CollisionResolver : MonoBehaviour
{
    [Header("Caja de Colisión")]
    public Vector2 size = new Vector2(0.45f, 0.9f);
    
    private TilemapCollisionDetector detector;
    private CollisionState collisions;

    public struct CollisionState
    {
        public bool left, right, top, bottom;
        
        public void Reset()
        {
            left = right = top = bottom = false;
        }
    }
    
    void Awake()
    {
        detector = GetComponent<TilemapCollisionDetector>();
    }

    public CollisionState GetCollisionState()
    {
        return collisions;
    }

    public void ResetCollisions()
    {
        collisions.Reset();
    }

    public Vector2 MoveHorizontal(Vector2 from, float deltaX, ref Vector2 velocity)
    {
        if (Mathf.Abs(deltaX) < 0.001f) return from;
        
        Vector2 to = from + new Vector2(deltaX, 0);
        Vector2 halfSize = size * 0.5f - Vector2.one * detector.skinWidth;
        
        // chequeamos si nos estampamos contra algo
        if (detector.CheckTileOverlap(to, halfSize))
        {
            float direction = Mathf.Sign(deltaX);
            float distance = Mathf.Abs(deltaX);
            float safeDistance = detector.BinarySearchCollision(from, new Vector2(direction, 0), distance, halfSize);
            
            to = from + new Vector2(direction * safeDistance, 0);
            velocity.x = 0; // frenazo en seco

            collisions.left = deltaX < 0;
            collisions.right = deltaX > 0;
        }
        
        return to;
    }

    public Vector2 MoveVertical(Vector2 from, float deltaY, ref Vector2 velocity, out bool grounded)
    {
        grounded = false;
        
        if (Mathf.Abs(deltaY) < 0.001f) return from;
        
        Vector2 to = from + new Vector2(0, deltaY);
        Vector2 halfSize = size * 0.5f - Vector2.one * detector.skinWidth;
        
        // miramos si nos damos en la cabeza o en los pies
        if (detector.CheckTileOverlap(to, halfSize))
        {
            float direction = Mathf.Sign(deltaY);
            float distance = Mathf.Abs(deltaY);
            float safeDistance = detector.BinarySearchCollision(from, new Vector2(0, direction), distance, halfSize);
            
            to = from + new Vector2(0, direction * safeDistance);
            velocity.y = 0; // Parada técnica

            collisions.bottom = deltaY < 0;
            collisions.top = deltaY > 0;
            grounded = collisions.bottom; // Estamos en el suelo
        }
        else
        {
            grounded = false;
        }
        
        return to;
    }

    public bool IsGroundBelow(Vector2 position)
    {
        return detector.CheckGroundBelow(position, size);
    }
}