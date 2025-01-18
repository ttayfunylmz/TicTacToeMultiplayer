using System;
using TMPro;
using UnityEngine;

public class PlayerUI : MonoBehaviour
{
    [SerializeField] private GameObject _crossArrowGameObject;
    [SerializeField] private GameObject _circleArrowGameObject;
    [SerializeField] private GameObject _crossYouTextGameObject;
    [SerializeField] private GameObject _circleYouTextGameObject;
    [SerializeField] private TMP_Text _playerCrossScoreText;
    [SerializeField] private TMP_Text _playerCircleScoreText;

    private void Awake() 
    {
        _crossArrowGameObject.SetActive(false);
        _circleArrowGameObject.SetActive(false);
        _crossYouTextGameObject.SetActive(false);
        _circleYouTextGameObject.SetActive(false); 

        _playerCrossScoreText.text = "";
        _playerCircleScoreText.text = "";   
    }

    private void Start() 
    {
        GameManager.Instance.OnGameStarted += GameManager_OnGameStarted;
        GameManager.Instance.OnCurrentPlayablePlayerTypeChanged += GameManager_OnCurrentPlayablePlayerTypeChanged;
        GameManager.Instance.OnScoreChanged += GameManager_OnScoreChanged;
    }

    private void GameManager_OnScoreChanged()
    {
        GameManager.Instance.GetScores(out int playerCrossScore, out int playerCircleScore);

        _playerCrossScoreText.text = playerCrossScore.ToString();
        _playerCircleScoreText.text = playerCircleScore.ToString();
    }

    private void GameManager_OnCurrentPlayablePlayerTypeChanged()
    {
        UpdateCurrentArrow();
    }

    private void GameManager_OnGameStarted()
    {
        if(GameManager.Instance.GetLocalPlayerType() == PlayerType.Cross)
        {
            _crossYouTextGameObject.SetActive(true);
        }
        else
        {
            _circleYouTextGameObject.SetActive(true);
        }

        _playerCrossScoreText.text = "0";
        _playerCircleScoreText.text = "0";  

        UpdateCurrentArrow();
    }

    private void UpdateCurrentArrow()
    {
        if(GameManager.Instance.GetCurrentPlayablePlayerType() == PlayerType.Cross)
        {
            _crossArrowGameObject.SetActive(true);
            _circleArrowGameObject.SetActive(false);
        }
        else
        {
            _crossArrowGameObject.SetActive(false);
            _circleArrowGameObject.SetActive(true);
        }
    }
}
