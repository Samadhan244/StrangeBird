using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;

public class Menu : MonoBehaviour
{
    // Main Menu
    [SerializeField] GameObject menu, loadingMenu, shopMenu, optionMenu, aboutText, back, exit;
    [SerializeField] Image loadingBar, background;
    [SerializeField] TextMeshProUGUI loadingText;
    [SerializeField] AudioSource audioSource; [SerializeField] AudioClip uiSelectSound;
    bool isLoadingStarted; float elapsedTime;

    // Shop Menu
    [SerializeField] GameObject shopMenu0, shopMenu1, shopMenu2, shopMenu3;
    [SerializeField] GameObject[] birds, birdBuyButtons, backgrounds, backgroundBuyButtons, obstacles, obstacleBuyButtons, skillBuyButtons;
    [SerializeField] TextMeshProUGUI[] birdStyleTexts, backgroundStyleTexts, obstacleStyleTexts, skillLevelTexts;
    [SerializeField] GameObject coinImage;
    [SerializeField] TextMeshProUGUI coinText;
    [SerializeField] AudioClip[] buySounds;
    bool[] birdsBought = new bool[8], backgroundsBought = new bool[8], obstaclesBought = new bool[6];
    int coin, birdSelected, backgroundSelected, obstacleSelected, skill1Level, skill2Level;

    // Options Menu
    [SerializeField] TextMeshProUGUI[] difficultyTexts;
    [SerializeField] TextMeshProUGUI fpsText;
    [SerializeField] GameObject[] soundsCheckmarks, fpsCheckmarks, birdsCheckmarks;
    int difficulty, showFps, spawnBirds;

    void Start()
    {
        LoadStats();
        if (showFps == 1) InvokeRepeating(nameof(ShowFps), 0, 1f);

        #if UNITY_ANDROID || UNITY_IOS
        exit.SetActive(false);
        #endif
    }

    void SaveStats()
    {
        PlayerPrefs.SetInt("Coin", coin);
        PlayerPrefs.SetInt("BirdSelected", birdSelected);
        for (int i = 1; i < birdsBought.Length; i++) PlayerPrefs.SetInt($"BirdsBought{i}", birdsBought[i] ? 1 : 0);
        PlayerPrefs.SetInt("BackgroundSelected", backgroundSelected);
        for (int i = 1; i < backgroundsBought.Length; i++) PlayerPrefs.SetInt($"BackgroundsBought{i}", backgroundsBought[i] ? 1 : 0);
        PlayerPrefs.SetInt("ObstacleSelected", obstacleSelected);
        for (int i = 1; i < obstaclesBought.Length; i++) PlayerPrefs.SetInt($"ObstaclesBought{i}", obstaclesBought[i] ? 1 : 0);
        PlayerPrefs.SetInt("Skill1Level", skill1Level);
        PlayerPrefs.SetInt("Skill2Level", skill2Level);
        PlayerPrefs.SetInt("Difficulty", difficulty);
        PlayerPrefs.SetFloat("GlobalVolume", AudioListener.volume);
        PlayerPrefs.SetInt("ShowFps", showFps);
        PlayerPrefs.SetInt("SpawnBirds", spawnBirds);
        PlayerPrefs.Save();
    }

