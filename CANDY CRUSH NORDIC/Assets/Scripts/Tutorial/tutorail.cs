using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tutorail : MonoBehaviour
{
    public GameObject circulo;
    public GameObject nose;
    public float condicion;
    public bool G = false;
    
    // Start is called before the first frame update
    void Start()
    {
       
      
    }

    // Update is called once per frame
    void Update()
    {
        condicion = condicion + Time.deltaTime;

        if (condicion >= 4)
        {
            circulo.transform.position = nose.transform.position;
            G = true;
        }
        if (G==true)
        {
            Debug.Log("Thnks");
        }
    }
}
