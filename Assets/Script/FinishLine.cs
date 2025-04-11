using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class LapCounter : MonoBehaviour
{
    public int totalLaps = 3;
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
