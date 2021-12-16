using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PixelCrushers.DialogueSystem;
using Sirenix.OdinInspector;
using UnityEngine.UI;

public class SubtitlePlacement : MonoBehaviour
{
    // [SerializeField, FoldoutGroup("References")]
    private StandardUISubtitlePanel subtitlePanel;
    // [SerializeField, FoldoutGroup("References")]
    private Image backgroundImage;
    [SerializeField, FoldoutGroup("References")]
    private Sprite SMSBackgroundLeft;
    [SerializeField, FoldoutGroup("References")]
    private Sprite SMSBackgroundRight;
    
    [System.Serializable]
    private class ActorSubtitleData
    {
        public string actor = "null";
        public Color color = Color.grey;

        public ActorSubtitleData(string name)
        {
            this.actor = name;
        }
    }

    [SerializeField] private List<ActorSubtitleData> actorData = new List<ActorSubtitleData>();
    [SerializeField] private ActorSubtitleData defaultData;
    
    // Start is called before the first frame update
    void Start()
    {
        subtitlePanel = GetComponent<StandardUISubtitlePanel>();
        backgroundImage = GetComponent<Image>();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (DialogueManager.IsConversationActive)
        {
            string speakerName = subtitlePanel.currentSubtitle.speakerInfo.nameInDatabase;
            
            ActorSubtitleData speakerSubtitleData = actorData.Find((x) => x.actor == speakerName) ?? defaultData;

            backgroundImage.color = speakerSubtitleData.color;
        }
    }
}
