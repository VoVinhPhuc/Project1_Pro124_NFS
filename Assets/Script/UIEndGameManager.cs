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
            StartCoroutine(CountAward(award1Text, topFinishers[0].reward));
        }

        // Top 2
        if (topFinishers.Count > 1)
        {
            top2Text.text = topFinishers[1].name;
            StartCoroutine(CountAward(award2Text, topFinishers[1].reward));
        }

        // Top 3
        if (topFinishers.Count > 2)
        {
            top3Text.text = topFinishers[2].name;
            StartCoroutine(CountAward(award3Text, topFinishers[2].reward));
        }
        backToMenuButton.onClick.AddListener(BackToMenu);
        playAgainButton.onClick.AddListener(PlayAgain);
    }

    IEnumerator CountAward(TMP_Text awardText, int targetValue)
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

        // Đảm bảo chính xác giá trị cuối cùng
        awardText.text = $"${targetValue}";

        rewardAnimationDoneCount++;
        if (rewardAnimationDoneCount == 3)
        {
            // Tính tổng coins của người chơi
            int totalCoins = 0;
            foreach (var finisher in GameManager.Instance.GetTopFinishers())
            {
                if (finisher.name == PlayerPrefs.GetString("NickName"))
                {
                    totalCoins += finisher.reward;
                }
            }

            // Cộng coins vào dữ liệu người chơi
            UserDataManager.AddCoinsToUser(LoginManager.loggedInEmail, totalCoins);
        }
    }
    void BackToMenu()
    {
        SceneManager.LoadScene("Menu");
    }

    void PlayAgain()
    {
        // Load lại scene đua vừa chơi
        string previousScene = PlayerPrefs.GetString("LastRaceScene", "RaceScene1"); // đặt mặc định là RaceScene1 nếu không có
        SceneManager.LoadScene(previousScene);
    }
}
