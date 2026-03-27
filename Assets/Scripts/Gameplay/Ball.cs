using UnityEngine;
using DG.Tweening;

public class Ball : MonoBehaviour
{
    [SerializeField] Color ballColor;
    [SerializeField] MeshRenderer meshRenderer;

    Grid grid;
    bool isExploding = false;

    public Color BallColor => ballColor;

    public void Init(Color color, Grid grid)
    {
        ballColor = color;
        this.grid = grid;

        if (meshRenderer != null)
        {
            meshRenderer.material.color = color;
        }
        isExploding = false;
        transform.localScale = Vector3.one;
        transform.localRotation = Quaternion.identity;
    }

    public void OnClicked()
    {
        if (isExploding || grid == null) return;
        grid.OnBallMatched(this);
    }

    public void Explode(System.Action onComplete)
    {
        isExploding = true;
   
        transform.DOScale(1.5f, 0.1f)
            .OnComplete(() =>
            {
                transform.DOScale(0f, 0.1f)
                    .OnComplete(() =>
                    {
                        onComplete?.Invoke();
                    });
            });
    }

    public void ReturnToPool()
    {
        grid = null;
        BallPool pool = ServiceLocator.Get<BallPool>();
        if (pool != null)
        {
            pool.Return(gameObject);
        }
    }
}