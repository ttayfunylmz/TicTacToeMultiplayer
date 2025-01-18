using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Button _rematchButton;
    [SerializeField] private Outline _outline;
    [SerializeField] private TMP_Text _resultText;

    [Header("Settings")]
    [SerializeField] private Color _winColor;
    [SerializeField] private Color _loseColor;
    [SerializeField] private Color _tieColor;

    private void Awake()
    {
        _rematchButton.onClick.AddListener(() =>
        {
            GameManager.Instance.RematchRpc();
        });
    }

    private void Start()
    {
        GameManager.Instance.OnGameWin += GameManager_OnGameWin;
        GameManager.Instance.OnRematch += GameManager_OnRematch;
        GameManager.Instance.OnGameTied += GameManager_OnGameTied;

        Hide();
    }

    private void GameManager_OnGameTied()
    {
        _resultText.text = "TIED!";
        _resultText.color = _tieColor;
        _outline.effectColor = _tieColor;
        Show();
    }

    private void GameManager_OnRematch()
    {
        Hide();
    }

    private void GameManager_OnGameWin(GameManager.Line line, PlayerType winPlayerType)
    {
        if (winPlayerType == GameManager.Instance.GetLocalPlayerType())
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
