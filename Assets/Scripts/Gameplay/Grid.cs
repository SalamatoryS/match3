using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    [SerializeField] GridConfigSO config;
    
    Ball[,] _grid;
    Dictionary<Ball, Vector2Int> _ballPositions;
    BallPool _pool;
    GameplayManager manager;

    public void Init(GameplayManager manager)
    {
        if (config == null)
        {
            Debug.LogError("GameplayConfigSO not assigned to Grid!");
            return;
        }
        this.manager = manager;

        int size = config.GridSize;
        _grid = new Ball[size, size];
        _ballPositions = new Dictionary<Ball, Vector2Int>();
        
        _pool = ServiceLocator.Get<BallPool>();
        
        GenerateGrid();
    }

    void GenerateGrid()
    {
        BallConfigSO colorConfig = ServiceLocator.Get<BallConfigSO>();
        int size = config.GridSize;

        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                CreateBallAtPosition(x, y, colorConfig);
            }
        }
    }

    void CreateBallAtPosition(int x, int y, BallConfigSO colorConfig)
    {
        GameObject obj = _pool.Get();
        Ball ball = obj.GetComponent<Ball>();
        
        ball.Init(colorConfig.GetRandomColor(), this);
        
        Vector3 worldPos = new Vector3(
            (x + config.GridOffset.x) * config.CellSize,
            (y + config.GridOffset.y) * config.CellSize,
            0f
        );
        obj.transform.position = worldPos;
        obj.transform.SetParent(transform);

        _grid[x, y] = ball;
        _ballPositions[ball] = new Vector2Int(x, y);
    }

    public void OnBallClicked(Ball ball)
    {
        if (!_ballPositions.ContainsKey(ball)) return;

        List<Ball> matches = MatchFinder.FindMatches(this, ball, out Vector2Int startPos);

        if (matches.Count >= 3)
        {
            if (manager != null)
            {
                manager.ProcessMatch(matches);
            }
        }
    }

    public Ball GetBallAtPosition(Vector2Int pos)
    {
        if (IsValidPosition(pos))
        {
            return _grid[pos.x, pos.y];
        }
        return null;
    }

    public Vector2Int GetBallPosition(Ball ball)
    {
        if (_ballPositions.TryGetValue(ball, out Vector2Int pos))
        {
            return pos;
        }
        return new Vector2Int(-1, -1);
    }

    public bool IsValidPosition(Vector2Int pos)
    {
        int size = config.GridSize;
        return pos.x >= 0 && pos.x < size && pos.y >= 0 && pos.y < size;
    }

    public void RemoveBall(Ball ball)
    {
        if (_ballPositions.TryGetValue(ball, out Vector2Int pos))
        {
            _grid[pos.x, pos.y] = null;
            _ballPositions.Remove(ball);
        }
    }

    public void RefillGrid()
    {
        BallConfigSO colorConfig = ServiceLocator.Get<BallConfigSO>();
        int size = config.GridSize;

        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                if (_grid[x, y] == null)
                {
                    CreateBallAtPosition(x, y, colorConfig);
                }
            }
        }
    }

    public int GetGridSize() => config.GridSize;
}