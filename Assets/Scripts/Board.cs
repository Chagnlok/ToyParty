 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DG.Tweening;

public class Board : MonoBehaviour
{
    //보드사이즈
    int _iSize = 7;
    //타일 종류
    public const int TILE_MAX = 5;
    //보드에 깔릴 타일
    public Tile[] _tiles;

    //팡 효과
    public PangEff[] _aniPangEffs;

    GameObject _prefTile;

    public static float _fTimeChangeTile = 0.2f;
    public static float _fTimeMoveTile = 0.15f;
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


        float fOneSize = 0.8f;
        float fSideStartX = -fOneSize * (_iSize - 1) / 2.0f;

        //팽이 위치 잡기
        int[] tops = { 1, 2, 11, 19, 22, 24, 30, 38, 39, 40  };
        for (int k = 0; k < tops.Length; k ++)
        {
            int cur = tops[k];
            if (_tiles[cur] == null )
            {
                int i = cur / _iSize;
                int j = cur % _iSize;
                int r = (int)Tile.TILE_IDX.TOP;
                Tile t0 = Instantiate<Tile>(_prefTile.transform.GetComponent<Tile>());
                t0.name = "Top : " + cur;
                t0.SetIdx(r);

                t0.transform.localScale = Vector3.one;

                t0.transform.parent = this.transform;
                float x = fSideStartX + fOneSize * j;
                float y = fSideStartX - fOneSize * i + 3.0f;
                y += fOneSize / 2 * j;
                Vector3 v = new Vector3(x, y, 0.0f);
                t0.transform.localPosition = Vector3.zero;
                t0.transform.DOLocalMove(v, 1f);

                _tiles[cur] = t0;
                _tiles[cur]._pos = v;
                _tiles[cur]._cur = cur;
                _tiles[cur]._board = this;
            }
            
        }

        // 초기 스테이즈 설정
        
        
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
                    t0.transform.DOLocalMove(v, 0.5f);
                    
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

    public void StartEffs(Tile t, int idx)
    {
        GameObject go = Instantiate(_aniPangEffs[0].gameObject) as GameObject;
        go.transform.parent = this.transform;
        go.transform.localScale = Vector3.one;
        go.transform.position = t.transform.position;

        PangEff ef = go.GetComponent<PangEff>();

        ef.SetIdx(idx);
        ef.StartAni();
    }

    //타일 터트리기
    public void Pang(int cur)
    {
        if (cur < 0)
            return;

        Tile t = _tiles[cur];

        int idx = t._idx;

        t.SetHide();

        StartEffs(t, idx);
        

        //근처 특수 타일 검색
        int i = cur / _iSize;
        int j = cur % _iSize;
        int s0 = GetTileIdx(i - 1, j - 1);
        int s1 = GetTileIdx(i - 1, j);
        //int s2 = GetTileIdx(i - 1, j + 1);
        int s3 = GetTileIdx(i, j - 1);
        int s5 = GetTileIdx(i, j + 1);
        //int s6 = GetTileIdx(i + 1, j - 1);
        int s7 = GetTileIdx(i + 1, j);
        int s8 = GetTileIdx(i + 1, j + 1);

        if (s0 == (int)Tile.TILE_IDX.TOP)
        {
            AddTopTile(cur - _iSize - 1);
        }
        if (s1 == (int)Tile.TILE_IDX.TOP)
        {
            AddTopTile(cur - _iSize);
        }
        if (s3 == (int)Tile.TILE_IDX.TOP)
        {
            AddTopTile(cur - 1);
        }
        if (s5 == (int)Tile.TILE_IDX.TOP)
        {
            AddTopTile(cur + 1);
        }
        if (s7 == (int)Tile.TILE_IDX.TOP)
        {
            AddTopTile(cur + _iSize);
        }
        if (s8 == (int)Tile.TILE_IDX.TOP)
        {
            AddTopTile(cur + _iSize + 1);
        }
    }

