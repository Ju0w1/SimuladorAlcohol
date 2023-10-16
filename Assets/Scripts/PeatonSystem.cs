using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PeatonSystem : MonoBehaviour
{
    public GameObject[] peatones;

    private List<List<Vector2>> cycles = new List<List<Vector2>>();
    private Dictionary<(int, int), (int, int)> jumps = new Dictionary<(int, int), (int, int)>();

    // Start is called before the first frame update
    void Start()
    {
        var cycle = new List<Vector2>();
        cycle.Add(new Vector2(-35.75f, -32.5499992f));
        cycle.Add(new Vector2(-39.5200005f, -26.9599991f));
        cycle.Add(new Vector3(-42.5999985f, -34.0999985f));
        cycles.Add(cycle);
        cycle = new List<Vector2>();
        cycle.Add(new Vector2(-35.75f + 10, -32.5499992f));
        cycle.Add(new Vector2(-39.5200005f + 10, -26.9599991f));
        cycle.Add(new Vector3(-42.5999985f + 10, -34.0999985f));
        cycles.Add(cycle);

        jumps.Add((0, 0), (1, 0));
    }

    void OnDrawGizmosSelected()
    {
        int y = 5;
        Gizmos.color = Color.green;
        foreach (var cycle in cycles)
        {
            for (int i = 0; i < cycle.Count - 1; i++)
            {
                Gizmos.DrawLine(new Vector3(cycle[i].x, y, cycle[i].y), new Vector3(cycle[i + 1].x, y, cycle[i + 1].y));
            }
            Gizmos.DrawLine(new Vector3(cycle[cycle.Count - 1].x, y, cycle[cycle.Count - 1].y), new Vector3(cycle[0].x, y, cycle[0].y));
        }
        Gizmos.color = Color.blue;
        foreach (KeyValuePair<(int, int), (int, int)> jump in jumps)
        {
            Gizmos.DrawLine(
                new Vector3(cycles[jump.Key.Item1][jump.Key.Item2].x, y, cycles[jump.Key.Item1][jump.Key.Item2].y),
                new Vector3(cycles[jump.Value.Item1][jump.Value.Item2].x, y, cycles[jump.Value.Item1][jump.Value.Item2].y));
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            //
        }
    }

}
