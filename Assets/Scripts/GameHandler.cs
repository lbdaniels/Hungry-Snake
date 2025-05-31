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

    // Classes
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
    [HideInInspector] public GameObject snakeHead;
    [HideInInspector] public Vector2Int playerPos;
    [HideInInspector] public float playerMoveTimerMax;
    [HideInInspector] public string playerMoveDirection;
    [HideInInspector] public List<Vector2Int> playerPositions = new List<Vector2Int>();

    // Snake Tail Var
    [HideInInspector] public float snakeSegmentDeathTimer;
    [HideInInspector] public int numOfTailSegments;

    [HideInInspector] public List<GameObject> tailSegments = new List<GameObject>();
    [HideInInspector] public int newTailSegmentIndex;

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

        // Get necessary components for Game Handler
        GetGameHandlerComponents();

        // Assign values to variables 
        AssignValues();
    }

    void Start()
    {
        // Confirm Game Start
        Debug.Log("Game start");

        // Initialize grid using GridHandler Script and get grid information
        Instantiate(gridAnchorSet);
        Instantiate(borderSet);

        gridData = GridHandler.CreateGrid(gridAnchorSet, borderSet);
        Debug.Log($" Stored GridBounds: MinX={gridData.Bounds.MinX}, MaxX={gridData.Bounds.MaxX}, MinY={gridData.Bounds.MinY}, MaxY={gridData.Bounds.MaxY}");

        // Spawn the player
        spawnHandler.SpawnSnake();

        // Spawn first food item
        SpawnCaller("Food", "Time For Food");
    }

    private void Update()
    {
        UpdateTimeInfo();
    }

    // Gets necessary components
    private void GetGameHandlerComponents()
    {
        gameTime = GetComponent<GameTime>();
        spawnHandler = GetComponent<SpawnHandler>();
        gameAssets = GetComponent<GameAssets>();
    }

    // Assigns values to necessary variables
    private void AssignValues()
    {
        gameTimerActive = gameTime.gameTimerActive;
        currentGameTime = gameTime.currentGameTime;

        playerMoveTimerMax = gameTime.snakeMoveTimerMax;
        snakeSegmentDeathTimer = gameTime.snakeSegmentDeathTimer;

        playerPos = new Vector2Int(25, 25);

        playerAteFood = false;
        timeForFood = false;
        timeForExtraFood = false;

        numOfTailSegments = tailSegments.Count;
    }

    // Changes a game objects color when provided the object and the desired color
    public void ChangeColor(GameObject targetObject, Color newColor)
    {
        SpriteRenderer spriteRenderer = targetObject.GetComponent<SpriteRenderer>();
        spriteRenderer.color = newColor;
    }

    // Converts a Vector3 into a Vector2Int
    public Vector2Int ConvertVector3ToVector2Int(Vector3 vector)
    {
        return new Vector2Int(Mathf.RoundToInt(vector.x), Mathf.RoundToInt(vector.y));
    }

    public void UpdateTimeInfo()
    {
        gameTimerActive = gameTime.gameTimerActive;
        currentGameTime = gameTime.currentGameTime;

        playerMoveTimerMax = gameTime.snakeMoveTimerMax;
        snakeSegmentDeathTimer = gameTime.snakeSegmentDeathTimer;
    }

    // Updates player info for the gamehandler
    public void UpdatePlayerInfo(Vector2Int playerGridPosition)
    {
        playerPos = playerGridPosition;

        playerPositions.Insert(0, playerPos);

        int maxPosHistory = tailSegments.Count + 2;

        if (playerPositions.Count > maxPosHistory)
        {
            playerPositions.RemoveAt(playerPositions.Count - 1);
        }
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
    }

    // Moves the tail segments. Primarily called by the MovePlayer function in the PlayerController class
    public void UpdateTailPositions()
    {
        if (tailSegments.Count == 0 || playerPositions.Count <= tailSegments.Count) return;

        for (int i = tailSegments.Count - 1; i >= 0; i--)
        {
            //Debug.Log($"Processing TailSegment[{i}], Position Before: {tailSegments[i].transform.position}");

            if (i < playerPositions.Count)
            {
                Vector2Int tailSegmentPos = ConvertVector3ToVector2Int(tailSegments[i].transform.position);
                RemoveOccupiedPosition(tailSegmentPos, occupiedCells);

                tailSegments[i].transform.position = new Vector3(playerPositions[i + 1].x,
                                                         playerPositions[i + 1].y,
                                                         tailSegments[i].transform.position.z);

                tailSegmentPos = ConvertVector3ToVector2Int(tailSegments[i].transform.position);
                AddOccupiedPosition(tailSegmentPos, tailSegments[i], occupiedCells);
            }

            //Debug.Log($"Processing TailSegment[{i}], Position After: {tailSegments[i].transform.position}");
        }

    }
}