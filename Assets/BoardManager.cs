using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Drawing;
using Microsoft.AspNet.SignalR.Client;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;

public class BoardManager : MonoBehaviour
{
    public GameObject matokno;
    public GameObject patokno;
    public Bierki[,] Bierki { set; get; }
    private Bierki wybranaBierka;
    private Bierki szachujacaBierka;
    private const float TILE_SIZE = 1.0f;
    private int wybraneX = -5;
    private int wybraneY = -5;
    private const float TILE_OFFSET = 0.5f;
    public int[] zPrzelotem { set; get; }
    public List<GameObject> BierkiPrefabs;
    public bool czyRuchBialego = true;
    public List<GameObject> BierkiwGrze;

    private Material nieWybranyMat;
    public Material WybranyMat;
    public static BoardManager Instance { set; get; }
    private bool[,] dozwoloneRuchy { set; get; }
    public int indexpromocja = -1;
    public GameObject oknomenu;
    public GameObject canvasokno;
    public bool czyBialySzach = false;//czy bialy szachuje
    public bool czyCzarnySzach = false;//czy czarny szachuje

    public bool sprawdzSzachCalosc()
    {

        czyCzarnySzach = false;
        czyBialySzach = false;
        var czarnyKrolX = pozycjaXkrolCzarny();
        var czarnyKrolY = pozycjaYkrolCzarny();
        var bialyKrolX = pozycjaXkrolbialy();
        var bialyKrolY = pozycjaYkrolbialy();

        for (int k = 0; k < 8; k++)
        {
            for (int j = 0; j < 8; j++)
            {
                Bierki b = Bierki[j, k];

                if (b == null)
                {
                    continue;
                }


                var dr = b.MozliweRuchy();
                if (b.czyBialy == true)
                {
                    if (dr[czarnyKrolX, czarnyKrolY])
                    {
                        czyCzarnySzach = true;
                        Debug.Log("Czarny szachowany");
                        return true;
                    }
                }
                else
                {

                    if (dr[bialyKrolX, bialyKrolY])
                    {
                        czyBialySzach = true;
                        Debug.Log("Bialy szachowany");
                        return true;
                    }
                }
            }
        }
        return false;
    }



    public bool sprawdzSzachBierki(int x, int y)
    { 
        Bierki b = Bierki[x, y];
        if (b == null)
        {
            return false;
        }

        var dr = b.MozliweRuchy();
        if (b.czyBialy != true)
        {

            if (dr[pozycjaXkrolbialy(), pozycjaYkrolbialy()])
            {
                czyBialySzach = true;
                Debug.Log("Bialy szachowany");
                return true;
            }
        }
        else
        {
            if (dr[pozycjaXkrolCzarny(), pozycjaYkrolCzarny()])
            {
                czyCzarnySzach = true;
                Debug.Log("Czarny szachowany");
                return true;
            }

        }
        return false;
    }
   
    public bool bialyKrotkaRoszada = true;
    public bool bialyDlugaRoszada = true;
    public bool czarnyKrotkaRoszada = true;
    public bool czarnyDlugaRoszada = true;
    private bool mojRuch = true;
    private Guid playerName = Guid.NewGuid();
    private IHubProxy myHub;

    List<Task> queue = new List<Task>();

    private void Start()
    {
       
        Instance = this;
        dozwoloneRuchy = new bool[8, 8];
        poczatekGry();
        var connection = new HubConnection(String.Concat("http://",MenuManager.ip1,":",MenuManager.port1,"/"));
        myHub = connection.CreateHubProxy("MyHub");

        connection.Start().Wait();
        myHub.On<Guid, int[]>("addMessage", (s1, message) =>
        {

            if (message[0] == -1)
            {
                queue.Add(new Task(() =>
                {
                    Debug.Log("MAT");
                    GameObject go = Instantiate(matokno) as GameObject;
                    go.SetActive(true);
                    //queue.Clear();
                }));
            }
            else if (message[0] == -2)
            {
                queue.Add(new Task(() =>
                   {
                       Debug.Log("PAT");
                       // throw new Exception("pat"); 
                       GameObject go = Instantiate(patokno) as GameObject;
                       go.SetActive(true);
                       //queue.Clear();
                   }));
            }

            else if (s1 != playerName && message[0] >= 0)
            {
                queue.Add(new Task(() =>
                {
                    wybranaBierka = Bierki[message[0], message[1]];
                    wybraneX = message[0];
                    wybraneY = message[1];
                    dozwoloneRuchy[message[2], message[3]] = true;
                    Roszada(message[2], message[3]);
                    RuchBierka(message[2], message[3]);
                    mojRuch = true;
                }));

            }
            else if (message[0] == -3 && s1 != playerName)
            {
                queue.Add(new Task(() =>
               {
                   BierkiwGrze.Remove(Bierki[message[2], message[3]].gameObject);
                   Destroy(Bierki[message[2], message[3]].gameObject);
                   BierkaSpawn(message[1], message[2], message[3], -90f, 0);
                   if (sprawdzSzachCalosc())
                   {
                       szachujacaBierka = Bierki[message[2], message[3]];
                   }
               }));
            }
        });
    }

