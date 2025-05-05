using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class UIEndGameManager : MonoBehaviour
{
    public static UIEndGameManager Instance;

    [Header("UI Elements")]
    public TMP_Text top1Text;
    public TMP_Text top2Text;
    public TMP_Text top3Text;
    public TMP_Text award1Text;
    public TMP_Text award2Text;
    public TMP_Text award3Text;

    [Header("Buttons")]
    public Button backToMenuButton;
    public Button playAgainButton;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        backToMenuButton.onClick.AddListener(BackToMenu);
        playAgainButton.onClick.AddListener(PlayAgain);
        ShowEndGameResults();
    }

    public void ShowEndGameResults()
    {
        List<RaceResult> topFinishers = GameManager.Instance.GetTopFinishers();

        DisplayTopFinisher(topFinishers, 0, top1Text, award1Text);
        DisplayTopFinisher(topFinishers, 1, top2Text, award2Text);
        DisplayTopFinisher(topFinishers, 2, top3Text, award3Text);
    }

    private void DisplayTopFinisher(List<RaceResult> topFinishers, int index, TMP_Text nameText, TMP_Text awardText)
    {
        if (topFinishers.Count > index)
        {
            var finisher = topFinishers[index];
            nameText.text = finisher.name;
            StartCoroutine(CountAward(awardText, finisher.reward, finisher.name));
        }
        else
        {
            nameText.text = "";
            awardText.text = "$0";
        }
    }

    IEnumerator CountAward(TMP_Text awardText, int targetValue, string finisherName)
    {
        float duration = 4f;
        float currentValue = 0f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            currentValue = Mathf.Lerp(0, targetValue, elapsed / duration);
            awardText.text = $"${Mathf.FloorToInt(currentValue)}";
            yield return null;
        }

        awardText.text = $"${targetValue}";

        // ✅ Cộng coin nếu là player
        if (finisherName == GameManager.Instance.GetPlayerNickName())
        {
            UserDataManager.AddCoinsToUser(GameManager.Instance.GetPlayerEmail(), targetValue);
        }
    }

    void BackToMenu()
    {
        SceneManager.LoadScene("Menu");
    }

    void PlayAgain()
    {
        string previousScene = GameManager.Instance.GetLastRaceScene();
        SceneManager.LoadScene(previousScene);
    }
}
