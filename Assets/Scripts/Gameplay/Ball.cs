using UnityEngine;
using DG.Tweening;

public class Ball : MonoBehaviour
{
    [SerializeField] Color ballColor;
    [SerializeField] MeshRenderer meshRenderer;
    
    Grid _grid;
    bool _isExploding = false;

    public Color BallColor => ballColor;

    public void Init(Color color, Grid grid)
    {
        ballColor = color;
        _grid = grid;
        
        if (meshRenderer != null)
        {
            meshRenderer.material.color = color;
        }
        _isExploding = false;
        transform.localScale = Vector3.one;
    }

    public void OnMouseDown()
    {
        if (_isExploding || _grid == null) return;
        
        _grid.OnBallClicked(this);
    }

    public void Explode(System.Action onComplete)
    {
        _isExploding = true;
        
        // Анимация взрыва через DOTween (ТЗ п.7)
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
        // ВАЖНО: Обнуляем ссылку на сетку перед возвратом в пул!
        // Иначе при следующей игре шарик будет ссылаться на уничтоженный Grid
        _grid = null; 
        
        BallPool pool = ServiceLocator.Get<BallPool>();
        if (pool != null)
        {
            pool.Return(gameObject);
        }
    }
}