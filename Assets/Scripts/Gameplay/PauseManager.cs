using UnityEngine;
using UnityEngine.UI;

public class PauseManager : MonoBehaviour
{
    [SerializeField] GameObject pausePanel;
    [SerializeField] Button btnResume;
    [SerializeField] Button btnToMenu;
    
    bool isPaused = false;
    
    void Awake()
    {
        if (pausePanel != null)
            pausePanel.SetActive(false);
        
        btnResume.onClick.AddListener(OnResumeClicked);
        btnToMenu.onClick.AddListener(OnToMenuClicked);
    }
    
    void OnDestroy()
    {
        btnResume.onClick.RemoveListener(OnResumeClicked);
        btnToMenu.onClick.RemoveListener(OnToMenuClicked);
    }
    
    public void RequestPause()
    {
        if (isPaused) return;
        
        isPaused = true;
        pausePanel.SetActive(true);
        Time.timeScale = 0f;

        GameEvents.RequestPause();
    }
    
    void OnResumeClicked()
    {
        if (!isPaused) return;
        
        isPaused = false;
        pausePanel.SetActive(false);
        Time.timeScale = 1f;

        GameEvents.RequestResume();
    }
    
    void OnToMenuClicked()
    {
        Time.timeScale = 1f;
        SceneNavigator.Instance.LoadScene("0_MainMenu");
    }
}