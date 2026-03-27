using UnityEngine;
using UnityEngine.UI;

public class SadMessageView : MonoBehaviour
{
    [SerializeField] GameObject sadPanel;
    [SerializeField] Text scoreText;
    [SerializeField] Button btnOK;
    
    void OnEnable()
    {
        GameEvents.OnSadMessageRequested += ShowSadMessage;
        btnOK.onClick.AddListener(OnOKClicked);
    }
    
    void OnDisable()
    {
        GameEvents.OnSadMessageRequested -= ShowSadMessage;
        btnOK.onClick.RemoveListener(OnOKClicked);
    }
    
    void Start()
    {
        if (sadPanel != null)
            sadPanel.SetActive(false);
    }
    
    void ShowSadMessage(int score)
    {
        if (scoreText != null)
            scoreText.text = $"Your score: {score}";
        
        if (sadPanel != null)
            sadPanel.SetActive(true);
    }
    
    void OnOKClicked() => SceneNavigator.Instance.LoadScene("0_MainMenu");
}
