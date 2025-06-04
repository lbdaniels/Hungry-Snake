using UnityEditorInternal;
using UnityEngine;

public static class PositionConversion
{
    // Convert Vector2 to Vector3 (defaults Z to zero)
    public static Vector3 Vector2To3(Vector2 position)
    {
        return new Vector3(position.x, position.y, 0f);
    }

    // Convert Vector2Int to Vector3 (defaults Z to zero)
    public static Vector3 Vector2IntTo3(Vector2Int position)
    {
        return new Vector3(position.x, position.y, 0f);
    }

    // Convert Vector2 to Vector2Int (rounds values)
    public static Vector2Int Vector2ToInt(Vector2 position)
    {
        return new Vector2Int(Mathf.RoundToInt(position.x), Mathf.RoundToInt(position.y));
    }

    // Convert Vector3 to Vector2Int (rounds values)
    public static Vector2Int Vector3ToInt(Vector3 position)
    {
        return new Vector2Int(Mathf.RoundToInt(position.x), Mathf.RoundToInt(position.y));
    }

    // Convert Vector2Int to Vector2
    public static Vector2 Vector2IntTo2(Vector2 position)
    {
        return new Vector2(position.x, position.y);
    }

    // Convert Vector3 to Vector2
    public static Vector2 Vector3To2(Vector3 position)
    {
        return new Vector2(position.x, position.y);
    }

    // Convert World Position to Local Position relative to a given transform
    public static Vector3 WorldToLocal(Transform reference, Vector3 worldPosition)
    {
        return reference.InverseTransformPoint(worldPosition);
    }

    // Convert Local Position to World Position relative to a given transform 
    public static Vector3 LocalToWorld(Transform reference, Vector3 localPosition)
    {
        return reference.TransformPoint(localPosition);
    }
}
