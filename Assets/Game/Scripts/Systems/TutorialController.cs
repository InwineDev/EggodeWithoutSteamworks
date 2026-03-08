using Mirror;
using Steamworks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialController : MonoBehaviour
{
    private void Start()
    {
        //PlayerPrefs.SetInt("FirstLaunch", 0);
        if (PlayerPrefs.GetInt("FirstLaunch") != 1)
        {
            PlayerPrefs.SetInt("FirstLaunch", 1);
            StartTutorial();
        }
    }

    public void StartTutorial()
    {
        if (!SteamManager.Initialized) 
        {
            print("Oh! SHIT!");
            PlayerPrefs.SetInt("FirstLaunch", 0);
            return; 
        }
        try
        {
            NetworkManager omg = FindObjectOfType<NetworkManager>();
            SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypePrivate, 1);
            omg.onlineScene = "Tutorial";
            //SceneManager.LoadScene("Tutorial");
            //login.urlMap = "";
            omg.StartHost();
        }
        catch 
        {
            PlayerPrefs.SetInt("FirstLaunch", 0);
        }
    }
}