    private void Update()
    {
        while (queue.Count > 0)
        {
            var task = queue.First();
            task.RunSynchronously();
            if (queue.Any())
                queue.RemoveAt(0);
        }
        if (!mojRuch)
        {

            Debug.Log("Nie moj ruch");
            return;
        }

        WybierzKursor();


        if (czyRuchBialego != true)
        {
            GameObject.Find("Main Camera").transform.position = new Vector3(4f, 6.5f, 8.5f);
            GameObject.Find("Main Camera").transform.rotation = Quaternion.Euler(57f, 180f, 0);
        }
        else
        {
            GameObject.Find("Main Camera").transform.position = new Vector3(4f, 6.5f, -1f);
            GameObject.Find("Main Camera").transform.rotation = Quaternion.Euler(57f, 0, 0);
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (wybraneX >= 0 && wybraneY >= 0)
            {

                if (wybranaBierka == null)
                {
                    WybierzBierke(wybraneX, wybraneY);

                }
                else
                {
                    Roszada(wybraneX, wybraneY);
                    RuchBierka(wybraneX, wybraneY);
                    //Roszada(wybraneX, wybraneY);

                }

            }
        }


    }

    private void Roszada(int wx, int wy)
    {
        if (dozwoloneRuchy[wx, wy] == true ){
            if ((czarnyKrotkaRoszada == true && wybranaBierka == Bierki[4, 7] && wx == 6 && wy == 7))
            {
                BierkiwGrze.Remove(Bierki[7, 7].gameObject);
                Destroy(Bierki[7, 7].gameObject);
                BierkaSpawn(8, 5, 7, -90f, 0);
                //if (mojRuch)
                    czarnyKrotkaRoszada = false;
            }
            if ((czarnyDlugaRoszada == true && wybranaBierka == Bierki[4, 7] && wx == 2 && wy == 7))
            {
                BierkiwGrze.Remove(Bierki[0, 7].gameObject);
                Destroy(Bierki[0, 7].gameObject);
                BierkaSpawn(8, 3, 7, -90f, 0);
                //if (mojRuch)
                    czarnyDlugaRoszada = false;
            }
            if ((bialyKrotkaRoszada == true && wybranaBierka == Bierki[4, 0] && wx == 6 && wy == 0))
            {
                BierkiwGrze.Remove(Bierki[7, 0].gameObject);
                Destroy(Bierki[7, 0].gameObject);
                BierkaSpawn(2, 5, 0, -90f, 0);
                //if (mojRuch)
                    bialyKrotkaRoszada = false;
            }
            if ((bialyDlugaRoszada == true && wybranaBierka == Bierki[4, 0] && wx == 2 && wy == 0))
            {
                BierkiwGrze.Remove(Bierki[0, 0].gameObject);
                Destroy(Bierki[0, 0].gameObject);
                BierkaSpawn(2, 3, 0, -90f, 0);
                //if (mojRuch)
                    bialyDlugaRoszada = false;
            }
        }
    }


