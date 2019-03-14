using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class goniec : Bierki
{

    public override bool[,] MozliweRuchy()
    {
        bool[,] tabRuchy = new bool[8, 8];
        Bierki b;
        int i, j;

        //goralewo
        i = pozycjaX;
        j = pozycjaY;
        while (true)
        {
            i--;
            j++;
            if (i < 0 || j >= 8)
            {
                break;
            }
            b = BoardManager.Instance.Bierki[i, j];
            if (b == null)
            {
                tabRuchy[i, j] = true;
            }
            else
            {
                if (czyBialy != b.czyBialy)
                {
                    tabRuchy[i, j] = true;
                }
                break;
            }
            
        }

        //goraprawo
        i = pozycjaX;
        j = pozycjaY;
        while (true)
        {
            i++;
            j++;
            if (i >=8  || j >= 8)
            {
                break;
            }
            b = BoardManager.Instance.Bierki[i, j];
            if (b == null)
            {
                tabRuchy[i, j] = true;
            }
            else
            {
                if (czyBialy != b.czyBialy)
                {
                    tabRuchy[i, j] = true;
                }
                break;
            }

        }

        //dollewo
        i = pozycjaX;
        j = pozycjaY;
        while (true)
        {
            i--;
            j--;
            if (i <0 || j<0)
            {
                break;
            }
            b = BoardManager.Instance.Bierki[i, j];
            if (b == null)
            {
                tabRuchy[i, j] = true;
            }
            else
            {
                if (czyBialy != b.czyBialy)
                {
                    tabRuchy[i, j] = true;
                }
                break;
            }

        }

        //dol prawo
        i = pozycjaX;
        j = pozycjaY;
        while (true)
        {
            i++;
            j--;
            if (j <0 || i >= 8)
            {
                break;
            }
            b = BoardManager.Instance.Bierki[i, j];
            if (b == null)
            {
                tabRuchy[i, j] = true;
            }
            else
            {
                if (czyBialy != b.czyBialy)
                {
                    tabRuchy[i, j] = true;
                }
                break;
            }

        }
        return tabRuchy;
    }
    
}
