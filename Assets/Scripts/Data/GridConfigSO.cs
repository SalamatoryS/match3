using UnityEngine;

[CreateAssetMenu(fileName = "GridConfigSO", menuName = "Game/GridConfigSO")]
public class GridConfigSO : ScriptableObject
{
    [SerializeField] int gridSize = 8;
    [SerializeField] float cellSize = 1.2f;
    [SerializeField] Vector2 gridOffset = new Vector2Int(-4, -4);

    public int GridSize => gridSize;
    public float CellSize => cellSize;
    public Vector2 GridOffset => gridOffset;
}
