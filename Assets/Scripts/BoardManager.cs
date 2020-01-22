
using System.Collections.Generic;               //Nos permite usar listas.
using UnityEngine;                              //Utiliza el motor de numeros aleatorios de Unity.
using System;
using Random = UnityEngine.Random;

public class BoardManager : MonoBehaviour
{
    // El uso de Serializable nos permite incrustar una clase con subpropiedades en el inspector.
    [Serializable]
    public class Count
    {
        public int minimum;             // Minimo valor para nuestro Count class.
        public int maximum;             // Maximo valor para nuestro Count class.

        // Constructor de asignacion.
        public Count (int min, int max)
        {
            minimum = min;
            maximum = max;
        }
    }

    // Numero de conlumnas en nuestro tablero de juego.
    public int columns = 8;
    // Numero de filas en nuestro tablero de juego.
    public int rows = 8;
    //Limite inferior y superior para nuestro numero aleatorio de paredes por nivel.
    public Count wallCount = new Count (5, 9);
    //Limite inferior y superior para nuestro numero aleatorio de alimentos por nivel.
    public Count foodCount = new Count (1, 5);
    // Prefab de aparicion para exit.
    public GameObject exit;
    // Array para prefabs de suelos.
    public GameObject[] floorTiles;
    // Array para prefabs de paredes.
    public GameObject[] wallTiles;
    // Array para prefabs de alimentos.
    public GameObject[] foodTiles;
    // Array para prefabs de enemigos.
    public GameObject[] enemyTiles;
    // Array para prefabs de baldosas exteriores.
    public GameObject[] outerWallTiles;

    // Variable para almacenar una referencia a la transformacion de nuestro objeto Board.
    private Transform boardHolder;
    // Una lista de posibles ubicaciones para colocar mosaicos.
    private List<Vector3> gridPositions = new List<Vector3>();

    // Borra nuestra lista gridPositions y la prepara para generar una nueva placa.
    void InitialiseList()
    {
        // Borra nuestra lista gridPositions.
        gridPositions.Clear();

        // Bucle que recorre el eje x (columnas).
        for (int x = 1; x < columns - 1; x++)
        {
            // Dentro de cada columna, recorre el eje y (filas).
            for (int y = 1; y < rows - 1; y++)
            {
                // En cada indice agrega un nuevo Vector3 a nuestra lista con las coordenadas x e y de esa posicion.
                gridPositions.Add(new Vector3(x, y, 0f));
            }
        }
    }

    // Configura las paredes exteriores y el suelo (fondo) del tablero de juego.

    void BoardSetup()
    {
        // Crea una instancia de Board y configura boardholder en su transformacion.
        boardHolder = new GameObject("Board").transform;

        //Bucle a lo largo del eje x, a partir de -1 (para rellenar esquina) con baldosas de borde de suelo o pared exterior.
        for (int x = -1; x < columns + 1; x++)
        {
            //Bucle a lo largo del eje y, comenzando desde -1 para colocar baldosas de suelo o pared exterior.
            for (int y = -1; y < rows + 1; y++)
            {
                //Elije un azulejo aleatorio de nuestro array de baldosas de suelo y prepara para instanciarla.
                GameObject toInstantiate = floorTiles[Random.Range(0, floorTiles.Length)];

                //Comprueba si la posicion actual esta en el borde del tablero, si es asi elige un prefab de pared exterior al azar de nuestro array de baldosas de pared exterior
                if (x == -1 || x == columns || y == -1 || y == rows)
                    toInstantiate = outerWallTiles[Random.Range(0, outerWallTiles.Length)];

                //Cree una instancia de GameObject utilizando el prefab elegido para toInstantiate en el Vector3 correspondiente a la posición actual de la cuadrícula en bucle, conservándolo en GameObject.
                GameObject instance = Instantiate(toInstantiate, new Vector3(x, y, 0f), Quaternion.identity) as GameObject;

                // Establece el padre de nuestra instancia recien creada en boardHolder, esto es solo para evitar el desorden en la jerarquía.
                instance.transform.SetParent(boardHolder);
            }
        }
    }

    //RandomPosition devuelve una posición aleatoria de nuestra lista gridPositions.
    Vector3 RandomPosition()
    {
        // Declara un entero, establece su valor en un número aleatorio entre 0 y el recuento de elementos en nuestra List gridPositions.
        int randomIndex = Random.Range(0, gridPositions.Count);

        // Declara una variable de tipo Vector3 y establece una posicion de la lista gridPositions con el indice aleatorio. 
        Vector3 randomPosition = gridPositions[randomIndex];
        
        // Elimina la posicion almacenada en ese índice para que no se pueda volver a usar.
        gridPositions.RemoveAt(randomIndex);

        //Devuelve la posición Vector3 seleccionada aleatoriamente.
        return randomPosition;
    }

    //LayoutObjectAtRandom acepta una matriz de objetos de juego para elegir junto con un rango mínimo y máximo para el número de objetos que se van a crear.
    void LayoutObjectAtRandom(GameObject[] tileArray, int minimum, int maximum)
    {
        // Elije un numero aleatorio de objetos para instanciar, dentro de los límites mínimo y máximo.
        int objectCount = Random.Range(minimum, maximum + 1);

        // Instancia objetos hasta que se alcance el límite elegido aleatoriamente por objectCount.
        for (int i = 0; i < objectCount; i++)
        {
            // Elije una posicion para randomPosition obteniendo una posición aleatoria de nuestra lista de disponibles almacenados en gridPositiion.
            Vector3 randomPosition = RandomPosition();

            // Elije un mosaico aleatorio del array tileArray, y los asigna ala variable tipo GameObject tileChoice.
            GameObject tileChoice = tileArray[Random.Range(0, tileArray.Length)];

            // Crea una instancia de tileChoice en la posición devuelta por RandomPosition sin cambios en la rotacion.
            Instantiate(tileChoice, randomPosition, Quaternion.identity);
        }
    }

    // SetupScene inicializa nuestro nivel y llama a las funciones anteriores para establecer el tablero de juego.
    public void SetupScene(int level)
    {
        // Crea las paredes exteriores y el suelo.
        BoardSetup();

        // Resetea nuestra lista de gridpositions.
        InitialiseList();

        // Instancia un número aleatorio de baldosas de paredes basadas en un mínimo y máximo, en posiciones aleatorias.
        LayoutObjectAtRandom(wallTiles, wallCount.minimum, wallCount.maximum);

        // Instancia un número aleatorio de baldosas de alimentos basadas en un mínimo y máximo, en posiciones aleatorias.
        LayoutObjectAtRandom(foodTiles, foodCount.minimum, foodCount.maximum);

        // Determina un número de enemigos basado en el nivel actual, en base a un logaritmo de progresión.
        int enemyCount = (int)Math.Log(level, 2f);

        // Instancia un número aleatorio de enemigos basado en un mínimo y un máximo, en posiciones aleatorias.
        LayoutObjectAtRandom(enemyTiles, enemyCount, enemyCount);

        // Crea una instancia del icono de salida en la esquina superior derecha de nuestro tablero de juego
        Instantiate(exit, new Vector3(columns - 1, rows - 1, 0f), Quaternion.identity);

    }
}
