using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class SoundControl : MonoBehaviour
{
    public Slider musicSlider, soundSlider;
    public bool showSettings = false;
    public AudioSource music, sounds;
    public Image SlidersBG;
    public void ShowSettings(bool closeSettings = false)
    {
        if (showSettings || closeSettings)
        {
            musicSlider.gameObject.SetActive(false);
            soundSlider.gameObject.SetActive(false);
            SlidersBG.gameObject.SetActive(false);
        }
        else
        {
            musicSlider.gameObject.SetActive(true);
            soundSlider.gameObject.SetActive(true);
            SlidersBG.gameObject.SetActive(true);
        }
        showSettings = !showSettings;
        if (closeSettings && showSettings)
            showSettings = false;

    }

    void Update()
    {
        music.volume = musicSlider.value;
        sounds.volume = soundSlider.value;
    }
}
