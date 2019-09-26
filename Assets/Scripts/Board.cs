 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DG.Tweening;

public class Board : MonoBehaviour
{
    int _iSize = 7;
    public const int TILE_MAX = 5;
    public Tile[] _tiles;

    GameObject _prefTile;
    
    const float _fTimeChangeTile = 0.2f;
    //0 1 2
    //3 4 5
    //6 7 8

    // 4-0 -n-1 
    // 4-1 -n
    // 4-3 -n+1 //안됌
    // 4-3 -1
    // 4-5 +1
    // 4-6 +n-1 //안됌
    // 4-7 +n
    // 4-8 +n+1 


    // Start is called before the first frame update
    void Start()
    {
        InitTiles();
    }

    void InitTiles()
    {
        int totalTilesCnt = _iSize * _iSize;
        _tiles = new Tile[totalTilesCnt];

        _prefTile = Resources.Load("Tiles/TilePrefab") as GameObject;
        

        // 강제로 스테이지를 설정합니다.
        // 대부분 상수처리로 하고 나중에 툴 값을 적용
        // 화면 사이즈가 6이라고 정


        // 안쓰는 타일 숨기기
        int[] locked = { 4, 5, 6, 12, 13 , 20, 21, 28, 29 ,35, 36, 37, 42 , 43 ,44, 45, 46, 47, 48 };
        for (int i = 0; i < locked.Length; i ++)
        {
            Tile t0 = Instantiate<Tile>(_prefTile.transform.GetComponent<Tile>());
            t0.transform.localScale = Vector3.one;
            t0.transform.parent = this.transform;
            t0._isLock = true;
            t0.gameObject.SetActive(false);
            _tiles[locked[i]] = t0;
        }

        // 초기 스테이즈 설정
        float fOneSize = 0.8f;
        float fSideStartX = - fOneSize * (_iSize-1) / 2.0f;
        
        int line = 0;
        for ( int i = 0; i < _iSize; i ++)
        {
            for (int j = 0; j < _iSize; j++)
            {
                int cur = line + j;
                if (_tiles[cur] == null )
                {
                    int r = GetSatrtTileIdx(i, j);
                    Tile t0 = Instantiate<Tile>(_prefTile.transform.GetComponent<Tile>());
                    t0.name = "tile : " + cur;
                    t0.SetIdx(r);

                    t0.transform.localScale = Vector3.one;

                    t0.transform.parent = this.transform;
                    float x = fSideStartX + fOneSize * j;
                    float y = fSideStartX - fOneSize * i + 3.0f;
                    y += fOneSize / 2 * j;
                    Vector3 v = new Vector3(x, y, 0.0f);
                    t0.transform.localPosition = Vector3.zero;
                    t0.transform.DOMove(v, 1f);
                    
                    _tiles[cur] = t0;
                    _tiles[cur]._pos = v;
                    _tiles[cur]._cur = cur;
                    _tiles[cur]._board = this;
                }
                else
                {
                    //Debug.Log(" out " + cur);
                }
                
            }

            line += _iSize;
        }


        // 새로 타일이 생성 되는 곳 지정
        int[] maker = { 3 };
        for ( int i = 0; i < maker.Length; i ++)
        {
            Tile makerTile = _tiles[maker[i]];
            makerTile._isMaker = true;
        }
        
    }

    int GetTileIdx(int i, int j)
    {

        if( i >=0  && i < _iSize)
        {
            if ( j >= 0 && j <_iSize)
            {
                int cur = i * _iSize + j;
                Tile t = _tiles[cur];
                if (t == null)
                    return -1;
                if ( t._isLock == true)
                {
                    return -1;
                }
                else
                {
                    return t._idx;
                }
            }
        }
        return -1;
    }

