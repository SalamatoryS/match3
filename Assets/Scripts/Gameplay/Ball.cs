using UnityEngine;
using DG.Tweening;

public class Ball : MonoBehaviour
{
    [SerializeField] Color ballColor;
    [SerializeField] MeshRenderer meshRenderer;
    
    Grid _grid;
    bool _isExploding = false;

    public Color BallColor => ballColor;

    // ✅ Инициализация при получении из пула
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

    // ✅ Вызывается из Grid.HandleClick() - шар реагирует на клик
    public void OnClicked()
    {
        if (_isExploding || _grid == null)
        {
            return;
        }
        
        // Шар сообщает Grid о клике, Grid проверяет совпадения
        _grid.OnBallMatched(this);
    }

    // ✅ Анимация взрыва через DOTween (ТЗ п.7)
    public void Explode(System.Action onComplete)
    {
        _isExploding = true;
        
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

    // ✅ Возврат в пул (ТЗ п.6 - оптимизация)
    public void ReturnToPool()
    {
        // Очищаем ссылку на сетку (важно для смены сцен!)
        _grid = null;
        
        BallPool pool = ServiceLocator.Get<BallPool>();
        if (pool != null)
        {
            pool.Return(gameObject);
        }
    }
}