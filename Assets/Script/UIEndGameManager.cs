using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIEndGameManager : MonoBehaviour
{
    public TMP_Text top1Text, top2Text, top3Text;
    public TMP_Text award1Text, award2Text, award3Text;

    private int rewardAnimationDoneCount = 0;

    [Header("Buttons")]
    public Button backToMenuButton;
    public Button playAgainButton;

    private void Start()
    {
        List<RaceResult> topFinishers = GameManager.Instance.GetTopFinishers();

        // Top 1
        if (topFinishers.Count > 0)
        {
            top1Text.text = topFinishers[0].name;
            StartCoroutine(CountAward(award1Text, topFinishers[0].reward, topFinishers[0].name));
        }
        else
        {
            top1Text.text = "";
            StartCoroutine(CountAward(award1Text, 0, ""));
        }

        // Top 2
        if (topFinishers.Count > 1)
        {
            top2Text.text = topFinishers[1].name;
            StartCoroutine(CountAward(award2Text, topFinishers[1].reward, topFinishers[1].name));
        }
        else
        {
            top2Text.text = "";
            StartCoroutine(CountAward(award2Text, 0, ""));
        }

        // Top 3
        if (topFinishers.Count > 2)
        {
            top3Text.text = topFinishers[2].name;
            StartCoroutine(CountAward(award3Text, topFinishers[2].reward, topFinishers[2].name));
        }
        else
        {
            top3Text.text = "";
            StartCoroutine(CountAward(award3Text, 0, ""));
        }

        backToMenuButton.onClick.AddListener(BackToMenu);
        playAgainButton.onClick.AddListener(PlayAgain);
    }

    IEnumerator CountAward(TMP_Text awardText, int targetValue, string finisherName)
    {
        float duration = 5f;
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

        // Nếu là người chơi, cộng vào tổng thưởng
        if (finisherName == PlayerPrefs.GetString("NickName"))
        {
            UserDataManager.AddCoinsToUser(LoginManager.loggedInEmail, targetValue);
        }

        rewardAnimationDoneCount++;
    }

    void BackToMenu()
    {
        SceneManager.LoadScene("Menu");
    }

    void PlayAgain()
    {
        string previousScene = PlayerPrefs.GetString("LastRaceScene", "RaceScene1");
        SceneManager.LoadScene(previousScene);
    }
}
