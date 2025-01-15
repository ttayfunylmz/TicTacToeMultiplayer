using UnityEngine;

public class GridPosition : MonoBehaviour
{
    [SerializeField] private int _xPosition;
    [SerializeField] private int _yPosition;

    private void OnMouseDown() 
    {
        GameManager.Instance.ClickedOnGridPositionRpc(_xPosition, _yPosition, GameManager.Instance.GetLocalPlayerType());
    }
}
