using System;
using Unity.Netcode;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    public static GameManager Instance { get; private set; }

    public event Action<int, int, PlayerType> OnClickedOnGridPosition;

    private PlayerType _localPlayerType;
    private PlayerType _currentPlayablePlayerType;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("More than one GameManager Instance!");
        }
        Instance = this;
    }

    public override void OnNetworkSpawn()
    {
        if(NetworkManager.Singleton.LocalClientId == 0)
        {
            _localPlayerType = PlayerType.Cross;
        }
        else
        {
            _localPlayerType = PlayerType.Circle;
        }

        if(IsServer)
        {
            _currentPlayablePlayerType = PlayerType.Cross;
        }
    }

    [Rpc(SendTo.Server)]
    public void ClickedOnGridPositionRpc(int x, int y, PlayerType playerType)
    {
        if(playerType != _currentPlayablePlayerType) { return; }

        OnClickedOnGridPosition?.Invoke(x, y, playerType);

        _currentPlayablePlayerType = _currentPlayablePlayerType switch
        {
            PlayerType.Circle => PlayerType.Cross,
            PlayerType.Cross => PlayerType.Circle,
            _ => PlayerType.Circle,
        };
    }

    public PlayerType GetLocalPlayerType()
    {
        return _localPlayerType;
    }
}
