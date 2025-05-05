using UnityEngine;
using System.IO;
using System.Collections.Generic;

[System.Serializable]
public class UserData
{
    public string email;
    public string password;
    public string nickName;
    public int coins;
    public List<string> ownedSkins = new List<string>();
    public string selectedSkin;
}

[System.Serializable]
public class UserList
{
    public List<UserData> users = new List<UserData>();
}

public static class UserDataManager
{
    private static string filePath = Path.Combine(Application.persistentDataPath, "UserData.txt");

    public static UserList LoadUsers()
    {
        if (!File.Exists(filePath))
            return new UserList();

        string json = File.ReadAllText(filePath);
        return JsonUtility.FromJson<UserList>(json) ?? new UserList();
    }

    public static void SaveUsers(UserList userList)
    {
        string json = JsonUtility.ToJson(userList, true);
        File.WriteAllText(filePath, json);
    }

    public static bool IsNickNameTaken(string nickName)
    {
        UserList userList = LoadUsers();
        foreach (var user in userList.users)
        {
            if (user.nickName == nickName)
                return true;
        }
        return false;
    }

    public static void UpdateNickName(string email, string nickName)
    {
        UserList userList = LoadUsers();
        foreach (var user in userList.users)
        {
            if (user.email == email)
            {
                user.nickName = nickName;
                SaveUsers(userList);
                return;
            }
        }
    }
    public static void AddCoinsToUser(string email, int coinsToAdd)
    {
        UserList userList = LoadUsers();
        foreach (var user in userList.users)
        {
            if (user.email == email)
            {
                user.coins += coinsToAdd;
                SaveUsers(userList);
                Debug.Log($"Đã cộng {coinsToAdd} coins cho {email}. Tổng mới: {user.coins}");
                return;
            }
        }
    }
}
