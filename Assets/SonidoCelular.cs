using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SonidoCelular : MonoBehaviour
{
    //public AudioClip notificacion;
    public int carwait = 10;
    bool keepPlaying = true;

	public int randomNumber = 60;
	public int posiSonido = 2;
	public AudioClip[] tonos;// [notificacion, skyline, deadpool, xiaomi, esponja, bells];

	private float timer = 0.0f;
	void Start()
	{
		// StartCoroutine(SoundOut());
	}

	void Update(){
		timer += Time.deltaTime;
		if (timer >= randomNumber)
		{
			// Realiza algo después de esperar X segundos
			randomNumber = Random.Range(0, 100);
			GetComponent<AudioSource>().PlayOneShot(tonos[posiSonido]);
			// Debug.Log("Nueva notificacion");
			posiSonido = Random.Range(0, tonos.Length);

			timer = 0.0f; // Reinicia el temporizador
		}
	}

	// IEnumerator SoundOut()
	// {
	// 	while (keepPlaying)
	// 	{
	// 		randomNumber = Random.Range(0, 100);
	// 		GetComponent<AudioSource>().PlayOneShot(tonos[posiSonido]);
	// 		// Debug.Log("Nueva notificacion");
	// 		posiSonido = Random.Range(0, tonos.Length);
	// 		// Debug.Log(randomNumber);
	// 		yield return new WaitForSeconds(randomNumber);
	// 	}
	// }
}
