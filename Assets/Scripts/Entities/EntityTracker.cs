using System.Collections.Generic;
using UnityEngine;

public static class EntityTracker
{
    public static Dictionary<int, (string type, GameObject entity)> entities = new Dictionary<int, (string type, GameObject entity)>();


    // Register the entity to entity dict
    public static void RegisterEntity(string type, GameObject entity)
    {
        int newID = entity.GetInstanceID();

        if (!entities.ContainsKey(newID) && entity != null)
        {
            entities[newID] = (type, entity);
            Debug.Log($"Entity registered. Object: {entities[newID].entity} ID: {newID} Type: {entities[newID].type}");
        }
    }

    // Unregister the entity from entity dict
    public static void UnregisterEntity(int id)
    {
        if (entities.ContainsKey(id) && entities[id].entity != null)
        {
            Debug.Log($"Entity removed. Object: {entities[id].entity} ID: {id}");
            entities.Remove(id);
        }
    }

    // Gets EntityID when passed a Game Object target
    public static int GetEntityID(GameObject target)
    {
        foreach (var entry in entities)
        {
            if (entry.Value.entity == target)
            {
                return entry.Key;
            }
        }

        Debug.LogWarning($"EntityID not found: {target.name}");
        return -1;
    }
    
    // Gets the entity position of type Vector2Int
    public static Vector2Int GetEntityPosition(int id)
    {
        foreach (var entry in entities)
        {
            if (entry.Key == id)
            {
                Vector2Int entityPos = PositionConversion.Vector3ToInt(entities[id].entity.transform.position);
                return entityPos;
            }
        }

        Debug.LogWarning($"Entity not found in entities");
        return new Vector2Int(-1, -1);
    }
}
