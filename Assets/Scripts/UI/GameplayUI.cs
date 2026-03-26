using UnityEngine;
using UnityEngine.UI;

public class GameplayUI : MonoBehaviour
{
    [SerializeField] Text scoreText;
    [SerializeField] Text movesText;
    
    void OnEnable()
    {
        GameEvents.OnScoreChanged += UpdateScore;
        GameEvents.OnMovesChanged += UpdateMoves;
    }
    
    void OnDisable()
    {
        GameEvents.OnScoreChanged -= UpdateScore;
        GameEvents.OnMovesChanged -= UpdateMoves;
    }
    
    //текст оставлю через код, что бы не добавлять таблицу локализации(например)
    void UpdateScore(int score)
    {
        if (scoreText != null)
        {
            scoreText.text = $"Scores: {score}";
        }
    }
    
    void UpdateMoves(int moves)
    {
        if (movesText != null)
        {
            movesText.text = $"Moves: {moves}";
        }
    }
}