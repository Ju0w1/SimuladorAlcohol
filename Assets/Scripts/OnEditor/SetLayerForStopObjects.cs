using UnityEngine;
using UnityEditor;

public class SetLayerForStopObjects : Editor
{
    [MenuItem("Custom/Set Layer for 'Stop' Objects")]
    static void SetLayer()
    {
        string targetLayerName = "TrafficLightsStopLayer"; // Change this to the desired layer name
        int targetLayer = LayerMask.NameToLayer(targetLayerName);

        GameObject[] stopObjects = GameObject.FindGameObjectsWithTag("Stop");

        foreach (GameObject stopObject in stopObjects)
        {
            stopObject.layer = targetLayer;
            EditorUtility.SetDirty(stopObject);
        }
    }
}
