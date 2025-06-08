using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEditor;
using UnityEngine;

public static class GridHandler
{
    //Classes
    //Structs

    public static GridBounds gridBounds;
    public static GridMetrics gridMetrics;
    public static GridBorders gridBorders;
    public static GridInfo gridInfo;

    //Public Var
    public static int gridMaxX;
    public static int gridMinX;
    public static int gridMaxY;
    public static int gridMinY;

    public static int gridWidth;
    public static int gridHeight;
    public static int gridMargin;

    public static Vector2Int gridCenter;
    public static Vector2Int gridRandPosition;

    [HideInInspector] public static List<Vector2Int> exemptSpawnPositions = new List<Vector2Int>();


    //Private Var
    private static GameObject gridAnchorMaxX;
    private static GameObject gridAnchorMinX;
    private static GameObject gridAnchorMaxY;
    private static GameObject gridAnchorMinY;

    public struct GridBounds
    {
        public int MinX, MaxX, MinY, MaxY;

        public GridBounds(int gridMinX, int gridMaxX, int gridMinY, int gridMaxY)
        {
            MinX = gridMinX;
            MaxX = gridMaxX;
            MinY = gridMinY;
            MaxY = gridMaxY;
        }
    }

    public struct GridMetrics
    {
        public int Width, Height, CenterX, CenterY, Margin;

        public GridMetrics(int gridHeight, int gridWidth, int gridCenterX, int gridCenterY, int gridMargin)
        {
            Width = gridWidth;
            Height = gridHeight;
            CenterX = gridCenterX;
            CenterY = gridCenterY;
            Margin = gridMargin;
        }
    }

    public struct GridBorders
    {
        public int TopBorder, BottomBorder, LeftBorder, RightBorder;

        public GridBorders(int gridTopBorder, int gridBottomBorder, int gridLeftBorder, int gridRightBorder)
        {
            TopBorder = gridTopBorder;
            BottomBorder = gridBottomBorder;
            LeftBorder = gridLeftBorder;
            RightBorder = gridRightBorder;
        }
    }

    public struct GridInfo
    {
        public GridBounds Bounds;
        public GridMetrics Metrics;
        //public GridBorders Borders;

        public GridInfo(GridBounds bounds, GridMetrics metrics)
        {
            Bounds = bounds;
            Metrics = metrics;
        }
    }

    public static GridInfo CreateGrid(GameObject gridAnchorSet, GameObject borderSet)
    {
        // Unpacks passed prefabs
        GetAnchorSetChildren(gridAnchorSet);
        GetBorderSetChildren(borderSet);

        // Gets, fills, and returns structs
        gridBounds = GetGridBounds();
        Debug.Log($"After GetGridBounds() Call: MinX={gridBounds.MinX}, MaxX={gridBounds.MaxX}, MinY={gridBounds.MinY}, MaxY={gridBounds.MaxY}");
        gridMetrics = GetGridMetrics();
        //gridBorders = GetGridBorders();

        return new GridInfo(gridBounds, gridMetrics);
    }

    private static void GetAnchorSetChildren(GameObject gridAnchorSet)
    {
        // Unpack prefab and find each anchor
        Transform gridAnchorSetTransform = gridAnchorSet.transform;

        gridAnchorMinX = gridAnchorSetTransform.Find("GridAnchorMinX").gameObject;
        gridAnchorMaxX = gridAnchorSetTransform.Find("GridAnchorMaxX").gameObject;

        gridAnchorMinY = gridAnchorSetTransform.Find("GridAnchorMinY").gameObject;
        gridAnchorMaxY = gridAnchorSetTransform.Find("GridAnchorMaxY").gameObject;
    }

    private static void GetBorderSetChildren(GameObject borderSet)
    {
        // Unpack prefab and find each border
        Transform borderSetTransform = borderSet.transform;

        GameObject borderSetLeft = borderSetTransform.Find("BorderLeft").gameObject;
        GameObject borderSetRight = borderSetTransform.Find("BorderRight").gameObject;
        GameObject borderSetTop = borderSetTransform.Find("BorderTop").gameObject;
        GameObject borderSetBottom = borderSetTransform.Find("BorderBottom").gameObject;
    }

    public static GridBounds GetGridBounds()
    {
        // Grid dimensions
        gridMaxX = Mathf.RoundToInt(gridAnchorMaxX.transform.position.x);
        gridMinX = Mathf.RoundToInt(gridAnchorMinX.transform.position.x);
        gridMaxY = Mathf.RoundToInt(gridAnchorMaxY.transform.position.y);
        gridMinY = Mathf.RoundToInt(gridAnchorMinY.transform.position.y);

        //Debug.Log(gridMaxX);
        //Debug.Log(gridMinX);
        //Debug.Log(gridMaxY);
        //Debug.Log(gridMinY);

        gridBounds = new GridBounds(gridMinX, gridMaxX, gridMinY, gridMaxY);
        Debug.Log($"GridBounds updated: MinX: {gridBounds.MinX}, MaxX: {gridBounds.MaxX}, MinY: {gridBounds.MinY}, MaxY: {gridBounds.MaxY}");


        return gridBounds;
    }

    public static GridMetrics GetGridMetrics()
    {
        // Gets Grid width and height
        gridWidth = gridMaxX - gridMinX;
        gridHeight = gridMaxY - gridMinY;

        // Creates Grid margin for food spawn
        gridMargin = 1;

        // Grid center position
        gridCenter = new Vector2Int((gridMinX + (gridWidth / 2)), (gridMinY + (gridHeight / 2)));
        int gridCenterX = gridCenter.x;
        int gridCenterY = gridCenter.y;

        gridMetrics = new GridMetrics(gridWidth, gridHeight, gridCenterX, gridCenterY, gridMargin);

        return gridMetrics;
    }

    //public static GridBorders GetGridBorders()

    // Returns random unoccupied grid position of type Vector2
    public static Vector2Int RandomGridPosition(List<Vector2Int> exemptSpawnPositions)
    {
        do
        {
            gridRandPosition.x = Random.Range((gridMinX + gridMargin), (gridMaxX - gridMargin));
            gridRandPosition.y = Random.Range((gridMinY + gridMargin), (gridMaxY - gridMargin));
        } while (exemptSpawnPositions.Contains(gridRandPosition));

        //Debug.Log($"Random Grid Position: {randomGridPosition}");

        return gridRandPosition;
    }

    // Adds an object and it's position to a dictionary to keep track of occupied grid cells
    public static void AddOccupiedPosition(Vector2Int objPos, GameObject obj, Dictionary<Vector2Int, GameObject> objectPositions)
    {
        if (!objectPositions.ContainsKey(objPos))
        {
            // Debug.Log($"Object {obj.name} registered at {objPos}");
            objectPositions[objPos] = obj;
            // Debug.Log($"Position: {objPos} added to {exemptSpawnPositions}");
            exemptSpawnPositions.Add(objPos);
        }
    }

    // Removes an object and its position from the occupied cell dictionary
    public static void RemoveOccupiedPosition(Vector2Int objPos, Dictionary<Vector2Int, GameObject> objectPositions)
    {
        if (objectPositions.ContainsKey(objPos))
        {
            // Debug.Log($"Object {objectPositions[objPos].name} removed from {objPos}");
            objectPositions.Remove(objPos);
            // Debug.Log($"Position: {objPos} removed from {exemptSpawnPositions}");
            exemptSpawnPositions.Remove(objPos);
        }
    }

}
