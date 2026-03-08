using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using Mirror;
using Steamworks;

public class SceneLoader : MonoBehaviour
{
    public string scenes;
    public bool onStart;
    public float seconds;

    private void Start()
    {
        if (onStart)
        {
            StartCoroutine(LoadNextScene());
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SceneManager.LoadScene(scenes);
        }
    }

    public void Starting()
    {
       StartCoroutine(LoadNextScene());
    }

    private IEnumerator LoadNextScene()
    {
        if (scenes == "MapEdit")
        {
            yield return new WaitForSeconds(seconds);
            SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypePrivate, 1);
            FindObjectOfType<NetworkManager>().StartHost();
            FindObjectOfType<NetworkManager>().ServerChangeScene("MapEdit");
            SceneManager.LoadScene(scenes);
        }
        yield return new WaitForSeconds(seconds);
        SceneManager.LoadScene(scenes);
    }
}
