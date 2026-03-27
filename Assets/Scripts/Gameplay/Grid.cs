using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;

public class Grid : MonoBehaviour
{
    [SerializeField] GridConfigSO gridConfig;
    
    GameplayManager manager;
    Ball[,] grid;
    Dictionary<Ball, Vector2Int> ballPositions;
    BallPool pool;
    Camera mainCamera;
    bool isInputLocked = false;
    bool isPause = false;
    float epsilon = 0.01f; // Минимальная погрешность

    public void Init(GameplayManager manager)
    {
        this.manager = manager;
        if (gridConfig == null) return;

        mainCamera = Camera.main;
        int size = gridConfig.GridSize;
        grid = new Ball[size, size];
        ballPositions = new Dictionary<Ball, Vector2Int>();

        pool = ServiceLocator.Get<BallPool>();
        if (pool == null) return;

        GenerateGrid();

        GameEvents.OnPauseRequested += OnPause;
        GameEvents.OnResumeRequested += OnResume;
    }

    private void Update()
    {
        if (isInputLocked || isPause) return;

        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            HandleClick();
        }
    }

    void HandleClick()
    {
        Vector2 mousePos = Mouse.current.position.ReadValue();
        float distanceFromCamera = -mainCamera.transform.position.z;
        Vector3 screenPos = new Vector3(mousePos.x, mousePos.y, distanceFromCamera);
        Vector3 worldPos = mainCamera.ScreenToWorldPoint(screenPos);
        worldPos.z = 0f;

        Vector2Int gridPos = WorldToGridPosition(worldPos);

        if (!IsValidPosition(gridPos)) return;

        Ball ball = GetBallAtPosition(gridPos);
        if (ball != null)
        {
            ball.OnClicked();
        }
    }

    Vector2Int WorldToGridPosition(Vector3 worldPos)
    {        
        int x = Mathf.RoundToInt((worldPos.x / gridConfig.CellSize) - gridConfig.GridOffset.x + epsilon);
        int y = Mathf.RoundToInt((worldPos.y / gridConfig.CellSize) - gridConfig.GridOffset.y + epsilon);
        
        return new Vector2Int(x, y);
    }

    void GenerateGrid()
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
    }

    void CreateBallAtPosition(int x, int y, BallConfigSO colorConfig)
    {
        GameObject obj = pool.Get();
        Ball ball = obj.GetComponent<Ball>();
        ball.Init(colorConfig.GetRandomColor(), this);

        Vector3 worldPos = new Vector3(
            (x + gridConfig.GridOffset.x) * gridConfig.CellSize,
            (y + gridConfig.GridOffset.y) * gridConfig.CellSize,
            0f
        );
        
        obj.transform.position = worldPos;
        obj.transform.SetParent(transform);
        grid[x, y] = ball;
        ballPositions[ball] = new Vector2Int(x, y);
    }
    
    Vector3 GetWorldPosition(int x, int y)
    {
        return new Vector3(
            (x + gridConfig.GridOffset.x) * gridConfig.CellSize,
            (y + gridConfig.GridOffset.y) * gridConfig.CellSize,
            0f
        );
    }

    public void OnBallMatched(Ball ball)
    {
        if (!ballPositions.ContainsKey(ball)) return;
        
        List<Ball> matches = MatchFinder.FindMatches(this, ball, out _);

        if (matches.Count < 3)
        {
            matches.Clear();
            matches.Add(ball);
        }
        
        if (manager != null)
        {
            manager.ProcessMatch(matches);
        }
    }

    public Ball GetBallAtPosition(Vector2Int pos)
    {
        if (IsValidPosition(pos)) return grid[pos.x, pos.y];
        return null;
    }

    public Vector2Int GetBallPosition(Ball ball)
    {
        if (ballPositions.TryGetValue(ball, out Vector2Int pos)) return pos;
        return new Vector2Int(-1, -1);
    }

    public bool IsValidPosition(Vector2Int pos)
    {
        int size = gridConfig.GridSize;
        return pos.x >= 0 && pos.x < size && pos.y >= 0 && pos.y < size;
    }

    public void RemoveBall(Ball ball)
    {
        if (ballPositions.TryGetValue(ball, out Vector2Int pos))
        {
            grid[pos.x, pos.y] = null;
            ballPositions.Remove(ball);
        }
    }

    public void ShiftDownAndRefill(System.Action onComplete)
    {
        StartCoroutine(ShiftDownCoroutine(onComplete));
    }

    IEnumerator ShiftDownCoroutine(System.Action onComplete)
    {
        isInputLocked = true;
        int size = gridConfig.GridSize;
        BallConfigSO colorConfig = ServiceLocator.Get<BallConfigSO>();

        for (int x = 0; x < size; x++)
        {
            int writeY = 0;
            for (int y = 0; y < size; y++)
            {
                if (grid[x, y] != null)
                {
                    if (y != writeY)
                    {
                        grid[x, writeY] = grid[x, y];
                        grid[x, y] = null;
                        
                        Ball ball = grid[x, writeY];
                        ballPositions[ball] = new Vector2Int(x, writeY);
                        
                        Vector3 targetPos = GetWorldPosition(x, writeY);
                        ball.transform.DOMove(targetPos, 0.3f);
                    }
                    writeY++;
                }
            }
        }

        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                if (grid[x, y] == null)
                {
                    GameObject obj = pool.Get();
                    Ball ball = obj.GetComponent<Ball>();
                    ball.Init(colorConfig.GetRandomColor(), this);
                    
                    grid[x, y] = ball;
                    ballPositions[ball] = new Vector2Int(x, y);
                    obj.transform.SetParent(transform);

                    Vector3 startPos = GetWorldPosition(x, y) + Vector3.up * 5f;
                    Vector3 targetPos = GetWorldPosition(x, y);
                    
                    obj.transform.position = startPos;
                    ball.transform.DOMove(targetPos, 0.4f).SetDelay(0.1f);
                }
            }
        }

        yield return new WaitForSeconds(0.5f);
        isInputLocked = false;
        onComplete?.Invoke();
    }

    public bool HasPossibleMoves()
    {
        int size = gridConfig.GridSize;
        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                if (grid[x, y] != null)
                {
                    List<Ball> matches = MatchFinder.FindMatches(this, grid[x, y], out _);
                    if (matches.Count >= 3) return true;
                }
            }
        }
        return false;
    }
    
    public void HighlightHint()
    {
        int size = gridConfig.GridSize;
        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                if (grid[x, y] != null)
                {
                    List<Ball> matches = MatchFinder.FindMatches(this, grid[x, y], out _);
                    if (matches.Count >= 3)
                    {
                        foreach(var b in matches)
                        {
                            b.transform.DOPunchScale(Vector3.one * 0.2f, 0.5f);
                        }
                        return;
                    }
                }
            }
        }
    }

    void OnPause() => isPause = true;
    
    void OnResume() => isPause = false;

    void OnDestroy()
    {
        GameEvents.OnPauseRequested -= OnPause;
        GameEvents.OnResumeRequested -= OnResume;
    }
}