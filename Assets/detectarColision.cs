using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class detectarColision : MonoBehaviour
{
	public int cantidadDeChoques = 0;

	//void OnCollisionEnter(Collision c)
	//{
	//	if (c.gameObject.tag == "esVehiculo")
	//	{
	//		cantidadDeChoques++;
	//	}

	//}

	void OnCollisionEnter(Collision collision)
	{
		if (collision.gameObject.tag == "IaCar")
        {
			cantidadDeChoques++;
		}
		Debug.Log("Entered collision with " + collision.gameObject.name);
	}
}