    private void poczatekGry()
    {
        czyRuchBialego = true;
        BierkiwGrze = new List<GameObject>();
        Bierki = new Bierki[8, 8];
        zPrzelotem = new int[2] { -1, -1 };

        //biali

        BierkaSpawn(0, 4, 0, -90f, 0);

        BierkaSpawn(1, 3, 0, -90f, 0);

        BierkaSpawn(2, 0, 0, -90f, 0);
        BierkaSpawn(2, 7, 0, -90f, 0);

        BierkaSpawn(4, 5, 0, -90f, 0);
        BierkaSpawn(4, 2, 0, -90f, 0);


        BierkaSpawn(3, 1, 0, -90f, 0);
        BierkaSpawn(3, 6, 0, -90f, 0);

        BierkaSpawn(5, 0, 1, 0, 0);
        BierkaSpawn(5, 1, 1, 0, 0);
        BierkaSpawn(5, 2, 1, 0, 0);
        BierkaSpawn(5, 3, 1, 0, 0);
        BierkaSpawn(5, 4, 1, 0, 0);
        BierkaSpawn(5, 5, 1, 0, 0);
        BierkaSpawn(5, 6, 1, 0, 0);
        BierkaSpawn(5, 7, 1, 0, 0);

        //czarni

        BierkaSpawn(6, 4, 7, -90f, 0);

        BierkaSpawn(7, 3, 7, -90f, 0);

        BierkaSpawn(8, 0, 7, -90f, 0);
        BierkaSpawn(8, 7, 7, -90f, 0);

        BierkaSpawn(10, 5, 7, -90f, 180f);
        BierkaSpawn(10, 2, 7, -90f, 180f);


        BierkaSpawn(9, 1, 7, -90f, 180f);
        BierkaSpawn(9, 6, 7, -90f, 180f);

        BierkaSpawn(11, 0, 6, 0, 0);
        BierkaSpawn(11, 1, 6, 0, 0);
        BierkaSpawn(11, 2, 6, 0, 0);
        BierkaSpawn(11, 3, 6, 0, 0);
        BierkaSpawn(11, 4, 6, 0, 0);
        BierkaSpawn(11, 5, 6, 0, 0);
        BierkaSpawn(11, 6, 6, 0, 0);
        BierkaSpawn(11, 7, 6, 0, 0);

        //rozowy
        nieWybranyMat = Bierki[0, 0].GetComponent<MeshRenderer>().material;

    }

    private void WybierzBierke(int x, int y)
    {
        if (wybranaBierka != null)
        {
            wybranaBierka.GetComponent<MeshRenderer>().material = WybranyMat;

            MozliweRuchy.Instance.ukryjMozliweRuchy();
            wybranaBierka = null;
        }

        if (Bierki[x, y] == null)
            return;

        if (Bierki[x, y].czyBialy != czyRuchBialego)
            return;
        dozwoloneRuchy = Bierki[x, y].MozliweRuchy();
        var ruchy = RuchyZUwzglednieneimSzachu(dozwoloneRuchy, Bierki[x, y]);
        bool czyMaRuch = false;
        for (int i = 0; i < 8; i++)
            for (int j = 0; j < 8; j++)
                if (ruchy[i, j])
                    czyMaRuch = true;

        if (!czyMaRuch)
            return;


        wybranaBierka = Bierki[x, y];

        nieWybranyMat = wybranaBierka.GetComponent<MeshRenderer>().material;
        WybranyMat.mainTexture = nieWybranyMat.mainTexture;
        wybranaBierka.GetComponent<MeshRenderer>().material = WybranyMat;
        MozliweRuchy.Instance.pokazMozliweRuchy(ruchy);
    }

    IEnumerator CorPromocja(int x, int y)
    {
        GameObject go = Instantiate(canvasokno) as GameObject;
        go.SetActive(true);
        yield return new WaitUntil(() => PromocjaObsluga.buttonPressed);
        go.SetActive(false);
	PromocjaObsluga.buttonPressed = false;
        var promocjaNumer = indexpromocja + (czyRuchBialego ? 6 : 0);
        BierkaSpawn(promocjaNumer, x, y, -90f, 0);

        myHub.Invoke<string>("Send", playerName, new[] { -3, promocjaNumer, x, y }).ContinueWith(task1 =>
        {
            if (task1.IsFaulted)
            {
                Debug.Log($"There was an error calling send: {task1.ToString()}");
            }
            else
            {
                Debug.Log(task1.Result);
            }
        });

    }


