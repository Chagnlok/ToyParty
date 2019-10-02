using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Facebook.Unity;

/*

    keytool -exportcert -alias toyparty -keystore /projects/ToyParty/ToyParty/android.keystore | openssl sha1 -binary | openssl base64
 
 */

public class UIMng : MonoBehaviour
{
    public Text _textQuestCnt;
    public Text _textMove;

    public GameObject _objInGameMenu;
    public GameObject _objLogin;
    public GameObject _objEndUI;

    public Board _board;

    public Text _textResult;
    public Text _user;

    void Awake()
    {
        if (!FB.IsInitialized)
        {
            // Initialize the Facebook SDK
            FB.Init(InitCallback, OnHideUnity);
        }
        else
        {
            // Already initialized, signal an app activation App Event
            FB.ActivateApp();
        }
    }

    

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
        var perms = new List<string>() { "public_profile" };
        FB.LogInWithReadPermissions(perms, AuthCallback);

    }
    public void OnClickGuest()
    {
        _objLogin.SetActive(false);
        _objInGameMenu.SetActive(true);
        _board.gameObject.SetActive(true);

        _user.text = "Guest";

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

    //페이스북

    private void InitCallback()
    {
        if (FB.IsInitialized)
        {
            // Signal an app activation App Event
            FB.ActivateApp();
            // Continue with Facebook SDK
            // ...
        }
        else
        {
            Debug.Log("Failed to Initialize the Facebook SDK");
        }
    }

    private void OnHideUnity(bool isGameShown)
    {
        if (!isGameShown)
        {
            // Pause the game - we will need to hide
            Time.timeScale = 0;
        }
        else
        {
            // Resume the game - we're getting focus again
            Time.timeScale = 1;
        }
    }

    private void AuthCallback(ILoginResult result)
    {
        if (FB.IsLoggedIn)
        {
            var aToken = Facebook.Unity.AccessToken.CurrentAccessToken;
            //Debug.Log(aToken.UserId);
            //foreach (string perm in aToken.Permissions)
            //{
            //    Debug.Log(perm);
            //}
                        

            _user.text = aToken.UserId;

            _objLogin.SetActive(false);
            _objInGameMenu.SetActive(true);
            _board.gameObject.SetActive(true);
            
            _board.InitTiles();
        }
        else
        {
            Debug.Log("User cancelled login");
        }
    }
}
