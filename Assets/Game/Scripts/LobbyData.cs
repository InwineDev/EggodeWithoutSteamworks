using Steamworks;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LobbyData : MonoBehaviour
{
    public CSteamID lobbyID;
    public string lobbyName;
    public TMP_Text lobbyTXT;

    public void SetLobbyData()
    {
        if (lobbyName == "")
        {
            lobbyTXT.text = "Empty";
        }
        else
        {
            lobbyTXT.text = lobbyName;
        }
    }

    public void JoinLobby()
    {
        FindObjectOfType<menuManager>().JoinLobby(lobbyID);
    }
}
