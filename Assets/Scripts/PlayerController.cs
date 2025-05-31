using System;
using System.Collections.Generic;
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

    // Structs
    private GridHandler.GridInfo gridData;
    private GridHandler.GridBounds gridBounds;
    private GridHandler.GridMetrics gridMetrics;

    
    // Var
    private float moveTimer;
    private float moveTimerMax;

    private InputAction upAction;
    private InputAction downAction;
    private InputAction leftAction;
    private InputAction rightAction;
    private Vector2Int moveDirection;
    private Vector2Int prevDirection;

    private Vector2Int playerGridPosition;

    private bool justWarped = false;
    private bool movedSinceWarp = true;

    private void Awake()
    {
        // Creates an instance of necessary classes such as GameHandler
        ClassInstanceHandler();

        // Uses the instance of the GridInfo struct in GameHandler to get grid data
        gridData = gameHandler.gridData;
        gridBounds = gridData.Bounds;
        gridMetrics = gridData.Metrics;

        //Debug.Log($"After PlayerController GridData instance: MinX ={ gridBounds.MinX}, MaxX ={ gridBounds.MaxX}, MinY ={ gridBounds.MinY}, MaxY ={ gridBounds.MaxY}");
  
        // Handles Player Input
        PlayerInputHandler();

        // Initializes values for later such as a move timer and player direction
        InitializeAwakeVar();
    }

    private void Start()
    {
        // Initializes values for later such as playerGridPosition
        InitializeStartVar();
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

    private void ClassInstanceHandler()
    {
        // Creates an instance of the GameHandler class
        gameHandler = GameHandler.Instance;
    }

    private void InitializeAwakeVar()
    {
        moveTimerMax = gameHandler.playerMoveTimerMax;
        moveTimer = moveTimerMax;
        moveDirection = new Vector2Int(1, 0);
        prevDirection = new Vector2Int(1, 0);
    }

    private void InitializeStartVar()
    {
        playerGridPosition = gameHandler.ConvertVector3ToVector2Int(transform.position);
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
            
            //Debug.Log($"Player grid position: {playerGridPosition}");
            if (gameHandler.playerPos != playerGridPosition)
            {
                gameHandler.UpdatePlayerInfo(playerGridPosition);
            }
        }

        Vector2Int playerPos = gameHandler.ConvertVector3ToVector2Int(transform.position);
        GridHandler.RemoveOccupiedPosition(playerPos, gameHandler.occupiedCells);

        transform.position = new Vector3(playerGridPosition.x, playerGridPosition.y);

        playerPos = gameHandler.ConvertVector3ToVector2Int(transform.position);
        GridHandler.AddOccupiedPosition(playerPos, gameObject, gameHandler.occupiedCells);

        if (gameHandler.tailSegments.Count > 0)
        {
           // Debug.Log("Before movement: " + gameHandler.tailSegments[0].transform.position);
            gameHandler.UpdateTailPositions();
           // Debug.Log("After movement: " + gameHandler.tailSegments[0].transform.position);
        }
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
        Vector2Int foodPos = gameHandler.ConvertVector3ToVector2Int(foodItem.transform.position);
        GridHandler.RemoveOccupiedPosition(foodPos, gameHandler.occupiedCells);

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

        List<GameObject> tailSegmentList = gameHandler.tailSegments;

        int deadTailSegmentIndex = tailSegmentList.IndexOf(tailSegment);
        if (deadTailSegmentIndex == -1) return;

        List<GameObject> segmentsToDestroy = tailSegmentList.GetRange(deadTailSegmentIndex, tailSegmentList.Count - deadTailSegmentIndex);

        foreach (GameObject segment in segmentsToDestroy)
        {
            Debug.Log(segment.name + "destroyed at" + segment.transform.position);

            Vector2Int segmentPos = gameHandler.ConvertVector3ToVector2Int(segment.transform.position);
            GridHandler.RemoveOccupiedPosition(segmentPos, gameHandler.occupiedCells);

            Destroy(segment);
        }

        tailSegmentList.RemoveRange(deadTailSegmentIndex, tailSegmentList.Count - deadTailSegmentIndex);
        gameHandler.numOfTailSegments = tailSegmentList.Count;
    }

}
