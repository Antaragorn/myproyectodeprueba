using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MovingObject : MonoBehaviour
{
    public float moveTime = 0.1f;    //Tiempo que tardara el objeto en moverse en segundos.                 
    public LayerMask blokingLayer;   //Capa en la que se comprobara la colision.           

    private BoxCollider2D boxCollider;  //El componente BoxCollider2D asociado a este objeto.
    private Rigidbody2D rb2D;       //El componente Rigidbody asociado a este objeto.
    private float inverseMoveTime;  //Se utiliza para hacer el movimiento más eficiente.

    //Las funciones virtuales protegidas se pueden invalidar heredando clases.
    protected virtual void Start()
    {
        //Obtener una referencia de componente a este objeto BoxCollider2D.
        boxCollider = GetComponent<BoxCollider2D>();

        //Obtener una referencia de componente a este objeto Rigidbody.
        rb2D = GetComponent<Rigidbody2D>();

        //Al almacenar el recíproco del tiempo de movimiento podemos usarlo multiplicando 
        //en lugar de dividir, esto es más eficiente
        inverseMoveTime = 1f / moveTime;

    }

    /*Move devuelve true si es capaz de moverse y false si no. 
    Mover toma parámetros para la dirección x, la dirección 
    y un RaycastHit2D para comprobar la colisión.*/
    protected bool Move(int xDir, int yDir, out RaycastHit2D hit)
    {
        /*Almacena la posición inicial desde la que moverse, en función 
         * de la posición de transformación actual de los objetos.*/
        Vector2 start = transform.position;

        /*Calcula la posición final en función de los parámetros de dirección 
         * pasados al llamar a Mover.*/
        Vector2 end = start + new Vector2(xDir, xDir);

        /*Deshabilite boxCollider para que linecast no golpee el propio colisionador 
         * de este objeto.*/
        boxCollider.enabled = false;

        //Lanzar una línea de punto inicial a punto final de comprobación de colisión en blockingLayer.
        hit = Physics2D.Linecast(start, end, blokingLayer);

        //reactiva el boxcolider despues del linecast.
        boxCollider.enabled = true;

        //Comprueba si algo a golpeado.
        if (hit.transform == null)
        {
            /*Si no se ha golpeado nada, inicie la rutina co-rutinaria 
             * de SmoothMovement pasando en el extremo Vector2 como destino*/
            StartCoroutine(SmoothMovement(end));

            //Devuelve true para decir que Move tuvo éxito
            return true;
        }
        //Si algo fue golpeado, devolver false, Move no tuvo exito.
        return false;
    }
    /*La corrutina para mover unidades de un espacio a otro, toma end 
     * de parámetro para especificar a dónde se va a mover.*/
    protected IEnumerator SmoothMovement (Vector3 end)
    {

        /*Calcula la distancia restante para moverse en función de la 
         * magnitud cuadrada de la diferencia entre la posición actual y el parámetro final. 
        La magnitud cuadrada se utiliza en lugar de magnitud porque es computacionalmente más barato.*/
        float sqrRemainingDistance = (transform.position - end).sqrMagnitude;

        //Mientras esa distancia es mayor que una cantidad muy pequeña (Epsilon, casi cero):
        while (sqrRemainingDistance>float.Epsilon)
        {
            //Encuentra una nueva posición proporcionalmente más cercana al final, basada en el moveTime
            Vector3 newPosition = Vector3.MoveTowards(rb2D.position, end, inverseMoveTime * Time.deltaTime);

            //Llam a MovePosition en el Rigidbody2D adjunto y lo mueve a la posición calculada.
            rb2D.MovePosition(newPosition);

            //Recalcula la distancia restante después de moverse.
            sqrRemainingDistance = (transform.position - end).sqrMagnitude;

            //vuelve al bucle hasta sqrRemainingDistance está lo suficientemente cerca como para poner fin a la función
            yield return null;
        }
    }
    /*La palabra clave virtual significa que AttemptMove se puede invalidar 
     * heredando clases mediante la palabra clave override.
    AttemptMove toma un parámetro genérico T para especificar el tipo de componente 
    * con el que esperamos que nuestra unidad interactúe si está bloqueada 
    * (Jugador para enemigos, Pared para jugador).*/
    protected virtual void AttemptMove<T>(int xDir, int yDir)
        where T : Component
    {
        //Hit almacenará lo que nuestro linecast golpee cuando se llame a Move.
        RaycastHit2D hit;

        //configura canMove en true si Move se realizó correctamente, false si se produjo un error.
        bool canMove = Move(xDir, yDir, out hit);

        //Comprueba si nada fue golpeado por linecast.
        if (hit.transform == null)
            //si no ha golpeado nada, vuelve y no ejecuta mas codigo.
            return;

        /*Obtener una referencia de componente al componente de tipo T 
         * asociado al objeto que se golpeó*/
        T hitComponent = hit.transform.GetComponent<T>();

        /*Si canMove es false y hitComponent no es igual a null, lo que 
         * significa que MovingObject está bloqueado y ha golpeado algo 
         * con lo que puede interactuar*/
        if (!canMove && hitComponent != null)
            //Llam a la función OnCantMove y pásela hitComponent como parámetro.
            OnCantMove(hitComponent);
    }
    /*El modificador abstract indica que el objeto que se está modificando 
     * tiene una implementación faltante o incompleta.
    OnCantMove se reemplazará por funciones en las clases heredadas.*/

    protected abstract void OnCantMove <T> (T component)
        where T : Component;
        
}
