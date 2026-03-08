using Mirror;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Steamworks;

public class chatcreator : NetworkBehaviour
{
    [SyncVar(hook = nameof(OnSyncedValueChanged))]
    public string syncedValue = "NULL";

    public TMP_Text textDisplay;
    public Button addButton;
    public TMP_InputField inputField;

    private void OnSyncedValueChanged(string oldValue, string newValue)
    {
        Debug.Log($"«начение syncedValue изменено: {oldValue} -> {newValue}");
        if (textDisplay != null)
        {
            textDisplay.text = syncedValue;
        }
    }

    [Command]
    private void CmdAddText(string newText)
    {
        syncedValue += newText;
    }

    private void AddText()
    {
        if (!isOwned)
        {
            return;
        }

        string newText = inputField.text;
        CmdAddText(newText);
        inputField.text = string.Empty;
    }

    public GameObject item228;


    private void Start()
    {
        nick = login.username;

        GameObject newObject = Instantiate(item228, item228.transform.position, Quaternion.identity);
        NetworkServer.Spawn(newObject);
        newObject.GetComponent<ChatIdentityFix>().syncedValue = "яйцо "+ SteamFriends.GetPersonaName().ToString() + " прилетело на планету.";
    }




    public void RpcSugoma()
    {
        if (isOwned && NetworkClient.isConnected)
        {
            print(item228);
            CmdSugoma(item228, inputField.text, nick);
        }
    }

    private string nick;


    [ClientRpc]
    private void RpcSugoma228(GameObject newItem)
    {
        CmdSugoma(newItem, inputField.text, nick);
    }

    [Command]
    private void CmdSugoma(GameObject newItem, string text, string nickname)
    {
            GameObject newObject = Instantiate(item228, item228.transform.position, Quaternion.identity);
            NetworkServer.Spawn(newObject);
            newObject.GetComponent<ChatIdentityFix>().syncedValue = "<" + nickname + "> " + text;
    }

    public override void OnStopClient()
    {
            GameObject newObject = Instantiate(item228, item228.transform.position, Quaternion.identity);
            NetworkServer.Spawn(newObject);
            newObject.GetComponent<ChatIdentityFix>().syncedValue = "яйцо " + SteamFriends.GetPersonaName().ToString() + " покинуло планету.";
    }
}
