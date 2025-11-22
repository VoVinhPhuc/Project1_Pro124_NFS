using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class GameManagerTests
{
    // Test Case 1: Kiểm tra logic tính điểm thưởng cho người về Nhất
    [Test]
    public void TestFirstPlaceReward()
    {
        // 1. Setup: Tạo GameObject giả và gắn script GameManager vào
        GameObject gameObj = new GameObject();
        var gm = gameObj.AddComponent<GameManager>();

        // 2. Action: Giả lập có người về đích đầu tiên
        gm.AddFinisher("NguoiChoi1", true);

        // 3. Assert: Lấy danh sách người về đích và kiểm tra điểm
        List<RaceResult> results = gm.GetTopFinishers();
        
        // Kiểm tra danh sách có 1 người
        Assert.AreEqual(1, results.Count);
        // Kiểm tra người đầu tiên có đúng là nhận 1000 điểm không
        Assert.AreEqual(1000, results[0].reward);
        // Kiểm tra tên có đúng không
        Assert.AreEqual("NguoiChoi1", results[0].name);

        // Dọn dẹp (Clean up)
        Object.DestroyImmediate(gameObj);
    }

    // Test Case 2: Kiểm tra logic tính điểm cho người về Nhì
    [Test]
    public void TestSecondPlaceReward()
    {
        // 1. Setup
        GameObject gameObj = new GameObject();
        var gm = gameObj.AddComponent<GameManager>();

        // 2. Action: Giả lập 2 người về đích liên tiếp
        gm.AddFinisher("NguoiChoi1", true); // Về nhất
        gm.AddFinisher("NPC_Enemy", false); // Về nhì

        // 3. Assert
        List<RaceResult> results = gm.GetTopFinishers();

        // Người thứ 2 (index 1) phải nhận được 500 điểm
        Assert.AreEqual(500, results[1].reward);
        Assert.AreEqual("NPC_Enemy", results[1].name);

        Object.DestroyImmediate(gameObj);
    }

    // Test Case 3: Kiểm tra chức năng lưu thông tin người chơi (Setter/Getter)
    [Test]
    public void TestSetPlayerInfo()
    {
        // 1. Setup
        GameObject gameObj = new GameObject();
        var gm = gameObj.AddComponent<GameManager>();

        string testName = "ProRacer";
        string testEmail = "test@gmail.com";

        // 2. Action
        gm.SetPlayerInfo(testName, testEmail);

        // 3. Assert: Kiểm tra xem dữ liệu lấy ra có giống dữ liệu nạp vào không
        Assert.AreEqual(testName, gm.GetPlayerNickName());
        Assert.AreEqual(testEmail, gm.GetPlayerEmail());

        Object.DestroyImmediate(gameObj);
    }
}