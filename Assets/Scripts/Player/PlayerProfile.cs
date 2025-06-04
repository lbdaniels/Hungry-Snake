using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Profiling;
using static PlayerDefaults;
public class PlayerProfile
{
    // PlayerProfile Instance
    public static PlayerProfile Instance { get; private set; }

    // PlayerDefaults
    private static PlayerDefaults defaultProfile;
    public PlayerStats stats;
    public PlayerMetaData meta;
    public PlayerScore score;
    public PlayerWinConditions winConditions;

    private PlayerProfile(PlayerDefaults profileSO)
    {
        meta = new PlayerMetaData(profileSO.playerMetaData);

        stats = new PlayerStats(profileSO.playerStats);
        Debug.Log($"[PlayerProfile] moveSpeed after clone: {stats.moveSpeed}");

        score = new PlayerScore(profileSO.playerScore);
        winConditions = new PlayerWinConditions(profileSO.playerWinConditions);
    }

    public static void Initialize(PlayerDefaults profileSO)
    {
        Debug.Log("PlayerDefaults Asset MoveSpeed: " + profileSO.playerStats.moveSpeed);
        defaultProfile = profileSO;
        Debug.Log("Stored defaultProfile MoveSpeed: " + defaultProfile.playerStats.moveSpeed);

        Instance = new PlayerProfile(profileSO);
    }

    public void ResetProfile()
    {
        Debug.Log("Before Reset MoveSpeed: " + stats.moveSpeed);

        meta = new PlayerMetaData(defaultProfile.playerMetaData);
        stats = new PlayerStats(defaultProfile.playerStats);
        score = new PlayerScore(defaultProfile.playerScore);
        winConditions = new PlayerWinConditions(defaultProfile.playerWinConditions);

        Debug.Log("After Reset MoveSpeed: " + stats.moveSpeed);

    }
}
