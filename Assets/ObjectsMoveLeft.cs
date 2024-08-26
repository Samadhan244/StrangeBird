using UnityEngine;

public class ObjectsMoveLeft : MonoBehaviour
{
    [SerializeField] float moveSpeed, pingPongSpeed;
    [SerializeField] bool isObstacle;
    [SerializeField] GameManager gameManager;
    [SerializeField] Rigidbody2D rb;

    void Awake()
    {
        moveSpeed = gameManager.difficulty == 1 ? -4f : gameManager.difficulty == 2 ? -5f : -7f;  // Speed depends on difficulty
    }

    void OnEnable()
    {
        transform.position = !isObstacle ? new Vector2(9f, Random.Range(-2f, 2f)) : new Vector2(6f, Random.Range(-1f, 1f));  // Start with random Y position
        // If it's "Coin" then it doesn't do "PingPong" and if it's an "Obstacle" its "PingPong" speed depends on difficulty
        pingPongSpeed = !isObstacle ? 0 : gameManager.difficulty == 1 ? 0.4f : gameManager.difficulty == 2 ? 0.6f : 0.8f;
        pingPongSpeed = Random.Range(1, 3) == 1 ? pingPongSpeed : -pingPongSpeed;  // Random movement direction at start
        CanMove(true);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Collider"))  // When it touches upper or lower collider, it will change its "PingPong" movement direction
        {
            pingPongSpeed = -pingPongSpeed;
            rb.velocity = new Vector2(moveSpeed, pingPongSpeed);
        }
        else if (other.gameObject.CompareTag("ObjectsDestroyer")) gameObject.SetActive(false);  // If it touches left collider, it will be disabled to be used again in the future
    }

    public void CanMove(bool trueOrFalse)
    {
        rb.velocity = trueOrFalse ? new Vector2(moveSpeed, pingPongSpeed) : Vector2.zero;
    }
}