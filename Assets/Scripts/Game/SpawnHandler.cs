using UnityEngine;

public class SpawnHandler : MonoBehaviour
{
    // Classes
    GameAssets gameAssets;
    GameHandler gameHandler;

    // Var
    public GameObject snakePrefab;
    public Color snakeColor;
    public Vector2Int snakeSpawnPos;

    public GameObject foodPrefab;
    public Color foodColor;

    public GameObject tailPrefab;
    private Transform tailParent;
    public Color tailColor;

    GameObject recentTailSegment;
    GridHandler.GridMetrics gridMetrics;
    GridHandler.GridBounds gridBounds;

    private void Awake()
    {
        // Creates an instance of necessary classes such as GameHandler
        ClassInstanceHandler();

        // Gets necessary components
        GetSpawnHandlerComponents();

        AssignValues();

        tailParent = GameObject.Find("TailSegmentsContainer").transform;
    }

    private void AssignValues()
    {
        gridMetrics = GridHandler.gridMetrics;
        gridBounds = GridHandler.gridBounds;
    }

    private void ClassInstanceHandler()
    {
        // Creates and instance of the GameHandler class
        gameHandler = GameHandler.Instance;
    }

    private void GetSpawnHandlerComponents()
    {
        // Creates an instance of the GameAssets component
        gameAssets = GetComponent<GameAssets>();
    }

    // Spawns the snake head (player) at the given spawn position
    public void SpawnSnake()
    {
        snakeSpawnPos = new Vector2Int(25, 25);
        Vector3 snakeSpawnPosV3 = new Vector3(snakeSpawnPos.x, snakeSpawnPos.y, 0);

        GameObject snakeHead = Instantiate(snakePrefab, snakeSpawnPosV3, Quaternion.identity);
        Debug.Log($"Player spawned at: {snakeSpawnPos}");

        gameHandler.snakeHead = snakeHead;
        EntityTracker.RegisterEntity("Player", snakeHead);

        Vector2Int snakePos = PositionConversion.Vector3ToInt( snakeSpawnPosV3 );
        GridHandler.AddOccupiedPosition(snakePos, snakeHead, gameHandler.occupiedCells);
    }

    // Spawns a food item using the food prefab and the passed spawn position
    public void SpawnFood()
    {
        Vector2Int foodSpawnPos = GridHandler.RandomGridPosition(GridHandler.exemptSpawnPositions);
        Vector3 foodSpawnPosV3 = new Vector3(foodSpawnPos.x, foodSpawnPos.y, 0);

        GameObject foodItem = Instantiate(foodPrefab, foodSpawnPosV3, Quaternion.identity);
        Debug.Log($"Food spawned at: {foodSpawnPos}");
        EntityTracker.RegisterEntity("Food", foodItem);

        Vector2Int foodPos = PositionConversion.Vector3ToInt( foodSpawnPosV3 );
        GridHandler.AddOccupiedPosition(foodPos, foodItem, gameHandler.occupiedCells);
    }

    // Spawns a tail segment at the given spawn position and names it according to its index
    public void SpawnTailSegment()
    {
        Debug.Log($"Before segment spawn call - Snake Segments: {gameHandler.numOfTailSegments}");

        Vector2Int tailSpawnPos = DetermineTailSpawnPos();
        Vector3 tailSpawnPosV3 = new Vector3(tailSpawnPos.x, tailSpawnPos.y, 0);

        int tailSegmentIndex = gameHandler.tailSegments.Count;

        GameObject latestTailSegment = Instantiate(tailPrefab, tailSpawnPosV3, Quaternion.identity);
        latestTailSegment.name = $"TailSegment{tailSegmentIndex}";
        latestTailSegment.transform.SetParent(tailParent);
        EntityTracker.RegisterEntity("Tail", latestTailSegment);

        Vector2Int tailPos = PositionConversion.Vector3ToInt(tailSpawnPosV3);
        GridHandler.AddOccupiedPosition(tailPos, latestTailSegment, gameHandler.occupiedCells);

        gameHandler.AddTailSegment(latestTailSegment);
        gameHandler.numOfTailSegments = gameHandler.tailSegments.Count;
        gameHandler.UpdateTailPositions();

        Debug.Log($"Tail Segment {tailSegmentIndex} spawned at: {tailSpawnPos}");
        Debug.Log($"After segment spawn call - Snake Segments: {gameHandler.numOfTailSegments}");
    }

    // Determines the spawn position of the next tail segment
    public Vector2Int DetermineTailSpawnPos()
    {
        int newTailSegmentIndex = gameHandler.tailSegments.Count;

        if (gameHandler.numOfTailSegments == 0)
        {
            recentTailSegment = gameHandler.snakeHead;
        }

        else
        {
            recentTailSegment = gameHandler.tailSegments[newTailSegmentIndex - 1];
            Debug.Log($"Checking for Latest Tail Segment{recentTailSegment}");
        }

        Vector2Int prevTailSegmentPos = new Vector2Int((int)recentTailSegment.transform.position.x, (int)recentTailSegment.transform.position.y);

        // If there are at least 2 tail segments, use the previous segment to find a direction. If not, use the player's
        Vector2Int secondLastSegmentPos = (gameHandler.numOfTailSegments > 1)
            ? new Vector2Int((int)gameHandler.tailSegments[newTailSegmentIndex - 2].transform.position.x, (int)gameHandler.tailSegments[newTailSegmentIndex - 2].transform.position.y)
            : gameHandler.playerPositions[gameHandler.playerPositions.Count - 1];

        //Debug.Log($"Player position at tail segment spawn call: {gameHandler.playerPos}");
        //Debug.Log($"Second last segment Pos: {secondLastSegmentPos}");
        //Debug.Log($"Previous player position for tail segment direction: {gameHandler.playerPositions[gameHandler.playerPositions.Count - 1]}");

        // Calculates the previous tail segment's direction
        Vector2Int tailDirection = prevTailSegmentPos - secondLastSegmentPos;

        //Debug.Log($"Tail direction: {tailDirection}");

        Vector2Int tailSpawnPos = prevTailSegmentPos + tailDirection;

        //Debug.Log($"Tail spawn determined: {tailSpawnPos}");

        return tailSpawnPos;
    }
}



