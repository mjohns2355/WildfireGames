using System.Collections.Generic;

public static class FYT_ScoreCalculator
{
    public static void Calculate(List<string> itemNames)
    {
        FYT_ScoreData.Reset();

        foreach (string name in itemNames)
        {
            FYT_ItemCatalog.ItemTier tier = FYT_ItemCatalog.GetTier(name);
            switch (tier)
            {
                case FYT_ItemCatalog.ItemTier.Essential:
                    FYT_ScoreData.essentialCollected++;
                    FYT_ScoreData.collectedEssentials.Add(name);
                    break;
                case FYT_ItemCatalog.ItemTier.Additional:
                    FYT_ScoreData.additionalCollected++;
                    FYT_ScoreData.collectedAdditional.Add(name);
                    break;
                case FYT_ItemCatalog.ItemTier.Unnecessary:
                    FYT_ScoreData.unnecessaryCollected++;
                    FYT_ScoreData.collectedUnnecessary.Add(name);
                    break;
            }
        }

        FYT_ScoreData.essentialTotal = FYT_ItemCatalog.GetEssentialCount();

        List<string> allEssentials = FYT_ItemCatalog.GetEssentialNames();
        HashSet<string> collectedSet = new HashSet<string>(itemNames);
        foreach (string essential in allEssentials)
        {
            if (!collectedSet.Contains(essential))
            {
                FYT_ScoreData.missedEssentials.Add(essential);
            }
        }

        if (FYT_ScoreData.essentialTotal > 0)
        {
            FYT_ScoreData.essentialScore =
                (float)FYT_ScoreData.essentialCollected / FYT_ScoreData.essentialTotal;
        }
    }
}
