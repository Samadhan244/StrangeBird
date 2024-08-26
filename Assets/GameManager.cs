using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public int difficulty;
    [SerializeField] Player player;
    [SerializeField] GameObject menu, loadingMenu, exit;
    [SerializeField] Image loadingBar;
    [SerializeField] TextMeshProUGUI loadingText;
    [SerializeField] GameObject[] coinsToPool, obstaclesToPool, birdsToPool;
    [SerializeField] ObjectsMoveLeft[] objectsMoveLeft;
    [SerializeField] BirdMove[] birdMove;
    [SerializeField] TextMeshProUGUI coinText, timerText, scoreText, deathText, fpsText;
    [SerializeField] SpriteRenderer background;
    [SerializeField] Transform sunMoonIcon;
    [SerializeField] AudioSource audioSource; [SerializeField] AudioClip[] birdSpawnSounds; [SerializeField] AudioClip newHighScoreSound, uiSelectSound;
    [SerializeField] int minutes = 0, hours = 12, score, highScore, coin, currentCoins, totalDeaths, backgroundSelected, skill1Level;
    [SerializeField] float spawnDelay, elapsedTime;
    bool isLoadingStarted, newHighScore, spawnBirds;

    void Update()
    {
        if (isLoadingStarted)
        {
            elapsedTime = Mathf.Min(2, elapsedTime + Time.deltaTime);
            loadingText.text = "Loading " + Mathf.RoundToInt((elapsedTime / 2) * 100).ToString() + "%";
            loadingBar.fillAmount = elapsedTime / 2;
        }
    }

    void Start()
    {
        LoadStats();
        coinText.text = coin.ToString();
        scoreText.text = score + " / " + highScore;
        fpsText.gameObject.SetActive(PlayerPrefs.GetInt("ShowFps") == 1);

        spawnDelay = difficulty == 1 ? 1.5f : difficulty == 2 ? 1.3f : 1f;
        spawnBirds = PlayerPrefs.GetInt("SpawnBirds") == 1;
        if (spawnBirds) InvokeRepeating(nameof(BirdsPool), 5f, 5f);  // Just spawns decorative flying Birds
        InvokeRepeating(nameof(ObstaclesPool), spawnDelay, spawnDelay);
        InvokeRepeating(nameof(DayNightCycle), 0.2f, 0.2f);  // Day/Night cycle and increase of score by difficulty
        InvokeRepeating(nameof(GainScore), 1f, 1f);

        #if UNITY_ANDROID || UNITY_IOS
        exit.SetActive(false);
        #endif
    }

    void LoadStats()
    {
        coin = PlayerPrefs.GetInt("Coin");
        highScore = PlayerPrefs.GetInt("HighestScore", 100);
        totalDeaths = PlayerPrefs.GetInt("TotalDeaths", 0);
        backgroundSelected = PlayerPrefs.GetInt("BackgroundSelected");
        skill1Level = PlayerPrefs.GetInt("Skill1Level");
        AudioListener.volume = PlayerPrefs.GetFloat("GlobalVolume");
        difficulty = PlayerPrefs.GetInt("Difficulty");
    }

    void SaveStats()
    {
        PlayerPrefs.SetInt("Coin", coin);
        PlayerPrefs.SetInt("HighestScore", highScore);
        PlayerPrefs.SetInt("TotalDeaths", totalDeaths);
        PlayerPrefs.Save();
    }
    void OnApplicationQuit() { SaveStats(); }

    void CoinsPool()
    {
        foreach (GameObject x in coinsToPool)
        {
            if (!x.activeInHierarchy)
            {
                x.SetActive(true);
                return;
            }
        }
    }

    void ObstaclesPool()
    {
        // Spawn Coin by chance from skill1Level
        if (skill1Level == 1) { if (Random.Range(0, 100) < 50) CoinsPool(); }
        else if (skill1Level == 2) { if (Random.Range(0, 100) < 70) CoinsPool(); }
        else if (skill1Level == 3) CoinsPool();

        foreach (GameObject x in obstaclesToPool)
        {
            if (!x.activeInHierarchy)
            {
                x.SetActive(true);
                return;
            }
        }
    }

    void BirdsPool()
    {
        if (Random.Range(1, 4) == 3)  // 33% chance to spawn moving decoration Bird
        {
            audioSource.PlayOneShot(birdSpawnSounds[Random.Range(0, 2)]);
            int randInt = Random.Range(0, birdsToPool.Length);
            birdMove[randInt].speed = difficulty == 1 ? -4f : difficulty == 2 ? -6f : -8f;
            birdsToPool[randInt].SetActive(true);
        }
    }

    public void TakeCoin()
    {
        coin += 1;
        currentCoins += 1;
        coinText.text = coin.ToString();
    }

    void GainScore()
    {
        score += difficulty == 1 ? 3 : difficulty == 2 ? 6 : 9;  // Increase score at a time by difficulty
        if (score > highScore)
        {
            highScore = score;
            if (!newHighScore)
            {
                audioSource.PlayOneShot(newHighScoreSound);
                newHighScore = true;
            }
        }
        else newHighScore = false;
        scoreText.text = score + " / " + highScore;
        fpsText.text = "Fps: " + Mathf.RoundToInt(1 / Time.deltaTime).ToString();
    }

    void DayNightCycle()
    {
        // Timer
        minutes = (minutes + 1) % 60;
        hours = minutes == 0 ? (hours + 1) % 24 : hours;
        timerText.text = $"{hours:00}:{minutes:00}";

        float timeOfDay = (hours + (minutes / 60f)) / 24f;  // Calculate time as a value between 0 and 1 (0 is midnight, 0.5 is noon, 1 is midnight)
        float adjustedTimeOfDay = (timeOfDay + 0.5f) % 1f;  // Adjust timeOfDay to ensure 12:00 is the peak daylight

        Color dayColor = new Color(1f, 1f, 1f);  // White (max light)
        Color nightColor = new Color(0f, 0f, 0f);  // Black (no light)

        if (adjustedTimeOfDay < 0.5f) background.color = Color.Lerp(dayColor, nightColor, adjustedTimeOfDay * 2);  // Morning to afternoon (dayColor to nightColor)
        else background.color = Color.Lerp(nightColor, dayColor, (adjustedTimeOfDay - 0.5f) * 2);  // Afternoon to morning (nightColor to dayColor)

        // Rotate the sun & moon icon based on time of day
        float rotationAngle = (timeOfDay * 360f + 180f) % 360f;  // Calculate the rotation angle starting from 180 degrees
        sunMoonIcon.localRotation = Quaternion.Euler(0, 0, rotationAngle);  // Apply the rotation to the RectTransform
    }

    public void CanSpawn(bool isActive)
    {
        if (isActive)  // Respawn
        {
            foreach (GameObject x in coinsToPool) x.SetActive(false);
            foreach (GameObject x in obstaclesToPool) x.SetActive(false);
            spawnDelay = difficulty == 1 ? 1.5f : difficulty == 2 ? 1.5f : 1f;
            if (spawnBirds) InvokeRepeating(nameof(BirdsPool), 5f, 5f);
            InvokeRepeating(nameof(ObstaclesPool), spawnDelay, spawnDelay);
            InvokeRepeating(nameof(DayNightCycle), 0.2f, 0.2f);
            InvokeRepeating(nameof(GainScore), 1f, 1f);
            score = 0;
            scoreText.text = score + " / " + highScore;
        }
        else  // Death
        {
            totalDeaths += 1;
            deathText.text = $"Total Deaths: {totalDeaths}\nHigh Score: {highScore}\nCoins Collected This Round: {currentCoins}";
            currentCoins = 0;
            foreach (ObjectsMoveLeft x in objectsMoveLeft) x.CanMove(false);
            if (spawnBirds) CancelInvoke(nameof(BirdsPool));
            CancelInvoke(nameof(ObstaclesPool));
            CancelInvoke(nameof(DayNightCycle));
            CancelInvoke(nameof(GainScore));
            this.Wait(0.5f, () => menu.SetActive(true));
        }
    }

    public void MenuSelection(int index)
    {
        audioSource.PlayOneShot(uiSelectSound);
        if (index == 1)  // Play
        {
            player.Respawn();
            menu.SetActive(false);
        }
        else if (index == 2)  // Menu
        {
            SaveStats();
            foreach (Transform child in menu.transform) child.gameObject.SetActive(false);
            loadingMenu.SetActive(true);
            isLoadingStarted = true;
            StartCoroutine(LoadSceneAsync());
        }
        else if (index == 3) Application.Quit();  // Exit
    }

    IEnumerator LoadSceneAsync()
    {
        yield return new WaitForSeconds(2f);  // Artificial delay timer
        // Start loading scene asynchronously and whait until it's done
        AsyncOperation operation = SceneManager.LoadSceneAsync(0);
        while (!operation.isDone) yield return null;
    }
}