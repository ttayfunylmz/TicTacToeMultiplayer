using UnityEngine;

public class GridPosition : MonoBehaviour
{
    [SerializeField] private int _xPosition;
    [SerializeField] private int _yPosition;

    private void OnMouseDown() 
    {
        Debug.Log("Click! " + _xPosition + "," + _yPosition);
        GameManager.Instance.ClickedOnGridPosition(_xPosition, _yPosition);
    }
}
