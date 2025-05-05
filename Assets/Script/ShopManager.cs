using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class ShopManager : MonoBehaviour
{
    public TextMeshProUGUI feedbackText; // Text báo kết quả (Success hoặc Not Enough Coins)
    public TextMeshProUGUI coinsText;    // Text hiển thị số coins
    public GameObject shopPanel;

    public GarageManager garageManager;  // Gán trong Inspector

    void Start()
    {
        shopPanel.SetActive(false);
        UpdateCoinsText();
    }

    void OnEnable()
    {
        UpdateCoinsText();
    }

    public void BuyMotorBikeSkin()
    {
        BuySkin("Player Variant 1", 2000);
    }

    public void BuyPoliceSkin()
    {
        BuySkin("Player Variant 2", 5000);
    }

    public void BuyFireTruckSkin()
    {
        BuySkin("Player Variant 3", 10000);
    }

    public void BuySkin(string skinName, int price)
    {
        string email = PlayerPrefs.GetString("email");

        UserList users = UserDataManager.LoadUsers();
        foreach (var user in users.users)
        {
            if (user.email == email)
            {
                if (user.coins >= price)
                {
                    bool isNewSkin = false;

                    // Nếu chưa sở hữu thì thêm vào
                    if (!user.ownedSkins.Contains(skinName))
                    {
                        user.ownedSkins.Add(skinName);
                        isNewSkin = true;
                    }

                    user.coins -= price;
                    UserDataManager.SaveUsers(users);

                    if (isNewSkin && garageManager != null)
                    {
                        GarageManager.VehicleData data = garageManager.vehicleDatas.Find(v => v.skinName == skinName);
                        if (data != null)
                        {
                            garageManager.SetupVehiclePanel(data);
                        }
                    }

                    feedbackText.text = "Success!";
                    feedbackText.color = Color.green;
                    feedbackText.gameObject.SetActive(true);
                    Invoke("HideFeedback", 3f);

                    UpdateCoinsText();
                }
                else
                {
                    feedbackText.text = "Not Enough Money!";
                    feedbackText.color = Color.red;
                    feedbackText.gameObject.SetActive(true);
                    Invoke("HideFeedback", 3f);
                }

                return;
            }
        }
    }

    public void UpdateCoinsText()
    {
        string email = PlayerPrefs.GetString("email");
        UserList users = UserDataManager.LoadUsers();

        foreach (var user in users.users)
        {
            if (user.email == email)
            {
                coinsText.text = $"$ {user.coins}";
                return;
            }
        }

        coinsText.text = "$ 0";
    }

    private void HideFeedback()
    {
        feedbackText.gameObject.SetActive(false);
    }

    public void CloseShop()
    {
        shopPanel.SetActive(false);
    }
}
