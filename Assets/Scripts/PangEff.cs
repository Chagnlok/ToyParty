using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DG.Tweening;

public class PangEff : MonoBehaviour
{
    public GameObject[] _goAni;
    public int _idx = 0;

    public void SetIdx(int idx = 0)
    {
        for ( int i = 0; i < _goAni.Length; i ++)
        {
            if ( idx == i )
            {
                _goAni[i].SetActive(true);
            }
            else
            {
                _goAni[i].SetActive(false);
            }
        }
        _idx = idx;
    }
    

    public void StartAni()
    {
        switch ((Tile.TILE_IDX)_idx)
        {
            case Tile.TILE_IDX.NORMAL_0:
            case Tile.TILE_IDX.NORMAL_1:
            case Tile.TILE_IDX.NORMAL_2:
            case Tile.TILE_IDX.NORMAL_3:
            case Tile.TILE_IDX.NORMAL_4:
                {
                    transform.DOScale(Vector3.zero, 1.0f).OnComplete(() =>
                    {
                        DestroyImmediate(this.gameObject);
                    });
                }
                break;
            case Tile.TILE_IDX.TOP:
                {
                    transform.DOMove(new Vector3(-2.3f, 3.4f, 0), 0.6f).SetEase(Ease.InOutBack).OnComplete(()=>
                    {
                        DestroyImmediate(this.gameObject);
                    });
                }
                break;
        }
                
    }

}
