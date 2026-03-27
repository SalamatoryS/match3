using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class LeaderboardView : MonoBehaviour
{
    [SerializeField] Text leaderboardText;
    [SerializeField] Button btnToMenu;
    string output;
    
    SaveService saveService;
    
    void OnEnable()
    {
        btnToMenu.onClick.AddListener(OnToMenuClicked);
    }
    
    void OnDisable()
    {
        btnToMenu.onClick.RemoveListener(OnToMenuClicked);
    }
    
    void Start()
    {
        saveService = ServiceLocator.Get<SaveService>();
        DisplayLeaderboard();
    }
    
    void DisplayLeaderboard()
    {
        if (leaderboardText == null) return;
        
        List<Record> records = saveService.GetRecords();
        int highlightPos = LeaderboardData.GetHighlightPosition();
        
        for (int i = 0; i < records.Count; i++)
        {
            if (i == highlightPos)
            {
                output += $"<color=#00FF00><b>#{i + 1}  {records[i].date}  {records[i].score}</b></color>\n";
            }
            else
            {
                output += $"#{i + 1}  {records[i].date}  {records[i].score}\n";
            }
        }
        
        leaderboardText.text = output;
        
        LeaderboardData.Clear();
    }
    
    void OnToMenuClicked() => SceneNavigator.Instance.LoadScene("0_MainMenu");
}