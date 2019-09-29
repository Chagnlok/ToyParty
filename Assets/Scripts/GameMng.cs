using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DG.Tweening;

public class GameMng : MonoBehaviour
{
    public GameObject _goBack;
    // Start is called before the first frame update
    void Start()
    {
        DOTween.Init();


#if UNITY_EDITOR
#else
        _goBack.SetActive(true);
#endif
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
