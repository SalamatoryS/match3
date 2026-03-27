public static class LeaderboardData
{
    static int highlightPosition = -1;
    
    public static void SetHighlightPosition(int position)
    {
        highlightPosition = position;
    }
    
    public static int GetHighlightPosition()
    {
        return highlightPosition;
    }
    
    public static void Clear()
    {
        highlightPosition = -1;
    }
}
