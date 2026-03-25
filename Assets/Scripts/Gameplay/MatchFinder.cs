using System.Collections.Generic;
using UnityEngine;

public static class MatchFinder
{
    // 8 направлений: верх, низ, лево, право + 4 диагонали
    static readonly Vector2Int[] _directions = new Vector2Int[]
    {
        new Vector2Int(0, 1),   // Up
        new Vector2Int(0, -1),  // Down
        new Vector2Int(-1, 0),  // Left
        new Vector2Int(1, 0),   // Right
        new Vector2Int(-1, 1),  // Up-Left
        new Vector2Int(1, 1),   // Up-Right
        new Vector2Int(-1, -1), // Down-Left
        new Vector2Int(1, -1)   // Down-Right
    };

    public static List<Ball> FindMatches(Grid grid, Ball startBall, out Vector2Int startPos)
    {
        List<Ball> matches = new List<Ball>();
        HashSet<Vector2Int> visited = new HashSet<Vector2Int>();
        
        startPos = grid.GetBallPosition(startBall);
        Color targetColor = startBall.BallColor;

        FindMatchesRecursive(grid, startPos, targetColor, matches, visited);

        return matches;
    }

    static void FindMatchesRecursive(Grid grid, Vector2Int pos, Color targetColor, 
                                      List<Ball> matches, HashSet<Vector2Int> visited)
    {
        if (!grid.IsValidPosition(pos)) return;
        if (visited.Contains(pos)) return;

        Ball ball = grid.GetBallAtPosition(pos);
        if (ball == null) return;
        if (ball.BallColor != targetColor) return;

        visited.Add(pos);
        matches.Add(ball);

        // Рекурсивно проверяем всех 8 соседей
        foreach (var dir in _directions)
        {
            Vector2Int neighborPos = new Vector2Int(pos.x + dir.x, pos.y + dir.y);
            FindMatchesRecursive(grid, neighborPos, targetColor, matches, visited);
        }
    }
}