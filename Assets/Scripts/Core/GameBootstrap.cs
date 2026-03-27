using UnityEngine;

public class GameBootstrap : MonoBehaviour
{
    [SerializeField] BallConfigSO ballConfig;
    [SerializeField] BallPool ballPool;

    static bool isInitialized = false;

    private void Awake()
    {
        if (isInitialized)
        {
            Destroy(gameObject);
            return;
        }

        isInitialized = true;
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