using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CarShop : MonoBehaviour
{
    public GameObject carPrefab; // Prefab của CarItem
    public Transform contentPanel; // Panel chứa các xe
    public TextMeshProUGUI moneyText;
    public int playerMoney = 5000;

    private void Start()
    {
        LoadCars();
        UpdateMoneyUI();
    }

    void LoadCars()
    {
        string[] carNames = { "Lamborghini", "Ferrari", "Porsche" };
        int[] carPrices = { 5000, 7000, 10000 };

        for (int i = 0; i < carNames.Length; i++)
        {
            GameObject carItem = Instantiate(carPrefab, contentPanel);
            carItem.transform.Find("CarName").GetComponent<TextMeshProUGUI>().text = carNames[i];
            carItem.transform.Find("Price").GetComponent<TextMeshProUGUI>().text = carPrices[i] + " $";
            
            Button buyButton = carItem.transform.Find("BuyButton").GetComponent<Button>();
            int price = carPrices[i];
            buyButton.onClick.AddListener(() => BuyCar(price));
        }
    }

    void BuyCar(int price)
    {
        if (playerMoney >= price)
        {
            playerMoney -= price;
            UpdateMoneyUI();
        }
        else
        {
            Debug.Log("Không đủ tiền!");
        }
    }

    void UpdateMoneyUI()
    {
        moneyText.text = "Tiền: " + playerMoney.ToString();
    }
}
