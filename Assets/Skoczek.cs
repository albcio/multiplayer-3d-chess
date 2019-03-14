using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skoczek : Bierki
{

    public override bool[,] MozliweRuchy()
    {
        bool[,] tabRuchy = new bool[8, 8];
        //goralewo
        SkoczekKrok(pozycjaX - 1, pozycjaY + 2, ref tabRuchy);
        //goraprawo
        SkoczekKrok(pozycjaX + 1, pozycjaY + 2, ref tabRuchy);
        //prawogora
        SkoczekKrok(pozycjaX + 2, pozycjaY + 1, ref tabRuchy);
        //prawodol
        SkoczekKrok(pozycjaX + 2, pozycjaY -1, ref tabRuchy);
        //dollewo
        SkoczekKrok(pozycjaX - 1, pozycjaY - 2, ref tabRuchy);
        //dolprawo
        SkoczekKrok(pozycjaX + 1, pozycjaY - 2, ref tabRuchy);
        //lewogora
        SkoczekKrok(pozycjaX - 2, pozycjaY +1, ref tabRuchy);
        //lewodol
        SkoczekKrok(pozycjaX -2 , pozycjaY -1, ref tabRuchy);
        return tabRuchy;

    }

    public void SkoczekKrok(int x, int y, ref bool[,] r)
    {
        Bierki b;

        if(x >= 0 && x < 8 && y >= 0 && y < 8)
        {
            b = BoardManager.Instance.Bierki[x, y];
            if (b == null)
                r[x, y] = true;
            else if (czyBialy != b.czyBialy)
                r[x, y] = true;
        }
    }
}