using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class LapCounter : MonoBehaviour
{
    public int totalLaps = 4;
    private int currentLap = 0;
    private bool canCountLap = true;
    public bool isPlayer = false;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("FinishLine") && canCountLap)
        {
            currentLap++;
            Debug.Log($"{gameObject.name} Lap: {currentLap}");

            if (currentLap >= totalLaps)
            {
                RaceFinished();
            }

            canCountLap = false;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("FinishLine"))
        {
            canCountLap = true;
        }
    }

    void RaceFinished()
    {
        string racerName = isPlayer ? PlayerPrefs.GetString("NickName", "Player") : gameObject.name; // hoặc đặt là "NPC1", "NPC2",...

        GameManager.Instance.AddFinisher(racerName, isPlayer);

        if (isPlayer)
        {
            GameManager.Instance.ShowVictoryPanel();
        }
        else
        {
            GameManager.Instance.NPCFinished(gameObject);
        }
    }
}
