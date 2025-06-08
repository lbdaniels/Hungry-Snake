using System.Collections.Generic;
using UnityEngine;

public class TailController : MonoBehaviour
{
    // Classes & Components
    GameHandler gameHandler;
    PlayerController playerController;

    // Player Var
    [HideInInspector] public GameObject snakeHead;
    [HideInInspector] public Vector2Int playerPos;
    [HideInInspector] public float playerMoveTimerMax;
    [HideInInspector] public string playerMoveDirection;
    [HideInInspector] public List<Vector2Int> playerPositions = new List<Vector2Int>();

    // Tail Var
    [HideInInspector] public int numOfTailSegments;
    [HideInInspector] public List<GameObject> tailSegments = new List<GameObject>();
    [HideInInspector] public int newTailSegmentIndex;

    private void Awake()
    {
        AssignValues();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (tailSegments.Count > 0)
        {
            // Debug.Log("Before movement: " + gameHandler.tailSegments[0].transform.position);
            //UpdateTailPositions();
            // Debug.Log("After movement: " + gameHandler.tailSegments[0].transform.position);
        }
    }

    private void AssignValues()
    {
        gameHandler = GameHandler.Instance;
        playerController = GetComponent<PlayerController>();
    }

    public void CreatePosHistory()
    {
        playerPos = playerController.playerGridPosition;

        playerPositions.Insert(0, playerPos);

        int maxPosHistory = tailSegments.Count + 2;

        if (playerPositions.Count > maxPosHistory)
        {
            playerPositions.RemoveAt(playerPositions.Count - 1);
        }

        //Debug.Log($"Latest player position (index 0): {playerPositions[0]}");
        //if (playerPositions.Count > 1)
        //{
           // Debug.Log($"Previous player position (index 1): {playerPositions[1]}");
            //Vector2Int diff = playerPositions[0] - playerPositions[1];
            //Debug.Log($"Difference: {diff}");
        //}
    }

    public void AddTailSegment(GameObject segment)
    {
        tailSegments.Add(segment);
        numOfTailSegments = tailSegments.Count;
        Debug.Log("Added segment. Total now: " + tailSegments.Count);
    }

    public void RemoveTailSegment(GameObject segment)
    {
        tailSegments.Remove(segment);
        numOfTailSegments = tailSegments.Count;
        Debug.Log("Removed segment. Total now: " + tailSegments.Count);
    }

    // Moves the tail segments. Primarily called by the MovePlayer function in the PlayerController class
    public void UpdateTailPositions()
    {
        if (tailSegments.Count == 0 || playerPositions.Count <= tailSegments.Count) return;

        for (int i = tailSegments.Count - 1; i >= 0; i--)
        {
            Debug.Log($"Processing TailSegment[{i}], Position Before: {tailSegments[i].transform.position}");

            if (i < playerPositions.Count)
            {
                Vector2Int tailSegmentPos = PositionConversion.Vector3ToInt(tailSegments[i].transform.position);
                GridHandler.RemoveOccupiedPosition(tailSegmentPos, gameHandler.occupiedCells);

                tailSegments[i].transform.position = new Vector3(playerPositions[i + 1].x,
                                                         playerPositions[i + 1].y,
                                                         tailSegments[i].transform.position.z);

                tailSegmentPos = PositionConversion.Vector3ToInt(tailSegments[i].transform.position);
                GridHandler.AddOccupiedPosition(tailSegmentPos, tailSegments[i], gameHandler.occupiedCells);
            }

            Debug.Log($"Processing TailSegment[{i}], Position After: {tailSegments[i].transform.position}");
        }
    }
}
