using System;

public static class GameEvents
{
    public static event Action<int> OnScoreChanged;
    public static event Action<int> OnMovesChanged;
    public static event Action OnGameEnded;
    
    public static event Action OnPauseRequested;
    public static event Action OnResumeRequested;

    public static void ScoreChanged(int score) => OnScoreChanged?.Invoke(score);
    public static void MovesChanged(int moves) => OnMovesChanged?.Invoke(moves);
    public static void GameEnded() => OnGameEnded?.Invoke();
    
    public static void RequestPause() => OnPauseRequested?.Invoke();
    public static void RequestResume() => OnResumeRequested?.Invoke();
}