using UnityEngine;

public class GameTime : MonoBehaviour
{
    // Game Timer
    public bool gameTimerActive = false;
    [HideInInspector] public float currentGameTime;
    public float gameTimerStart = 600;

    // Snake Timers
    public float snakeMoveTimerMax = 0.2f; // Higher is slower
    public float snakeSegmentDeathTimer = 5f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

}
