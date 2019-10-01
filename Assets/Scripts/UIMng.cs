using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIMng : MonoBehaviour
{
    public Text _textQuestCnt;
    public Text _textMove;

    public GameObject _objInGameMenu;
    public GameObject _objLogin;
    public GameObject _objEndUI;

    public Board _board;

    public Text _textResult;


    private void Start()
    {
        _objLogin.SetActive(true);
        _objInGameMenu.SetActive(false);
        _board.gameObject.SetActive(false);
        _objEndUI.SetActive(false);
    }


    public void SetQuestCnt (int n )
    {
        _textQuestCnt.text = "" + n;
    }

    public void SetMoveCnt(int n)
    {
        _textMove.text = "" + n;
    }

    public void SetEndUI(bool result)
    {        
        if ( result == true )
        {
            _textResult.text = "Clear !!";
        }
        else
        {
            _textResult.text = "Fail !!";
        }
        _objEndUI.SetActive(true);
        _objInGameMenu.SetActive(false);
    }

    public void OnClickFaceBook()
    {

    }
    public void OnClickGuest()
    {
        _objLogin.SetActive(false);
        _objInGameMenu.SetActive(true);
        _board.gameObject.SetActive(true);

        _board.InitTiles();
    }
    public void OnClickEndUI_OK()
    {
        _objLogin.SetActive(true);
        _objInGameMenu.SetActive(false);
        _objEndUI.SetActive(false);
        _board.gameObject.SetActive(false);
    }
    public void OnClickInGame_Cancel()
    {
        _objLogin.SetActive(true);
        _objInGameMenu.SetActive(false);
        _objEndUI.SetActive(false);
        _board.gameObject.SetActive(false);
    }
}
