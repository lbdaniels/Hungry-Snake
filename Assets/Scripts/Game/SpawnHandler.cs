using UnityEngine;

public class SpawnHandler : MonoBehaviour
{
    // Classes
    GameAssets gameAssets;
    GameHandler gameHandler;
    TailController tailController;

    GridHandler.GridMetrics gridMetrics;
    GridHandler.GridBounds gridBounds;
    Vector2Int gridCenter;

    // Var
    public GameObject snakePrefab;
    public Color snakeColor;
    private int playerSpawnMargin;
    private GameObject playerObj;

    public GameObject foodPrefab;
    private Transform foodParent;
    public Color foodColor;

    public GameObject tailPrefab;
    private Transform tailParent;
    public Color tailColor;

    GameObject recentTailSegment;

    public GameObject stonePrefab;
    private Transform stoneParent;
    public Color stoneColor;

    // Hunter

    // Mouse

    // Ant

    // Zone

    // Path

    private void Awake()
    {
        AssignValues();
    }

    private void Start()
    {
        
    }

    private void InstanceChecker()
    {

    }

    private void AssignValues()
    {
        // Creates an instance of the GameHandler class
        gameHandler = GameHandler.Instance;

        // Creates an instance of the GameAssets component
        gameAssets = GetComponent<GameAssets>();

        gridMetrics = GridHandler.gridMetrics;
        gridBounds = GridHandler.gridBounds;
        gridCenter = GridHandler.gridCenter;

        foodParent = GameObject.Find("FoodContainer").transform;
        tailParent = GameObject.Find("TailSegmentsContainer").transform;
        stoneParent = GameObject.Find("StoneContainer").transform;

        playerSpawnMargin = 4;
    }

    // Reserve spawn area for player using exempt position system in GridHandler
    private void ReservePlayerSpawnPos()
    {
        // Adds the grid center to list of spawn positions exempt from random generation

        for (int i = 0; i <= playerSpawnMargin; ++i)
        {
            Vector2Int areaRight = new Vector2Int(gridCenter.x + i, gridCenter.y);
            Vector2Int areaLeft = new Vector2Int(gridCenter.x - i, gridCenter.y);
            Vector2Int areaUp = new Vector2Int(gridCenter.x, gridCenter.y + i);
            Vector2Int areaDown = new Vector2Int(gridCenter.x, gridCenter.y - i);

            GridHandler.AddOccupiedPosition(areaRight, null, gameHandler.occupiedCells);
            GridHandler.AddOccupiedPosition(areaLeft, null, gameHandler.occupiedCells);
            GridHandler.AddOccupiedPosition(areaUp, null, gameHandler.occupiedCells);
            GridHandler.AddOccupiedPosition(areaDown, null, gameHandler.occupiedCells);

            for (int j = 1; j <= playerSpawnMargin; ++j)
            {
                Vector2Int areaRightUp = new Vector2Int(areaRight.x, areaRight.y + j);
                Vector2Int areaRightDown = new Vector2Int(areaRight.x, areaRight.y - j);

                Vector2Int areaLeftUp = new Vector2Int(areaRight.x, areaRight.y + j);
                Vector2Int areaLeftDown = new Vector2Int(areaRight.x, areaRight.y - j);

                GridHandler.AddOccupiedPosition(areaRightUp, null, gameHandler.occupiedCells);
                GridHandler.AddOccupiedPosition(areaLeftUp, null, gameHandler.occupiedCells);
                GridHandler.AddOccupiedPosition(areaRightDown, null, gameHandler.occupiedCells);
                GridHandler.AddOccupiedPosition(areaLeftDown, null, gameHandler.occupiedCells);
            }
        }
    }

    // Unreserves spawn area used for player so other entities can now spawn
    private void UnreservePlayerSpawnPos()
    {
        for (int i = 0; i <= playerSpawnMargin; ++i)
        {
            Vector2Int areaRight = new Vector2Int(gridCenter.x + i, gridCenter.y);
            Vector2Int areaLeft = new Vector2Int(gridCenter.x - i, gridCenter.y);
            Vector2Int areaUp = new Vector2Int(gridCenter.x, gridCenter.y + i);
            Vector2Int areaDown = new Vector2Int(gridCenter.x, gridCenter.y - i);

            GridHandler.RemoveOccupiedPosition(areaRight, gameHandler.occupiedCells);
            GridHandler.RemoveOccupiedPosition(areaLeft, gameHandler.occupiedCells);
            GridHandler.RemoveOccupiedPosition(areaUp, gameHandler.occupiedCells);
            GridHandler.RemoveOccupiedPosition(areaDown, gameHandler.occupiedCells);

            for (int j = 1; j <= playerSpawnMargin; ++j)
            {
                Vector2Int areaRightUp = new Vector2Int(areaRight.x, areaRight.y + j);
                Vector2Int areaRightDown = new Vector2Int(areaRight.x, areaRight.y - j);

                Vector2Int areaLeftUp = new Vector2Int(areaRight.x, areaRight.y + j);
                Vector2Int areaLeftDown = new Vector2Int(areaRight.x, areaRight.y - j);

                GridHandler.RemoveOccupiedPosition(areaRightUp, gameHandler.occupiedCells);
                GridHandler.RemoveOccupiedPosition(areaLeftUp, gameHandler.occupiedCells);
                GridHandler.RemoveOccupiedPosition(areaRightDown, gameHandler.occupiedCells);
                GridHandler.RemoveOccupiedPosition(areaLeftDown, gameHandler.occupiedCells);
            }
        }
    }

    // First spawns for a new game including the player
    public void FirstSpawns()
    {
        ReservePlayerSpawnPos();

        // Spawn the player
        SpawnSnake();

        // Spawn other first entities
        SpawnFood();
        SpawnStone();

        UnreservePlayerSpawnPos();
    }

