using UnityEditor;
using UnityEngine;
using static UnityEditorInternal.ReorderableList;

[CreateAssetMenu(fileName = "PlayerDefaults", menuName = "Scriptable Objects/PlayerDefaults")]
public class PlayerDefaults : ScriptableObject
{
    public PlayerStats playerStats;
    public PlayerMetaData playerMetaData;
    public PlayerScore playerScore;
    public PlayerWinConditions playerWinConditions;

    
    public class PlayerMetaData
    {
        public string tag;

        public PlayerMetaData() { }

        public PlayerMetaData(PlayerMetaData defaults)
        {
            this.tag = "Player";
        }
    }

    [System.Serializable]
    public class PlayerStats
    {
        public float moveSpeed;
        public float strength;
        public float mutation;
        public float resistance;

        public PlayerStats() { }

        public PlayerStats(PlayerStats defaults)
        {
            if (defaults == null)
                throw new System.ArgumentNullException(nameof(defaults));

            this.moveSpeed = defaults.moveSpeed;
            this.strength = defaults.strength;
            this.mutation = defaults.mutation;
            this.resistance = defaults.resistance;
        }
    }

    [System.Serializable]
    public class PlayerScore
    {
        public int hunterPoints;
        public int pathPoints;
        public int mutationPoints;
        public int cannibalismPoints;
        public int vegetarianPoints;

        public PlayerScore() { }

        public PlayerScore(PlayerScore defaults)
        {
            if (defaults == null)
                throw new System.ArgumentNullException(nameof(defaults));

            hunterPoints = defaults.hunterPoints;
            pathPoints = defaults.pathPoints;
            mutationPoints = defaults.mutationPoints;
            cannibalismPoints = defaults.cannibalismPoints;
            vegetarianPoints = defaults.vegetarianPoints;
        }
    }

    [System.Serializable]
    public class PlayerWinConditions
    {
        // Gains enough hunter points by eating hunters
        public bool hunterWin;

        // Gains enough path points by navigating paths
        public bool pathWin;

        // Gains enough mutation points. Has to survive negative tradeoffs
        public bool mutationWin;

        // Survives until the game ends
        public bool survivalWin;

        // Eats a total amount of other snakes. Has to balance being hit to spawn other snakes and hunting them with survival.
        public bool cannibalismWin;

        // Eats a total amount of foods that are not meat. Meat subtracts from the score.
        public bool vegetarianWin;

        public PlayerWinConditions() { }

        public PlayerWinConditions(PlayerWinConditions defaults)
        {
            if (defaults == null)
                throw new System.ArgumentNullException(nameof(defaults));

            hunterWin = defaults.hunterWin;
            pathWin = defaults.pathWin;
            mutationWin = defaults.mutationWin;
            survivalWin = defaults.survivalWin;
            cannibalismWin = defaults.cannibalismWin;
            vegetarianWin = defaults.vegetarianWin;
        }
    }
}
