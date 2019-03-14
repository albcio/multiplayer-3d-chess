using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PromocjaObsluga : MonoBehaviour {
    
  //  public static int index;

    public static bool buttonPressed = false;
    public static bool buttonPressed2 = false;
    public static PromocjaObsluga Instance { set; get; }

    public void ButtonClick()
    {
        Debug.Log("goniec");
      //  index = 4;
        GameObject.Find("Szachownica").GetComponent<BoardManager>().indexpromocja = 4;
        buttonPressed = true;
        buttonPressed2 = true;


    }
    public void ButtonClick2()
    {
        Debug.Log("hetman");
       // index = 1;
        GameObject.Find("Szachownica").GetComponent<BoardManager>().indexpromocja = 1;
        buttonPressed = true;
        buttonPressed2 = true;

    }
    public void ButtonClick3()
    {
        Debug.Log("skoczek");
       // index = 3;
        GameObject.Find("Szachownica").GetComponent<BoardManager>().indexpromocja = 3;
        buttonPressed = true;
        buttonPressed2 = true;

    }
    public void ButtonClick4()
    {
        Debug.Log("wieza");
       // index = 2;
        GameObject.Find("Szachownica").GetComponent<BoardManager>().indexpromocja = 2;
        buttonPressed = true;
        buttonPressed2 = true;

    }




}
