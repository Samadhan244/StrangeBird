using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

public class SpriteAtlasScript : MonoBehaviour
{
    SpriteAtlas spriteAtlas;
    [SerializeField] string spriteName;
    [SerializeField] Image image;
    [SerializeField] SpriteRenderer sprite;
    [SerializeField] bool isBackground, isObstacle;

    void Start()
    {
        spriteAtlas = GlobalSpriteAtlas.atlas;
        if (isBackground) sprite.sprite = spriteAtlas.GetSprite("Background" + (PlayerPrefs.GetInt("BackgroundSelected", 0) + 1) + "_0");
        else if (isObstacle) sprite.sprite = spriteAtlas.GetSprite("Pipe" + (PlayerPrefs.GetInt("ObstacleSelected", 0) + 1) + "_0");
        else
        {
            if (image) image.sprite = spriteAtlas.GetSprite(spriteName);
            else if (sprite) sprite.sprite = spriteAtlas.GetSprite(spriteName);
        }
    }
}