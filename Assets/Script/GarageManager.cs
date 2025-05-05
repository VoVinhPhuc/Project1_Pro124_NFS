using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GarageManager : MonoBehaviour
{
    [System.Serializable]
    public class VehicleData
    {
        public string skinName;             // Tên lưu trong UserData
        public string displayName;          // Tên hiển thị UI
        public Sprite vehicleSprite;        // Ảnh xe
        public GameObject mapSelectPanel;   // Panel map riêng
        public GameObject vehiclePanel;     // Panel xe tương ứng trong UI
    }
    public Button bikeSelectButton;
    public Button policeSelectButton;
    public Button fireTruckSelectButton;

    public List<VehicleData> vehicleDatas;       // Gán data cho từng xe trong Inspector

    private string currentUserEmail;

    void Start()
    {
        currentUserEmail = PlayerPrefs.GetString("email");

        bikeSelectButton.onClick.AddListener(() => SelectVehicle("Player Variant 1"));
        policeSelectButton.onClick.AddListener(() => SelectVehicle("Player Variant 2"));
        fireTruckSelectButton.onClick.AddListener(() => SelectVehicle("Player Variant 3"));

        LoadGarage();
    }

    void LoadGarage()
    {
        UserList users = UserDataManager.LoadUsers();
        foreach (var user in users.users)
        {
            if (user.email == currentUserEmail)
            {
                // Ẩn tất cả panel xe trước
                foreach (var vehicle in vehicleDatas)
                {
                    vehicle.vehiclePanel.SetActive(false);
                }

                // Hiển thị panel xe mà người chơi sở hữu
                foreach (string skinName in user.ownedSkins)
                {
                    VehicleData data = vehicleDatas.Find(v => v.skinName == skinName);
                    if (data != null)
                    {
                        SetupVehiclePanel(data); // Thiết lập panel xe
                    }
                }
                break;
            }
        }
    }

    public void SetupVehiclePanel(VehicleData data)
    {
        if (data.vehiclePanel == null)
        {
            Debug.LogError($"Vehicle panel chưa được gán cho {data.skinName}");
            return;
        }

        data.vehiclePanel.SetActive(true);

        // Kiểm tra các thành phần UI
        Transform panelVehicle = data.vehiclePanel.transform.Find("Panel Vehicle");
        if (panelVehicle == null)
        {
            Debug.LogError($"Không tìm thấy 'Panel Vehicle' trong {data.vehiclePanel.name}");
            return;
        }

        // Thiết lập thông tin xe
        TextMeshProUGUI nameText = panelVehicle.Find("VehicleName")?.GetComponent<TextMeshProUGUI>();
        Image vehicleImage = panelVehicle.Find("VehicleImage")?.GetComponent<Image>();
        Button selectBtn = panelVehicle.Find("SelectButton")?.GetComponent<Button>();

        if (nameText != null) nameText.text = data.displayName;
        else Debug.LogError("Không tìm thấy VehicleName");

        if (vehicleImage != null) vehicleImage.sprite = data.vehicleSprite;
        else Debug.LogError("Không tìm thấy VehicleImage");

        if (selectBtn != null)
        {
            selectBtn.onClick.RemoveAllListeners();
            selectBtn.onClick.AddListener(() => SelectVehicle(data.skinName));
        }
        else Debug.LogError("Không tìm thấy SelectButton");
    }

    void SelectVehicle(string skinName)
    {
        Debug.Log($"Đang chọn xe: {skinName}");

        // 1. Lưu skin được chọn
        UserList users = UserDataManager.LoadUsers();
        foreach (var user in users.users)
        {
            if (user.email == currentUserEmail)
            {
                user.selectedSkin = skinName;
                UserDataManager.SaveUsers(users);
                Debug.Log($"Đã lưu skin: {skinName}");
                break;
            }
        }

        // 2. Tắt tất cả map panel trước
        foreach (var vehicle in vehicleDatas)
        {
            if (vehicle.mapSelectPanel != null)
            {
                vehicle.mapSelectPanel.SetActive(false);
                Debug.Log($"Đã tắt: {vehicle.skinName}'s map panel");
            }
        }

        // 3. Bật panel map tương ứng
        VehicleData selectedData = vehicleDatas.Find(v => v.skinName == skinName);
        if (selectedData != null && selectedData.mapSelectPanel != null)
        {
            selectedData.mapSelectPanel.SetActive(true);
            Debug.Log($"Đã bật map panel cho: {selectedData.skinName}");
        }
        else
        {
            Debug.LogError($"Không tìm thấy map panel cho: {skinName}");
        }
    }
}