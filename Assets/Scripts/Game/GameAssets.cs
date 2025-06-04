using UnityEngine;

public class GameAssets : MonoBehaviour
{
    public static GameAssets instance;

    public Sprite backgroundSprite;
    public Sprite snakeSprite;
    public Sprite snakeTailSprite;
    public Sprite foodSprite;
    //public Sprite stoneSprite;
    //public Sprite hunterSprite;
    //public Sprite pathSprite;
    //public Sprite campfireSprite;

    private void Awake()
    {
        instance = this;
    }
}
