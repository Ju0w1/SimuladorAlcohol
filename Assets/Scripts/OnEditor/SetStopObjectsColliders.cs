using UnityEngine;
using UnityEditor;

public class SetStopObjectsColliders : Editor
{
    [MenuItem("Custom/Set Collider for 'Stop' Objects")]
    static void SetColliderStopHeight()
    {
        GameObject[] stopObjects = GameObject.FindGameObjectsWithTag("Stop");

        foreach (GameObject stopObject in stopObjects)
        {
            // stopObject.layer = targetLayer;
            var box = stopObject.GetComponent<BoxCollider>();
            box.size = new Vector3(box.size.x, 12.0f, box.size.z);
            EditorUtility.SetDirty(stopObject);
        }
    }
}
