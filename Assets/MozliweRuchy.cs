using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MozliweRuchy : MonoBehaviour {

	public static MozliweRuchy Instance { set; get; }

    public GameObject ruchPrefab;
    private List<GameObject> ruchy;

    

    private void Start()
    {
        Instance = this;
        ruchy = new List<GameObject>();
    }

    private GameObject getMozliwyRuch()
    {
        GameObject tmp = ruchy.Find(g => !g.activeSelf);

        if (tmp == null)
        {
            tmp = Instantiate(ruchPrefab);
            ruchy.Add(tmp);
        }
        return tmp;
    }

    public void pokazMozliweRuchy(bool[,] ruchy){
        for(int i=0; i<8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                if (ruchy[i,j] == true)
                {
                    GameObject tmp = getMozliwyRuch();
                    tmp.SetActive(true);
                    tmp.transform.position = new Vector3(i+0.5f,0,j+0.5f);
                }
            }
        }
    }

    public void ukryjMozliweRuchy()
    {
        foreach (GameObject tmp in ruchy)
            tmp.SetActive(false);
    }
}
