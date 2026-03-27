using UnityEngine;
using UnityEngine.UI;

public class AboutView : MonoBehaviour
{
    [SerializeField] Button btnToMenu;
    [SerializeField] Button btnSocialLink;
    
    [SerializeField] string socialUrl;
    
    void OnEnable()
    {
        btnToMenu.onClick.AddListener(OnToMenuClicked);
        btnSocialLink.onClick.AddListener(OnSocialClicked);
    }
    
    void OnDisable()
    {
        btnToMenu.onClick.RemoveListener(OnToMenuClicked);
        btnSocialLink.onClick.RemoveListener(OnSocialClicked);
    }
    
    void OnToMenuClicked() => SceneNavigator.Instance.LoadScene("0_MainMenu");
    
    void OnSocialClicked() => Application.OpenURL(socialUrl);
}