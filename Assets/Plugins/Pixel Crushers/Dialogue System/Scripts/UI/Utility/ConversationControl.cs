using UnityEngine;

/// <summary>
/// Provides AutoPlay and SkipAll functionality. To add "Auto Play" and/or 
/// "Skip All" buttons that advances the current conversation:
/// 
/// - Add this script to the dialogue UI.
/// - Add Auto Play and/or Skip All buttons to your subtitle panel(s). Configure 
/// their OnClick() events to call the dialogue UI's ConversationControl.ToggleAutoPlay 
/// and/or ConversationControl.SkipAll methods.
/// </summary>

namespace PixelCrushers.DialogueSystem
{
	
	[AddComponentMenu("")] // Use wrapper.
	[RequireComponent(typeof(AbstractDialogueUI))]
	public class ConversationControl : MonoBehaviour // Add to dialogue UI. Connect to Skip All and Auto Play buttons.
	{
		public bool skipAll;

		private AbstractDialogueUI dialogueUI;

		private void Awake()
		{
			dialogueUI = GetComponent<AbstractDialogueUI>() ?? FindObjectOfType<AbstractDialogueUI>();
		}

		public void ToggleAutoPlay()
		{
			var mode = DialogueManager.displaySettings.subtitleSettings.continueButton;
			var newMode = (mode == DisplaySettings.SubtitleSettings.ContinueButtonMode.Never) ? DisplaySettings.SubtitleSettings.ContinueButtonMode.Always : DisplaySettings.SubtitleSettings.ContinueButtonMode.Never;
			DialogueManager.displaySettings.subtitleSettings.continueButton = newMode;
			if (newMode == DisplaySettings.SubtitleSettings.ContinueButtonMode.Never) dialogueUI.OnContinueConversation();
		}

		public void SkipAll()
		{
			skipAll = true;
			if (dialogueUI != null) dialogueUI.OnContinueConversation();
		}

		public void OnConversationStart(Transform actor)
		{
			skipAll = false;
		}

		public void OnConversationLine(Subtitle subtitle)
		{
			if (skipAll)
			{
				subtitle.sequence = "Continue(); " + subtitle.sequence;
			}
		}

		public void OnConversationResponseMenu(Response[] responses)
		{
			if (skipAll)
			{
				skipAll = false;
				if (dialogueUI != null) dialogueUI.ShowSubtitle(DialogueManager.currentConversationState.subtitle);
			}
		}
	}
}
