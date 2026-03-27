using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameplayManager : MonoBehaviour
{
    [SerializeField] Grid grid;
    [SerializeField] int startingMoves = 20;

    int score = 0;
    int moves = 0;
    bool isProcessing = false;
    bool isGameOver = false;

    void Start()
    {
        moves = startingMoves;
        if (grid != null)
            grid.Init(this);

        GameEvents.ScoreChanged(score);
        GameEvents.MovesChanged(moves);
        
        Invoke(nameof(ShowHint), 1f);
    }

    //Тут я оставлю немного магических цифр которые нужно по конфигам и константам разносить
    //для тестового задания это будет лишним, как мне кажется
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
        if (isProcessing || isGameOver) return;
        isProcessing = true;

        moves--;
        
        int matchCount = matches.Count;
        int bonusMoves = CalculateBonusMoves(matchCount);
        int points = CalculateScore(matchCount);

        score += points;
        moves += bonusMoves;

        GameEvents.ScoreChanged(score);
        GameEvents.MovesChanged(moves);

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
                            isProcessing = false;
                            CheckGameState();
                        });
                    }
                    else
                    {
                        isProcessing = false;
                        CheckGameState();
                    }
                }
            });
        }
    }

    void CheckGameState()
    {
        if (moves <= 0)
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
        if (!isGameOver && isProcessing == false)
        {
            grid.HighlightHint();
        }
    }

    void EndGame()
    {
        if (isGameOver) return;
        isGameOver = true;
        
        SaveService saveService = ServiceLocator.Get<SaveService>();
        string date = System.DateTime.Now.ToString("dd.MM.yyyy");
        
        bool isTopScore = saveService.IsTopScore(score);
        
        saveService.SaveRecord(date, score);
        
        int position = saveService.GetScorePosition(score);
        
        GameEvents.GameEnded();
        
        if (isTopScore)
        {
            LeaderboardData.SetHighlightPosition(position);
            LoadLeaderboard();
        }
        else
        {
            Debug.Log("end game in manager");
            ShowSadMessage();
        }
    }

    void ShowSadMessage() => GameEvents.ShowSadMessage(score); 
    void LoadLeaderboard() => SceneNavigator.Instance.LoadScene("2_Leaderboard");
}