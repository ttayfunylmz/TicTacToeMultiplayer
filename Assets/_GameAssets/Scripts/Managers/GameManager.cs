using System;
using Unity.Netcode;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    public static GameManager Instance { get; private set; }

    public event Action<int, int, PlayerType> OnClickedOnGridPosition;
    public event Action OnGameStarted;
    public event Action OnCurrentPlayablePlayerTypeChanged;

    private PlayerType _localPlayerType;
    private NetworkVariable<PlayerType> _currentPlayablePlayerType = new NetworkVariable<PlayerType>();
    private PlayerType[,] _playerTypeArray;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("More than one GameManager Instance!");
        }
        Instance = this;

        _playerTypeArray = new PlayerType[3, 3];
    }

    public override void OnNetworkSpawn()
    {
        if (NetworkManager.Singleton.LocalClientId == 0)
        {
            _localPlayerType = PlayerType.Cross;
        }
        else
        {
            _localPlayerType = PlayerType.Circle;
        }

        if (IsServer)
        {
            NetworkManager.Singleton.OnClientConnectedCallback += NetworkManager_OnClientConnectedCallback;
        }

        _currentPlayablePlayerType.OnValueChanged += (PlayerType oldPlayerType, PlayerType newPlayerType) =>
        {
            OnCurrentPlayablePlayerTypeChanged?.Invoke();
        };
    }

    private void NetworkManager_OnClientConnectedCallback(ulong clientId)
    {
        if (NetworkManager.Singleton.ConnectedClientsList.Count == 2)
        {
            // START GAME
            _currentPlayablePlayerType.Value = PlayerType.Cross;
            TriggerOnGameStartedRpc();
        }
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void TriggerOnGameStartedRpc()
    {
        OnGameStarted?.Invoke();
    }

    [Rpc(SendTo.Server)]
    public void ClickedOnGridPositionRpc(int x, int y, PlayerType playerType)
    {
        if (playerType != _currentPlayablePlayerType.Value) { return; }

        if(_playerTypeArray[x, y] != PlayerType.None) { return; }

        _playerTypeArray[x, y] = playerType;

        OnClickedOnGridPosition?.Invoke(x, y, playerType);

        _currentPlayablePlayerType.Value = _currentPlayablePlayerType.Value switch
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

    public PlayerType GetCurrentPlayablePlayerType()
    {
        return _currentPlayablePlayerType.Value;
    }
}
