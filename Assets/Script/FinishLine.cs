using UnityEngine;

public class LapCounter : MonoBehaviour
{
    public int totalLaps = 3;  // Số vòng cần hoàn thành
    private int currentLap = 0; // Số vòng hiện tại
    private bool canCountLap = true; // Kiểm soát việc tăng số vòng

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("FinishLine") && canCountLap)
        {
            currentLap++;
            Debug.Log("Lap: " + currentLap);

            if (currentLap >= totalLaps)
            {
                Debug.Log("Race Finished!");

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
        Debug.Log("Bạn đã về đích!");
    }
}

