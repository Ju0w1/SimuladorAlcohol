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

    public void ConfigurarVolumen(float volumen)
    {
        audioMixer.SetFloat("volumen", volumen);
    }

    private void Start()
    {
        Debug.Log(PlayerPrefs.GetFloat("ebriedad", ebriedad));
    }

    private void Update()
    {
        ebriedad = sliderEbriedad.value;
        PlayerPrefs.SetFloat("ebriedad", ebriedad);
    }

    public void VolverBoton()
    {
        Time.timeScale = 1f;
    }
    
}
