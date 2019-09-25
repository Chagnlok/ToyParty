using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    int _iSize = 6;
    public Tile[] _tiles;

    GameObject[] _prefTiles;

    //0 1 2
    //3 4 5
    //6 7 8

    // 4-1 -n
    // 4-2 -n+1
    // 4-3 -1
    // 4-5 +1
    // 4-6 +n-1
    // 4-7 +n
          

    // Start is called before the first frame update
    void Start()
    {
        InitTiles();
    }

    // Update is called once per frame
    void Update()
    {

    }


    void InitTiles()
    {
        _tiles = new Tile[_iSize * _iSize];

        object[] temp = Resources.LoadAll("Tiles");
        _prefTiles = new GameObject[temp.Length];
        for ( int i  = 0; i < temp.Length; i ++)
        {
            _prefTiles[i] = temp[i] as GameObject;
        }

        

        for ( int i = 0; i < _iSize; i ++)
        {
            for (int j = 0; j < _iSize; j++)
            {
                int r = (int)Random.Range(0, _prefTiles.Length );
                Tile t0 = Instantiate<Tile>(_prefTiles[r].transform.GetComponent<Tile>());

                t0.transform.localScale = Vector3.one;

                t0.transform.parent = this.transform;
                Vector3 v = new Vector3( -2.0f + (4.0f/(_iSize-1)) * j, 0.2f - (4.0f / (_iSize-1)) * i, 0.0f);
                t0.transform.localPosition = v;
            }
        }
    }
}
