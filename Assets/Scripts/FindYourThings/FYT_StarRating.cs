public static class FYT_StarRating
{
    // expressed as max % of essentials missed, adjust for difficulty
    public static float ThreeStarMaxMissed = 0.00f;    // 0% missed → 3 stars
    public static float TwoHalfStarMaxMissed = 0.25f;  // <25% missed → 2.5 stars
    public static float TwoStarMaxMissed = 0.50f;      // <50% missed → 2 stars

    public static float Calculate(int essentialCollected, int essentialTotal)
    {
        if (essentialTotal == 0) return 3f;

        float missedRatio = 1f - ((float)essentialCollected / essentialTotal);

        if (missedRatio <= ThreeStarMaxMissed) return 3f;
        if (missedRatio < TwoHalfStarMaxMissed) return 2.5f;
        if (missedRatio < TwoStarMaxMissed) return 2f;
        return 1f;
    }
}
