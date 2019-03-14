using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Bierki : MonoBehaviour {

    public int pozycjaX{set;get;}
    public int pozycjaY{set;get;}
    public bool czyBialy;

    public void zmienUstawienie(int x, int y)
    {
        pozycjaX = x;
        pozycjaY = y;
    }

	public abstract bool[,] MozliweRuchy();

    
}
