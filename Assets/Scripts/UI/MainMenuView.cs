using UnityEngine;
using UnityEngine.UI;

public class MainMenuView : MonoBehaviour
{
    [SerializeField] Button btnNewGame;
    [SerializeField] Button btnLeaderboard;
    [SerializeField] Button btnAbout;
    [SerializeField] Button btnExit;
    
    [SerializeField] GameObject exitDialog;

    private void Awake()
    {
        btnNewGame.onClick.AddListener(OnNewGameClicked);
        btnLeaderboard.onClick.AddListener(OnLeaderboardClicked);
        btnAbout.onClick.AddListener(OnAboutClicked);
        btnExit.onClick.AddListener(OnExitClicked);

        if (exitDialog != null) exitDialog.SetActive(false);
    }

    void OnDestroy()
    {
        btnNewGame.onClick.RemoveListener(OnNewGameClicked);
        btnLeaderboard.onClick.RemoveListener(OnLeaderboardClicked);
        btnAbout.onClick.RemoveListener(OnAboutClicked);
        btnExit.onClick.RemoveListener(OnExitClicked);
    }

    void OnNewGameClicked() => SceneNavigator.Instance.LoadScene("1_Gameplay");
    void OnLeaderboardClicked() => SceneNavigator.Instance.LoadScene("2_Leaderboard");
    void OnAboutClicked() => SceneNavigator.Instance.LoadScene("3_About");
    void OnExitClicked() =>  exitDialog.SetActive(true);

    public void ConfirmExit() => SceneNavigator.Instance.ExitGame();
    public void CancelExit() => exitDialog.SetActive(false); 
}