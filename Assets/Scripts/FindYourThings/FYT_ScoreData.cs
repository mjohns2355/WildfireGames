using System.Collections.Generic;

public static class FYT_ScoreData
{
    public static int essentialCollected;
    public static int essentialTotal;

    public static List<string> collectedEssentials = new List<string>();
    public static List<string> missedEssentials = new List<string>();

    public static float essentialScore;
    public static float starRating;

    public static void Reset()
    {
        essentialCollected = 0;
        essentialTotal = 0;
        essentialScore = 0f;
        starRating = 0f;
        collectedEssentials = new List<string>();
        missedEssentials = new List<string>();
    }
}
