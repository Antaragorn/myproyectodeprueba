using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//hola
public class Wall : MonoBehaviour
{
    //Sprite alternativo para mostrar después de que Muro haya sido atacado por el jugador.
    public Sprite dmgSprite; 
    
    //Puntos de golpe del muro.
    public int hp = 4;

    ///Almacena una referencia de componente del SpriteRenderer adjunto.
    private SpriteRenderer spriteRenderer;


    void Awake()
    {
       // Obtiene una referencia de componente para SpriteRenderer.
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    //DamageWall sera llamado cuando el jugador ataque a un muro.
    public void DamageWall(int loss)
    {
        //configura el spriteRenderer en el sprite de muro dañado.
        spriteRenderer.sprite = dmgSprite;
        //resta loss a los puntos de golpe totales.
        hp -= loss;

        //si los puntos de golpe son iguales o menores que 0, desactiva el gameObject.
        if (hp <= 0)
            gameObject.SetActive(false);
    }
}
