using UnityEngine;
using Unity.Netcode;
using TMPro;
using System.Collections.Generic;
using Unity.Collections;
using System;

public class RoomManager : NetworkBehaviour
{
    public TMP_Text roomIdText;
    public TMP_Text playerListText;

    private NetworkList<NicknameData> playerNicknames;

    private void Awake()
    {
        Debug.Log("✅ RoomManager đã được tạo trong Scene Room!");
        // Khởi tạo danh sách người chơi đồng bộ trên mạng
        if (playerNicknames == null)
        {
            playerNicknames = new NetworkList<NicknameData>();
        }
    }

    private void Start()
    {
        Debug.Log("✅ RoomManager Start() chạy!");

        if (IsServer)
        {
            Debug.Log("🟢 Là Host, thử gọi OnNetworkSpawn()");
            OnNetworkSpawn(); // Gọi thủ công nếu nó chưa chạy
        }

        roomIdText.text = "Room ID Test";
        playerListText.text = "Waiting for players...";
        if (roomIdText != null)
        {
            roomIdText.text = "Room ID: " + NetworkManagerUI.RoomID;
        }

        if (playerListText == null)
        {
            Debug.LogError("[RoomManager] Lỗi: playerListText chưa được gán!");
        }

        // Lắng nghe sự thay đổi danh sách người chơi
        playerNicknames.OnListChanged += (changeEvent) =>
        {
            Debug.Log($"[RoomManager] 🔄 NetworkList thay đổi! Tổng số người chơi: {playerNicknames.Count}");
            UpdatePlayerList();
        };
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        Debug.Log("✅ OnNetworkSpawn() của RoomManager đã chạy!");

        string nickname = PlayerPrefs.GetString("NickName", "Unknown");
        Debug.Log($"[RoomManager] 🎭 NickName lấy từ PlayerPrefs: {nickname}");

        if (IsServer)
        {
            Debug.Log("Host đang tự Spawn RoomManager!");
            SpawnRoomManagerServerRpc();

            ulong hostClientId = NetworkManager.Singleton.LocalClientId;
            Debug.Log($"[RoomManager] 🏠 Host ID: {hostClientId}");
            playerNicknames.Add(new NicknameData(hostClientId, nickname));

            Debug.Log($"[RoomManager] 🏠 Host vào phòng - NickName: {nickname}");
            AddPlayer(NetworkManager.Singleton.LocalClientId, "Host: " + nickname);

            Debug.Log($"[RoomManager] 📝 Danh sách sau khi Host vào: {playerNicknames.Count} người chơi");
            UpdatePlayerList();
        }
        else
        {
            Debug.Log($"[RoomManager] 📡 Client gửi NickName lên Host: {nickname}");
            RequestNickNameServerRpc(NetworkManager.Singleton.LocalClientId, nickname);
        }
    }
    [ServerRpc]
    private void SpawnRoomManagerServerRpc()
    {
        Debug.Log("Server đang Spawn RoomManager...");
        GetComponent<NetworkObject>().Spawn();
    }
    [ServerRpc(RequireOwnership = false)]
    public void RequestNickNameServerRpc(ulong clientId, string nickname)
    {
        Debug.Log($"[RoomManager] 📩 Client {clientId} gửi NickName: {nickname} lên Server");
        AddPlayer(clientId, nickname);
    }

    private void AddPlayer(ulong clientId, string nickname)
    {
        // Kiểm tra xem ClientId đã có trong danh sách chưa
        foreach (var player in playerNicknames)
        {
            if (player.ClientId == clientId) return;
        }

        // Thêm người chơi vào danh sách
        playerNicknames.Add(new NicknameData(clientId, nickname));
        UpdatePlayerList();
    }

    private void UpdatePlayerList()
    {
        Debug.Log("[RoomManager] Cập nhật danh sách người chơi trên UI.");
        //if (playerListText == null) return;

        playerListText.text = "Players in Room:\n";
        if (playerNicknames.Count == 0)
        {
            Debug.LogWarning("[RoomManager] ❌ Không có người chơi nào trong danh sách!");
        }
        foreach (var player in playerNicknames)
        {
            Debug.Log($"[RoomManager] Cập nhật UI - NickName: {player.NickName}");
            playerListText.text += player.NickName.ToString() + "\n";
        }
        Invoke(nameof(ForceRefreshUI), 0.1f);
    }
    private void ForceRefreshUI()
    {
        Debug.Log("[RoomManager] 🔃 Refresh UI bằng ForceMeshUpdate");
        playerListText.ForceMeshUpdate();
    }
}

// Lớp dữ liệu để lưu NickName
[System.Serializable]
public struct NicknameData : INetworkSerializable, IEquatable<NicknameData>
{
    public ulong ClientId;
    public FixedString32Bytes NickName;

    public NicknameData(ulong clientId, FixedString32Bytes nickName)
    {
        ClientId = clientId;
        NickName = nickName;
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref ClientId);
        serializer.SerializeValue(ref NickName);
    }

    public bool Equals(NicknameData other)
    {
        return ClientId == other.ClientId && NickName.Equals(other.NickName);
    }

    public override int GetHashCode()
    {
        return ClientId.GetHashCode() ^ NickName.GetHashCode();
    }
}
