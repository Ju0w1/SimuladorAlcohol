using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GearShifterController : MonoBehaviour
{
    public Queue<Vector2> animacion = new Queue<Vector2>();
    public float velocidad_proceso = 1;

    public Vector2 rotation = new Vector2(-7, -7);

    public float timer;

    // Start is called before the first frame update
    void Start()
    {
        timer = Time.time + 1;
    }

    // Update is called once per frame
    void Update()
    {
        transform.localRotation = Quaternion.Euler(rotation.x, rotation.y, 180);
        if (animacion.Count != 0)
        {
            rotation = Vector2.MoveTowards(rotation, animacion.Peek(), Time.deltaTime * velocidad_proceso);
            if (rotation == animacion.Peek())
                animacion.Dequeue();
        }
    }

    float ObtenerCambioX(int cambio)
    {
        if (cambio == -1)
            return 7;
        if (cambio == 0)
            return 0;
        return (((cambio - 1) % 2 * 2) - 1) * 7;
    }

    float ObtenerCambioY(int cambio, int cambio_ultimo)
    {
        if (cambio == -1)
            return -7;
        if (cambio == 0)
        {
            if (cambio_ultimo != 0)
                return ObtenerCambioY(cambio_ultimo, cambio_ultimo);
            else
                return 0;
        }
        return (Mathf.Floor((cambio - 1) / 2) - 1) * -7;
    }

    public void HacerCambio(int cambio_actual, int cambio_nuevo)
    {
        // animacion.Clear();
        float cambio_actual_col = ObtenerCambioY(cambio_actual, cambio_actual);
        float cambio_nuevo_col = ObtenerCambioY(cambio_nuevo, cambio_actual);
        if (cambio_nuevo_col == rotation.y)
            animacion.Enqueue(new Vector2(ObtenerCambioX(cambio_nuevo), ObtenerCambioY(cambio_nuevo, cambio_actual)));
        else
        {
            animacion.Enqueue(new Vector2(0, ObtenerCambioY(cambio_actual, cambio_actual)));
            animacion.Enqueue(new Vector2(0, ObtenerCambioY(cambio_nuevo, cambio_actual)));
            animacion.Enqueue(new Vector2(ObtenerCambioX(cambio_nuevo), ObtenerCambioY(cambio_nuevo, cambio_actual)));
        }
    }

    public void RestaurarNeutro()
    {
        if (animacion.Count == 0)
            animacion.Enqueue(new Vector2(0, 0));
    }
}
