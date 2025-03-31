using UnityEngine;

[CreateAssetMenu(fileName = "MessageBubbleConfig", menuName = "UI/MessageBubbleConfig")]
public class MessageBubbleConfig : ScriptableObject
{
    public Sprite playerBGShort, playerBGLong,playerTypingIndicator;
    public Sprite choiceSprite,descriptionBG;

    public Sprite firefighterBGShort, firefighterBGLong, firefighterNamePlate,firefighterTypingIndicator;
    public Sprite petHomeBGShort, petHomeBGLong, petHomeNamePlate,petTypingIndicator;
    public Sprite elderlyHomeBGShort, elderlyHomeBGLong, elderlyHomeNamePlate, elderlyTypingIndicator;
    public Sprite twoCarsHomeBGShort, twoCarsHomeBGLong, twoCarsHomeNamePlate, twoCarTypingIndicator;
    public Sprite wuiHomeBGShort, wuiHomeBGLong, wuiHomeNamePlate, wuiTypingIndicator;
    public Sprite kidsHomeBGShort, kidsHomeBGLong, kidsHomeNamePlate, kidsTypingIndicator;

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