    void LoadStats()
    {
        // Shop Menu
        coin = PlayerPrefs.GetInt("Coin", 100);
        coinText.text = coin.ToString();

        birdSelected = PlayerPrefs.GetInt("BirdSelected", 0);
        birdsBought[0] = true;
        for (int i = 1; i < birdsBought.Length; i++) birdsBought[i] = PlayerPrefs.GetInt($"BirdsBought{i}", 0) == 1;

        backgroundSelected = PlayerPrefs.GetInt("BackgroundSelected", 0);
        backgroundsBought[0] = true;
        for (int i = 1; i < backgroundsBought.Length; i++) backgroundsBought[i] = PlayerPrefs.GetInt($"BackgroundsBought{i}", 0) == 1;

        obstacleSelected = PlayerPrefs.GetInt("ObstacleSelected", 0);
        obstaclesBought[0] = true;
        for (int i = 1; i < obstaclesBought.Length; i++) obstaclesBought[i] = PlayerPrefs.GetInt($"ObstaclesBought{i}", 0) == 1;

        skill1Level = PlayerPrefs.GetInt("Skill1Level", 1);
        skill2Level = PlayerPrefs.GetInt("Skill2Level", 1);

        // Options Menu
        difficulty = PlayerPrefs.GetInt("Difficulty", 1);
        difficultyTexts[difficulty - 1].color = Color.yellow;

        AudioListener.volume = PlayerPrefs.GetFloat("GlobalVolume", 1);
        if (AudioListener.volume == 1) soundsCheckmarks[0].SetActive(true);
        else soundsCheckmarks[1].SetActive(true);

        showFps = PlayerPrefs.GetInt("ShowFps", 0);
        if (showFps == 1) fpsCheckmarks[0].SetActive(true);
        else { fpsCheckmarks[1].SetActive(true); fpsText.gameObject.SetActive(false); }
        fpsText.gameObject.SetActive(PlayerPrefs.GetInt("ShowFps", 0) == 1);

        spawnBirds = PlayerPrefs.GetInt("SpawnBirds", 1);
        if (spawnBirds == 1) birdsCheckmarks[0].SetActive(true);
        else birdsCheckmarks[1].SetActive(true);
    }
    void OnApplicationQuit() { SaveStats(); }

    void Update()
    {
        // Main Menu
        if (isLoadingStarted)
        {
            elapsedTime = Mathf.Min(2, elapsedTime + Time.deltaTime);
            loadingText.text = "Loading " + Mathf.RoundToInt((elapsedTime / 2) * 100).ToString() + "%";
            loadingBar.fillAmount = elapsedTime / 2;
        }
    }

    public void MenuSelection(int index)
    {
        audioSource.PlayOneShot(uiSelectSound);
        if (index == 0)  // Play
        {
            SaveStats();
            foreach (Transform child in menu.transform) child.gameObject.SetActive(false);
            loadingMenu.SetActive(true);
            isLoadingStarted = true;
            back.SetActive(false);
            StartCoroutine(LoadSceneAsync());
        }
        else if (index == 1)  // Shop Menu
        {
            coinImage.SetActive(true);
            coinText.gameObject.SetActive(true);
            background.color = new Color(0.2f, 0.2f, 0.2f);
            menu.SetActive(false);
            shopMenu.SetActive(true);
            back.SetActive(true);
        }
        else if (index == 2)  // Options Menu
        {
            background.color = new Color(0.2f, 0.2f, 0.2f);
            menu.SetActive(false);
            optionMenu.SetActive(true);
            back.SetActive(true);
        }
        else if (index == 3)  // About Text
        {
            background.color = new Color(0.2f, 0.2f, 0.2f);
            menu.SetActive(false);
            aboutText.SetActive(true);
            back.SetActive(true);
        }
        else if (index == 4) Application.Quit();  // Exit
        else if (index == 5)  // Back to Main Menu
        {
            background.color = Color.white;
            back.SetActive(false);
            menu.SetActive(true);
            shopMenu.SetActive(false);
            shopMenu0.SetActive(false);
            shopMenu1.SetActive(false);
            shopMenu2.SetActive(false);
            shopMenu3.SetActive(false);
            optionMenu.SetActive(false);
            aboutText.SetActive(false);
            coinImage.SetActive(false);
            coinText.gameObject.SetActive(false);
        }
    }

    IEnumerator LoadSceneAsync()
    {
        yield return new WaitForSeconds(2f);  // Artificial delay timer
        AsyncOperation operation = SceneManager.LoadSceneAsync(1);  // Start loading scene asynchronously and whait until it's done
        while (!operation.isDone) yield return null;
    }

