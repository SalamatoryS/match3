using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

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

    public void LoadScene(string sceneName)
    {
        StartCoroutine(LoadSceneAsync(sceneName));
    }

    IEnumerator LoadSceneAsync(string sceneName)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
        operation.allowSceneActivation = true;

        while (!operation.isDone)
        {
            yield return null;
        }
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}