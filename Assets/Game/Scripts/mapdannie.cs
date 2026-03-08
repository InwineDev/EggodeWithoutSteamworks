using Mirror;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using Steamworks;
using UnityEngine.SceneManagement;
using System.Linq;


public class mapdannie : MonoBehaviour
{
    public TMP_Text nameText;
    public TMP_Text authorText;
    public string jsonFilePath;
    public NetworkManager omg;
    public Image icon;
    public Texture2D texture22;
    public Sprite sprite;
    public Image background;
    public Image backgroundMema;
    public GameObject hostButton;
    public GameObject ErrorText;
    [SerializeField] private Color averageColor;

    public void Starting()
    {

    }
    public void ButtonGame()
    {
        login.urlMap = jsonFilePath;
        hostSettings.onChangeMap?.Invoke(icon.sprite, nameText.text, authorText.text, averageColor);
    }

    public IEnumerator Pon(string textureUrl)
    {
        using (UnityWebRequest www = UnityWebRequestTexture.GetTexture(textureUrl))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.Log("Error loading image: " + www.error);
            }
            else
            {
                Texture2D texture = DownloadHandlerTexture.GetContent(www);
                sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                icon.sprite = sprite;
                background.sprite = sprite;
                averageColor = GetAverageColor(sprite.texture);
                backgroundMema.color = averageColor;
            }
        }
    }
    Color GetAverageColor(Texture2D texture)
    {
        Color32[] pixels = texture.GetPixels32();
        long totalR = 0;
        long totalG = 0;
        long totalB = 0;
        long totalA = 0;

        for (int i = 0; i < pixels.Length; i++)
        {
            totalR += pixels[i].r;
            totalG += pixels[i].g;
            totalB += pixels[i].b;
            totalA += pixels[i].a;
        }

        float averageR = (float)totalR / pixels.Length / 255.0f;
        float averageG = (float)totalG / pixels.Length / 255.0f;
        float averageB = (float)totalB / pixels.Length / 255.0f;
        float averageA = (float)totalA / pixels.Length / 255.0f;

        return new Color(averageR, averageG, averageB, averageA);
    }
    private void OnEnable()
    {
        if (!SteamManager.Initialized) { return; }
        omg = FindObjectOfType<NetworkManager>();
        hostButton.SetActive(false);
        string jsonText = System.IO.File.ReadAllText(jsonFilePath);
        MapData mapData1 = JsonUtility.FromJson<MapData>(jsonText);
        nameText.text = mapData1.mapname;
        authorText.text = "Автор: " + mapData1.author;
        if (mapData1.modsDependence.All(mod => CsModsManager.modsForServer.Contains(mod)))
        {
            hostButton.SetActive(true);
            ErrorText.SetActive(false);
        }
        StartCoroutine(Pon(mapData1.icon));
    }
}
