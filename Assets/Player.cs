using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    [SerializeField] float jumpForce = 4.5f, skill2Cd, skill2CdReady = 9f;
    [SerializeField] bool isAlive = true, isInvulnerable;
    [SerializeField] GameManager gameManager;
    [SerializeField] Image skill2;
    [SerializeField] RuntimeAnimatorController[] animatorControllers;
    [SerializeField] Animator animator, skill1Animator, skill2Animator;
    [SerializeField] Rigidbody2D rb;
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip[] sounds;

    void Start()
    {
        animator.runtimeAnimatorController = animatorControllers[PlayerPrefs.GetInt("BirdSelected", 0)];
        skill2CdReady = PlayerPrefs.GetInt("Skill2Level", 1) == 1 ? 9f : PlayerPrefs.GetInt("Skill2Level", 1) == 2 ? 8f : 7f;
    }

    void Update()
    {
        JumpNGravity();
        AutoUseSkill2();
    }

    void JumpNGravity()
    {
        if (isAlive)
        {
            rb.velocityY -= 12 * Time.deltaTime;  // Gravity
            if (Input.GetMouseButtonDown(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began) || Input.GetKeyDown(KeyCode.Space))
            {
                audioSource.PlayOneShot(sounds[0]);
                rb.velocity = Vector2.zero;  // Reset vertical velocity before applying the jump force
                rb.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);  // An upward force(Jump)
            }
        }
    }

    void AutoUseSkill2()
    {
        if (isAlive)
        {
            if (skill2Cd < skill2CdReady) skill2Cd += Time.deltaTime;
            else
            {
                audioSource.PlayOneShot(sounds[2]);
                skill2Cd = 0;
                isInvulnerable = true;
                skill2.color = new Color(1, 1, 1, 0.5f);
                spriteRenderer.color = new Color(1, 1, 1, 0.5f);

                this.Wait(2f, () =>
                {
                    audioSource.PlayOneShot(sounds[3]);
                    isInvulnerable = false;
                    skill2Animator.Play("AnimateSkill");
                    skill2.color = new Color(1, 1, 1, 1);
                    spriteRenderer.color = new Color(1, 1, 1, 1);
                });
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (isAlive)
        {
            if (other.gameObject.CompareTag("Coin"))
            {
                other.gameObject.SetActive(false);
                audioSource.PlayOneShot(sounds[1]);
                skill1Animator.Play("AnimateSkill");
                gameManager.TakeCoin();
            }
            else if (other.gameObject.CompareTag("Enemy") && !isInvulnerable) Death();
        }
    }

    void Death()
    {
        audioSource.PlayOneShot(sounds[4]);
        isAlive = false;
        animator.enabled = false;
        rb.velocity = Vector2.zero;
        gameManager.CanSpawn(false);
    }

    public void Respawn()
    {
        skill2Cd = 0;
        isAlive = true;
        animator.enabled = true;
        gameManager.CanSpawn(true);
        transform.position = new Vector3(-1.5f, 0);
    }
}