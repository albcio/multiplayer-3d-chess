using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour {
    static public string ip1;
    static public string port1;
    
    public void ButtonClick1()
    {
        SceneManager.LoadScene("plansza", LoadSceneMode.Single);
   }

    public void ipzmiana(string txt)
    {
        ip1 = txt;
    }

    public void portzmiana(string txt)
    {
        port1 = txt;

    }
}
