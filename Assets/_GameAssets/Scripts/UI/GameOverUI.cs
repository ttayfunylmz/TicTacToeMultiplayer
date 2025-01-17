using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] private Outline _outline;
    [SerializeField] private TMP_Text _resultText;
    [SerializeField] private Color _winColor;
    [SerializeField] private Color _loseColor;

    private void Start() 
    {
        GameManager.Instance.OnGameWin += GameManager_OnGameWin;

        Hide();    
    }

    private void GameManager_OnGameWin(GameManager.Line line, PlayerType winPlayerType)
    {
        if(winPlayerType == GameManager.Instance.GetLocalPlayerType())
        {
            _resultText.text = "YOU WIN!";
            _resultText.color = _winColor;
            _outline.effectColor = _winColor;
        }
        else
        {
            _resultText.text = "YOU LOSE!";
            _resultText.color = _loseColor;
            _outline.effectColor = _loseColor;
        }

        Show();
    }

    private void Show()
    {
        gameObject.SetActive(true);
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }
}
