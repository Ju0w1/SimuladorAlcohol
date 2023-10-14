using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuPrincipal : MonoBehaviour
{
    public void ComenzarSimulacion()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void CerrarSimulacion()
    {
        Debug.Log("Se presion� el bot�n de salir");
        Application.Quit();
    }
}
