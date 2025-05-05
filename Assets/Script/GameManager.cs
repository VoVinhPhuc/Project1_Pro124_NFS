using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class RaceResult
{
    public string name; // Nickname hoặc "NPCx"
    public int reward;
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("UI Panels")]
    public GameObject victoryPanel;
    public TMP_Text npcFinishedText;
    public GameObject outOfTimePanel;
    public TMP_Text timerText;
    public GameObject pausePanel;

    [Header("Audio UI")]
    public Slider volumeSlider;

    [Header("Race Logic")]
    public float countdownTime = 120f;
    private int npcFinishedCount = 0;
    private List<RaceResult> topFinishers = new List<RaceResult>();
    private int totalFinishers = 0;
    private bool isPaused = false;
    private bool gameEnded = false;

    private string playerNickName = "";
    private string playerEmail = "";
    private string lastRaceScene = "";

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(this.gameObject);
    }

    private void Start()
    {
        outOfTimePanel.SetActive(false);
        pausePanel.SetActive(false);

        volumeSlider.onValueChanged.AddListener(SetVolume);
        volumeSlider.value = AudioListener.volume;

        lastRaceScene = SceneManager.GetActiveScene().name;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P) && !gameEnded)
        {
            TogglePause();
        }
    }

    void TogglePause()
    {
        isPaused = !isPaused;
        pausePanel.SetActive(isPaused);
        Time.timeScale = isPaused ? 0f : 1f;
        AudioListener.pause = isPaused;
    }

    public void SetVolume(float volume)
    {
        AudioListener.volume = volume;
    }

    public void OnRestartButton()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void OnBackToMenuButton()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Menu");
    }

    public void StartCountdownTimer()
    {
        StartCoroutine(CountdownTimer());
    }

    IEnumerator CountdownTimer()
    {
        float timeRemaining = countdownTime;
        bool flashingStarted = false;

        while (timeRemaining > 0)
        {
            if (gameEnded) yield break;

            timeRemaining -= Time.deltaTime;
            int minutes = Mathf.FloorToInt(timeRemaining / 60f);
            int seconds = Mathf.FloorToInt(timeRemaining % 60f);
            timerText.text = $"{minutes:00}:{seconds:00}";

            if (timeRemaining <= 10f && !flashingStarted)
            {
                StartCoroutine(FlashTimerText());
                flashingStarted = true;
            }

            yield return null;
        }

        if (!gameEnded)
        {
            StartCoroutine(HandleOutOfTime());
        }
    }

    IEnumerator FlashTimerText()
    {
        bool visible = true;
        while (true)
        {
            timerText.color = visible ? Color.red : Color.white;
            visible = !visible;
            yield return new WaitForSeconds(0.3f);
        }
    }

    IEnumerator HandleOutOfTime()
    {
        gameEnded = true;
        outOfTimePanel.SetActive(true);

        yield return new WaitForSeconds(3f);
        outOfTimePanel.SetActive(false);

        if (topFinishers.Count > 0)
        {
            victoryPanel.SetActive(true);
            yield return new WaitForSeconds(5f);
        }

        SceneManager.LoadScene("UI end game");
    }

    public void NPCFinished(GameObject npc)
    {
        npcFinishedCount++;
        npc.SetActive(false);

        npcFinishedText.text = $"{npcFinishedCount} NPC finished";
        npcFinishedText.gameObject.SetActive(true);

        Invoke(nameof(HideNPCMessage), 3f);
    }

    void HideNPCMessage()
    {
        npcFinishedText.gameObject.SetActive(false);
    }

    public void AddFinisher(string name, bool isPlayer)
    {
        if (gameEnded) return;

        int reward = 0;
        switch (topFinishers.Count)
        {
            case 0: reward = 1000; break;
            case 1: reward = 500; break;
            case 2: reward = 200; break;
        }

        topFinishers.Add(new RaceResult { name = name, reward = reward });
        totalFinishers++;

        if (totalFinishers == 3)
        {
            gameEnded = true;
            StartCoroutine(ShowTop3AndEndGame());
        }
    }

    IEnumerator ShowTop3AndEndGame()
    {
        victoryPanel.SetActive(true);
        yield return new WaitForSeconds(5f);
        SceneManager.LoadScene("UI end game");
    }

    public List<RaceResult> GetTopFinishers()
    {
        return topFinishers;
    }

    // ✅ Thêm để đồng bộ với UIEndGameManager
    public void SetPlayerInfo(string nickName, string email)
    {
        playerNickName = nickName;
        playerEmail = email;
    }

    public string GetPlayerNickName() => playerNickName;
    public string GetPlayerEmail() => playerEmail;
    public string GetLastRaceScene() => lastRaceScene;
}
