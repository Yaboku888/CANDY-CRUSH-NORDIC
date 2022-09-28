using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BarraDeVida : MonoBehaviour
{
    public Image barradevida;
    public float vidaActual;
    public float vidaMaxima;
    public float vidanormal=1;
    public float vidaActual2;
    public float vidaActual3;
    public bool nose = false;
    public enemigo g;
    public float tiempo;

    private void Start()
    {
       // vidaMaxima = vidanormal;
        vidaActual = vidaMaxima;
        
    }

    void Update()
    {
        if(g.y==false)
        {
            barradevida.fillAmount = vidaActual / vidaMaxima;
        }
        if(vidaActual<=0&&g.z==true)
        {
            barradevida.fillAmount = vidaActual2 / vidaMaxima;
            
        }
        if(vidaActual2<=0&& g.x==true)
        {
           barradevida.fillAmount = vidaActual3 / vidaMaxima;
        }

        tiempo += Time.deltaTime;

        if(tiempo>=1)
        {
            nose = true;    
        }
    }

    public void hacerDaño()
    {
        if (nose == true)
        {
            float daño = 1;
            vidaActual = vidaActual - daño;
            vidaActual2 = vidaActual2 - daño;
            vidaActual3 = vidaActual3 - daño;
        }
    }

}
