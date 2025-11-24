using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapCollisionDetector : MonoBehaviour
{
    [Header("Tilemap Reference")]
    public Tilemap tilemap;
    
    [Header("Collision Settings")]
    public float skinWidth = 0.01f;

    public bool CheckTileOverlap(Vector2 center, Vector2 halfSize)
    {
        if (tilemap == null) return false;
        
        Vector2 min = center - halfSize;
        Vector2 max = center + halfSize;
        
        Vector3Int cellMin = tilemap.WorldToCell(min);
        Vector3Int cellMax = tilemap.WorldToCell(max);
        
        for (int x = cellMin.x; x <= cellMax.x; x++)
        {
            for (int y = cellMin.y; y <= cellMax.y; y++)
            {
                Vector3Int cellPos = new Vector3Int(x, y, 0);
                if (!tilemap.HasTile(cellPos)) continue;
                
                if (tilemap.GetColliderType(cellPos) == Tile.ColliderType.None) continue;
                
                return true;
            }
        }
        
        return false;
    }

    public float BinarySearchCollision(Vector2 from, Vector2 direction, float maxDistance, Vector2 halfSize)
    {
        float min = 0f;
        float max = maxDistance;

        for (int i = 0; i < 6; i++)
        {
            float mid = (min + max) * 0.5f;
            Vector2 testPos = from + direction * mid;
            
            if (CheckTileOverlap(testPos, halfSize))
                max = mid;
            else
                min = mid;
        }
        
        return min;
    }

    public bool CheckGroundBelow(Vector2 position, Vector2 size)
    {
        Vector2 halfSize = size * 0.5f - Vector2.one * skinWidth;
        Vector2 checkPos = position + new Vector2(0, -size.y * 0.5f - 0.05f);
        
        return CheckTileOverlap(checkPos, halfSize);
    }

    public List<Vector3Int> GetOverlappingTiles(Vector2 center, Vector2 halfSize)
    {
        var tiles = new List<Vector3Int>();
        
        if (tilemap == null) return tiles;
        
        Vector2 min = center - halfSize;
        Vector2 max = center + halfSize;
        Vector3Int cellMin = tilemap.WorldToCell(min);
        Vector3Int cellMax = tilemap.WorldToCell(max);
        
        for (int x = cellMin.x; x <= cellMax.x; x++)
        {
            for (int y = cellMin.y; y <= cellMax.y; y++)
            {
                Vector3Int cellPos = new Vector3Int(x, y, 0);
                if (tilemap.HasTile(cellPos))
                    tiles.Add(cellPos);
            }
        }
        
        return tiles;
    }

    public Vector3 GetCellCenterWorld(Vector3Int cellPos)
    {
        return tilemap != null ? tilemap.GetCellCenterWorld(cellPos) : Vector3.zero;
    }

    public Vector3 GetCellSize()
    {
        return tilemap != null ? tilemap.cellSize : Vector3.one;
    }
}