using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Threading;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public class PlayerController : MonoBehaviour
{
    // Classes
    InputManager inputManager;
    GameHandler gameHandler;
    public PlayerProfile playerProfile;
    public PlayerDefaults playerDefaults;

    // Components
    SpriteRenderer spriteRenderer;
    BoxCollider2D boxCollider;
    Rigidbody2D rigidBody;
    StatusHandler statusHandler;
    TailController tailController;

    // Structs
    private GridHandler.GridInfo gridData;
    private GridHandler.GridBounds gridBounds;
    private GridHandler.GridMetrics gridMetrics;

    
    // Var
    private float moveTimer;
    private float moveTimerMax;

    public float resistance;
    public float moveSpeed;
    public float mutation;
    public float strength;

    private InputAction upAction;
    private InputAction downAction;
    private InputAction leftAction;
    private InputAction rightAction;
    private Vector2Int moveDirection;
    private Vector2Int prevDirection;

    public Vector2Int playerGridPosition;

    private bool justWarped = false;
    private bool movedSinceWarp = true;

    List<GameObject> tailSegmentList = new List<GameObject>();

    private void Awake()
    {
        // Handles Player Input
        PlayerInputHandler();

        // Initializes values for later such as a move timer and player direction
        AwakeVar();
    }

    private void Start()
    {
        // Initializes values for later such as playerGridPosition
        StartVar();

        //StatChanger();
    }

    private void Update()
    {
        // Updates necessary variables for runtime use
        UpdateVar();

        // Gets the players move direction using their input
        GetMoveDirection();

        // Moves player in the move direction when the move timer ends
        MovePlayer();
    }

    private void AwakeVar()
    {
        gameHandler = GameHandler.Instance;

        InitializePlayerProfile();

        spriteRenderer = GetComponent<SpriteRenderer>();
        boxCollider = GetComponent<BoxCollider2D>();
        rigidBody = GetComponent<Rigidbody2D>();
        statusHandler = GetComponent<StatusHandler>();
        tailController = GetComponent<TailController>();

        // Uses the instance of the GridInfo struct in GameHandler to get grid data
        gridData = gameHandler.gridData;
        gridBounds = gridData.Bounds;
        gridMetrics = gridData.Metrics;
    }

    private void StartVar()
    {
        resistance = playerProfile.stats.resistance;
        moveSpeed = playerProfile.stats.moveSpeed;
        mutation = playerProfile.stats.mutation;
        strength = playerProfile.stats.strength;

        moveTimerMax = moveSpeed;
        moveTimer = moveTimerMax;

        moveDirection = new Vector2Int(1, 0);
        prevDirection = new Vector2Int(1, 0);

        playerGridPosition = PositionConversion.Vector3ToInt(transform.position);
    }

    private void UpdateVar()
    {
        moveTimerMax = gameHandler.playerMoveTimerMax;
    }

    private void PlayerInputHandler()
    {
        inputManager = gameObject.AddComponent<InputManager>();

        inputManager.GetInputActions();
        upAction = inputManager.upAction;
        downAction = inputManager.downAction;
        leftAction = inputManager.leftAction;
        rightAction = inputManager.rightAction;
    }

    private void InitializePlayerProfile()
    {
        playerProfile = PlayerProfile.Instance;

        PlayerProfile.Initialize(playerDefaults);
        playerProfile = PlayerProfile.Instance;

        Debug.Log(PlayerProfile.Instance != null ? "PlayerProfile initialized!" : "PlayerProfile is NULL!");
    }

    // Gets player input to decide the direction of the snake when it moves on timer reset
    private void GetMoveDirection()
    {
        if (upAction.IsPressed())
        {
            if (prevDirection.y != -1)
            {
                
                moveDirection.x = 0;
                moveDirection.y = 1;
                moveDirection = new Vector2Int(moveDirection.x, moveDirection.y);
            }
        }

        if (downAction.IsPressed())
        {
            if (prevDirection.y != 1) 
            {
                
                moveDirection.x = 0;
                moveDirection.y = -1;
                moveDirection = new Vector2Int(moveDirection.x, moveDirection.y);
            }
        }

        if (leftAction.IsPressed())
        {
            if (prevDirection.x != 1)
            {
                
                moveDirection.x = -1;
                moveDirection.y = 0;
                moveDirection = new Vector2Int(moveDirection.x, moveDirection.y);
            }
        }

        if (rightAction.IsPressed())
        {
            if (prevDirection.x != -1)
            {
                
                moveDirection.x = 1;
                moveDirection.y = 0;
                moveDirection = new Vector2Int(moveDirection.x, moveDirection.y);
            }
        }
    }

    // Moves the snake on a timer. Can change the timer value in the Game Time component in the inspector
    private void MovePlayer()
    {
        moveTimerMax = moveSpeed;
        moveTimer += Time.deltaTime;
        if (moveTimer >= moveTimerMax)
        {
            playerGridPosition += moveDirection;
            moveTimer = 0f;
            prevDirection = moveDirection;

            if (!justWarped)
            {
                WarpPlayer();
            }

            justWarped = false;
            movedSinceWarp = true;

            tailController.CreatePosHistory();
            tailController.UpdateTailPositions();

            Vector2Int playerPosRemove = PositionConversion.Vector3ToInt(transform.position);
            GridHandler.RemoveOccupiedPosition(playerPosRemove, gameHandler.occupiedCells);
        }

        transform.position = new Vector3(playerGridPosition.x, playerGridPosition.y);
        
        Vector2Int playerPosAdd = PositionConversion.Vector3ToInt(transform.position);
        GridHandler.AddOccupiedPosition(playerPosAdd, gameObject, gameHandler.occupiedCells);
    }

    // Warps the snake if it moves out of bounds. Flips the player to the opposite x or y position in the grid
    private void WarpPlayer()
    {
        if (movedSinceWarp)
        {
            if (playerGridPosition.x > gridBounds.MaxX)
            {
                //Debug.Log("Warping from MaxX to MinX");
                playerGridPosition.x = gridBounds.MinX;
                justWarped = true;
            }

            if (playerGridPosition.x < gridBounds.MinX)
            {
                //Debug.Log("Warping from MinX to MaxX");
                playerGridPosition.x = gridBounds.MaxX;
                justWarped = true;
            }

            if (playerGridPosition.y > gridBounds.MaxY)
            {
                //Debug.Log("Warping from MaxY to MinY");
                playerGridPosition.y = gridBounds.MinY;
                justWarped = true;
            }

            if (playerGridPosition.y < gridBounds.MinY)
            {
                //Debug.Log("Warping from MinX to MaxX");
                playerGridPosition.y = gridBounds.MaxY;
                justWarped = true;
            }
        }
    }

    private void StatChanger()
    {
        Debug.Log($"Player stats before change: Speed: {moveSpeed} Strength: {strength} Resistance: {resistance} Mutation: {mutation}");

        moveSpeed += 1;
        strength += 1;
        resistance += 1;
        mutation += 1;

        Debug.Log($"Player stats after change: Speed: {moveSpeed} Strength: {strength} Resistance: {resistance} Mutation: {mutation}");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Food"))
        {
            Debug.Log($"Collision detected: {other} at {playerGridPosition}");
            FoodCollision(other.gameObject);
        }

        if (other.CompareTag("Tail"))
        {
            Debug.Log($"Collision detected: {other} at {playerGridPosition}");
            TailCollision(other.gameObject);
        }
    }

    private void FoodCollision(GameObject foodItem)
    {
        Debug.Log($"Food Destroyed at {foodItem.transform.position}");
        Vector2Int foodPos = PositionConversion.Vector3ToInt(foodItem.transform.position);
        GridHandler.RemoveOccupiedPosition(foodPos, gameHandler.occupiedCells);

        EntityTracker.UnregisterEntity(EntityTracker.GetEntityID(foodItem));
        Destroy(foodItem);

        string spawnFood = "Food";
        string spawnTail = "Tail";
        string spawnReason = "Player Ate Food";

        gameHandler.playerAteFood = true;
        gameHandler.SpawnCaller(spawnFood, spawnReason);
        gameHandler.SpawnCaller(spawnTail, spawnReason);
    }

    private void TailCollision(GameObject tailSegment)
    {
        Debug.Log($"Snake ate its tail. Collision function called...");

        int deadTailSegmentIndex = tailController.tailSegments.IndexOf(tailSegment);
        if (deadTailSegmentIndex == -1) return;

        List<GameObject> segmentsToDestroy = tailController.tailSegments.GetRange(deadTailSegmentIndex, tailController.tailSegments.Count - deadTailSegmentIndex);

        foreach (GameObject segment in segmentsToDestroy)
        {
            Debug.Log(segment.name + "destroyed at" + segment.transform.position);

            Vector2Int segmentPos = PositionConversion.Vector3ToInt(segment.transform.position);
            GridHandler.RemoveOccupiedPosition(segmentPos, gameHandler.occupiedCells);

            EntityTracker.UnregisterEntity(EntityTracker.GetEntityID(segment));
            tailController.RemoveTailSegment(segment);
            Destroy(segment);
        }

        if (!statusHandler.effects.Any(effect => effect is BleedingEffect && effect.Active))
        {
            Bleed();
        }
    }

    public void Burn()
    {
        BurningEffect burn = new BurningEffect(10f, 5f, playerProfile.stats.resistance);
        statusHandler.ActivateEffect(burn);
    }

    public void Bleed()
    {
        BleedingEffect bleed = new BleedingEffect(resistance);
        statusHandler.ActivateEffect(bleed);
    }
}
