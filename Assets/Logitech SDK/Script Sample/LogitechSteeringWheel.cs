using UnityEngine;
using System.Collections;
using System.Text;
using UnityEngine.UI;

public class LogitechSteeringWheel : MonoBehaviour
{

    LogitechGSDK.LogiControllerPropertiesData properties;
    public CarController car_controller;
    private string actualState;
    private string activeForces;
    private float hSliderValue;
    private string propertiesEdit;
    private string buttonStatus;
    private string forcesLabel;
    string[] activeForceAndEffect;
    public DrunkEffects drunk_effects;


    private float ebrio;

    // Use this for initialization
    void Start()
    {
        ebrio = PlayerPrefs.GetFloat("ebriedad");
        // Debug.Log(ebrio);
    }
    // Update is called once per frame
    void Update()
    {
        drunk_effects.alcolemia = PlayerPrefs.GetFloat("ebriedad");
        // Debug.Log(PlayerPrefs.GetFloat("ebriedad"));
    }



}
