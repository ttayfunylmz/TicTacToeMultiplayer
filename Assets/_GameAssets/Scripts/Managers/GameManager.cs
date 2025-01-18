using System;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    public static GameManager Instance { get; private set; }

    public event Action<int, int, PlayerType> OnClickedOnGridPosition;
    public event Action OnGameStarted;
    public event Action<Line, PlayerType> OnGameWin;
    public event Action OnCurrentPlayablePlayerTypeChanged;
    public event Action OnRematch;
    public event Action OnGameTied;
    public event Action OnScoreChanged;

    public struct Line
    {
        public List<Vector2Int> _gridVector2IntList;
        public Vector2Int _centerGridPosition;
        public OrientationType _orientationType;
    }

    private PlayerType _localPlayerType;
    private NetworkVariable<PlayerType> _currentPlayablePlayerType = new NetworkVariable<PlayerType>();
    private PlayerType[,] _playerTypeArray;
    private List<Line> _lineList;
    private NetworkVariable<int> _playerCrossScore = new NetworkVariable<int>();
    private NetworkVariable<int> _playerCircleScore = new NetworkVariable<int>();

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("More than one GameManager Instance!");
        }
        Instance = this;

        _playerTypeArray = new PlayerType[3, 3];

        _lineList = new List<Line>
        {
            // HORIZONTAL
            new Line
            {
                _gridVector2IntList = new List<Vector2Int> { new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(2, 0) },
                _centerGridPosition = new Vector2Int(1, 0),
                _orientationType = OrientationType.Horizontal
            },
            new Line
            {
                _gridVector2IntList = new List<Vector2Int> { new Vector2Int(0, 1), new Vector2Int(1, 1), new Vector2Int(2, 1) },
                _centerGridPosition = new Vector2Int(1, 1),
                _orientationType = OrientationType.Horizontal
            },
            new Line
            {
                _gridVector2IntList = new List<Vector2Int> { new Vector2Int(0, 2), new Vector2Int(1, 2), new Vector2Int(2, 2) },
                _centerGridPosition = new Vector2Int(1, 2),
                _orientationType = OrientationType.Horizontal
            },

            // VERTICAL
            new Line
            {
                _gridVector2IntList = new List<Vector2Int> { new Vector2Int(0, 0), new Vector2Int(0, 1), new Vector2Int(0, 2) },
                _centerGridPosition = new Vector2Int(0, 1),
                _orientationType = OrientationType.Vertical
            },
            new Line
            {
                _gridVector2IntList = new List<Vector2Int> { new Vector2Int(1, 0), new Vector2Int(1, 1), new Vector2Int(1, 2) },
                _centerGridPosition = new Vector2Int(1, 1),
                _orientationType = OrientationType.Vertical
            },
            new Line
            {
                _gridVector2IntList = new List<Vector2Int> { new Vector2Int(2, 0), new Vector2Int(2, 1), new Vector2Int(2, 2) },
                _centerGridPosition = new Vector2Int(2, 1),
                _orientationType = OrientationType.Vertical
            },

            // DIAGONALS
            new Line
            {
                _gridVector2IntList = new List<Vector2Int> { new Vector2Int(0, 0), new Vector2Int(1, 1), new Vector2Int(2, 2) },
                _centerGridPosition = new Vector2Int(1, 1),
                _orientationType = OrientationType.DiagonalA
            },
            new Line
            {
                _gridVector2IntList = new List<Vector2Int> { new Vector2Int(0, 2), new Vector2Int(1, 1), new Vector2Int(2, 0) },
                _centerGridPosition = new Vector2Int(1, 1),
                _orientationType = OrientationType.DiagonalB
            },
        };
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

        _playerCrossScore.OnValueChanged += (int oldScore, int newScore) =>
        {
            OnScoreChanged?.Invoke();
        };
        _playerCircleScore.OnValueChanged += (int oldScore, int newScore) =>
        {
            OnScoreChanged?.Invoke();
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

        if (_playerTypeArray[x, y] != PlayerType.None) { return; }

        _playerTypeArray[x, y] = playerType;

        OnClickedOnGridPosition?.Invoke(x, y, playerType);

        _currentPlayablePlayerType.Value = _currentPlayablePlayerType.Value switch
        {
            PlayerType.Circle => PlayerType.Cross,
            PlayerType.Cross => PlayerType.Circle,
            _ => PlayerType.Circle,
        };

        TestWinner();
    }

    private bool TestWinnerLine(PlayerType aPlayerType, PlayerType bPlayerType, PlayerType cPlayerType)
    {
        return
            aPlayerType != PlayerType.None &&
            aPlayerType == bPlayerType &&
            bPlayerType == cPlayerType;
    }

    private bool TestWinnerLine(Line line)
    {
        return TestWinnerLine
        (
            _playerTypeArray[line._gridVector2IntList[0].x, line._gridVector2IntList[0].y],
            _playerTypeArray[line._gridVector2IntList[1].x, line._gridVector2IntList[1].y],
            _playerTypeArray[line._gridVector2IntList[2].x, line._gridVector2IntList[2].y]
        );
    }

    private void TestWinner()
    {
        for (int i = 0; i < _lineList.Count; ++i)
        {
            Line line = _lineList[i];
            if (TestWinnerLine(line))
            {
                Debug.Log("WIN!");
                _currentPlayablePlayerType.Value = PlayerType.None;

                PlayerType winPlayerType = _playerTypeArray[line._centerGridPosition.x, line._centerGridPosition.y];
                switch (winPlayerType)
                {
                    case PlayerType.Cross:
                        _playerCrossScore.Value++;
                        break;
                    case PlayerType.Circle:
                        _playerCircleScore.Value++;
                        break;
                }

                TriggerOnGameWinRpc(i, winPlayerType);
                return;
            }
        }

        bool hasTie = true;
        for (int x = 0; x < _playerTypeArray.GetLength(0); ++x)
        {
            for (int y = 0; y < _playerTypeArray.GetLength(1); ++y)
            {
                if (_playerTypeArray[x, y] == PlayerType.None)
                {
                    hasTie = false;
                    break;
                }
            }
        }

        if (hasTie)
        {
            TriggerOnGameTiedEventRpc();
        }
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void TriggerOnGameTiedEventRpc()
    {
        OnGameTied?.Invoke();
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void TriggerOnGameWinRpc(int lineIndex, PlayerType winPlayerType)
    {
        Line line = _lineList[lineIndex];

        OnGameWin?.Invoke(line, winPlayerType);
    }

    [Rpc(SendTo.Server)]
    public void RematchRpc()
    {
        for (int x = 0; x < _playerTypeArray.GetLength(0); ++x)
        {
            for (int y = 0; y < _playerTypeArray.GetLength(1); ++y)
            {
                _playerTypeArray[x, y] = PlayerType.None;
            }
        }

        _currentPlayablePlayerType.Value = PlayerType.Cross;
        TriggerOnRematchRpc();
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void TriggerOnRematchRpc()
    {
        OnRematch?.Invoke();
    }

    public PlayerType GetLocalPlayerType()
    {
        return _localPlayerType;
    }

    public PlayerType GetCurrentPlayablePlayerType()
    {
        return _currentPlayablePlayerType.Value;
    }

    public void GetScores(out int playerCrossScore, out int playerCircleScore)
    {
        playerCrossScore = _playerCrossScore.Value;
        playerCircleScore = _playerCircleScore.Value;
    }
}