    private void RuchBierka(int x, int y)
    {
        if (dozwoloneRuchy[x, y] == true)
        {
            
            Point przedRuchem = new Point(wybranaBierka.pozycjaX, wybranaBierka.pozycjaY);
            Bierki b = Bierki[x, y];
            if (b != null && b.czyBialy != czyRuchBialego)
            {
                BierkiwGrze.Remove(b.gameObject);
                Destroy(b.gameObject);
            }
            if (x == zPrzelotem[0] && y == zPrzelotem[1])
            {
                if (czyRuchBialego == true)
                {
                    b = Bierki[x, y - 1];
                    BierkiwGrze.Remove(b.gameObject);
                    Destroy(b.gameObject);
                }
                else
                {
                    b = Bierki[x, y + 1];
                    BierkiwGrze.Remove(b.gameObject);
                    Destroy(b.gameObject);
                }
            }
            zPrzelotem[0] = -1;
            zPrzelotem[1] = -1;

            if (wybranaBierka.GetType() == typeof(Pionek) && mojRuch)
            {
                if (czyRuchBialego == true && y == 7)
                {

                    BierkiwGrze.Remove(wybranaBierka.gameObject);
                    Destroy(wybranaBierka.gameObject);
                    StartCoroutine(CorPromocja(x, y));

                }
                else if (czyRuchBialego != true && y == 0)
                {
                    BierkiwGrze.Remove(wybranaBierka.gameObject);
                    Destroy(wybranaBierka.gameObject);
                    StartCoroutine(CorPromocja(x, y));
                }
            }
            if (wybranaBierka.GetType() == typeof(Pionek)) { 
                if (wybranaBierka.pozycjaY == 1 && y == 3)
                {
                    zPrzelotem[0] = x;
                    zPrzelotem[1] = y - 1;
                }
                else if (wybranaBierka.pozycjaY == 6 && y == 4)
                {
                    zPrzelotem[0] = x;
                    zPrzelotem[1] = y + 1;
                }
            }
          
            if (czyRuchBialego && (bialyDlugaRoszada == true || bialyKrotkaRoszada == true))
            {
                if (wybranaBierka.pozycjaX == 4 && wybranaBierka.pozycjaY == 0)
                {
                    bialyDlugaRoszada = false;
                    bialyKrotkaRoszada = false;
                }
                else if (wybranaBierka.pozycjaX == 7 && wybranaBierka.pozycjaY == 0)
                {
                    bialyKrotkaRoszada = false;
                }
                else if (wybranaBierka.pozycjaX == 0 && wybranaBierka.pozycjaY == 0)
                {
                    bialyDlugaRoszada = false;
                }
            }
            else if (czyRuchBialego != true && (czarnyDlugaRoszada == true || czarnyKrotkaRoszada == true))
            {
                if (wybranaBierka.pozycjaX == 4 && wybranaBierka.pozycjaY == 7)
                {
                    czarnyDlugaRoszada = false;
                    czarnyKrotkaRoszada = false;
                    Debug.Log("czarni // Ruch krolem, obie roszady false");
                }
                else if (wybranaBierka.pozycjaX == 7 && wybranaBierka.pozycjaY == 7)
                {
                    czarnyKrotkaRoszada = false;
                    Debug.Log("czarni // Ruch wieza blizej krola // roszada false");
                }
                else if (wybranaBierka.pozycjaX == 0 && wybranaBierka.pozycjaY == 7)
                {
                    czarnyDlugaRoszada = false;
                    Debug.Log("czarni // Ruch wieza dalej krola // roszada false");
                }
            }
            Bierki[wybranaBierka.pozycjaX, wybranaBierka.pozycjaY] = null;
            wybranaBierka.transform.position = GetPosition(x, y);
            wybranaBierka.zmienUstawienie(x, y);
            Bierki[x, y] = wybranaBierka;
            wybranaBierka.pozycjaX = x;
            wybranaBierka.pozycjaY = y;
            czyRuchBialego = !czyRuchBialego;

            if (sprawdzSzachCalosc())
            {
                szachujacaBierka = Bierki[x, y];

                if (CzyMat(szachujacaBierka))
                {
                    myHub.Invoke<string>("Send", playerName, new[] { -1, -1, -1, -1 }).ContinueWith(task1 =>
                    {
                        if (task1.IsFaulted)
                        {
                            Debug.Log($"There was an error calling send: {task1.ToString()}");
                        }
                        else
                        {
                            Debug.Log(task1.Result);
                        }
                    });

                }
            }
            else
            {   
                if (CzyPat(Bierki[x, y]))
                {
                    myHub.Invoke<string>("Send", playerName, new[] { -2, -1, -1, -1 }).ContinueWith(task1 =>
                    {
                        if (task1.IsFaulted)
                        {
                            Debug.Log($"There was an error calling send: {task1.ToString()}");
                        }
                        else
                        {
                            Debug.Log(task1.Result);
                        }
                    });

                }
            }

            if (mojRuch)
            {

                var points = new Point[] { przedRuchem, new Point(x, y) };
                myHub.Invoke<string>("Send", playerName, new[] { points[0].X, points[0].Y, points[1].X, points[1].Y }).ContinueWith(task1 =>
                    {
                        if (task1.IsFaulted)
                        {
                            Debug.Log($"There was an error calling send: {task1.ToString()}");
                        }
                        else
                        {
                            Debug.Log(task1.Result);
                        }
                    });


                mojRuch = false;
                wybranaBierka.GetComponent<MeshRenderer>().material = nieWybranyMat; 
                MozliweRuchy.Instance.ukryjMozliweRuchy();
            }




            wybranaBierka = null;
        }
        else
        {
            wybranaBierka.GetComponent<MeshRenderer>().material = nieWybranyMat;
            MozliweRuchy.Instance.ukryjMozliweRuchy();

            wybranaBierka = null;
        }
    }

