using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public GameObject victoryPanel;
    public GameObject loadingPanel;
    public TMP_Text npcFinishedText;

    private int npcFinishedCount = 0;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void ShowVictoryPanel()
    {
        victoryPanel.SetActive(true);
        StartCoroutine(LoadEndGameScene());
    }

    public void NPCFinished(GameObject npc)
    {
        npcFinishedCount++;
        npc.SetActive(false); // Ẩn NPC đã về đích

        npcFinishedText.text = $"{npcFinishedCount} NPC finished";
        npcFinishedText.gameObject.SetActive(true);

        Invoke(nameof(HideNPCMessage), 3f); // Ẩn sau 3s
    }

    void HideNPCMessage()
    {
        npcFinishedText.gameObject.SetActive(false);
    }

    IEnumerator LoadEndGameScene()
    {
        yield return new WaitForSeconds(5f);
        loadingPanel.SetActive(true);
        yield return new WaitForSeconds(2f); // Loading effect
        SceneManager.LoadScene("UI end game");
    }
}
