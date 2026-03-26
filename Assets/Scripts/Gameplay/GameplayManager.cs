using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameplayManager : MonoBehaviour
{
    [SerializeField] Grid grid;
    [SerializeField] int startingMoves = 20;

    int _score = 0;
    int _moves = 0;
    bool _isProcessing = false;
    bool _isGameOver = false;

    void Start()
    {
        _moves = startingMoves;
        if (grid != null)
            grid.Init(this);

        GameEvents.OnScoreChanged += UpdateScoreUI;
        GameEvents.OnMovesChanged += UpdateMovesUI;

        GameEvents.ScoreChanged(_score);
        GameEvents.MovesChanged(_moves);
        
        Invoke(nameof(ShowHint), 1f);
    }

    void OnDestroy()
    {
        GameEvents.OnScoreChanged -= UpdateScoreUI;
        GameEvents.OnMovesChanged -= UpdateMovesUI;
    }

    int CalculateBonusMoves(int matchCount)
    {
        if (matchCount < 3) return 0;
        return Mathf.Min(matchCount - 1, 4);
    }

    int CalculateScore(int matchCount)
    {
        if (matchCount < 3) return 0;
        return Mathf.Min((matchCount - 2) * 10, 30);
    }

    public void ProcessMatch(List<Ball> matches)
    {
        if (_isProcessing || _isGameOver) return;
        _isProcessing = true;

        _moves--;
        
        int matchCount = matches.Count;
        int bonusMoves = CalculateBonusMoves(matchCount);
        int points = CalculateScore(matchCount);

        _score += points;
        _moves += bonusMoves;

        GameEvents.ScoreChanged(_score);
        GameEvents.MovesChanged(_moves);

        int explodedCount = 0;
        foreach (Ball ball in matches)
        {
            ball.Explode(() =>
            {
                if (grid != null) grid.RemoveBall(ball);
                ball.ReturnToPool();
                explodedCount++;

                if (explodedCount == matches.Count)
                {
                    if (grid != null)
                    {
                        grid.ShiftDownAndRefill(() =>
                        {
                            _isProcessing = false;
                            CheckGameState();
                        });
                    }
                    else
                    {
                        _isProcessing = false;
                        CheckGameState();
                    }
                }
            });
        }
    }

    void CheckGameState()
    {
        if (_moves <= 0)
        {
            EndGame();
            return;
        }

        if (!grid.HasPossibleMoves())
        {
            EndGame();
            return;
        }
    }

    void ShowHint()
    {
        if (!_isGameOver && _isProcessing == false)
        {
            grid.HighlightHint();
        }
    }

    void UpdateScoreUI(int score) { /* Здесь можно подключить текстовое поле UI */ }
    void UpdateMovesUI(int moves) { /* Здесь можно подключить текстовое поле UI */ }

    void EndGame()
    {
        if (_isGameOver) return;
        _isGameOver = true;

        SaveService saveService = ServiceLocator.Get<SaveService>();
        string date = System.DateTime.Now.ToString("dd.MM.yyyy");
        saveService.SaveRecord(date, _score);
        
        GameEvents.GameEnded();

        Invoke(nameof(LoadLeaderboard), 2f);
    }

    void LoadLeaderboard()
    {
        SceneNavigator.Instance.LoadScene("2_Leaderboard");
    }
}