using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiscordManager : MonoBehaviour
{

    Discord.Discord discord;
    private string name24;
    private string gde = "Ìåíþ";
    public long time;

    void Start()
    {
        discord = new Discord.Discord(1215401955057213552, (ulong)Discord.CreateFlags.NoRequireDiscord);
        name = login.username;
        time = DateTimeOffset.Now.ToUnixTimeSeconds();
        ChangeActivity();
    }

    private void OnDisable()
    {
        discord.Dispose();
    }

    public void ChangeActivity()
    {
        string state = settingsController.nickname + " ÄÅËÀÅÒ Æ¨ÑÒÊÈß ßÉÖÀ";
        var activityManager = discord.GetActivityManager();
        var activity = new Discord.Activity
        {
            State = state,
        Details = gde,
        Assets =
        {
        LargeImage = "logo",
        LargeText = gde,
        SmallImage = "logo",
        SmallText = name24
        },
        Timestamps =
        {

         Start = time
        }
        };
        activityManager.UpdateActivity(activity, (res) =>
        {
            print("ACTIVITY UPDATED!");
        });
    }


    public void buton(string newprichina)
    {
        gde = newprichina;
        ChangeActivity();
    }

    void Update()
    {
        discord.RunCallbacks();
    }
}
