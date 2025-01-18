using System;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [SerializeField] private Transform _placeSFXPrefab;
    [SerializeField] private Transform _winSFXPrefab;
    [SerializeField] private Transform _loseSFXPrefab;

    private void Start() 
    {
        GameManager.Instance.OnPlacedObject += GameManager_OnPlacedObject;  
        GameManager.Instance.OnGameWin += GameManager_OnGameWin;  
    }

    private void GameManager_OnGameWin(GameManager.Line line, PlayerType winPlayerType)
    {
        if(GameManager.Instance.GetLocalPlayerType() == winPlayerType)
        {
            Transform sfxTransform = Instantiate(_winSFXPrefab);
            Destroy(sfxTransform.gameObject, 5f);
        }
        else
        {
            Transform sfxTransform = Instantiate(_loseSFXPrefab);
            Destroy(sfxTransform.gameObject, 5f);
        }
    }

    private void GameManager_OnPlacedObject()
    {
        Transform sfxTransform = Instantiate(_placeSFXPrefab);
        Destroy(sfxTransform.gameObject, 5f);
    }
}