    // Shop Menu
    public void ShopSelection(int index)
    {
        audioSource.PlayOneShot(uiSelectSound);
        if (index == 0)  // Bird Models
        {
            shopMenu.SetActive(false);
            shopMenu0.SetActive(true);
            foreach (GameObject bird in birds) bird.SetActive(false);  // Deactivate all Bird icons first
            birds[birdSelected].SetActive(true);  // Activate only the selected Bird icon
            // Unlocked Bird style texts are white and locked texts are grey
            for (int i = 0; i < birdStyleTexts.Length; i++) birdStyleTexts[i].color = birdsBought[i] ? Color.white : Color.grey;
            birdStyleTexts[birdSelected].color = Color.yellow;  // Selected Bird text is yellow
        }
        else if (index == 1)  // Backgrounds
        {
            shopMenu.SetActive(false);
            shopMenu1.SetActive(true);
            foreach (GameObject background in backgrounds) background.SetActive(false);  // Deactivate all Background icons first
            backgrounds[backgroundSelected].SetActive(true);  // Activate only the selected Background icon
            // Unlocked Background style texts are white and locked texts are grey
            for (int i = 0; i < backgroundStyleTexts.Length; i++) backgroundStyleTexts[i].color = backgroundsBought[i] ? Color.white : Color.grey;
            backgroundStyleTexts[backgroundSelected].color = Color.yellow;  // Selected Background text is yellow
        }
        else if (index == 2)  // Obstacles
        {
            shopMenu.SetActive(false);
            shopMenu2.SetActive(true);
            foreach (GameObject obstacle in obstacles) obstacle.SetActive(false);  // Deactivate all Obstacle icons first
            obstacles[obstacleSelected].SetActive(true);  // Activate only the selected Obstacle icon
            // Unlocked Obstacle style texts are white and locked texts are grey
            for (int i = 0; i < obstacleStyleTexts.Length; i++) obstacleStyleTexts[i].color = obstaclesBought[i] ? Color.white : Color.grey;
            obstacleStyleTexts[obstacleSelected].color = Color.yellow;  // Selected Obstacle text is yellow
        }
        else if (index == 3)  // Skills
        {
            shopMenu.SetActive(false);
            shopMenu3.SetActive(true);
            if (skill1Level >= 3) skillBuyButtons[0].SetActive(false);
            if (skill2Level >= 3) skillBuyButtons[1].SetActive(false);
            skillLevelTexts[0].text = skill1Level + " / 3";
            skillLevelTexts[1].text = skill2Level + " / 3";
        }
    }

    public void BirdSelection(int index)
    {
        audioSource.PlayOneShot(uiSelectSound);
        foreach (GameObject bird in birds) bird.SetActive(false);
        birds[index].SetActive(true);
        birdSelected = birdsBought[index] ? index : birdSelected;
        for (int i = 0; i < birdStyleTexts.Length; i++) birdStyleTexts[i].color = birdsBought[i] ? Color.white : Color.grey;
        birdStyleTexts[birdSelected].color = Color.yellow;
        birdBuyButtons[index].SetActive(birdsBought[index] ? false : true);
    }

    public void BackgroundSelection(int index)
    {
        audioSource.PlayOneShot(uiSelectSound);
        foreach (GameObject background in backgrounds) background.SetActive(false);
        backgrounds[index].SetActive(true);
        backgroundSelected = backgroundsBought[index] ? index : backgroundSelected;
        for (int i = 0; i < backgroundStyleTexts.Length; i++) backgroundStyleTexts[i].color = backgroundsBought[i] ? Color.white : Color.grey;
        backgroundStyleTexts[backgroundSelected].color = Color.yellow;
        backgroundBuyButtons[index].SetActive(backgroundsBought[index] ? false : true);
    }

    public void ObstacleSelection(int index)
    {
        audioSource.PlayOneShot(uiSelectSound);
        foreach (GameObject obstacle in obstacles) obstacle.SetActive(false);
        obstacles[index].SetActive(true);
        obstacleSelected = obstaclesBought[index] ? index : obstacleSelected;
        for (int i = 0; i < obstacleStyleTexts.Length; i++) obstacleStyleTexts[i].color = obstaclesBought[i] ? Color.white : Color.grey;
        obstacleStyleTexts[obstacleSelected].color = Color.yellow;
        obstacleBuyButtons[index].SetActive(obstaclesBought[index] ? false : true);
    }

