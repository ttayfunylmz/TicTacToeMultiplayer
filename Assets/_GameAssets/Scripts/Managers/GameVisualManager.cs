using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class GameVisualManager : NetworkBehaviour
{
    private const float GRID_SIZE = 3.1f;

    [SerializeField] private Transform _crossPrefab;
    [SerializeField] private Transform _circlePrefab;
    [SerializeField] private Transform _lineCompletePrefab;

    private List<GameObject> _visualGameObjectList;

    private void Awake() 
    {
        _visualGameObjectList = new List<GameObject>();    
    }

    private void Start()
    {
        GameManager.Instance.OnClickedOnGridPosition += GameManager_OnClickedOnGridPosition;
        GameManager.Instance.OnGameWin += GameManager_OnGameWin;
        GameManager.Instance.OnRematch += GameManager_OnRematch;
    }

    private void GameManager_OnRematch()
    {
        if(!NetworkManager.Singleton.IsServer) { return; }

        foreach(GameObject visualGameObject in _visualGameObjectList)
        {
            Destroy(visualGameObject);
        }

        _visualGameObjectList.Clear();
    }

    private void GameManager_OnGameWin(GameManager.Line line, PlayerType winPlayerType)
    {
        if(!NetworkManager.Singleton.IsServer) { return; }

        float eulerZ = line._orientationType switch
        {
            OrientationType.Vertical => 90f,
            OrientationType.DiagonalA => 45f,
            OrientationType.DiagonalB => -45,
            _ => 0f,
        };

        Transform lineCompleteTransform
            = Instantiate(_lineCompletePrefab, GetGridWorldPosition
                (line._centerGridPosition.x, line._centerGridPosition.y), Quaternion.Euler(0f, 0f, eulerZ));
        
        lineCompleteTransform.GetComponent<NetworkObject>().Spawn(true);

        _visualGameObjectList.Add(lineCompleteTransform.gameObject);
    }

    private void GameManager_OnClickedOnGridPosition(int x, int y, PlayerType playerType)
    {
        SpawnObjectRpc(x, y, playerType);
    }

    [Rpc(SendTo.Server)]
    private void SpawnObjectRpc(int x, int y, PlayerType playerType)
    {
        Transform prefab = playerType switch
        {
            PlayerType.Cross => _crossPrefab,
            PlayerType.Circle => _circlePrefab,
            _ => _crossPrefab,
        };

        Transform spawnedTransform 
            = Instantiate(prefab, GetGridWorldPosition(x, y), Quaternion.identity);
        spawnedTransform.GetComponent<NetworkObject>().Spawn(true);

        _visualGameObjectList.Add(spawnedTransform.gameObject);
    }

    private Vector2 GetGridWorldPosition(int x, int y)
    {
        return new Vector2(-GRID_SIZE + x * GRID_SIZE, -GRID_SIZE + y * GRID_SIZE);
    }
}
