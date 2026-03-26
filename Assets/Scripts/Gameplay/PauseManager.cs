using UnityEngine;
using UnityEngine.UI;

public class PauseManager : MonoBehaviour
{
    [SerializeField] GameObject pausePanel;
    [SerializeField] Button btnResume;
    [SerializeField] Button btnToMenu;
    
    bool _isPaused = false;
    
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
        if (_isPaused) return;
        
        _isPaused = true;
        pausePanel.SetActive(true);
        Time.timeScale = 0f;
    }
    
    void OnResumeClicked()
    {
        if (!_isPaused) return;
        
        _isPaused = false;
        pausePanel.SetActive(false);
        Time.timeScale = 1f;
    }
    
    void OnToMenuClicked()
    {
        Time.timeScale = 1f;
        SceneNavigator.Instance.LoadScene("0_MainMenu");
    }
}