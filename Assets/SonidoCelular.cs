using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SonidoCelular : MonoBehaviour
{
    public AudioClip notificacion;
    public int carwait = 10;
    bool keepPlaying = true;

	void Start()
	{
		StartCoroutine(SoundOut());
	}

	IEnumerator SoundOut()
	{
		while (keepPlaying)
		{
			GetComponent<AudioSource>().PlayOneShot(notificacion);
			Debug.Log("Nueva notificacion");
			yield return new WaitForSeconds(carwait);
		}
	}
}
