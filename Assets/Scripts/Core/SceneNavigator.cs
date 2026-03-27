using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class SceneNavigator : MonoBehaviour
{
    public static SceneNavigator Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public async void LoadScene(string sceneName)
    {
        DOTween.KillAll();
        await SceneManager.LoadSceneAsync(sceneName);
    }

    public void ExitGame() => Application.Quit();
}