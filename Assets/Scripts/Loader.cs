using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Loader : MonoBehaviour
{
    public GameManager gameManager;   //Variable para adjuntar el prefab Gamemanager en el inspector e instanciarlo.


    
    void Awake ()
    {
        // Comprueba si un GameManager ya se ha asignado a la variable estática GamaManager.instance o si sigue siendo nulo.
        if (GameManager.instance == null)
            //instancia el prefab de gameManager 
            Instantiate(gameManager);
    }

 
}
