using UnityEngine;

[CreateAssetMenu(fileName = "MessageBubbleConfig", menuName = "UI/MessageBubbleConfig")]
public class MessageBubbleConfig : ScriptableObject
{
    public Sprite playerBGShort, playerBGLong;
    public Sprite choiceSprite;

    public Sprite firefighterBGShort, firefighterBGLong, firefighterNamePlate;
    public Sprite petHomeBGShort, petHomeBGLong, petHomeNamePlate;
    public Sprite elderlyHomeBGShort, elderlyHomeBGLong, elderlyHomeNamePlate;
    public Sprite twoCarsHomeBGShort, twoCarsHomeBGLong, twoCarsHomeNamePlate;
    public Sprite wuiHomeBGShort, wuiHomeBGLong, wuiHomeNamePlate;
    public Sprite kidsHomeBGShort, kidsHomeBGLong, kidsHomeNamePlate;

    private static MessageBubbleConfig _instance;

    public static MessageBubbleConfig Instance
    {
        get
        {
            if (_instance == null)
                _instance = Resources.Load<MessageBubbleConfig>("AlertTheCity/MessageBubbleConfig");
            return _instance;
        }
    }
}
