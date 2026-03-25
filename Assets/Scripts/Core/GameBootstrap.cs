using UnityEngine;

public class GameBootstrap : MonoBehaviour
{
    [SerializeField] BallConfigSO ballConfig;
    [SerializeField] BallPool ballPool;

    private void Awake()
    {
        // Инициализация пула
        ballPool.Init();
        ServiceLocator.Register<BallPool>(ballPool);
        
        // Инициализация сохранений
        SaveService saveService = new SaveService();
        saveService.Init();
        ServiceLocator.Register<SaveService>(saveService);
        
        // Конфиг
        ServiceLocator.Register<BallConfigSO>(ballConfig);

        Debug.Log("[Bootstrap] Game Initialized");
    }
}