using UnityEngine;
using UnityEngine.U2D;

public class GlobalSpriteAtlas : MonoBehaviour
{
    [SerializeField] SpriteAtlas spriteAtlas;
    public static SpriteAtlas atlas;

    void Awake() { atlas = spriteAtlas; }
}