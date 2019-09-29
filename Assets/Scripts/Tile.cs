using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DG.Tweening;

public class Tile : MonoBehaviour
{
        
    public Transform[] _obj;

    //0부터 4까진 일반티일
    //5는 팽이

    public enum TILE_IDX
    {
        NORMAL_0 = 0,
        NORMAL_1 ,
        NORMAL_2 ,
        NORMAL_3 ,
        NORMAL_4 ,
        TOP , //팽이

    };
    
    public int _idx = 0; 


    public int _cur = 0; //배열 위치
    public Vector3 _pos = Vector3.zero; //원래 위치
    public bool _isLock = false; // 잠깐 이동중 락 걸기
    public bool _isPang = false; //터지는 타일
    public bool _isMaker = false; //새로운 타일 나오는 곳
    public bool _isSelected = false; //선택됨
    public bool _isTop = false; //팽이
    public int _iLife = 1;

    public Board _board;

    private void Update()
    {
        if ( _isTop == true)
        {
            if ( _iLife == 1)
            {
                transform.Rotate(Vector3.forward, 1.1f);
            }
        }
    }

    public void SetIdx(int idx)
    {        
        for ( int i = 0; i < _obj.Length; i++)
        {
            if (i == idx)
                _obj[i].gameObject.SetActive(true);
            else
                _obj[i].gameObject.SetActive(false);
        }

        if (idx == (int)TILE_IDX.TOP)
        {
            _isTop = true;
            _iLife = 2;  
        }
        else
        {
            _isTop = false;
        }

        transform.localScale = Vector3.one;
        transform.localRotation = Quaternion.identity;
        //if (idx >= 0)
        //  _isPang = false;

        _idx = idx;
    }

    public void Copy(Tile from)
    {
        Tile f = from;
        Tile t = this;

        switch ( (TILE_IDX)from._idx)
        {
            case TILE_IDX.NORMAL_0:
            case TILE_IDX.NORMAL_1:
            case TILE_IDX.NORMAL_2:
            case TILE_IDX.NORMAL_3:
            case TILE_IDX.NORMAL_4:
                {
                    int newIdx = f._idx;

                    f.SetHide();
                    t.SetIdx(newIdx);
                    t._isLock = true;

                    //t.transform.localPosition = f._pos;
                    t.transform.localPosition = f.transform.localPosition;
                    t.transform.DOLocalMove(t._pos, Board._fTimeMoveTile)
                        .OnComplete(() =>
                        {
                            t._isPang = false;
                            t._isLock = false;
                            _board.AddMoveTile(t._cur);
                        });
                }
                break;
            case TILE_IDX.TOP:
                {
                    int newIdx = f._idx;
                    int life = f._iLife;
                    Quaternion q = f.transform.localRotation;

                    f.SetHide();
                    t._isLock = true;

                    t.SetIdx(newIdx);
                    t._iLife = life;
                    t.transform.localRotation = q;
                    t.transform.localPosition = f.transform.localPosition;
                    t.transform.DOLocalMove(t._pos, Board._fTimeMoveTile)
                        .OnComplete(() =>
                        {
                            t._isPang = false;
                            t._isLock = false;
                            _board.AddMoveTile(t._cur);
                        });
                }
                break;
        }
    }

    public void SetHide()
    {
        SetIdx(-1);
        transform.localScale = Vector3.one;
        transform.localRotation = Quaternion.identity;
        _isPang = true;
    }

    public void Damaged()
    {
        if(_isTop == true)
        {
            _iLife--;
            if (_iLife <= 0)
            {               
                _board.AddWaitingTile(_cur);
                

                _board.StartEffs(this, _idx);

                SetIdx(-1);
            }
        }
    }

    private void OnMouseDown()
    {
        if (_idx < 0)
            return;

        //Debug.Log("OnMouseDown : " + _cur);

        //_board.Pang(_cur);
        _board.OnTouchDown(_cur);

    }

    private void OnMouseUp()
    {
        if (_idx < 0)
            return;

        //Debug.Log("OnMouseUp : " + _cur);
        _board.OnTouchUp(_cur);
    }

    private void OnMouseExit()
    {
        if (_idx < 0)
            return;

        
        _board.OnTouchExit(_cur);
    }

    private void OnMouseEnter()
    {
        if (_idx < 0)
            return;

        
        _board.OnTouchEnter(_cur);
    }

    public void SetSelect(bool b)
    {
        if (b == _isSelected)
            return;

        _isSelected = b;

        if ( _isSelected == true)
        {
            transform.DOScale(Vector3.one * 1.4f, 0.2f);
        }
        else
        {
            transform.DOScale(Vector3.one, 0.2f);
        }
    }
}
