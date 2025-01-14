using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public event Action<int, int> OnClickedOnGridPosition;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("More than one GameManager Instance!");
        }
        Instance = this;
    }

    public void ClickedOnGridPosition(int x, int y)
    {
        OnClickedOnGridPosition?.Invoke(x, y);
        Debug.Log("ClickedOnGridPosition" + x + "," + y);
    }
}
