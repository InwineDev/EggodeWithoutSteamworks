using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TMPro;
using System;
using UnityEngine.ParticleSystemJobs;
using Steamworks;

public class menuManager : NetworkBehaviour
{
    public NetworkManager networkManager;
    public TMP_InputField address;
    public GameObject openmama;
    public GameObject LobbyListSUS;
    public string version;
    public static string publicVersion;

    public TMP_Text versionText;

    private void Awake()
    {
        publicVersion = version;
        versionText.text = publicVersion;
        Cursor.lockState = CursorLockMode.None;
    }

    public void openmamavoid()
    {
        openmama.SetActive(true);
    }

    public void closemamavoid()
    {
        openmama.SetActive(false);
    }

    public void openmamasvoid()
    {
        LobbyListSUS.SetActive(true);
    }

    public void closemamasvoid()
    {
        LobbyListSUS.SetActive(false);
    }

    public void CloseGame()
    {
        Application.Quit();
    }
    public void JoinClient()
    {
        // Разделите ввод по запятой (или другому разделителю, если необходимо)

        string[] address_code = address.text.Split(',');
        print(address_code[0] + " " + address_code[1] + " " + address_code[2] + " " + address_code[3]);
        // Проверьте, что у вас есть достаточно элементов
        if (address_code.Length == 4)
        {
            // Конвертируйте значения из строк в числа
            int part0 = int.Parse(address_code[0]);
            int part2 = int.Parse(address_code[1]);
            int part3 = int.Parse(address_code[2]);
            int part4 = int.Parse(address_code[3]);

            // Выполните вычисления для получения нового IP-адреса
            int newPart0 = (part0 - 641) / 2;
            int newPart2 = (part2 - 641) / 2;
            int newPart3 = (part3 - 641) / 2;
            int newPart4 = (part4 - 641) / 2;
            print(newPart0 + " " + newPart2 + " " + newPart3 + " " + newPart4);

            // Установите новый IP-адрес в networkManager
            networkManager.networkAddress = $"{newPart0}.{newPart2}.{newPart3}.{newPart4}";
            networkManager.StartClient();
        }
        else
        {
            networkManager.networkAddress = "localhost";
            networkManager.StartClient();
        }
    }
    public void JoinClientas()
    {
        GameLobbyJoinRequested_t s = new GameLobbyJoinRequested_t();
        if (address.text == "EpicSUS")
        {
            networkManager.networkAddress = "26.216.118.78";
            SteamMatchmaking.JoinLobby(s.m_steamIDLobby);
            networkManager.StartClient();
        } else if (address.text == "693,1073,877,797")
        {
            networkManager.networkAddress = "26.216.118.78";
            SteamMatchmaking.JoinLobby(s.m_steamIDLobby);
            networkManager.StartClient();
        }
        else
        {
            networkManager.networkAddress = address.text;
            SteamMatchmaking.JoinLobby(s.m_steamIDLobby);
            networkManager.StartClient();
        }
    }

    public void JoinLobby(CSteamID lobbyID)
    {
        if (!SteamAPI.Init())
        {
            Debug.LogError("SteamAPI не инициализирован!");
            return;
        }
        SteamMatchmaking.JoinLobby(lobbyID);
    }
    protected Callback<LobbyEnter_t> lobbyEnteredCallback;

    private void OnEnable()
    {
        lobbyEnteredCallback = Callback<LobbyEnter_t>.Create(OnLobbyEntered);
        networkManager.StopClient();
        networkManager.StopHost();
    }

    private void OnLobbyEntered(LobbyEnter_t callback)
    {
        if (callback.m_EChatRoomEnterResponse == (uint)EChatRoomEnterResponse.k_EChatRoomEnterResponseSuccess)
        {
            Debug.Log("Успешно вошли в лобби!");
            // Здесь можно запускать клиент Mirror
            networkManager.StartClient();
        }
        else
        {
            Debug.LogError($"Ошибка входа: {callback.m_EChatRoomEnterResponse}");
        }
    }
    public GameObject selectMap;

    public void HostGame()
    {
        selectMap.SetActive(true);
    }
}

