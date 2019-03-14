using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pionek : Bierki {

	public override bool[,] MozliweRuchy()
    {
        bool[,] tabRuchy = new bool[8, 8];
        Bierki b, b2;
        //biale pionki ruchy

        if (czyBialy == true)
        {
            //lewo
            if(pozycjaX!=0 && pozycjaY != 7)
            {
                int[] e = BoardManager.Instance.zPrzelotem;
                if (e[0] == pozycjaX - 1 && e[1] == pozycjaY + 1)
                {
                    tabRuchy[pozycjaX - 1, pozycjaY + 1] = true;
                    
                }


                b = BoardManager.Instance.Bierki[pozycjaX - 1, pozycjaY + 1];
                if (b != null && !b.czyBialy)
                    tabRuchy[pozycjaX - 1, pozycjaY + 1] = true;
            }
            //prawo

            if (pozycjaX != 7 && pozycjaY != 7)
            {
                int[] e = BoardManager.Instance.zPrzelotem;
                if (e[0] == pozycjaX + 1 && e[1] == pozycjaY + 1)
                {
                    tabRuchy[pozycjaX + 1, pozycjaY + 1] = true;
                    
                }
                b = BoardManager.Instance.Bierki[pozycjaX + 1, pozycjaY + 1];
                if (b != null && !b.czyBialy)
                    tabRuchy[pozycjaX + 1, pozycjaY + 1] = true;
            }

            //przod

        if(pozycjaY != 7)
            {
                b = BoardManager.Instance.Bierki[pozycjaX, pozycjaY + 1];
                if (b == null)
                {
                    tabRuchy[pozycjaX, pozycjaY + 1] = true;
                }
            }

            //2 do przodu

            if (pozycjaY == 1)
            {
                b = BoardManager.Instance.Bierki[pozycjaX, pozycjaY + 1];
                b2 = BoardManager.Instance.Bierki[pozycjaX, pozycjaY + 2];
                if(b==null && b2 == null){
                    tabRuchy[pozycjaX, pozycjaY + 2] = true;
                }
            }
        }
        else{
            //czarni
            
            //lewo

            if (pozycjaX != 0 && pozycjaY != 0)
            {
                int[] e = BoardManager.Instance.zPrzelotem;
                if (e[0] == pozycjaX - 1 && e[1] == pozycjaY - 1)
                {
                    tabRuchy[pozycjaX - 1, pozycjaY - 1] = true;

                }
                b = BoardManager.Instance.Bierki[pozycjaX - 1, pozycjaY - 1];
                if (b != null && b.czyBialy)
                    tabRuchy[pozycjaX - 1, pozycjaY -1] = true;
            }
            
            //prawo

            if (pozycjaX != 7 && pozycjaY != 0)
            {
                int[] e = BoardManager.Instance.zPrzelotem;
                if (e[0] == pozycjaX + 1 && e[1] == pozycjaY - 1)
                {
                    tabRuchy[pozycjaX + 1, pozycjaY - 1] = true;

                }
                b = BoardManager.Instance.Bierki[pozycjaX + 1, pozycjaY - 1];
                if (b != null && b.czyBialy)
                    tabRuchy[pozycjaX + 1, pozycjaY - 1] = true;
            }

            //przod

            if (pozycjaY != 0)
            {
                b = BoardManager.Instance.Bierki[pozycjaX, pozycjaY - 1];
                if (b == null)
                {
                    tabRuchy[pozycjaX, pozycjaY - 1] = true;
                }
            }

            //2 do przodu

            if (pozycjaY == 6)
            {
                b = BoardManager.Instance.Bierki[pozycjaX, pozycjaY - 1];
                b2 = BoardManager.Instance.Bierki[pozycjaX, pozycjaY - 2];
                if (b == null & b2 == null)
                {
                    tabRuchy[pozycjaX, pozycjaY - 2] = true;
                }
            }
        }
        return tabRuchy;
    }
}
