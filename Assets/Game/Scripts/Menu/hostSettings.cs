using Steamworks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class hostSettings : MonoBehaviour
{
    [SerializeField] private Image uiToChangeImage;
    [SerializeField] private Image uiToChangeImageBackground;
    [SerializeField] private Image uiToChangeImageBackgroundUp;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text authorText;

    [SerializeField] private TMP_Dropdown onlineMode;

    [SerializeField] private TMP_InputField playersValue;
    public static Action<Sprite, string, string, Color> onChangeMap;
    private void OnEnable()
    {
        onChangeMap += Change;
    }

    private void OnDisable()
    {
        onChangeMap -= Change;
    }
    public void Change(Sprite sp, string name, string author, Color color)
    {
        uiToChangeImage.sprite = sp;
        uiToChangeImageBackground.sprite = sp;
        Color color1 = color;
        color1.a = 255;
        uiToChangeImageBackgroundUp.color = color1;
        nameText.text = name;
        authorText.text = author;
    }

    public void host()
    {
        ELobbyType lobbyType = ELobbyType.k_ELobbyTypePublic;

        switch (onlineMode.value)
        {
            case 0:
                lobbyType = ELobbyType.k_ELobbyTypePublic;
                print(0);
                break;
            case 1:
                lobbyType = ELobbyType.k_ELobbyTypeFriendsOnly;
                print(1);
                break;
            case 2:
                lobbyType = ELobbyType.k_ELobbyTypePrivate;
                print(2);
                break;
        }
        print(playersValue.text);
        if(login.urlMap == null)
        {
            string[] paths = Directory.GetFiles(Path.Combine(Path.GetDirectoryName(Application.dataPath), "maps"), "*.eggodemap", SearchOption.AllDirectories);
            login.urlMap = paths[UnityEngine.Random.Range(0, paths.Length)];
        }
        //SteamMatchmaking.CreateLobby(lobbyType, int.Parse(playersValue.text));
        MultiplayerManager.instance.CreateLobby(lobbyType, int.Parse(playersValue.text));
    }
}