    // Spawns the snake head (player) at the given spawn position
    public void SpawnSnake()
    {
        Vector2Int snakeSpawnPos = new Vector2Int(25, 25);
        Vector3 snakeSpawnPosV3 = new Vector3(snakeSpawnPos.x, snakeSpawnPos.y, 0);

        GameObject snakeHead = Instantiate(snakePrefab, snakeSpawnPosV3, Quaternion.identity);
        Debug.Log($"Player spawned at: {snakeSpawnPosV3}");
        snakeHead.name = "Player";

        playerObj = snakeHead;
        tailController = playerObj.GetComponent<TailController>();

        EntityTracker.RegisterEntity("Player", snakeHead);

        Vector2Int snakePos = PositionConversion.Vector3ToInt( snakeSpawnPosV3 );
        GridHandler.AddOccupiedPosition(snakePos, snakeHead, gameHandler.occupiedCells);
    }

    // Spawns a food item using the food prefab and the passed spawn position
    public void SpawnFood()
    {
        Vector2Int foodSpawnPos = GridHandler.RandomGridPosition(GridHandler.exemptSpawnPositions);
        Vector3 foodSpawnPosV3 = PositionConversion.Vector2IntTo3(foodSpawnPos);

        GameObject foodEntity = Instantiate(foodPrefab, foodSpawnPosV3, Quaternion.identity);
        foodEntity.transform.SetParent(foodParent);

        Debug.Log($"Food spawned at: {foodSpawnPos}");
        EntityTracker.RegisterEntity("Food", foodEntity);

        Vector2Int foodPos = foodSpawnPos;
        GridHandler.AddOccupiedPosition(foodPos, foodEntity, gameHandler.occupiedCells);
    }

    // Spawns a tail segment at the given spawn position and names it according to its index
    public void SpawnTailSegment()
    {
        Debug.Log($"Before segment spawn call - Snake Segments: {tailController.tailSegments.Count}");

        Vector2Int tailSpawnPos = DetermineTailSpawnPos();
        Vector3 tailSpawnPosV3 = new Vector3(tailSpawnPos.x, tailSpawnPos.y, 0);

        int tailSegmentIndex = tailController.tailSegments.Count;

        GameObject latestTailSegment = Instantiate(tailPrefab, tailSpawnPosV3, Quaternion.identity);
        latestTailSegment.name = $"TailSegment{tailSegmentIndex}";
        latestTailSegment.transform.SetParent(tailParent);
        EntityTracker.RegisterEntity("Tail", latestTailSegment);

        Vector2Int tailPos = PositionConversion.Vector3ToInt(tailSpawnPosV3);
        GridHandler.AddOccupiedPosition(tailPos, latestTailSegment, gameHandler.occupiedCells);

        tailController.AddTailSegment(latestTailSegment);
        tailController.UpdateTailPositions();

        Debug.Log($"Tail Segment {tailSegmentIndex} spawned at: {tailSpawnPos}");
        Debug.Log($"After segment spawn call - Snake Segments: {tailController.tailSegments.Count}");
    }

    // Determines the spawn position of the next tail segment
    public Vector2Int DetermineTailSpawnPos()
    {
        int newTailSegmentIndex = tailController.tailSegments.Count;

        if (tailController.numOfTailSegments == 0)
        {
            recentTailSegment = playerObj;
        }

        else
        {
            recentTailSegment = tailController.tailSegments[newTailSegmentIndex - 1];
            Debug.Log($"Checking for Latest Tail Segment{recentTailSegment}");
        }

        Vector2Int prevTailSegmentPos = new Vector2Int((int)recentTailSegment.transform.position.x, (int)recentTailSegment.transform.position.y);

        // If there are at least 2 tail segments, use the previous segment to find a direction. If not, use the player's
        Vector2Int secondLastSegmentPos = (tailController.tailSegments.Count > 1)
            ? new Vector2Int((int)tailController.tailSegments[newTailSegmentIndex - 2].transform.position.x, (int)tailController.tailSegments[newTailSegmentIndex - 2].transform.position.y)
            : tailController.playerPositions[tailController.playerPositions.Count - 1];

        //Debug.Log($"Player position at tail segment spawn call: {playerObj.transform.position}");
        //Debug.Log($"Second last segment Pos: {secondLastSegmentPos}");
        //Debug.Log($"Previous player position for tail segment direction: {tailController.playerPositions[tailController.playerPositions.Count - 1]}");

        // Calculates the previous tail segment's direction
        Vector2Int tailDirection = prevTailSegmentPos - secondLastSegmentPos;

        //Debug.Log($"Tail direction: {tailDirection}");

        Vector2Int tailSpawnPos = prevTailSegmentPos - tailDirection;

        //Debug.Log($"Tail spawn determined: {tailSpawnPos}");

        return tailSpawnPos;
    }

    public void SpawnStone()
    {
        Vector2Int stoneSpawnPos = GridHandler.RandomGridPosition(GridHandler.exemptSpawnPositions);
        Vector3 stoneSpawnPosV3 = PositionConversion.Vector2IntTo3(stoneSpawnPos);

        GameObject stoneEntity = Instantiate(stonePrefab, stoneSpawnPosV3, Quaternion.identity);
        stoneEntity.transform.SetParent(stoneParent);
        Debug.Log($"Stone spawned at: {stoneSpawnPos}");
        EntityTracker.RegisterEntity("Stone", stoneEntity);

        Vector2Int stonePos = stoneSpawnPos;
        GridHandler.AddOccupiedPosition(stonePos, stoneEntity, gameHandler.occupiedCells);
    }
}



