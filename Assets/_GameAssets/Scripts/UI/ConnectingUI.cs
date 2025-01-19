using System;
using UnityEngine;

public class ConnectingUI : MonoBehaviour
{
    private void Start()
    {
        GameManager.Instance.OnGameStarted += GameManager_OnGameStarted;    
    }

    private void GameManager_OnGameStarted(object sender, EventArgs e)
    {
        Hide();
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }
}
