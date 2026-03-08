using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteanInit : MonoBehaviour
{
    public GameObject steamInfo;
    void Start()
    {
        if (!SteamManager.Initialized) steamInfo.SetActive(true);
    }
}
