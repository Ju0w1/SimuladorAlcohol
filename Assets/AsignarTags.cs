using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsignarTags : MonoBehaviour
{
    public string tagName = "Colisionable";
    // Start is called before the first frame update
    void Start()
    {
        var gameObjects = GameObject.FindObjectsOfType<GameObject>();

        foreach (GameObject go in gameObjects)
        {
            if (go.tag == "Untagged")
            {
                if (go.name == "Capsule" || go.name.Contains("BusStop") || go.name.Contains("hydrant-") || go.name.Contains("pedestrian") || go.name.Contains("ElectricalBox-") || go.name.Contains("Flowerbed-")
                        || go.name.Contains("Trash-") || go.name.Contains("Traffic_Light") || go.name.Contains("Collider") || go.name.Contains("EB-") || go.name.Contains("StreetLight")
                        )
                {
                    go.tag = tagName;
                }
            }
        }
    }

}
