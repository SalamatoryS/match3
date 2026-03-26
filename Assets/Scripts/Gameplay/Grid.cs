using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem; // ← НОВЫЙ INPUT SYSTEM

public class Grid : MonoBehaviour
{
    [SerializeField] GridConfigSO gridConfig;
    
    GameplayManager _manager;
    Ball[,] _grid;
    Dictionary<Ball, Vector2Int> _ballPositions;
    BallPool _pool;
    Camera _mainCamera;

    public void Init(GameplayManager manager)
    {
        _manager = manager;
        if (gridConfig == null)
        {
            Debug.LogError("[Grid] GridConfigSO not assigned!");
            return;
        }

        _mainCamera = Camera.main;
        if (_mainCamera == null)
        {
            Debug.LogError("[Grid] No Main Camera found!");
            return;
        }

        int size = gridConfig.GridSize;
        _grid = new Ball[size, size];
        _ballPositions = new Dictionary<Ball, Vector2Int>();
        
        _pool = ServiceLocator.Get<BallPool>();
        if (_pool == null)
        {
            Debug.LogError("[Grid] BallPool not found in ServiceLocator!");
            return;
        }
        
        GenerateGrid();
    }

    // ✅ НОВЫЙ INPUT SYSTEM
    private void Update()
    {
        // ← Изменили на новый API
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            HandleClick();
        }
    }

    // ✅ НОВЫЙ INPUT SYSTEM - получение позиции мыши
    private void HandleClick()
    {
        Vector2 mousePos = Mouse.current.position.ReadValue();
    
        // ✅ Расстояние от камеры до плоскости сетки (Z=0)
        float distanceFromCamera = -_mainCamera.transform.position.z;
        Vector3 screenPos = new Vector3(mousePos.x, mousePos.y, distanceFromCamera);
        Vector3 worldPos = _mainCamera.ScreenToWorldPoint(screenPos);
        
        // Фиксируем Z=0 для 2D сетки
        worldPos.z = 0f;
        
        Debug.Log($"[Grid] Mouse: {mousePos}, World: {worldPos}");
        
        Vector2Int gridPos = WorldToGridPosition(worldPos);
        
        Debug.Log($"[Grid] Grid position: {gridPos}");
        
        if (!IsValidPosition(gridPos))
        {
            Debug.Log($"[Grid] Click outside grid!");
            return;
        }
        
        Ball ball = GetBallAtPosition(gridPos);
        
        if (ball != null)
        {
            Debug.Log($"[Grid] Ball clicked! Color: {ball.BallColor}");
            Debug.Log($"[Grid] Ball actual position: {ball.transform.position}");
            Debug.Log($"[Grid] Distance to ball: {Vector3.Distance(worldPos, ball.transform.position)}");
            ball.OnClicked();
        }
        else
        {
            Debug.Log($"[Grid] No ball at position {gridPos}");
            
            // ✅ ОТЛАДКА: Ищем ближайший шар
            FindNearestBall(worldPos);
        }
    }

    void FindNearestBall(Vector3 worldPos)
    {
        float minDistance = float.MaxValue;
        Vector2Int nearestPos = Vector2Int.zero;
        
        for (int x = 0; x < gridConfig.GridSize; x++)
        {
            for (int y = 0; y < gridConfig.GridSize; y++)
            {
                Ball ball = GetBallAtPosition(new Vector2Int(x, y));
                if (ball != null)
                {
                    float distance = Vector3.Distance(worldPos, ball.transform.position);
                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        nearestPos = new Vector2Int(x, y);
                    }
                }
            }
        }
        
        Debug.Log($"[Grid] Nearest ball at {nearestPos}, distance: {minDistance}");
    }

    private Vector2Int WorldToGridPosition(Vector3 worldPos)
    {
        Vector2 adjustedPos = new Vector2(
        worldPos.x - (gridConfig.GridOffset.x * gridConfig.CellSize),
        worldPos.y - (gridConfig.GridOffset.y * gridConfig.CellSize));
        
        // ✅ ДОБАВЛЯЕМ SMALL EPSILON для защиты от погрешностей
        float epsilon = 0.1f;
        
        int x = Mathf.FloorToInt((adjustedPos.x / gridConfig.CellSize) + epsilon);
        int y = Mathf.FloorToInt((adjustedPos.y / gridConfig.CellSize) + epsilon);
        
        return new Vector2Int(x, y);
    }

    private void GenerateGrid()
    {
        BallConfigSO colorConfig = ServiceLocator.Get<BallConfigSO>();
        int size = gridConfig.GridSize;

        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                CreateBallAtPosition(x, y, colorConfig);
            }
        }
        
        Debug.Log($"[Grid] Generation complete. Total balls: {size * size}");
    }

    private void CreateBallAtPosition(int x, int y, BallConfigSO colorConfig)
    {
        GameObject obj = _pool.Get();
        Ball ball = obj.GetComponent<Ball>();
        
        ball.Init(colorConfig.GetRandomColor(), this);
        
        Vector3 worldPos = new Vector3(
            (x + gridConfig.GridOffset.x) * gridConfig.CellSize,
            (y + gridConfig.GridOffset.y) * gridConfig.CellSize,
            0f
        );
        obj.transform.position = worldPos;
        obj.transform.SetParent(transform);

        _grid[x, y] = ball;
        _ballPositions[ball] = new Vector2Int(x, y);
    }

    public void SetManager(GameplayManager manager)
    {
        _manager = manager;
    }

    public void OnBallMatched(Ball ball)
    {
        if (!_ballPositions.ContainsKey(ball)) return;

        List<Ball> matches = MatchFinder.FindMatches(this, ball, out Vector2Int startPos);

        if (matches.Count >= 3)
        {
            if (_manager != null)
            {
                _manager.ProcessMatch(matches);
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
        int size = gridConfig.GridSize;
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
        int size = gridConfig.GridSize;

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

    public int GetGridSize() => gridConfig.GridSize;
}