    // 게임 처음 시작 할 때 바로 터지는 경우가 없도록
    int GetSatrtTileIdx(int i, int j)
    {
        int cur = i * _iSize + j;

        int v0 = GetTileIdx(i - 1, j - 1);
        int v0_0 = GetTileIdx(i - 2, j - 2);

        int v1 = GetTileIdx(i - 1, j );
        int v1_1 = GetTileIdx(i - 2, j);
        
        int v3 = GetTileIdx(i , j - 1);
        int v3_3 = GetTileIdx(i, j - 2);

        //int v2 = GetTileIdx(i - 1, j + 1);
        //int v4 = GetTileIdx(i , j);
        //int v5 = GetTileIdx(i , j + 1);

        //int v6 = GetTileIdx(i + 1, j - 1);
        //int v7 = GetTileIdx(i + 1, j);
        //int v8 = GetTileIdx(i + 1, j + 1);
                
        List<int> usedIdx = new List<int>();

        //왼쪽 아래검사 4-3
        if (v3 >= 0 && v3_3 >= 0 && v3 == v3_3)
        {
            usedIdx.Add(v3);
        }
        //위 4-1
        if (v1 >= 0 && v1_1 >= 0 && v1 == v1_1)
        {
            if (usedIdx.Contains(v1) == false)
                usedIdx.Add(v1);
        }
        //왼쪽 위 4-0
        if (v0 >= 0 && v0_0 >= 0 && v0 == v0_0)
        {
            if (usedIdx.Contains(v0) == false)
                usedIdx.Add(v0);
        }
        //네모 검사
        if ( v0 >= 0 && v1 >= 0 && v3 >= 0)
        {
            if ( v0 == v1 && v1 == v3  )
            {
                if (usedIdx.Contains(v0) == false)
                    usedIdx.Add(v0);
            }
        }


        int r = GetRandomIdx();
        while (usedIdx.Contains(r) == true)
        {
            r = (r + 1) % TILE_MAX;
            
        }

        return r;
    }

    int GetRandomIdx()
    {
        return (int)Random.Range(0, TILE_MAX); 
    }

    public void Pang(int cur)
    {
        if (cur < 0)
            return;

        Tile t = _tiles[cur];

        t.transform.DOPunchScale(Vector3.one / 5, 0.4f).OnComplete(() =>
        {
            t.SetHide();
            MoveDownAfterPang(cur);
        });
    }

    void MoveDownAfterPang(int cur)
    {
        //0 1 2
        //3 4 5
        //6 7 8

        // 4-0 -n-1 
        // 4-1 -n
        // 4-3 -n+1 //안됌
        // 4-3 -1
        // 4-5 +1
        // 4-6 +n-1 //안됌
        // 4-7 +n
        // 4-8 +n+1

        int ii = cur / _iSize;
        int jj = cur % _iSize;

        Tile t = _tiles[cur];

        // 생성 가능한 타일인경우
        if ( t._isMaker == true)
        {
            int r = GetRandomIdx();
            t.SetIdx(r);
            Vector3 pos = t._pos;
            pos.y += 0.8f;
            t.transform.localPosition = pos;
            t.transform.DOMove(t._pos, 0.3f);
            return;
        }

        //1
        int v1 = GetTileIdx(ii - 1, jj);
        if ( v1 >= 0 )
        {
            int v1Cur = (ii - 1) * _iSize + jj;

            MoveTo(v1Cur, cur);

            Tile t1 = _tiles[v1Cur];
            t1.SetHide();
                       
            MoveDownAfterPang(v1Cur);
            return;
        }

        //0
        int v0 = GetTileIdx(ii - 1, jj - 1);
        if (v0 >= 0)
        {
            int v0Cur = (ii - 1) * _iSize + jj - 1;

            MoveTo(v0Cur, cur);

            Tile t1 = _tiles[v0Cur];
            t1.SetHide();

            MoveDownAfterPang(v0Cur);
            return;
        }

        //5
        int v5 = GetTileIdx(ii , jj + 1);
        if (v5 >= 0)
        {
            int v5Cur = (ii) * _iSize + jj + 1;

            MoveTo(v5Cur, cur);

            Tile t1 = _tiles[v5Cur];
            t1.SetHide();

            MoveDownAfterPang(v5Cur);
            return;
        }
    }

