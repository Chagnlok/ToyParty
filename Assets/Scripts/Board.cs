using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    public int _iSize = 3;
    public Tile[] _tiles;

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

        GameObject prefTile = Resources.Load("Tiles/Fruit_Blue_1") as GameObject;
        Tile t0 = Instantiate<Tile>(prefTile.transform.GetComponent<Tile>());

        t0.transform.localScale = Vector3.one;

        t0.transform.parent = this.transform;
        t0.transform.localPosition = Vector3.zero;
    }
}
