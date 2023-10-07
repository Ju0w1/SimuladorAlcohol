using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgregarTags : MonoBehaviour
{
    public string tagName = "Colisionable";

    void Start()
    {
        //Debug.Log("funciona");
        //// Find all objects with a specific name and tag them
        //GameObject[] objectsToTag = GameObject.FindGameObjectsWithTag("Untagged");

        //Debug.Log("largo: "+ objectsToTag.Length);

        var gameObjects = GameObject.FindObjectsOfType<GameObject>();

        foreach (GameObject go in gameObjects)
        {
            if (go.tag == "Untagged")
            {
                if (go.name == "Capsule" || go.name.Contains("BusStop") || go.name.Contains("hydrant-") || go.name.Contains("pedestrian") || go.name.Contains("ElectricalBox-") || go.name.Contains("Flowerbed-")
                        || go.name.Contains("Trash-") || go.name.Contains("Traffic_Light") || go.name.Contains("Collider") || go.name.Contains("EB-")
                        )
                {
                    go.tag = tagName;
                }
            }
        }

        ////Debug.Log("lista"+objectsToTag);
        //foreach (GameObject obj in objectsToTag)
        //{
        //    Debug.Log("nombre: " + obj.name);
        //    //if (obj.name == "Capsule" || obj.name.Contains("BusStop") || obj.name.Contains("hydrant-") || obj.name.Contains("pedestrian") || obj.name.Contains("ElectricalBox-") || obj.name.Contains("Flowerbed-")
        //    //    || obj.name.Contains("Trash-") || obj.name.Contains("Traffic_Light") || obj.name.Contains("Collider") || obj.name.Contains("EB-")
        //    //    )
        //    //{
        //    //    obj.tag = tagName;
        //    //}
        //}
    }
}