    void MoveTo(int from, int to)
    {
        Tile f = _tiles[from];
        Tile t = _tiles[to];

        t.SetIdx(f._idx);
        t.transform.localPosition = f.transform.localPosition;
        t.transform.DOMove(t._pos, 0.3f);
    }

    //------------터치 작업들--------------
    int _touchDownCur = -1;
    int _touchEnterCur = -1;
    int _touchUpCur = -1;

    int _iFirstTileCur = -1;
    int _iSecondTileCur = -1;

    List<int> _listMoveTiles = new List<int>();

    //터치
    public void OnTouchDown(int cur)
    {
        _touchEnterCur = -1;
        _touchUpCur = -1;
        _touchDownCur = cur;

        
        
    }

    public void OnTouchUp(int cur)
    {
        
        _touchDownCur = -1;
        _touchEnterCur = -1;

        _touchUpCur = cur;

    }

    public void OnTouchEnter(int cur)
    {
        if (_touchDownCur < 0)
            return;

        if (_iSecondTileCur >= 0)
            return;

        if (_touchEnterCur == cur)
            return;

        //Debug.Log("OnMouseEnter : " + cur);

        _touchEnterCur = cur;

    }

    public void OnTouchExit(int cur)
    {
        if (_touchDownCur < 0)
            return;
        if (_touchDownCur == cur)
            return;

        

        //Debug.Log("OnMouseExit : " + cur);

    }

    // Update is called once per frame
    void Update()
    {
        //터치 시작
        if ( _touchDownCur >= 0 )
        {
            Tile t = _tiles[_touchDownCur];
            
            t.SetSelect(true);

            _iFirstTileCur = _touchDownCur;
            _iSecondTileCur = -1;
        }
        //드래그 중
        if (_touchEnterCur >= 0 )
        {
            if ( IsSide(_iFirstTileCur, _touchEnterCur) == true )
            {
                //Debug.Log("---- " + _touchEnterCur);
                Tile t = _tiles[_touchEnterCur];
                t.SetSelect(true);
                
                SwapTiles(_iFirstTileCur, _touchEnterCur);

                

                _iSecondTileCur = _touchEnterCur;
                _touchEnterCur = -1;
                _touchDownCur = -1;
            }
        }
        //드래그 끝
        if ( _touchUpCur >= 0 )
        {
            if ( _iFirstTileCur >= 0 )
            {
                Tile t = _tiles[_iFirstTileCur];

                t.SetSelect(false);

                _iFirstTileCur = -1;
            }

            if (_iSecondTileCur >= 0)
            {
                Tile t = _tiles[_iSecondTileCur];
                t.SetSelect(false);
                _iSecondTileCur = -1;
            }

            _touchUpCur = -1;
        }
    }

    // 이동 가능한 위치인지 검사 
    public bool IsSide(int from, int to)
    {
        int dif = to - from;


        if (dif == -_iSize - 1)
            return true;
        if (dif == -_iSize)
            return true;
        if (dif == -1)
            return true;
        if (dif == 1)
            return true;
        if (dif == _iSize)
            return true;
        if (dif == _iSize + 1)
            return true;

        return false;
    }

    public void SwapTiles(int cur1, int cur2)
    {
        Tile t = _tiles[cur1];
        Tile f = _tiles[cur2];

        t.transform.DOMove(f._pos, _fTimeChangeTile).OnComplete(() =>
        {

        });
        f.transform.DOMove(t._pos, _fTimeChangeTile).OnComplete(() =>
        {
            //t.transform.DOMove(t._pos, _fTimeChangeTile);
            //f.transform.DOMove(f._pos, _fTimeChangeTile);

            t.SetSelect(false);
            f.SetSelect(false);

            int temp = t._idx;
            t.SetIdx(f._idx);
            t.transform.localPosition = t._pos;
            f.SetIdx(temp);
            f.transform.localPosition = f._pos;


            // 교체 후 팡를 것 찾아 검사
            _listMoveTiles.Clear();
            _listMoveTiles.Add(cur1);
            _listMoveTiles.Add(cur2);

            int result = FindPangFromList();
            Debug.Log(" pang cnt : " + result);
            if (result == 0)
            {
                //바뀐 내용이 없으면 다시 타일 복구
                t.transform.DOMove(f._pos, _fTimeChangeTile);
                f.transform.DOMove(t._pos, _fTimeChangeTile).OnComplete(() =>
                {
                    temp = t._idx;
                    t.SetIdx(f._idx);
                    t.transform.localPosition = t._pos;
                    f.SetIdx(temp);
                    f.transform.localPosition = f._pos;
                });
            }
        });
    }

