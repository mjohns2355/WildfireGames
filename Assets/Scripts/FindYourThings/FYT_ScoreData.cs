using System.Collections.Generic;

public static class FYT_ScoreData
{
    public static int essentialCollected;
    public static int additionalCollected;
    public static int unnecessaryCollected;

    public static int essentialTotal;

    public static List<string> collectedEssentials = new List<string>();
    public static List<string> collectedAdditional = new List<string>();
    public static List<string> collectedUnnecessary = new List<string>();

    public static List<string> missedEssentials = new List<string>();

    public static float essentialScore;

    public static void Reset()
    {
        essentialCollected = 0;
        additionalCollected = 0;
        unnecessaryCollected = 0;
        essentialTotal = 0;
        essentialScore = 0f;
        collectedEssentials = new List<string>();
        collectedAdditional = new List<string>();
        collectedUnnecessary = new List<string>();
        missedEssentials = new List<string>();
    }
}