    public void BuyBird(int index)
    {
        if (coin >= 50)
        {
            audioSource.PlayOneShot(buySounds[Random.Range(0, buySounds.Length)]);
            coin -= 50;
            coinText.text = coin.ToString();
            birdsBought[index] = true;
            birdStyleTexts[index].color = Color.white;
            birdBuyButtons[index].SetActive(false);
            birdSelected = index;
            for (int i = 0; i < birdStyleTexts.Length; i++) birdStyleTexts[i].color = birdsBought[i] ? Color.white : Color.grey;
            birdStyleTexts[index].color = Color.yellow;
        }
    }

    public void BuyBackground(int index)
    {
        if (coin >= 50)
        {
            audioSource.PlayOneShot(buySounds[Random.Range(0, buySounds.Length)]);
            coin -= 50;
            coinText.text = coin.ToString();
            backgroundsBought[index] = true;
            backgroundStyleTexts[index].color = Color.white;
            backgroundBuyButtons[index].SetActive(false);
            backgroundSelected = index;
            for (int i = 0; i < backgroundStyleTexts.Length; i++) backgroundStyleTexts[i].color = backgroundsBought[i] ? Color.white : Color.grey;
            backgroundStyleTexts[index].color = Color.yellow;
        }
    }

    public void BuyObstacle(int index)
    {
        if (coin >= 50)
        {
            audioSource.PlayOneShot(buySounds[Random.Range(0, buySounds.Length)]);
            coin -= 50;
            coinText.text = coin.ToString();
            obstaclesBought[index] = true;
            obstacleStyleTexts[index].color = Color.white;
            obstacleBuyButtons[index].SetActive(false);
            obstacleSelected = index;
            for (int i = 0; i < obstacleStyleTexts.Length; i++) obstacleStyleTexts[i].color = obstaclesBought[i] ? Color.white : Color.grey;
            obstacleStyleTexts[index].color = Color.yellow;
        }
    }

    public void BuySkill(int index)
    {
        if (coin >= 100)
        {
            audioSource.PlayOneShot(buySounds[Random.Range(0, buySounds.Length)]);
            coin -= 100;
            coinText.text = coin.ToString();
            if (index == 0) { skill1Level += 1; skillLevelTexts[0].text = skill1Level + " / 3"; }
            else if (index == 1) { skill2Level += 1; skillLevelTexts[1].text = skill2Level + " / 3"; }
            if (skill1Level >= 3) skillBuyButtons[0].SetActive(false);
            if (skill2Level >= 3) skillBuyButtons[1].SetActive(false);
        }
    }

    public void OptionsSelection(int index)  // Options Menu
    {
        audioSource.PlayOneShot(uiSelectSound);
        if (index <= 3)  // Change Difficulty
        {
            difficulty = index;
            foreach (TextMeshProUGUI text in difficultyTexts) text.color = Color.white;
            difficultyTexts[index - 1].color = Color.yellow;
        }
        else if (index == 4)  // Toggle Sound
        {
            soundsCheckmarks[0].SetActive(!soundsCheckmarks[0].activeSelf);
            soundsCheckmarks[1].SetActive(!soundsCheckmarks[1].activeSelf);
            if (AudioListener.volume == 1) AudioListener.volume = 0;
            else AudioListener.volume = 1;
        }
        else if (index == 5)  // Toggle Fps
        {
            fpsText.gameObject.SetActive(!fpsText.gameObject.activeSelf);
            fpsCheckmarks[0].SetActive(!fpsCheckmarks[0].activeSelf);
            fpsCheckmarks[1].SetActive(!fpsCheckmarks[1].activeSelf);
            showFps = showFps == 1 ? 0 : 1;
            if (showFps == 1) InvokeRepeating(nameof(ShowFps), 0, 1f);
            else CancelInvoke(nameof(ShowFps));
        }
        else if (index == 6)  // Toggle Birds
        {
            birdsCheckmarks[0].SetActive(!birdsCheckmarks[0].activeSelf);
            birdsCheckmarks[1].SetActive(!birdsCheckmarks[1].activeSelf);
            spawnBirds = spawnBirds == 1 ? 0 : 1;
        }
    }
    void ShowFps() { fpsText.text = "Fps: " + Mathf.RoundToInt(1 / Time.deltaTime).ToString(); }
}