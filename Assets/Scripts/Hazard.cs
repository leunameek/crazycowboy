using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class Hazard : MonoBehaviour
{
    [Header("Limites del Peligro el cactus chuza")]
    [Tooltip("Opcional. Si lo dejas a cero, usa el tamaño del BoxCollider2D")]
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
        Vector2 hazardHalfSize = GetHalfSize();
        Vector2 hazardCenter = (Vector2)transform.position + offset;

        return Mathf.Abs(targetCenter.x - hazardCenter.x) <= (targetHalfSize.x + hazardHalfSize.x) &&
               Mathf.Abs(targetCenter.y - hazardCenter.y) <= (targetHalfSize.y + hazardHalfSize.y);
    }

    Vector2 GetHalfSize()
    {
        if (sizeOverride != Vector2.zero)
            return sizeOverride * 0.5f;

        if (box != null)
            return box.size * 0.5f;

        return Vector2.one * 0.5f;
    }
}
