using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class enemigo : MonoBehaviour
{
    public BarraDeVida vida;
    public GameObject enemigo1;
    public GameObject enemigo2;
    public GameObject enemigo3;
    public GameObject fondo;
    public bool y = false;
    public bool x = false;
    public bool z = false;
    public Board k;
    public GameObject sonido2;
    // Start is called before the first frame update
    void Start()
    {
        enemigo2.SetActive(false);
        enemigo3.SetActive(false);
        sonido2.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if(vida.vidaActual<=0)
        {
            enemigo1.SetActive(false);
            enemigo2.SetActive(true);
            if (y == false)
            {
                vida.vidaMaxima += vida.vidanormal;
                vida.vidaActual2 = vida.vidaMaxima;

                y = true;
                z = true;
            }
        }
        if(vida.vidaActual2<=0 && y==true)
        {
            
            enemigo1.SetActive(false);
            enemigo2.SetActive(false);
            enemigo3.SetActive(true);
            fondo.SetActive(false);
            if(y==true&& x== false)
            {
                vida.vidaMaxima += vida.vidanormal;
                vida.vidaActual3 = vida.vidaMaxima;
                x = true;
                k.Sonido.SetActive(false);
                sonido2.SetActive(true);
            }
            
        }
        if(vida.vidaActual3<=0 && x== true)
        {
            SceneManager.LoadScene("Levels");
        }
    }
}
