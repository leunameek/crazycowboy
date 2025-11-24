using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class LevelExit : MonoBehaviour
{
    [Header("Exit Settings")]
    [Tooltip("Name of the specific level to load. If empty, loads the next level in Build Settings.")]
    public string nextLevelName;

    [Header("Collision Bounds")]
    [Tooltip("Optional override. If left at zero, the BoxCollider2D size is used.")]
    public Vector2 sizeOverride;
    public Vector2 offset;

    private BoxCollider2D box;

    void Awake()
    {
        box = GetComponent<BoxCollider2D>();
        if (box != null)
        {
            box.isTrigger = true;
        }
    }

    public bool Overlaps(Vector2 targetCenter, Vector2 targetHalfSize)
    {
        Vector2 exitHalfSize = GetHalfSize();
        Vector2 exitCenter = (Vector2)transform.position + offset;

        return Mathf.Abs(targetCenter.x - exitCenter.x) <= (targetHalfSize.x + exitHalfSize.x) &&
               Mathf.Abs(targetCenter.y - exitCenter.y) <= (targetHalfSize.y + exitHalfSize.y);
    }

    Vector2 GetHalfSize()
    {
        if (sizeOverride != Vector2.zero)
            return sizeOverride * 0.5f;

        if (box != null)
            return box.size * 0.5f;

        return Vector2.one * 0.5f;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = new Color(0, 1, 0, 0.5f);
        Vector2 center = (Vector2)transform.position + offset;
        Vector2 size = GetHalfSize() * 2;
        Gizmos.DrawCube(center, size);
    }
}
