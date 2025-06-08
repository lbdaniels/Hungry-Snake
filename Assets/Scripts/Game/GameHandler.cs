using System;
using System.Collections.Generic;
using NUnit.Framework.Constraints;
using UnityEngine;
using UnityEngine.Tilemaps;
using static GridHandler;

public class GameHandler : MonoBehaviour
{
    // GameHandler Instance
    public static GameHandler Instance { get; private set; }

    // Classes & Structs
    GameTime gameTime;
    SpawnHandler spawnHandler;
    GameAssets gameAssets;

    // Grid Var
    public GameObject gridAnchorSet;
    public GameObject borderSet;
    [HideInInspector] public GridInfo gridData;

    // Game Timer Var
    [HideInInspector] public float currentGameTime;

    // Snake Head (Player) Var
  
    [HideInInspector] public Vector2Int playerPos;
    [HideInInspector] public float playerMoveTimerMax;
    [HideInInspector] public string playerMoveDirection;
    [HideInInspector] public List<Vector2Int> playerPositions = new List<Vector2Int>();

    // Snake Tail Var

    // Occupied Positions
    [HideInInspector] public Dictionary<Vector2Int, GameObject> occupiedCells = new Dictionary<Vector2Int, GameObject>();

    // Bools
    [HideInInspector] public bool gameTimerActive;
    [HideInInspector] public bool playerAteFood;
    [HideInInspector] public bool timeForFood;
    [HideInInspector] public bool timeForExtraFood;



    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        // Assign values to variables 
        AssignValues();

        // Initialize grid using GridHandler Script and get grid information
        InitializeGrid();

        // Reserve spawn area for player
        
    }

    void Start()
    {
        // Confirm Game Start
        Debug.Log("Game start");

        spawnHandler.FirstSpawns();
    }

    private void Update()
    {
        
    }

    private void InitializeGrid()
    {
        Instantiate(gridAnchorSet);
        Instantiate(borderSet);

        gridData = GridHandler.CreateGrid(gridAnchorSet, borderSet);
        Debug.Log($" Stored GridBounds: MinX={gridData.Bounds.MinX}, MaxX={gridData.Bounds.MaxX}, MinY={gridData.Bounds.MinY}, MaxY={gridData.Bounds.MaxY}");
    }

    // Assigns values to necessary variables
    private void AssignValues()
    {
        gameTime = GetComponent<GameTime>();
        spawnHandler = GetComponent<SpawnHandler>();
        gameAssets = GetComponent<GameAssets>();

        gameTimerActive = gameTime.gameTimerActive;
        //currentGameTime = gameTime.currentGameTime;

        playerPos = new Vector2Int(25, 25);

        playerAteFood = false;
        timeForFood = false;
        timeForExtraFood = false;
    }

    // Changes a game objects color when provided the object and the desired color
    public void ChangeColor(GameObject targetObject, Color newColor)
    {
        SpriteRenderer spriteRenderer = targetObject.GetComponent<SpriteRenderer>();
        spriteRenderer.color = newColor;
    }

    // Function that calls various spawn functions based on passed targets and spawn reason
    public void SpawnCaller(string spawnTarget, string spawnReason)
    {
        Debug.Log($"SpawnCaller called. Target:{spawnTarget} Reason:{spawnReason}");

        if (spawnTarget == "Food")
        {
            if (spawnReason == "Player Ate Food")
            {
                spawnHandler.SpawnFood();
                playerAteFood = false;
            }

            if (spawnReason == "Time For Food")
            {
                spawnHandler.SpawnFood();
                timeForFood = false;
            }

            if (spawnReason == "Time For Extra Food")
            {
                spawnHandler.SpawnFood();
                timeForExtraFood = false;
            }
        }

        if (spawnTarget == "Tail")
        {
            spawnHandler.SpawnTailSegment();
        }

        if (spawnTarget == "Stone")
        {
            if (spawnReason == "Time For Stone")
            {
                spawnHandler.SpawnStone();
            }
        }
    }
}