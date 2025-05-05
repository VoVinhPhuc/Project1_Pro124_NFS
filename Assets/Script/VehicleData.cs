using UnityEngine;

[System.Serializable]
public class VehicleData
{
    public string skinName;          // Tên skin của xe (Player Variant 1, Player Variant 2, ...)
    public string vehicleName;       // Tên xe hiển thị (Motorbike, Police Car, ...)
    public Sprite vehicleSprite;     // Hình ảnh của xe
    public int mapPanelIndex;        // Chỉ số map (1, 2, 3) sẽ được hiển thị khi chọn xe
}
