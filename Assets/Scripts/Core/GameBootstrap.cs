using UnityEngine;

public class GameBootstrap : MonoBehaviour
{
    [SerializeField] BallConfigSO ballConfig;
    [SerializeField] BallPool ballPool;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);

        ballPool.Init();
        ServiceLocator.Register(ballPool);
        
        SaveService saveService = new SaveService();
        saveService.Init();
        ServiceLocator.Register(saveService);
        
        ServiceLocator.Register(ballConfig);

        Debug.Log("[Bootstrap] Game Initialized");
    }
}