    private bool[,] RuchyZUwzglednieneimSzachu(bool[,] dozwoloneRuchy, Bierki wybranaBierka)
    {
        bool[,] ruchy = new bool[8, 8];
        for (int i = 0; i < 8; i++)
            for (int j = 0; j < 8; j++)
                ruchy[i, j] = dozwoloneRuchy[i, j];

        //nie ma szachu - sprawdzamy czy nei ma szachu po ruchu
        if (!czyBialySzach && !czyCzarnySzach)
        {
            //bierka znika z pola czy jest szach
            if (!TestujPoleBrakBierki())
            {
                for (int i = 0; i < 8; i++)
                {
                    for (int j = 0; j < 8; j++)
                    {
                        ruchy[i, j] = false;
                    }
                }
            }

            sprawdzSzachCalosc();
            //czy ruch króla spowoduje szach
            var krol = wybranaBierka as Krol;
            if (krol != null)
            {
                for (int i = 0; i < 8; i++)
                    for (int j = 0; j < 8; j++)
                        if (ruchy[i, j])
                        {
                            if (szachKrola(i, j, krol))
                                ruchy[i, j] = false;
                        }


            }
            sprawdzSzachCalosc();
            return ruchy;
        }

        if (szachujacaBierka == null)
        {
            return ruchy;
        }
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                if (ruchy[i, j] == false)
                {
                    continue;
                }

                
                ruchy[i, j] = TestujPole(i, j, ruchy[i,j]);

            }
        }
        //pozycja wyjscia szach
        sprawdzSzachCalosc();
        return ruchy;
    }

    public bool szachKrola(int i, int j, Bierki krol)
    {
        var tmp = Bierki[i, j];
        Bierki[i, j] = krol;
        Bierki[krol.pozycjaX, krol.pozycjaY] = null;
        try
        {
            if (sprawdzSzachCalosc())
            {
                return true;
            }
        }
        finally
        {
            Bierki[krol.pozycjaX, krol.pozycjaY] = Bierki[i, j];
            Bierki[i, j] = tmp;
        }
        return false;
    }

    public bool TestujPole(int i, int j, bool defaultValue = true)
    {

        Instance = this;
        var tmp = Bierki[i, j];
        Bierki[i, j] = Bierki[wybraneX, wybraneY];
        var tmp2 = Bierki[wybraneX, wybraneY];
        Bierki[wybraneX, wybraneY] = null;
        try
        {

            if (sprawdzSzachBierki(szachujacaBierka.pozycjaX, szachujacaBierka.pozycjaY))
            {
                return false;
            }
            if(tmp2 is Krol && sprawdzSzachCalosc())
            {
                return false;
            }
            return defaultValue;
        }
        finally
        {

            Bierki[wybraneX, wybraneY] = Bierki[i, j];
            Bierki[i, j] = tmp;
        }
    }




    private bool TestujPoleBrakBierki()
    {

        Instance = this;
        var tmp = Bierki[wybraneX, wybraneY];
        Bierki[wybraneX, wybraneY] = null;
        if (sprawdzSzachCalosc())
        {
            Bierki[wybraneX, wybraneY] = tmp;
            return false;
        }

        Bierki[wybraneX, wybraneY] = tmp;
        return true;
    }

    private void WybierzKursor()
    {
        RaycastHit kursor;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out kursor, 25.0f, LayerMask.GetMask("ChessPlane")))
        {
            wybraneX = (int)kursor.point.x;
            wybraneY = (int)kursor.point.z;

        }
        else
        {
            wybraneX = -5;
            wybraneY = -5;
        }
    }

    public void BierkaSpawn(int index, int x, int y, float quaterx, float quatery)
    {
        GameObject go = Instantiate(BierkiPrefabs[index], GetPosition(x, y), Quaternion.Euler(quaterx, quatery, 0)) as GameObject;
        go.transform.SetParent(transform);
        Bierki[x, y] = go.GetComponent<Bierki>();
        Bierki[x, y].zmienUstawienie(x, y);
        BierkiwGrze.Add(go);
    }

    private Vector3 GetPosition(int x, int z)
    {
        Vector3 v = Vector3.zero;
        v.x += (TILE_SIZE * x) + TILE_OFFSET;
        v.z += (TILE_SIZE * z) + TILE_OFFSET;
        return v;

    }

    public int pozycjaXkrolbialy()
    {
        for (int k = 0; k < 8; k++)
        {
            for (int j = 0; j < 8; j++)
            {
                Bierki b = Bierki[j, k];
                if (b != null && b.czyBialy == true && b.GetType() == typeof(Krol))
                {
                    return j;
                }
            }
        }
        return wybraneX;
    }

    public int pozycjaYkrolbialy()
    {
        for (int k = 0; k < 8; k++)
        {
            for (int j = 0; j < 8; j++)
            {
                Bierki b = Bierki[j, k];
                if (b != null && b.czyBialy == true && b.GetType() == typeof(Krol))
                {
                    return k;
                }
            }
        }
        return wybraneY;
    }
    public int pozycjaXkrolCzarny()
    {
        for (int k = 0; k < 8; k++)
        {
            for (int j = 0; j < 8; j++)
            {
                Bierki b = Bierki[j, k];
                if (b != null && b.czyBialy != true && b.GetType() == typeof(Krol))
                {
                    return j;
                }
            }
        }
        return wybraneX;
    }

    public int pozycjaYkrolCzarny()
    {
        for (int k = 0; k < 8; k++)
        {
            for (int j = 0; j < 8; j++)
            {
                Bierki b = Bierki[j, k];
                if (b != null && b.czyBialy != true && b.GetType() == typeof(Krol))
                {
                    return k;
                }
            }
        }
        return wybraneY;
    }

    private bool CzyPat(Bierki bierka)
    {
        return !CzyKolorMaRuch(bierka.czyBialy) && !sprawdzSzachCalosc();
        //return false;
    }

    private bool CzyMat(Bierki bierka)
    {
        return !CzyKolorMaRuch(bierka.czyBialy) && sprawdzSzachCalosc();
    }

    private bool CzyKolorMaRuch(bool bialy)
    {

        //sprawdzamy wszystkie bierki innego koloru czy maja ruch ktory blokuje szach
        for (int k = 0; k < 8; k++)
        {
            for (int j = 0; j < 8; j++)
            {
                Bierki b = Bierki[j, k];

                if (b == null || b.czyBialy == bialy)
                {
                    continue;
                }

                var tmpX = wybraneX;
                var tmpY = wybraneY;
                wybraneX = j;
                wybraneY = k;
                var dr = RuchyZUwzglednieneimSzachu(b.MozliweRuchy(), b);
                wybraneY = tmpY;
                wybraneX = tmpX;
                for (int x = 0; x < 8; x++)
                {
                    for (int y = 0; y < 8; y++)
                    {
                        //bierka ma ruch ktora blokuje szach - mata nie ma
                        if (dr[x, y])
                        {
                            return true;
                        }
                    }
                }
            }
        }
        return false;
    }
}

