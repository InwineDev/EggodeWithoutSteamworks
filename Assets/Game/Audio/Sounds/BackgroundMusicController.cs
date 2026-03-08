using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundMusicController : MonoBehaviour
{
    public List<AudioClip> bgm = new List<AudioClip>();
    private AudioSource me;
    void Start()
    {
        me = GetComponent<AudioSource>();
        StartMusic();
    }

    private void StartMusic()
    {
        if (!me.isPlaying)
        {
            me.clip = bgm[Random.Range(0, bgm.Count)];
            me.Play(0);
        }
        Invoke("StartMusic", Random.Range(1, 100));
    }
}
