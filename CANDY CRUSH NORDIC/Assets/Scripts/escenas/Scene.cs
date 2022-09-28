using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class Scene : MonoBehaviour
{
    public void Levels()
    {
        SceneManager.LoadScene("Levels");
    }

    public void Lv1()
    {
        SceneManager.LoadScene("Nivel1");
    }

    public void Lv2()
    {
        SceneManager.LoadScene("Nivel2");
    }

    public void Lv3()
    {
        SceneManager.LoadScene("Nivel3");
    }

    public void Lv4()
    {
        SceneManager.LoadScene("Nivel4");
    }
    public void Lv5()
    {
        SceneManager.LoadScene("Nivel5");
    }

    public void Menu()
    {
        SceneManager.LoadScene("Menu");
    }
}
