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

    public static RoomManager Instance { get; private set; }

    private void Awake()
    {
        Debug.Log("✅ RoomManager đã được tạo trong Scene Room!");

        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        // ✅ KHỞI TẠO NetworkList TRONG AWAKE
        playerNicknames = new NetworkList<NicknameData>();

        if (IsServer)
        {
            Debug.Log("✅ Host đã khởi tạo NetworkList<NicknameData>!");
        }
    }

    private void Start()
    {
        Debug.Log("✅ RoomManager Start() chạy!");

        if (roomIdText == null)
        {
            Debug.LogError("[RoomManager] ❌ roomIdText chưa được gán trong Inspector!");
        }
        else
        {
            roomIdText.text = "Room ID: " + NetworkManagerUI.RoomID;
        }

        if (playerListText == null)
        {
            Debug.LogError("[RoomManager] ❌ playerListText chưa được gán trong Inspector!");
        }
        else
        {
            playerListText.text = "Waiting for players...";
        }

        if (IsServer)
        {
            Debug.Log("🟢 Là Host, thử gọi OnNetworkSpawn()");
            OnNetworkSpawn();
        }
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        Debug.Log("✅ OnNetworkSpawn() của RoomManager đã chạy!");

        if (playerNicknames == null)
        {
            Debug.LogError("❌ LỖI: playerNicknames vẫn NULL, đang khởi tạo lại!");
            playerNicknames = new NetworkList<NicknameData>();
        }

        playerNicknames.OnListChanged += (changeEvent) =>
        {
            Debug.Log($"[RoomManager] 🔄 NetworkList thay đổi! Tổng số người chơi: {playerNicknames.Count}");
            UpdatePlayerList();
        };

        string nickname = PlayerPrefs.GetString("NickName", "Unknown");
        Debug.Log($"🎭 NickName lấy từ PlayerPrefs: {nickname}");

        if (IsServer)
        {
            ulong hostClientId = NetworkManager.Singleton.LocalClientId;
            Debug.Log($"🏠 Host ID: {hostClientId}");

            playerNicknames.Add(new NicknameData(hostClientId, nickname));
            UpdatePlayerList();
        }
        else
        {
            Debug.Log($"📡 Client gửi NickName lên Host: {nickname}");
            RequestNickNameServerRpc(NetworkManager.Singleton.LocalClientId, nickname);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void RequestNickNameServerRpc(ulong clientId, string nickname)
    {
        Debug.Log($"[RoomManager] 📩 Client {clientId} gửi NickName: {nickname} lên Server");

        if (!IsServer)
        {
            Debug.LogError("[RoomManager] ❌ RequestNickNameServerRpc được gọi trên Client!");
            return;
        }
        string uniqueNick = $"{nickname}_{clientId}"; // Đảm bảo nickname không bị trùng
        Debug.Log($"📡 Client {clientId} gửi NickName: {uniqueNick}");

        AddPlayer(clientId, nickname);
    }
    [ServerRpc(RequireOwnership = false)]
    public void RequestRoomIdServerRpc(ulong clientId)
    {
        Debug.Log($"📡 Client {clientId} yêu cầu Room ID.");
        SendRoomIdClientRpc(NetworkManagerUI.RoomID);
    }
    [ClientRpc]
    public void SendRoomIdClientRpc(string roomId)
    {
        Debug.Log($"✅ Nhận Room ID từ Host: {roomId}");
        if (roomIdText != null)
        {
            roomIdText.text = "Room ID: " + roomId;
        }
    }
    private void AddPlayer(ulong clientId, string nickname)
    {
        foreach (var player in playerNicknames)
        {
            if (player.ClientId == clientId) return;
        }

        playerNicknames.Add(new NicknameData(clientId, nickname));
        UpdatePlayerList();
    }

    private void UpdatePlayerList()
    {
        if (playerListText == null)
        {
            Debug.LogError("[RoomManager] ❌ playerListText NULL! Không thể cập nhật UI.");
            return;
        }

        Debug.Log("[RoomManager] 🔄 Cập nhật danh sách người chơi trên UI.");
        playerListText.text = "Players in Room:\n";

        foreach (var player in playerNicknames)
        {
            Debug.Log($"[RoomManager] 📋 Người chơi: {player.NickName}");
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
