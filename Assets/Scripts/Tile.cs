using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DG.Tweening;

public class Tile : MonoBehaviour
{
    public Transform[] _obj;

    public int _idx = 0;
    public int _cur = 0;
    public Vector3 _pos = Vector3.zero;
    public bool _isLock = false;
    public bool _isPang = false;
    public bool _isMaker = false;

    public Board _board;


    // Start is called before the first frame update


    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetIdx(int idx)
    {
        _idx = idx;
        for ( int i = 0; i < _obj.Length; i++)
        {
            if (i == idx)
                _obj[i].gameObject.SetActive(true);
            else
                _obj[i].gameObject.SetActive(false);
        }
    }

    public void SetHide()
    {
        SetIdx(-1);
        _isPang = true;
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

        //Debug.Log("OnMouseExit : " + _cur);
        _board.OnTouchExit(_cur);
    }

    private void OnMouseEnter()
    {
        if (_idx < 0)
            return;

        //Debug.Log("OnMouseEnter : " + _cur);
        _board.OnTouchEnter(_cur);
    }


}
