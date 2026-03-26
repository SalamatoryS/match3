using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameplayManager : MonoBehaviour
{
    // ✅ Константы вместо ScriptableObject (KISS principle)
    const int MIN_MATCH_COUNT = 3;
    const int MAX_BONUS_MOVES = 4;
    const int MAX_SCORE_PER_MATCH = 30;
    const int SCORE_MULTIPLIER = 10;
    
    [SerializeField] Grid grid;
    [SerializeField] int startingMoves = 20;
    
    int _score = 0;
    int _moves = 0;
    bool _isProcessing = false;

    void Start()
    {
        _moves = startingMoves;
        
        if (grid != null)
            grid.Init(this);
        
        GameEvents.OnScoreChanged += UpdateScoreUI;
        GameEvents.OnMovesChanged += UpdateMovesUI;
        
        GameEvents.ScoreChanged(_score);
        GameEvents.MovesChanged(_moves);
    }

    void OnDestroy()
    {
        GameEvents.OnScoreChanged -= UpdateScoreUI;
        GameEvents.OnMovesChanged -= UpdateMovesUI;
    }

    // Формула для бонусных ходов: 3=+2, 4=+3, 5+=+4
    int CalculateBonusMoves(int matchCount)
    {
        if (matchCount < MIN_MATCH_COUNT) return 0;
        return Mathf.Min(matchCount - 1, MAX_BONUS_MOVES);
    }

    // Формула для очков: 3=10, 4=20, 5+=30
    int CalculateScore(int matchCount)
    {
        if (matchCount < MIN_MATCH_COUNT) return 0;
        return Mathf.Min((matchCount - 2) * SCORE_MULTIPLIER, MAX_SCORE_PER_MATCH);
    }

    public void ProcessMatch(List<Ball> matches)
    {
        if (_isProcessing) return;
        _isProcessing = true;

        int matchCount = matches.Count;
        int movesCost = 1;
        int bonusMoves = CalculateBonusMoves(matchCount);
        int points = CalculateScore(matchCount);

        _score += points;
        _moves = _moves - movesCost + bonusMoves;

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
                    if (grid != null) grid.RefillGrid();
                    
                    _isProcessing = false;
                    
                    if (_moves <= 0)
                    {
                        EndGame();
                    }
                    else
                    {
                        GameEvents.ScoreChanged(_score);
                        GameEvents.MovesChanged(_moves);
                    }
                }
            });
        }
    }

    void UpdateScoreUI(int score)
    {
        Debug.Log($"[UI] Score updated: {score}"); 
    }

    void UpdateMovesUI(int moves)
    {
        Debug.Log($"[UI] Moves updated: {moves}");
    }


    void EndGame()
    {
        SaveService saveService = ServiceLocator.Get<SaveService>();
        string date = System.DateTime.Now.ToString("dd.MM.yyyy");
        saveService.SaveRecord(date, _score);

        GameEvents.GameEnded();
        
        Invoke(nameof(LoadLeaderboard), 2f);
    }

    void LoadLeaderboard()
    {
        SceneManager.LoadScene("2_Leaderboard");
    }
}