using System.Collections;
using System.Collections.Generic;
using UnityEngine;



//public enum MaterialType { Wooden, Metal, Brick, FireProof };
public enum HousePartType { Roof, Wall, Floor, Door, Gable, Fence, Window, Gutter, Vent, Drain, Ground, Stairs};

public enum GameStage {BeforeGame, RoundStart, Fire, Competition, RoundEnd, GameEnd};
public enum BurnStage { Igniting, Burning, Smoldering, BurnedOut};

public enum MaterialClass { A, B, C, F };

public static class MaterialClassExtensions
{
    public static float GetMaterialScore(this MaterialClass materialClass)
    {
        return materialClass switch
        {
            MaterialClass.A => 10f,
            MaterialClass.B => 6f,
            MaterialClass.C => 4f,
            MaterialClass.F => 1f,
            _ => 0f
        };
    }

    public static float GetHousePartWeight(this HousePartType housePart)
    {
        return housePart switch { 
            HousePartType.Roof => 1.5f,
            HousePartType.Wall => 1.2f,
            HousePartType.Door => 0.6f,
            HousePartType.Fence => 0.3f,
            HousePartType.Window => 0.6f,
            HousePartType.Gutter => 0.2f,
            HousePartType.Drain => 0.2f,
            HousePartType.Vent => 0.3f,
            HousePartType.Ground => 1f,
            HousePartType.Stairs => 0.2f,
            _ => 1f
        };
    }
}