    public void MoveDownAfterPang(int cur)
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
        if (t._isMaker == true)
        {
            int r = GetRandomIdx();
            t.SetIdx(r);
            t._isLock = true;
            Vector3 pos = t._pos;
            pos.y += 0.5f;
            t.transform.localPosition = pos;
            t.transform.DOLocalMove(t._pos, _fTimeMoveTile).OnComplete(() =>
            {
                t._isPang = false;
                t._isLock = false;
                AddMoveTile(t._cur);
            });
            return;
        }

        //위에 타일이 비어있는 경우 찰때까지 기다린다
        if ( ii - 1 >= 0 )
        {
            Tile upTile = _tiles[cur - _iSize];
            if (upTile._isPang == true)
            {
                AddWaitingTile(cur);
                return;
            }
        }

        //1
        int v1 = GetTileIdx(ii - 1, jj);
        if ( v1 >= 0 )
        {
            int v1Cur = (ii - 1) * _iSize + jj;

            MoveTo(v1Cur, cur);

            
            return;
        }

        //0
        int v0 = GetTileIdx(ii - 1, jj - 1);
        if (v0 >= 0)
        {
            int v0Cur = (ii - 1) * _iSize + jj - 1;

            MoveTo(v0Cur, cur);

            
            return;
        }

        //5
        int v5 = GetTileIdx(ii , jj + 1);
        if (v5 >= 0)
        {
            int v5Cur = (ii) * _iSize + jj + 1;

            MoveTo(v5Cur, cur);

           
            return;
        }

