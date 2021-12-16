using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PixelCrushers.DialogueSystem;
using Sirenix.OdinInspector;
using UnityEngine.UI;
using CharacterInfo = PixelCrushers.DialogueSystem.CharacterInfo;

public class SubtitlePlacement : MonoBehaviour
{
    // [SerializeField, FoldoutGroup("References")]
    private RectTransform rectTransform;
    // [SerializeField, FoldoutGroup("References")]
    private StandardUISubtitlePanel subtitlePanel;
    // [SerializeField, FoldoutGroup("References")]
    private Image backgroundImage;
    [SerializeField, FoldoutGroup("References")]
    private Sprite SMSBackgroundLeft;
    [SerializeField, FoldoutGroup("References")]
    private Sprite SMSBackgroundRight;
    [SerializeField, FoldoutGroup("References")]
    private HorizontalLayoutGroup layoutGroup;

    [SerializeField] private Transform speaker = null;

    private RectTransformData originalLocation;
    
    [System.Serializable]
    private class ActorSubtitleData
    {
        public string actor = "null";
        public Color color = Color.grey;
        public bool repositionSubtitle = false;

        public ActorSubtitleData(string name)
        {
            this.actor = name;
        }
    }
    
    private class RectTransformData
    {
        public Vector2 pivot;
        public Vector2 anchoredPosition;
        public Vector2 anchorMin;
        public Vector2 anchorMax;
        public Vector3 position;

        public RectTransformData(Vector2 pivot, Vector2 anchoredPosition, Vector2 anchorMin, Vector2 anchorMax, Vector3 position)
        {
            this.pivot = pivot;
            this.anchoredPosition = anchoredPosition;
            this.anchorMin = anchorMin;
            this.anchorMax = anchorMax;
            this.position = position;
        }
    }

    void SetRectTransform(RectTransform target, RectTransformData original)
    {
        target.pivot = original.pivot;
        target.anchoredPosition = original.anchoredPosition;
        target.anchorMin = original.anchorMin;
        target.anchorMax = original.anchorMax;
    }

    [SerializeField] private List<ActorSubtitleData> actorData = new List<ActorSubtitleData>();
    [SerializeField] private ActorSubtitleData defaultData;
    
    // Start is called before the first frame update
    void Start()
    {
        subtitlePanel = GetComponent<StandardUISubtitlePanel>();
        backgroundImage = GetComponent<Image>();
        layoutGroup = GetComponent<HorizontalLayoutGroup>();
        rectTransform = (RectTransform) transform;
        
        originalLocation = new RectTransformData(rectTransform.pivot, rectTransform.anchoredPosition, rectTransform.anchorMin, rectTransform.anchorMax, rectTransform.position);
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (DialogueManager.IsConversationActive)
        {
            CharacterInfo speakerInfo
                = subtitlePanel.currentSubtitle.speakerInfo;
            speaker = speakerInfo.transform;
            
            ActorSubtitleData speakerSubtitleData = actorData.Find((x) => x.actor == speakerInfo.nameInDatabase) ?? defaultData;

            backgroundImage.color = speakerSubtitleData.color;

            if (speaker != null && speakerSubtitleData.repositionSubtitle == true)
            {
                Vector2 speakerScreenPos = RectTransformUtility.WorldToScreenPoint(Camera.main, speaker.position);
                bool isSpeakerLeft = speakerScreenPos.x < (Screen.width/2);
                Debug.Log("Speaker: " + speaker + "         ScreenPos" + speakerScreenPos + "            isLeft: " + isSpeakerLeft);

                if (isSpeakerLeft)
                {
                    // Use right background sprite
                    backgroundImage.sprite = SMSBackgroundLeft;
                    rectTransform.anchorMin = Vector2.zero;
                    rectTransform.anchorMax = Vector2.zero;
                    rectTransform.pivot = Vector2.zero;
                    rectTransform.position = new Vector3(speakerScreenPos.x, speakerScreenPos.y, rectTransform.position.z);
                    layoutGroup.reverseArrangement = false;
                }
                else
                {
                    // Use left background sprite
                    backgroundImage.sprite = SMSBackgroundRight;
                    rectTransform.anchorMin = Vector2.right;
                    rectTransform.anchorMax = Vector2.right;
                    rectTransform.pivot = Vector2.right;
                    rectTransform.position = new Vector3(speakerScreenPos.x, speakerScreenPos.y, rectTransform.position.z);
                    layoutGroup.reverseArrangement = true;
                }
            }
            else
            {
                SetRectTransform(rectTransform, originalLocation);
                layoutGroup.reverseArrangement = false;
                backgroundImage.sprite = SMSBackgroundLeft;
            }
        }
    }
}
