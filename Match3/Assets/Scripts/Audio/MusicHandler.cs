using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MusicHandler : MonoBehaviour
{
    [Header("Audio sources")]
    public AudioSource bgm;
    public AudioSource sfx;

    [Header("Ui stuff")]
    public Text audioText;

    [Header("Text")]
    public string nowPlaying = "Now Playing: Nouveile Noel by Kevin Macleod";

    //control vars
    bool isMuted = false;

    // Start is called before the first frame update
    void Start()
    {
        audioText.text = nowPlaying;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //mute button
    public void MuteButtonLogic()
    {
        //unmute
        if (isMuted)
        {
            bgm.mute = false;
            sfx.mute = false;
            audioText.text = nowPlaying;
            isMuted = false;
        }
        //mute
        else
        {
            bgm.mute = true;
            sfx.mute = true;
            audioText.text = "";
            isMuted = true;
        }
    }
}
