using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Krol : Bierki
{

    public override bool[,] MozliweRuchy()
    {
        bool[,] tabRuchy = new bool[8, 8];
        Bierki b;
        int i, j;

        //prawo
        if (pozycjaX != 7)
        {
            b = BoardManager.Instance.Bierki[pozycjaX + 1, pozycjaY];
            if (b == null)
                tabRuchy[pozycjaX + 1, pozycjaY] = true;
            else if (czyBialy != b.czyBialy)
                tabRuchy[pozycjaX + 1, pozycjaY] = true;
        }

        //lewo
        if (pozycjaX != 0)
        {
            b = BoardManager.Instance.Bierki[pozycjaX - 1, pozycjaY];
            if (b == null)
                tabRuchy[pozycjaX - 1, pozycjaY] = true;
            else if (czyBialy != b.czyBialy)
                tabRuchy[pozycjaX - 1, pozycjaY] = true;
        }



        //gora
        i = pozycjaX - 1;
        j = pozycjaY + 1;
        if (j <= 7)
        {
            for (int k = 0; k < 3; k++)
            {
                if (i >= 0 && i < 8)
                {
                    b = BoardManager.Instance.Bierki[i, j];
                    if (b == null)
                        tabRuchy[i, j] = true;
                    else if (czyBialy != b.czyBialy)
                        tabRuchy[i, j] = true;

                }
                i++;
            }
        }

        //dol
        i = pozycjaX - 1;
        j = pozycjaY - 1;
        if (j >= 0)
        {
            for (int k = 0; k < 3; k++)
            {
                if (i >= 0 && i < 8)
                {
                    b = BoardManager.Instance.Bierki[i, j];
                    if (b == null)
                        tabRuchy[i, j] = true;
                    else if (czyBialy != b.czyBialy)
                        tabRuchy[i, j] = true;

                }
                i++;
            }
        }
        if (!czyBialy && BoardManager.Instance.czarnyKrotkaRoszada && BoardManager.Instance.Bierki[pozycjaX + 1, pozycjaY] == null && BoardManager.Instance.Bierki[pozycjaX + 2, pozycjaY] == null && !SzachRoszada())
            tabRuchy[pozycjaX + 2, pozycjaY] = true;
        if (czyBialy != true && BoardManager.Instance.czarnyDlugaRoszada == true && BoardManager.Instance.Bierki[pozycjaX - 1, pozycjaY] == null && BoardManager.Instance.Bierki[pozycjaX - 2, pozycjaY] == null && BoardManager.Instance.Bierki[pozycjaX - 3, pozycjaY] == null && !SzachRoszada(-1))
            tabRuchy[pozycjaX - 2, pozycjaY] = true;
        if (czyBialy == true && BoardManager.Instance.bialyKrotkaRoszada == true && BoardManager.Instance.Bierki[pozycjaX + 1, pozycjaY] == null && BoardManager.Instance.Bierki[pozycjaX + 2, pozycjaY] == null && !SzachRoszada())
            tabRuchy[pozycjaX + 2, pozycjaY] = true;
        if (czyBialy == true && BoardManager.Instance.bialyDlugaRoszada == true && BoardManager.Instance.Bierki[pozycjaX - 1, pozycjaY] == null && BoardManager.Instance.Bierki[pozycjaX - 2, pozycjaY] == null && BoardManager.Instance.Bierki[pozycjaX - 3, pozycjaY] == null && !SzachRoszada(-1))
            tabRuchy[pozycjaX - 2, pozycjaY] = true;


        return tabRuchy;
    }

    private bool SzachRoszada(int mod=1)
    {
        return BoardManager.Instance.szachKrola(pozycjaX +mod* 1, pozycjaY, this) || BoardManager.Instance.szachKrola(pozycjaX +mod* 2, pozycjaY, this);
    }
}
