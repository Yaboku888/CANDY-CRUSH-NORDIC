using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
public class cronometro : MonoBehaviour
{
    public TextMeshProUGUI contador;
    public float tiempo;
    public bool reloj = false;

    // Start is called before the first frame update
    void Start()
    {
        contador.text = "" + tiempo; 
        
    }

    // Update is called once per frame
    void Update()
    {
        tiempo -= Time.deltaTime;
        contador.text = "" + tiempo.ToString("f0");
        if (tiempo <= 0)
        {
            contador.text = "0";
            reloj = true;
        }
        if(reloj==true)
        {
            SceneManager.LoadScene("Over");
        }

        
    }
}
