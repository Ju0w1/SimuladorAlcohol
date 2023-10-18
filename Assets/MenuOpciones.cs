using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class MenuOpciones : MonoBehaviour
{
    public AudioMixer audioMixer;
    public Slider sliderEbriedad;
    public float ebriedad;
    public Slider sliderVolumen;
    public float volumen;

    public GameObject menu;
    public int isMenuActive;

    // public void ConfigurarVolumen(float volumen)
    // {
    //     audioMixer.SetFloat("volumen", volumen);
    // }

    private void Start()
    {
        Debug.Log(PlayerPrefs.GetFloat("ebriedad", ebriedad));

        isMenuActive = 0;
    }

    private void Update()
    {
        // audioMixer.volumen = audioMixer.value;
        ebriedad = sliderEbriedad.value;
        // volumen = sliderEbriedad.value;
        PlayerPrefs.SetFloat("ebriedad", ebriedad);

        volumen = sliderVolumen.value;
        audioMixer.SetFloat("volumen", volumen);

        isMenuActive = PlayerPrefs.GetInt("isMenuActive");
    }

    public void VolverBoton()
    {
        if (isMenuActive == 1)
        {
            Time.timeScale = 1f;
            menu.SetActive(false);
            isMenuActive = 0;
            PlayerPrefs.SetInt("isMenuActive", isMenuActive);
        }
        else
        {
            Time.timeScale = 0f;
            menu.SetActive(true);
            isMenuActive = 1;
            PlayerPrefs.SetInt("isMenuActive", isMenuActive);
        }
    }
    
}
