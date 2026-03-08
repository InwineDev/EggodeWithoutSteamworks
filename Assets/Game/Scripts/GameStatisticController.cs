using Steamworks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using UnityEngine;
using UnityEngine.Networking;

public class GameStatisticController : MonoBehaviour
{
    private void OnApplicationQuit()
    {
        StartCoroutine(GetJsonFromAddress("»грок покинул игру"));
    }

    public IEnumerator GetJsonFromAddress(string message)
    {
        if (settingsController.sborDannie == true)
        {
            string data = $"{message}\nSteamLogin:{SteamUser.GetSteamID().ToString()}\nSteamName:{SteamFriends.GetPersonaName()}\nGameSettings:\n{settingsController.jsonSettings}\nVersion:{menuManager.publicVersion}";

            string encodedData = Uri.EscapeDataString(data);

            string url = $"https://www.epicsusgames.ru/eggode2/data/sendinfo.php?moreinfo={encodedData}";

            UnityWebRequest request = UnityWebRequest.Get(url);
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("Error: " + request.error);
            }
            else
            {
                Debug.Log("Successfully sent data!");
            }
        }
    }

    public void buttonEvent(string what)
    {
        StartCoroutine(GetJsonFromAddress(what));
    }
    void Start()
    {
        StartCoroutine(GetJsonFromAddress("»грок зашЄл в меню"));
    }
}