    //----------검색-------
    int FindPangFromList()
    {
        int result = 0;
        while ( _listMoveTiles.Count > 0)
        {
            int cur = _listMoveTiles[0];
            _listMoveTiles.Remove(cur);

            result += FindPang(cur);
        }
        return result;
    }

    int FindPang(int cur)
    {
        int curV = _tiles[cur]._idx;

        int i = cur / _iSize;
        int j = cur % _iSize;

        int v0 = GetTileIdx(i - 1, j - 1);
        int v0_0 = GetTileIdx(i - 2, j - 2);

        int v1 = GetTileIdx(i - 1, j);
        int v1_1 = GetTileIdx(i - 2, j);

        int v3 = GetTileIdx(i, j - 1);
        int v3_3 = GetTileIdx(i, j - 2);

        //int v2 = GetTileIdx(i - 1, j + 1);
        //int v4 = GetTileIdx(i , j);
        //int v5 = GetTileIdx(i , j + 1);

        //int v6 = GetTileIdx(i + 1, j - 1);
        //int v7 = GetTileIdx(i + 1, j);
        //int v8 = GetTileIdx(i + 1, j + 1);

        int result = 0;

        result += FindTile_0(curV, i, j, 1);
        result += FindTile_1(curV, i, j, 1);
        result += FindTile_3(curV, i, j, 1);
        result += FindTile_5(curV, i, j, 1);
        result += FindTile_7(curV, i, j, 1);
        result += FindTile_8(curV, i, j, 1);

        return result;
    }

    int FindTile_0(int idx, int i, int j, int cnt)
    {
        i --;
        j --;
        int v0 = GetTileIdx(i, j);
        if (v0 >= 0 && v0 == idx )
        {
            cnt++;
            return FindTile_0(idx, i, j, cnt);
        }
        if ( cnt > 2)
            return cnt;
        return 0;
    }

    int FindTile_1(int idx, int i, int j, int cnt)
    {
        i--;
        
        int v0 = GetTileIdx(i, j);
        if (v0 >= 0 && v0 == idx)
        {
            cnt++;
            return FindTile_1(idx, i, j, cnt);
        }
        if (cnt > 2)
            return cnt;
        return 0;
    }

    int FindTile_3(int idx, int i, int j, int cnt)
    {
        j--;

        int v0 = GetTileIdx(i, j);
        if (v0 >= 0 && v0 == idx)
        {
            cnt++;
            return FindTile_3(idx, i, j, cnt);
        }
        if (cnt > 2)
            return cnt;
        return 0;
    }

    int FindTile_5(int idx, int i, int j, int cnt)
    {
        j++;

        int v0 = GetTileIdx(i, j);
        if (v0 >= 0 && v0 == idx)
        {
            cnt++;
            return FindTile_5(idx, i, j, cnt);
        }
        if (cnt > 2)
            return cnt;
        return 0;
    }

    int FindTile_7(int idx, int i, int j, int cnt)
    {
        i++;

        int v0 = GetTileIdx(i, j);
        if (v0 >= 0 && v0 == idx)
        {
            cnt++;
            return FindTile_7(idx, i, j, cnt);
        }
        if (cnt > 2)
            return cnt;
        return 0;
    }
    int FindTile_8(int idx, int i, int j, int cnt)
    {
        i++;
        j++;
        int v0 = GetTileIdx(i, j);
        if (v0 >= 0 && v0 == idx)
        {
            cnt++;
            return FindTile_8(idx, i, j, cnt);
        }
        if (cnt > 2)
            return cnt;
        return 0;
    }
}
