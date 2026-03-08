using Mirror;
using Steamworks;
using System.Collections.Generic;
using UnityEngine;

public class MultiplayerManager : MonoBehaviour
{
    protected Callback<LobbyCreated_t> LobbyCreated;
    protected Callback<GameLobbyJoinRequested_t> JoinRequest;
    protected Callback<LobbyEnter_t> LobbyEntered;

    protected Callback<LobbyMatchList_t> LobbyList;
    protected Callback<LobbyDataUpdate_t> LobbyDataUpdated;

    public List<CSteamID> lobbyIDs = new List<CSteamID>();

    private CSteamID currentLobbyId;
    public static MultiplayerManager instance;
    private NetworkManager networkManager;

    private void Start()
    {
        if (instance == null) { instance = this; }

        networkManager = FindObjectOfType<NetworkManager>();

        JoinRequest = Callback<GameLobbyJoinRequested_t>.Create(OnJoinRequest);
        LobbyEntered = Callback<LobbyEnter_t>.Create(OnLobbyEntered);
        LobbyCreated = Callback<LobbyCreated_t>.Create(OnLobbyCreated);
        LobbyList = Callback<LobbyMatchList_t>.Create(OnLobbyList);
        LobbyDataUpdated = Callback<LobbyDataUpdate_t>.Create(OnLobbyData);
    }

    // 🟢 Call this to create a public lobby
    public void CreateLobby(ELobbyType type, int peoples)
    {
        SteamMatchmaking.CreateLobby(type, peoples);
    }

    // 🔵 Get available public lobbies
    public void GetLobbiesList()
    {
        lobbyIDs.Clear();
        SteamMatchmaking.AddRequestLobbyListStringFilter("Eggode2", "Eggode2", ELobbyComparison.k_ELobbyComparisonEqual);
        SteamMatchmaking.RequestLobbyList();
    }

    private void OnLobbyCreated(LobbyCreated_t callback)
    {
        if (callback.m_eResult != EResult.k_EResultOK)
        {
            Debug.LogError("Lobby creation failed!");
            return;
        }

        Debug.Log("Lobby successfully created!");

        networkManager.StartHost();
        currentLobbyId = new CSteamID(callback.m_ulSteamIDLobby);

        // 🔴 Set lobby metadata so it can be found by others
        SteamMatchmaking.SetLobbyData(currentLobbyId, "HostAddress", SteamUser.GetSteamID().ToString());
        SteamMatchmaking.SetLobbyData(currentLobbyId, "Eggode2", "Eggode2"); // Used for filtering
        SteamMatchmaking.SetLobbyData(currentLobbyId, "name", SteamFriends.GetPersonaName() + "'s Lobby");
    }

    private void OnJoinRequest(GameLobbyJoinRequested_t callback)
    {
        Debug.Log("Join request received");
        SteamMatchmaking.JoinLobby(callback.m_steamIDLobby);
    }

    private void OnLobbyEntered(LobbyEnter_t callback)
    {
        currentLobbyId = new CSteamID(callback.m_ulSteamIDLobby);

        if (!NetworkServer.active)
        {
            string hostAddress = SteamMatchmaking.GetLobbyData(currentLobbyId, "HostAddress");
            networkManager.networkAddress = hostAddress;
            networkManager.StartClient();
        }

        Debug.Log("Entered Lobby: " + currentLobbyId);
    }

    private void OnLobbyList(LobbyMatchList_t result)
    {
        Debug.Log("Found lobbies: " + result.m_nLobbiesMatching);
        LobbyListManager.instance.DestroyLobbies();

        for (int i = 0; i < result.m_nLobbiesMatching; i++)
        {
            CSteamID lobbyID = SteamMatchmaking.GetLobbyByIndex(i);
            lobbyIDs.Add(lobbyID);
            SteamMatchmaking.RequestLobbyData(lobbyID);
        }
    }

    private void OnLobbyData(LobbyDataUpdate_t result)
    {
        if (result.m_bSuccess != 1)
        {
            Debug.LogWarning("Failed to get lobby data");
            return;
        }

        LobbyListManager.instance.DisplayLobbies(lobbyIDs, result);
    }
}