        AddWaitingTile(cur);
    }

    void MoveTo(int from, int to)
    {
        Tile f = _tiles[from];
        Tile t = _tiles[to];

        t.Copy(f);

        MoveDownAfterPang(from);


        //Tile t1 = _tiles[from];
        //t1.SetHide();
    }

    //------------터치 작업들--------------
    int _touchDownCur = -1;
    int _touchEnterCur = -1;
    int _touchUpCur = -1;

    int _iFirstTileCur = -1;
    int _iSecondTileCur = -1;

    //움직이는 타일
    List<int> _listMoveTiles = new List<int>();
    //터트리는 타일
    List<int> _listPangTiles = new List<int>();
    //대기중인 타일
    List<int> _listWaitingTiles = new List<int>();
    //팽이 특수 타일
    List<int> _listTopTiles = new List<int>();


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

        //타일 터트리기
        while (_listPangTiles.Count > 0)
        {
            List<int> listTemp = new List<int>();
            foreach (int one in _listPangTiles)
            {
                listTemp.Add(one);
            }
            _listPangTiles.Clear();

            listTemp.Sort(CompareDepts);

            foreach (int one in listTemp)
            {
                Pang(one);
                AddWaitingTile(one);
            }

        }

        //특수 타일 - 팽이
        if ( _listTopTiles.Count > 0)
        {
            foreach (int one in _listTopTiles)
            {
                _tiles[one].Damaged();
            }
            _listTopTiles.Clear();
        }
            
        if (_listWaitingTiles.Count > 0)
        {
            List<int> listTemp = new List<int>();
            foreach (int one in _listWaitingTiles)
            {
                listTemp.Add(one);
            }
            _listWaitingTiles.Clear();

            listTemp.Sort(CompareDepts);
            foreach (int one in listTemp)
            {
                MoveDownAfterPang(one);
            }
          
        }

        if (_listWaitingTiles.Count < 1)
        {
            FindPangFromList();
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

    public int CompareDepts(int a, int b)
    {
        int i = a / _iSize;
        int j = a % _iSize;
        int a1 = i * (-10) + j * 5;

        int ii = b / _iSize;
        int jj = b % _iSize;
        int a2 = ii * (-10) + jj * 5;

        if ( a1 > a2 )
        { 
            return  1;
        }
        else if ( a1 < a2 )
        {
            return -1;
        }
        else
        {
            return 0;
        }        
    }

    public void SwapTiles(int cur1, int cur2)
    {
        Tile t = _tiles[cur1];
        Tile f = _tiles[cur2];

        t.transform.DOLocalMove(f._pos, _fTimeChangeTile).OnComplete(() =>
        {

        });
        f.transform.DOLocalMove(t._pos, _fTimeChangeTile).OnComplete(() =>
        {

            t.SetSelect(false);
            f.SetSelect(false);

            int temp = t._idx;
            t.SetIdx(f._idx);
            t.transform.localPosition = t._pos;
            f.SetIdx(temp);
            f.transform.localPosition = f._pos;


            // 교체 후 팡를 것 찾아 검사
            ClearMoveTiles();
            AddMoveTile(cur1);
            AddMoveTile(cur2);
            

            _listPangTiles.Clear();

            int result = FindPangFromList();
            Debug.Log(" pang cnt : " + result);
            if (result == 0)
            {
                //바뀐 내용이 없으면 다시 타일 복구
                t.transform.DOLocalMove(f._pos, _fTimeChangeTile);
                f.transform.DOLocalMove(t._pos, _fTimeChangeTile).OnComplete(() =>
                {
                    temp = t._idx;
                    t.SetIdx(f._idx);
                    t.transform.localPosition = t._pos;
                    f.SetIdx(temp);
                    f.transform.localPosition = f._pos;
                });
            }
            else
            {
                //팡이 있으면 팡처리
                //string s = "---";
                //for ( int i = 0; i < _listPangTiles.Count; i ++)
                //{
                //    int v = _listPangTiles[i];
                //    s += v + " ";
                //}
                //Debug.Log(s);
                
            }
        });
    }

    // 보드 전체를 검색하는 것이 아니라 움직은 타일만 검색을 함
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

    int FindPangFromList_Debug()
    {
        int cnt = _listMoveTiles.Count;

        int result = 0;
        string s = "";
        while (_listMoveTiles.Count > 0)
        {
            int cur = _listMoveTiles[0];

            s += cur + ", ";
            _listMoveTiles.Remove(cur);

            result += FindPang(cur);
        }

        if (cnt != 0)
        {
            Debug.Log(_listPangTiles.Count + " : " + s);
        }

        return result;
    }

    int FindPang(int cur)
    {
        if (_tiles[cur]._isTop == true)
        {
            //팽이라면 검색할 필요가 없음
            return 0;
        }

        int curV = _tiles[cur]._idx;

        int i = cur / _iSize;
        int j = cur % _iSize;
       
        int result = 0;

        //한쪽 방향으로 검색 3개
        result += FindTile_0(curV, i, j, 1, 3);
        result += FindTile_1(curV, i, j, 1, 3);
        result += FindTile_3(curV, i, j, 1, 3);
        result += FindTile_5(curV, i, j, 1, 3);
        result += FindTile_7(curV, i, j, 1, 3);
        result += FindTile_8(curV, i, j, 1, 3);

        //양쪽 방향 3개
        {
            List<int> listTemp = new List<int>();
            int s0 = FindTile_0(curV, i, j, 1, 2, listTemp);
            int s8 = FindTile_8(curV, i, j, 1, 2, listTemp);
            if ( s0 + s8 >= 4)
            {
                result += s0 + s8;
                foreach(int one in listTemp)
                {
                    AddPangTile(one);
                }
            }

            listTemp.Clear();
            int s1 = FindTile_1(curV, i, j, 1, 2, listTemp);
            int s7 = FindTile_7(curV, i, j, 1, 2, listTemp);
            if (s1 + s7 >= 4)
            {
                result += s1 + s7;
                foreach (int one in listTemp)
                {
                    AddPangTile(one);
                }
            }

            listTemp.Clear();
            int s3 = FindTile_3(curV, i, j, 1, 2, listTemp);
            int s5 = FindTile_5(curV, i, j, 1, 2, listTemp);
            if (s3 + s5 >= 4)
            {
                result += s3 + s5;
                foreach (int one in listTemp)
                {
                    AddPangTile(one);
                }
            }
        }
        //주변 검색 4개
        {
            List<int> listTemp = new List<int>();
            int sameCnt = 0;
            int s0 = GetTileIdx(i - 1, j - 1);
            int s1 = GetTileIdx(i - 1, j);
            int s2 = GetTileIdx(i - 1, j + 1);
            int s3 = GetTileIdx(i , j - 1);
            int s5 = GetTileIdx(i , j + 1);
            int s6 = GetTileIdx(i + 1, j - 1);
            int s7 = GetTileIdx(i + 1, j );
            int s8 = GetTileIdx(i + 1, j + 1);
            if (s0 == curV)
            {
                sameCnt++;
                listTemp.Add(cur - _iSize - 1);
            }
            if (s1 == curV)
            {
                sameCnt++;
                listTemp.Add(cur - _iSize);
            }
            if (s2 == curV)
            {
                sameCnt++;
                listTemp.Add(cur - _iSize + 1);
            }
            if (s3 == curV)
            {
                sameCnt++;
                listTemp.Add(cur - 1);
            }
            if (s5 == curV)
            {
                sameCnt++;
                listTemp.Add(cur + 1);
            }
            if (s6 == curV)
            {
                sameCnt++;
                listTemp.Add(cur + _iSize - 1);
            }
            if (s7 == curV)
            {
                sameCnt++;
                listTemp.Add(cur + _iSize);
            }
            if (s8 == curV)
            {
                sameCnt++;
                listTemp.Add(cur + _iSize + 1);
            }
            if (sameCnt >= 3)
            {
                result += sameCnt + 1;
                foreach (int one in listTemp)
                {
                    AddPangTile(one);
                }
            }
            else
            {
                // 0부터 8까지 못 찾았을경우 추가 검사
                if (s0 == curV)
                {
                    if (s1 == curV)
                    {
                        int s0_1 = GetTileIdx(i - 2, j - 1);
                        if (s0_1 == curV)
                        {
                            AddPangTile(cur - _iSize - 1);
                            AddPangTile(cur - _iSize);
                            AddPangTile(cur - _iSize * 2 - 1);
                            result += 4;
                        }
                    }
                    else if (s3 == curV)
                    {
                        int s3_0 = GetTileIdx(i - 1, j - 2);
                        if (s3_0 == curV)
                        {
                            AddPangTile(cur - _iSize - 1);
                            AddPangTile(cur - 1);
                            AddPangTile(cur - _iSize - 2);
                            result += 4;
                        }
                    }
                }
                if(s8 == curV)
                {
                    if (s5 == curV)
                    {
                        int s8_5 = GetTileIdx(i + 1, j + 2);
                        if (s8_5 == curV)
                        {
                            AddPangTile(cur + _iSize + 1);
                            AddPangTile(cur + 1);
                            AddPangTile(cur + _iSize + 2);
                            result += 4;
                        }
                    }
                    else if(s7 == curV)
                    {
                        int s8_7 = GetTileIdx(i + 2, j + 1);
                        if (s8_7 == curV)
                        {
                            AddPangTile(cur + _iSize + 1);
                            AddPangTile(cur + _iSize);
                            AddPangTile(cur + _iSize * 2 + 1);
                            result += 4;
                        }
                    }
                }
            }

        }

                

        if ( result > 2 )
        {
            AddPangTile(cur);
        }

        return result;
    }

    int FindTile_0(int idx, int i, int j, int cnt , int needCnt, List<int> listTemp = null)
    {
        i --;
        j --;
        int v0 = GetTileIdx(i, j);
        if (v0 >= 0 && v0 == idx )
        {
            cnt++;
            int result = FindTile_0(idx, i, j, cnt, needCnt);
            if ( result >= needCnt )
            {
                int cur = i * _iSize + j;
                if( listTemp != null)
                {
                    if (listTemp.Contains(cur) == false)
                        listTemp.Add(cur);
                }
                else
                {
                    AddPangTile(cur);
                }
                
            }
            return result;
        }
        if ( cnt >= needCnt )
            return cnt;
        return 0;
    }

    int FindTile_1(int idx, int i, int j, int cnt, int needCnt, List<int> listTemp = null)
    {
        i--;
        
        int v0 = GetTileIdx(i, j);
        if (v0 >= 0 && v0 == idx)
        {
            cnt++;
            int result = FindTile_1(idx, i, j, cnt, needCnt);
            if (result >= needCnt)
            {
                int cur = i * _iSize + j;
                if (listTemp != null)
                {
                    if (listTemp.Contains(cur) == false)
                        listTemp.Add(cur);
                }
                else
                {
                    AddPangTile(cur);
                }
            }
            return result;
        }
        if (cnt >= needCnt)
            return cnt;
        return 0;
    }

    int FindTile_3(int idx, int i, int j, int cnt, int needCnt, List<int> listTemp = null)
{
        j--;

        int v0 = GetTileIdx(i, j);
        if (v0 >= 0 && v0 == idx)
        {
            cnt++;
            int result = FindTile_3(idx, i, j, cnt, needCnt);
            if (result >= needCnt)
            {
                int cur = i * _iSize + j;
                if (listTemp != null)
                {
                    if (listTemp.Contains(cur) == false)
                        listTemp.Add(cur);
                }
                else
                {
                    AddPangTile(cur);
                }
            }
            return result;
        }
        if (cnt >= needCnt)
            return cnt;
        return 0;
    }

    int FindTile_5(int idx, int i, int j, int cnt, int needCnt, List<int> listTemp = null)
{
        j++;

        int v0 = GetTileIdx(i, j);
        if (v0 >= 0 && v0 == idx)
        {
            cnt++;
            int result = FindTile_5(idx, i, j, cnt, needCnt);
            if (result >= needCnt)
            {
                int cur = i * _iSize + j;
                if (listTemp != null)
                {
                    if (listTemp.Contains(cur) == false)
                        listTemp.Add(cur);
                }
                else
                {
                    AddPangTile(cur);
                }
            }
            return result;
        }
        if (cnt >= needCnt)
            return cnt;
        return 0;
    }

    int FindTile_7(int idx, int i, int j, int cnt, int needCnt, List<int> listTemp = null)
{
        i++;

        int v0 = GetTileIdx(i, j);
        if (v0 >= 0 && v0 == idx)
        {
            cnt++;
            int result = FindTile_7(idx, i, j, cnt, needCnt);
            if (result >= needCnt)
            {
                int cur = i * _iSize + j;
                if (listTemp != null)
                {
                    if (listTemp.Contains(cur) == false)
                        listTemp.Add(cur);
                }
                else
                {
                    AddPangTile(cur);
                }
            }
            return result;
        }
        if (cnt >= needCnt)
            return cnt;
        return 0;
    }
    int FindTile_8(int idx, int i, int j, int cnt, int needCnt, List<int> listTemp = null)
{
        i++;
        j++;
        int v0 = GetTileIdx(i, j);
        if (v0 >= 0 && v0 == idx)
        {
            cnt++;
            int result = FindTile_8(idx, i, j, cnt, needCnt);
            if (result >= needCnt)
            {
                int cur = i * _iSize + j;
                if (listTemp != null)
                {
                    if (listTemp.Contains(cur) == false)
                        listTemp.Add(cur);
                }
                else
                {
                    AddPangTile(cur);
                }
            }
            return result;
        }
        if (cnt >= needCnt)
            return cnt;
        return 0;
    }

    void ClearMoveTiles()
    {
        _listMoveTiles.Clear();
    }
    public void AddMoveTile(int cur)
    {
        if (_listMoveTiles.Contains(cur) == false)
            _listMoveTiles.Add(cur);
    }

    public void AddPangTile(int cur )
    {
        Debug.Log("add pang tile : " + cur);
        if (_listPangTiles.Contains(cur) == false)
            _listPangTiles.Add(cur);
    }

    void AddTopTile(int cur)
    {
        if (_listTopTiles.Contains(cur) == false)
            _listTopTiles.Add(cur);
    }
    public void AddWaitingTile(int cur)
    {
        if (_listWaitingTiles.Contains(cur) == false)
            _listWaitingTiles.Add(cur);
    }
}
