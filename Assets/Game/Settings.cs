using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
using System;


public class Settings : MonoBehaviour
{
    [SerializeField] private RenderPipelineAsset[] levelsettings;
    [SerializeField] private TMP_Dropdown dropdown;
    [SerializeField] private TMP_Dropdown res;
    [SerializeField] private Toggle sborDannih;
    private int zagruzeno = 0;
    private int maxFPS = 500;
    public Slider FPS;
    public Slider MUSIC;
    public TMP_Text FPSTXT;
    public TMP_Text MUSICTXT;

    public GameObject nastroiki;
    [SerializeField] private AudioSource musicSource;

    public void ChangeLevel(int value)
    {
        QualitySettings.SetQualityLevel(value);
        QualitySettings.renderPipeline = levelsettings[value];
        PlayerPrefs.SetInt("GraphicsSettings", value);
        PlayerPrefs.Save();
        print("saved!");
    }

    public void ChangeLevelResol(int value22)
    {
        Resolution[] resolutions = Screen.resolutions;
        Resolution selectedResolution = resolutions[value22];
        Screen.SetResolution(selectedResolution.width, selectedResolution.height, true);
        print(Screen.currentResolution);
    }

    private void Start()
    {
        Resolution[] resolutions = Screen.resolutions;
        res.ClearOptions();

        List<string> options = new List<string>();
        foreach (Resolution res in resolutions)
        {
            options.Add(res.width + "x" + res.height);
        }
        res.AddOptions(options);

        if (PlayerPrefs.HasKey("GraphicsSettings"))
        {
            int graph = PlayerPrefs.GetInt("GraphicsSettings");
            dropdown.value = graph;
            QualitySettings.SetQualityLevel(graph);
            QualitySettings.renderPipeline = levelsettings[graph];
            zagruzeno++;
        } else
        {
            zagruzeno++;
        }

        if (PlayerPrefs.HasKey("FpsSettings"))
        {
            FPS.value = PlayerPrefs.GetFloat("FpsSettings");
            SetFPS();
            zagruzeno++;
        }
        else
        {
            zagruzeno++;
        }
        if (PlayerPrefs.HasKey("MusicVolumeSettings"))
        {
            MUSIC.value = PlayerPrefs.GetFloat("MusicVolumeSettings");
            SetMUSIC();
        }

        if (PlayerPrefs.HasKey("SborDannih"))
        {
            if(PlayerPrefs.GetInt("SborDannih") == 1)
            {
                sborDannih.isOn = false;
            }
        }
        Application.targetFrameRate = maxFPS;

        if(zagruzeno == 2)
        {
            nastroiki.SetActive(false);
        }
    }

    public void OnChangeDannie()
    {
        if (sborDannih.isOn)
        {
            PlayerPrefs.SetInt("SborDannih", 0);
            PlayerPrefs.Save();
        } else
        {
            PlayerPrefs.SetInt("SborDannih", 1);
            PlayerPrefs.Save();
        }
    }

    public void SetFPS()
    {
        PlayerPrefs.SetFloat("FpsSettings", FPS.value);
        PlayerPrefs.Save();
        print("saved!");
        if (FPS.value == 0)
        {
            QualitySettings.vSyncCount = 1;
            FPSTXT.text = "V-Syns";
        }
        else
        {
            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = (int)FPS.value;
            maxFPS = (int)FPS.value;
            FPSTXT.text = maxFPS.ToString();
        }

    }
    public void SetMUSIC()
    {
        PlayerPrefs.SetFloat("MusicVolumeSettings", MUSIC.value);
        PlayerPrefs.Save();
        print("saved!");
        musicSource.volume = MUSIC.value;
        MUSICTXT.text = (MUSIC.value * 100).ToString();
    }

}
