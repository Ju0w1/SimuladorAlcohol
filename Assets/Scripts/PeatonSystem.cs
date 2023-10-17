using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PeatonSystem : MonoBehaviour
{
    public GameObject[] peatones;

    public List<List<Vector2>> cycles;
    public Dictionary<(int, int), (int, int)> jumps;
    private Dictionary<(int, int), (int, int)> one_way_jumps;

    public float y_spawn_point = 0.15f;

    public bool spawn_peaton = false;

    // Start is called before the first frame update
    void Start()
    {
        FillCycles();
    }

    void FillCycles()
    {
        if (cycles == null)
        {
            cycles = new List<List<Vector2>>();
            jumps = new Dictionary<(int, int), (int, int)>();
            one_way_jumps = new Dictionary<(int, int), (int, int)>();
            var cycle = new List<Vector2>();
            cycle.Add(new Vector2(-35.75f, -32.5499992f));
            cycle.Add(new Vector2(-39.5200005f, -26.9599991f));
            cycle.Add(new Vector3(-42.5999985f, -34.0999985f));
            cycles.Add(cycle);
            cycle = new List<Vector2>();
            cycle.Add(new Vector2(-35.75f + 10, -32.5499992f + 20));
            cycle.Add(new Vector2(-39.5200005f + 10, -26.9599991f + 20));
            cycle.Add(new Vector3(-42.5999985f + 10, -34.0999985f + 20));
            cycles.Add(cycle);

            one_way_jumps.Add((0, 0), (1, 0));
            // Inverting the jumps
            foreach (KeyValuePair<(int, int), (int, int)> jump in one_way_jumps)
            {
                jumps.Add(jump.Key, jump.Value);
                jumps.Add(jump.Value, jump.Key);
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        FillCycles();
        int y = 5;
        Gizmos.color = Color.green;
        foreach (var cycle in cycles)
        {
            for (int i = 0; i < cycle.Count - 1; i++)
                Gizmos.DrawLine(new Vector3(cycle[i].x, y, cycle[i].y), new Vector3(cycle[i + 1].x, y, cycle[i + 1].y));
            Gizmos.DrawLine(new Vector3(cycle[cycle.Count - 1].x, y, cycle[cycle.Count - 1].y), new Vector3(cycle[0].x, y, cycle[0].y));
        }
        Gizmos.color = Color.blue;
        foreach (KeyValuePair<(int, int), (int, int)> jump in one_way_jumps)
        {
            Gizmos.DrawLine(
                new Vector3(cycles[jump.Key.Item1][jump.Key.Item2].x, y, cycles[jump.Key.Item1][jump.Key.Item2].y),
                new Vector3(cycles[jump.Value.Item1][jump.Value.Item2].x, y, cycles[jump.Value.Item1][jump.Value.Item2].y));
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (spawn_peaton)
        {
            spawn_peaton = false;
            int cycle_index = Random.Range(0, cycles.Count);
            var cycle = cycles[cycle_index];
            int point_index = Random.Range(0, cycle.Count);
            var peaton = Instantiate(peatones[Random.Range(0, peatones.Length)], transform);
            var peaton_controller = peaton.GetComponent<PeatonController>();
            peaton_controller.peaton_system = this;
            peaton_controller.peaton_cycle = cycle_index;
            peaton_controller.peaton_cycle_point = point_index;
            peaton_controller.vel = 6;
            peaton_controller.transform.position = new Vector3(cycle[point_index].x, y_spawn_point, cycle[point_index].y);
            // Debug.Log(cycle_index + " " + point_index);
            peaton_controller.transform.forward = (NextCycle(cycle_index, point_index) - CurrentCycle(cycle_index, point_index)).normalized;
        }
    }

    public int NextPointIndex(int cycle_index, int point_index)
    {
        if (point_index >= cycles[cycle_index].Count - 1)
            return 0;
        else
            return point_index + 1;
    }

    public (int, int) DestinoJump(int cycle_index, int point_index)
    {
        // Debug.Log(cycle_index + " " + point_index);
        if (jumps.ContainsKey((cycle_index, point_index)))
            return jumps[(cycle_index, point_index)];
        return (-1, -1);
    }

    public Vector3 CurrentCycle(int cycle_index, int point_index)
    {
        return new Vector3(cycles[cycle_index][point_index].x, y_spawn_point, cycles[cycle_index][point_index].y);
    }

    public Vector3 NextCycle(int cycle_index, int point_index)
    {
        if (point_index >= cycles[cycle_index].Count - 1)
            return new Vector3(cycles[cycle_index][0].x, y_spawn_point, cycles[cycle_index][0].y);
        else
            return new Vector3(cycles[cycle_index][point_index + 1].x, y_spawn_point, cycles[cycle_index][point_index + 1].y);
    }

    public void UpdatePeatonCycle(PeatonController peaton)
    {
        if (peaton.peaton_cycle_point >= cycles[peaton.peaton_cycle].Count - 1)
            peaton.peaton_cycle_point = 0;
        else
            peaton.peaton_cycle_point = peaton.peaton_cycle_point + 1;
    }

}
