using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BallConfigSO", menuName = "Game/BallConfigSO")]
public class BallConfigSO : ScriptableObject
{
    [SerializeField] List<Color> ballColors;
    
    public Color GetRandomColor()
    {
        if (ballColors == null || ballColors.Count == 0) return Color.white;
        return ballColors[Random.Range(0, ballColors.Count)];
    }
}
