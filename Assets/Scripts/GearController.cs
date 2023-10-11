using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GearController : MonoBehaviour
{
    public Queue<Vector2> animacion = new Queue<Vector2>();
    public float velocidad_proceso = 1;

    public Vector2 rotation;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.localRotation = Quaternion.Euler(rotation.x, rotation.y, 180);
        if (animacion.Count != 0)
        {
            rotation = Vector2.MoveTowards(rotation, animacion.Peek(), Time.deltaTime);
        }
    }

    float ObtenerCambioX(int cambio)
    {
        return 7;
    }

    float ObtenerCambioY(int cambio)
    {
        return 7;
    }

    void HacerCambio(int cambio_actual, int cambio_nuevo)
    {
        animacion.Clear();
        animacion.Enqueue(new Vector2(ObtenerCambioX(cambio_nuevo), ObtenerCambioY(cambio_nuevo)));
        // int cambio_actual_col = ColumnaDeCambio(cambio_actual);
        // int cambio_nuevo_col = ColumnaDeCambio(cambio_nuevo);
        // if (cambio_actual_col == cambio_nuevo_col)
        // {
        //     //
        // }
    }
}
