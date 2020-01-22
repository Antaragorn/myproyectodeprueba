using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;
    //variable estatica de GameManager que permite ser accedida desde cualquier script. 
    public BoardManager boardScript;
    //Almacena una referencia de nuestro BoardManager que configurará el nivel.

    public int playerFoodPoints= 100;
    [HideInInspector] public bool playersTurn = true;

    private int level = 3;
    //Número actual del nivel, expresado en el juego como "Dia 1"

   //Siempre se llama a Awake antes de Cualquier funcion Start.
    void Awake ()
    {
        //Comprueba si la instancia ya existe.
        if (instance == null)
            //si no, establece la instancia en este. 
            instance = this;
        //si la instancia ya existe y no esta en este.
        else if (instance != this)
            //Entonces destruye this. Esto aplica nuestro patrón singleton, lo que significa que sólo puede haber una instancia de un GameManager
            Destroy(gameObject);

        //Esto impide que se destruyan gameObject al recargar la escena.
        DontDestroyOnLoad(gameObject);

        //obtiene una referencia del componente al script BoardManager adjunto. 
        boardScript = GetComponent<BoardManager>();

        //Llama a la funcion InitGame para inicializar el primer nivel.
        InitGame();

    }
    //inicializa el juego para cada nivel
    void InitGame()
    {
        //llama a la funcion  SetupScene del script BoardManager, pasando el número actual de nivel.
        boardScript.SetupScene(level);

    }

    public void GameOver()
    {
        enabled = false;
    }

    // Update is called once per frame
    void Update ()
    {
        
    }
}
