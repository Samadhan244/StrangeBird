using TMPro;
using UnityEngine;

public class BirdMove : MonoBehaviour
{
    [SerializeField] Rigidbody2D rb;
    [SerializeField] GameObject[] chatObjects;
    [SerializeField] TextMeshPro chat;
    public float speed;
    string[] birdChats = new string[]
{
    "What's quackin', good lookin'?",
    "Hey, wingman!",
    "Tweet dreams!",
    "Fly high, don't be shy!",
    "Keep calm and chirp on!",
    "You're eggcellent!",
    "Winging it today?",
    "What the flock!",
    "Peck on, peck off.",
    "Just winging by!",
    "Spread your wings and fly!",
    "You're tweet-tastic!",
    "Flying solo today?",
    "Keep your beak up!",
    "Chirp happens!",
    "A little birdie told me...",
    "Flap it out!",
    "Life's a chirp!",
    "Eggciting times ahead!",
    "What's up, beak face?",
    "You're so fly!",
    "Bird is the word!",
    "Let's get this bread!",
    "Shake your tail feathers!",
    "Wing it like a boss!",
    "Feathered and fabulous!",
    "Stay chirpy!",
    "You're the tweetest!",
    "Flap till you drop!",
    "Peckish, aren't we?",
    "Squawk to the walk!",
    "Catch you on the fly!",
    "Don't ruffle my feathers!",
    "High-flying fun!",
    "Quirky chirp!",
    "Feather in your cap!",
    "Keep flapping!",
    "Feathered friends forever!",
    "Tweet it out!",
    "Birds of paradise!",
    "Squawk and awe!",
    "Up, up, and away!",
    "Beak sneak!",
    "Chirp-tastic!",
    "Flap happy!",
    "Bird-brained fun!",
    "Fluff and stuff!"
};

    void OnEnable()
    {
        transform.position = new Vector2(6f, Random.Range(-1.5f, 1.5f));  // Teleport back to starting position with random Y
        rb.velocity = new Vector2(speed, 0);  // Move left

        if (Random.Range(0, 100) < 60)
        {
            foreach (GameObject x in chatObjects) x.SetActive(true);
            chat.text = birdChats[Random.Range(0, birdChats.Length)];
        }
        else foreach (GameObject x in chatObjects) x.SetActive(false);

        this.Wait(3f, () => gameObject.SetActive(false));
    }
}