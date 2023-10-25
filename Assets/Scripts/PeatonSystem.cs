using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PeatonSystem : MonoBehaviour
{
    public GameObject[] peatones;
    public TextAsset peaton_points_file;

    public Transform player_transform;

    public Vector2 offset;
    public float zoom;

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


    // Esto crea los ciclos que luego seran utilizados para guiar a los peatones
    void FillCycles()
    {
        if (cycles == null)
        {
            cycles = new List<List<Vector2>>();
            jumps = new Dictionary<(int, int), (int, int)>();
            one_way_jumps = new Dictionary<(int, int), (int, int)>();
            string[] lines = peaton_points_file.text.Split('\n');

            var cycle = new List<Vector2>();
            foreach (string line in lines)
            {
                if (line.All(char.IsWhiteSpace))
                {
                    cycles.Add(cycle);
                    cycle = new List<Vector2>();
                }
                else
                {
                    string[] values = line.Split(' ');
                    if (values.Length == 2)
                        cycle.Add((new Vector2(float.Parse(values[0]), float.Parse(values[1])) + offset) * zoom);
                    else
                        one_way_jumps.Add((int.Parse(values[0]), int.Parse(values[1])), (int.Parse(values[2]), int.Parse(values[3])));
                }
            }

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

    private const int distancia_a_player_maxima = 100;
    private const int distancia_a_player_minima = 50;
    private const int peatones_minimos_en_entorno = 25;

    // Update is called once per frame
    void Update()
    {
        int peatones_en_entorno = 0;
        foreach (GameObject peaton in GameObject.FindGameObjectsWithTag("Peaton"))
        {
            if (Vector3.Distance(peaton.transform.position, player_transform.position) > distancia_a_player_maxima)
                peaton.GetComponent<PeatonController>().Destruir(10.0f);
            else
                peatones_en_entorno++;
        }
        if (spawn_peaton || peatones_en_entorno < peatones_minimos_en_entorno)
        {
            spawn_peaton = false;
            int cycle_index = Random.Range(0, cycles.Count);
            var cycle = cycles[cycle_index];
            int point_index = Random.Range(0, cycle.Count);
            float distance;
            do
            {
                cycle_index = Random.Range(0, cycles.Count);
                cycle = cycles[cycle_index];
                point_index = Random.Range(0, cycle.Count);
                distance = Vector3.Distance(new Vector3(cycle[point_index].x, y_spawn_point, cycle[point_index].y), player_transform.position);
            } while (distance > distancia_a_player_maxima || distance < distancia_a_player_minima);
            bool do_spawn = true;
            foreach (GameObject p in GameObject.FindGameObjectsWithTag("Peaton"))
            {
                if (Vector3.Distance(new Vector3(cycle[point_index].x, y_spawn_point, cycle[point_index].y), p.transform.position) < 5)
                    do_spawn = false;
            }
            if (do_spawn)
            {
                var peaton = Instantiate(peatones[Random.Range(0, peatones.Length)], transform);
                var peaton_controller = peaton.GetComponent<PeatonController>();
                peaton_controller.peaton_system = this;
                peaton_controller.peaton_cycle = cycle_index;
                peaton_controller.peaton_cycle_point = point_index;
                peaton_controller.vel = 2;
                peaton_controller.transform.position = new Vector3(cycle[point_index].x, y_spawn_point, cycle[point_index].y);
                // Debug.Log(cycle_index + " " + point_index);
                peaton_controller.flip_direction = Random.Range(0.0f, 1.0f) > 0.5f;
                peaton_controller.transform.forward = (NextCycle(peaton_controller, cycle_index, point_index) - CurrentCycle(peaton_controller, cycle_index, point_index)).normalized;
                peaton_controller.forever_height = 0.1669998f;
            }
        }
    }

    private int do_variant(PeatonController pec, int point_index, int container_size)
    {
        if (pec.flip_direction)
        {
            if (point_index == 0)
                return container_size - 1;
            return point_index - 1;
        }
        return (point_index + 1) % container_size;
    }

    public int NextPointIndex(PeatonController pec, int cycle_index, int point_index)
    {
        // if (point_index >= cycles[cycle_index].Count - 1)
        //     return 0;
        // else
        //     return point_index + 1;
        return do_variant(pec, point_index, cycles[cycle_index].Count);
    }

    public (int, int) DestinoJump(PeatonController pec, int cycle_index, int point_index)
    {
        // Debug.Log(cycle_index + " " + point_index);
        if (jumps.ContainsKey((cycle_index, point_index)))
            return jumps[(cycle_index, point_index)];
        return (-1, -1);
    }

    public Vector3 CurrentCycle(PeatonController pec, int cycle_index, int point_index)
    {
        return new Vector3(cycles[cycle_index][point_index].x, y_spawn_point, cycles[cycle_index][point_index].y);
    }

    public Vector3 NextCycle(PeatonController pec, int cycle_index, int point_index)
    {
        // if (point_index >= cycles[cycle_index].Count - 1)
        //     return new Vector3(cycles[cycle_index][0].x, y_spawn_point, cycles[cycle_index][0].y);
        // else
        //     return new Vector3(cycles[cycle_index][point_index + 1].x, y_spawn_point, cycles[cycle_index][point_index + 1].y);
        int v = do_variant(pec, point_index, cycles[cycle_index].Count);
        // Debug.Log("miooooo " + v + " " + point_index + " " + v % cycles[cycle_index].Count);
        return new Vector3(cycles[cycle_index][v].x, y_spawn_point, cycles[cycle_index][v].y);
    }

    public void UpdatePeatonCycle(PeatonController peaton)
    {
        // if (peaton.peaton_cycle_point >= cycles[peaton.peaton_cycle].Count - 1)
        //     peaton.peaton_cycle_point = 0;
        // else
        //     peaton.peaton_cycle_point = peaton.peaton_cycle_point + 1
        peaton.peaton_cycle_point = do_variant(peaton, peaton.peaton_cycle_point, cycles[peaton.peaton_cycle].Count);
    }

}
