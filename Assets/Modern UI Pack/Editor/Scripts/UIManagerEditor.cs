using UnityEngine;
using UnityEditor;
using UnityEditor.Presets;

namespace Michsky.UI.ModernUIPack
{
    [CustomEditor(typeof(UIManager))]
    [System.Serializable]
    public class UIManagerEditor : Editor
    {
        GUISkin customSkin;
        protected static string buildID = "B16-20211205";
        protected static float foldoutItemSpace = 2;
        protected static float foldoutTopSpace = 5;
        protected static float foldoutBottomSpace = 2;

        protected static bool showAnimatedIcon = false;
        protected static bool showButton = false;
        protected static bool showContext = false;
        protected static bool showDropdown = false;
        protected static bool showHorSelector = false;
        protected static bool showInputField = false;
        protected static bool showModalWindow = false;
        protected static bool showNotification = false;
        protected static bool showProgressBar = false;
        protected static bool showScrollbar = false;
        protected static bool showSlider = false;
        protected static bool showSwitch = false;
        protected static bool showToggle = false;
        protected static bool showTooltip = false;

        private void OnEnable() 
        {
            if (EditorGUIUtility.isProSkin == true) { customSkin = (GUISkin)Resources.Load("Editor\\MUI Skin Dark"); }
            else { customSkin = (GUISkin)Resources.Load("Editor\\MUI Skin Light"); }
        }

        public override void OnInspectorGUI()
        {
            if (customSkin == null)
            {
                EditorGUILayout.HelpBox("Editor variables are missing. You can manually fix this by deleting " +
                    "Modern UI Pack > Resources folder and then re-import the package. \n\nIf you're still seeing this " +
                    "dialog even after the re-import, contact me with this ID: " + buildID, MessageType.Error);
              
                if (GUILayout.Button("Contact")) { Email(); }
                return;
            }

            // Foldout style
            GUIStyle foldoutStyle = customSkin.FindStyle("UIM Foldout");

            // UIM Header
            GUILayout.Space(8);
            GUILayout.Box(new GUIContent(""), customSkin.FindStyle("UIM Header"));
            GUILayout.BeginVertical(EditorStyles.helpBox);

            // Animated Icon
            var animatedIconColor = serializedObject.FindProperty("animatedIconColor");
          
            GUILayout.Space(foldoutTopSpace);
            GUILayout.BeginHorizontal();
            showAnimatedIcon = EditorGUILayout.Foldout(showAnimatedIcon, "Animated Icon", true, foldoutStyle);
            showAnimatedIcon = GUILayout.Toggle(showAnimatedIcon, new GUIContent(""), customSkin.FindStyle("Toggle Helper"));
            GUILayout.EndHorizontal();
            GUILayout.Space(foldoutBottomSpace);

            if (showAnimatedIcon)
            {
                GUILayout.BeginHorizontal(EditorStyles.helpBox);

                EditorGUILayout.LabelField(new GUIContent("Color"), customSkin.FindStyle("Text"), GUILayout.Width(120));
                EditorGUILayout.PropertyField(animatedIconColor, new GUIContent(""));

                GUILayout.EndHorizontal();
            }

            GUILayout.EndVertical();
            GUILayout.Space(foldoutItemSpace);
            GUILayout.BeginVertical(EditorStyles.helpBox);

            // Button
            var buttonTheme = serializedObject.FindProperty("buttonThemeType");
            var buttonFont = serializedObject.FindProperty("buttonFont");
            var buttonFontSize = serializedObject.FindProperty("buttonFontSize");
            var buttonBorderColor = serializedObject.FindProperty("buttonBorderColor");
            var buttonFilledColor = serializedObject.FindProperty("buttonFilledColor");
            var buttonTextBasicColor = serializedObject.FindProperty("buttonTextBasicColor");
            var buttonTextColor = serializedObject.FindProperty("buttonTextColor");
            var buttonTextHighlightedColor = serializedObject.FindProperty("buttonTextHighlightedColor");
            var buttonIconBasicColor = serializedObject.FindProperty("buttonIconBasicColor");
            var buttonIconColor = serializedObject.FindProperty("buttonIconColor");
            var buttonIconHighlightedColor = serializedObject.FindProperty("buttonIconHighlightedColor");

            GUILayout.Space(foldoutTopSpace);
            GUILayout.BeginHorizontal();
            showButton = EditorGUILayout.Foldout(showButton, "Button", true, foldoutStyle);
            showButton = GUILayout.Toggle(showButton, new GUIContent(""), customSkin.FindStyle("Toggle Helper"));
            GUILayout.EndHorizontal();
            GUILayout.Space(foldoutBottomSpace);

            if (showButton && buttonTheme.enumValueIndex == 0)
            {
                GUILayout.BeginHorizontal(EditorStyles.helpBox);

                EditorGUILayout.LabelField(new GUIContent("Theme Type"), customSkin.FindStyle("Text"), GUILayout.Width(120));
                EditorGUILayout.PropertyField(buttonTheme, new GUIContent(""));

                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal(EditorStyles.helpBox);

                EditorGUILayout.LabelField(new GUIContent("Font"), customSkin.FindStyle("Text"), GUILayout.Width(120));
                EditorGUILayout.PropertyField(buttonFontSize, new GUIContent(""), GUILayout.Width(40));
                EditorGUILayout.PropertyField(buttonFont, new GUIContent(""));

                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal(EditorStyles.helpBox);

                EditorGUILayout.LabelField(new GUIContent("Primary Color"), customSkin.FindStyle("Text"), GUILayout.Width(120));
                EditorGUILayout.PropertyField(buttonBorderColor, new GUIContent(""));

                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal(EditorStyles.helpBox);

                EditorGUILayout.LabelField(new GUIContent("Secondary Color"), customSkin.FindStyle("Text"), GUILayout.Width(120));
                EditorGUILayout.PropertyField(buttonFilledColor, new GUIContent(""));

                GUILayout.EndHorizontal();
            }

            if (showButton && buttonTheme.enumValueIndex == 1)
            {
                GUILayout.BeginHorizontal(EditorStyles.helpBox);

                EditorGUILayout.LabelField(new GUIContent("Theme Type"), customSkin.FindStyle("Text"), GUILayout.Width(120));
                EditorGUILayout.PropertyField(buttonTheme, new GUIContent(""));

                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal(EditorStyles.helpBox);

                EditorGUILayout.LabelField(new GUIContent("Font"), customSkin.FindStyle("Text"), GUILayout.Width(120));
                EditorGUILayout.PropertyField(buttonFontSize, new GUIContent(""), GUILayout.Width(40));
                EditorGUILayout.PropertyField(buttonFont, new GUIContent(""));

                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal(EditorStyles.helpBox);

                EditorGUILayout.LabelField(new GUIContent("Border Color"), customSkin.FindStyle("Text"), GUILayout.Width(120));
                EditorGUILayout.PropertyField(buttonBorderColor, new GUIContent(""));

                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal(EditorStyles.helpBox);

                EditorGUILayout.LabelField(new GUIContent("Filled Color"), customSkin.FindStyle("Text"), GUILayout.Width(120));
                EditorGUILayout.PropertyField(buttonFilledColor, new GUIContent(""));

                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal(EditorStyles.helpBox);

                EditorGUILayout.LabelField(new GUIContent("Text Basic Color"), customSkin.FindStyle("Text"), GUILayout.Width(120));
                EditorGUILayout.PropertyField(buttonTextBasicColor, new GUIContent(""));

                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal(EditorStyles.helpBox);

                EditorGUILayout.LabelField(new GUIContent("Text Color"), customSkin.FindStyle("Text"), GUILayout.Width(120));
                EditorGUILayout.PropertyField(buttonTextColor, new GUIContent(""));

                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal(EditorStyles.helpBox);

                EditorGUILayout.LabelField(new GUIContent("Text Hover Color"), customSkin.FindStyle("Text"), GUILayout.Width(120));
                EditorGUILayout.PropertyField(buttonTextHighlightedColor, new GUIContent(""));

                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal(EditorStyles.helpBox);

                EditorGUILayout.LabelField(new GUIContent("Icon Basic Color"), customSkin.FindStyle("Text"), GUILayout.Width(120));
                EditorGUILayout.PropertyField(buttonIconBasicColor, new GUIContent(""));

                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal(EditorStyles.helpBox);

                EditorGUILayout.LabelField(new GUIContent("Icon Color"), customSkin.FindStyle("Text"), GUILayout.Width(120));
                EditorGUILayout.PropertyField(buttonIconColor, new GUIContent(""));

                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal(EditorStyles.helpBox);

                EditorGUILayout.LabelField(new GUIContent("Icon Hover Color"), customSkin.FindStyle("Text"), GUILayout.Width(120));
                EditorGUILayout.PropertyField(buttonIconHighlightedColor, new GUIContent(""));

                GUILayout.EndHorizontal();
            }

            GUILayout.EndVertical();
            GUILayout.Space(foldoutItemSpace);
            GUILayout.BeginVertical(EditorStyles.helpBox);

            // Context Menu
            var contextBackgroundColor = serializedObject.FindProperty("contextBackgroundColor");

            GUILayout.Space(foldoutTopSpace);
            GUILayout.BeginHorizontal();
            showContext = EditorGUILayout.Foldout(showContext, "Context Menu", true, foldoutStyle);
            showContext = GUILayout.Toggle(showContext, new GUIContent(""), customSkin.FindStyle("Toggle Helper"));
            GUILayout.EndHorizontal();
            GUILayout.Space(foldoutBottomSpace);

            if (showContext)
            {
                GUILayout.BeginHorizontal(EditorStyles.helpBox);

                EditorGUILayout.LabelField(new GUIContent("Background Color"), customSkin.FindStyle("Text"), GUILayout.Width(120));
                EditorGUILayout.PropertyField(contextBackgroundColor, new GUIContent(""));

                GUILayout.EndHorizontal();
                EditorGUILayout.HelpBox("Context Menu is currently in beta, expect major changes in future releases.", MessageType.Info);
            }

            GUILayout.EndVertical();
            GUILayout.Space(foldoutItemSpace);
            GUILayout.BeginVertical(EditorStyles.helpBox);

            // Dropdown
            var dropdownTheme = serializedObject.FindProperty("dropdownThemeType");
            var dropdownAnimationType = serializedObject.FindProperty("dropdownAnimationType");
            var dropdownFont = serializedObject.FindProperty("dropdownFont");
            var dropdownFontSize = serializedObject.FindProperty("dropdownFontSize");
            var dropdownItemFont = serializedObject.FindProperty("dropdownItemFont");
            var dropdownItemFontSize = serializedObject.FindProperty("dropdownItemFontSize");
            var dropdownColor = serializedObject.FindProperty("dropdownColor");
            var dropdownTextColor = serializedObject.FindProperty("dropdownTextColor");
            var dropdownIconColor = serializedObject.FindProperty("dropdownIconColor");
            var dropdownItemColor = serializedObject.FindProperty("dropdownItemColor");
            var dropdownItemTextColor = serializedObject.FindProperty("dropdownItemTextColor");
            var dropdownItemIconColor = serializedObject.FindProperty("dropdownItemIconColor");

            GUILayout.Space(foldoutTopSpace);
            GUILayout.BeginHorizontal();
            showDropdown = EditorGUILayout.Foldout(showDropdown, "Dropdown", true, foldoutStyle);
            showDropdown = GUILayout.Toggle(showDropdown, new GUIContent(""), customSkin.FindStyle("Toggle Helper"));
            GUILayout.EndHorizontal();
            GUILayout.Space(foldoutBottomSpace);

            if (showDropdown && dropdownTheme.enumValueIndex == 0)
            {
                GUILayout.BeginHorizontal(EditorStyles.helpBox);

                EditorGUILayout.LabelField(new GUIContent("Theme Type"), customSkin.FindStyle("Text"), GUILayout.Width(120));
                EditorGUILayout.PropertyField(dropdownTheme, new GUIContent(""));

                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal(EditorStyles.helpBox);

                EditorGUILayout.LabelField(new GUIContent("Animation Type"), customSkin.FindStyle("Text"), GUILayout.Width(120));
                EditorGUILayout.PropertyField(dropdownAnimationType, new GUIContent(""));

                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal(EditorStyles.helpBox);

                EditorGUILayout.LabelField(new GUIContent("Font"), customSkin.FindStyle("Text"), GUILayout.Width(120));
                EditorGUILayout.PropertyField(dropdownFontSize, new GUIContent(""), GUILayout.Width(40));
                EditorGUILayout.PropertyField(dropdownFont, new GUIContent(""));

                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal(EditorStyles.helpBox);

                EditorGUILayout.LabelField(new GUIContent("Primary Color"), customSkin.FindStyle("Text"), GUILayout.Width(120));
                EditorGUILayout.PropertyField(dropdownColor, new GUIContent(""));

                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal(EditorStyles.helpBox);

                EditorGUILayout.LabelField(new GUIContent("Secondary Color"), customSkin.FindStyle("Text"), GUILayout.Width(120));
                EditorGUILayout.PropertyField(dropdownTextColor, new GUIContent(""));

                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal(EditorStyles.helpBox);

                EditorGUILayout.LabelField(new GUIContent("Item Background"), customSkin.FindStyle("Text"), GUILayout.Width(120));
                EditorGUILayout.PropertyField(dropdownItemColor, new GUIContent(""));

                GUILayout.EndHorizontal();
                EditorGUILayout.HelpBox("Item values will be applied at start.", MessageType.Info);
            }

            if (showDropdown && dropdownTheme.enumValueIndex == 1)
            {
                GUILayout.BeginHorizontal(EditorStyles.helpBox);

                EditorGUILayout.LabelField(new GUIContent("Theme Type"), customSkin.FindStyle("Text"), GUILayout.Width(120));
                EditorGUILayout.PropertyField(dropdownTheme, new GUIContent(""));

                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal(EditorStyles.helpBox);

                EditorGUILayout.LabelField(new GUIContent("Animation Type"), customSkin.FindStyle("Text"), GUILayout.Width(120));
                EditorGUILayout.PropertyField(dropdownAnimationType, new GUIContent(""));

                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal(EditorStyles.helpBox);

                EditorGUILayout.LabelField(new GUIContent("Font"), customSkin.FindStyle("Text"), GUILayout.Width(120));
                EditorGUILayout.PropertyField(dropdownFontSize, new GUIContent(""), GUILayout.Width(40));
                EditorGUILayout.PropertyField(dropdownFont, new GUIContent(""));

                GUILayout.EndHorizontal();;
                GUILayout.BeginHorizontal(EditorStyles.helpBox);

                EditorGUILayout.LabelField(new GUIContent("Item Font"), customSkin.FindStyle("Text"), GUILayout.Width(120));
                EditorGUILayout.PropertyField(dropdownItemFontSize, new GUIContent(""), GUILayout.Width(40));
                EditorGUILayout.PropertyField(dropdownItemFont, new GUIContent(""));

                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal(EditorStyles.helpBox);

                EditorGUILayout.LabelField(new GUIContent("Color"), customSkin.FindStyle("Text"), GUILayout.Width(120));
                EditorGUILayout.PropertyField(dropdownColor, new GUIContent(""));

                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal(EditorStyles.helpBox);

                EditorGUILayout.LabelField(new GUIContent("Text Color"), customSkin.FindStyle("Text"), GUILayout.Width(120));
                EditorGUILayout.PropertyField(dropdownTextColor, new GUIContent(""));

                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal(EditorStyles.helpBox);

                EditorGUILayout.LabelField(new GUIContent("Icon Color"), customSkin.FindStyle("Text"), GUILayout.Width(120));
                EditorGUILayout.PropertyField(dropdownIconColor, new GUIContent(""));

                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal(EditorStyles.helpBox);

                EditorGUILayout.LabelField(new GUIContent("Item Color"), customSkin.FindStyle("Text"), GUILayout.Width(120));
                EditorGUILayout.PropertyField(dropdownItemColor, new GUIContent(""));

                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal(EditorStyles.helpBox);

                EditorGUILayout.LabelField(new GUIContent("Item Text Color"), customSkin.FindStyle("Text"), GUILayout.Width(120));
                EditorGUILayout.PropertyField(dropdownItemTextColor, new GUIContent(""));

                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal(EditorStyles.helpBox);

                EditorGUILayout.LabelField(new GUIContent("Item Icon Color"), customSkin.FindStyle("Text"), GUILayout.Width(120));
                EditorGUILayout.PropertyField(dropdownItemIconColor, new GUIContent(""));

                GUILayout.EndHorizontal();
                EditorGUILayout.HelpBox("Item values will be applied at start.", MessageType.Info);
            }

            GUILayout.EndVertical();
            GUILayout.Space(foldoutItemSpace);
            GUILayout.BeginVertical(EditorStyles.helpBox);

            // Horizontal Selector
            var selectorFont = serializedObject.FindProperty("selectorFont");
            var hSelectorFontSize = serializedObject.FindProperty("hSelectorFontSize");
            var selectorColor = serializedObject.FindProperty("selectorColor");
            var selectorHighlightedColor = serializedObject.FindProperty("selectorHighlightedColor");
            var hSelectorInvertAnimation = serializedObject.FindProperty("hSelectorInvertAnimation");
            var hSelectorLoopSelection = serializedObject.FindProperty("hSelectorLoopSelection");

            GUILayout.Space(foldoutTopSpace);
            GUILayout.BeginHorizontal();
            showHorSelector = EditorGUILayout.Foldout(showHorSelector, "Horizontal Selector", true, foldoutStyle);
            showHorSelector = GUILayout.Toggle(showHorSelector, new GUIContent(""), customSkin.FindStyle("Toggle Helper"));
            GUILayout.EndHorizontal();
            GUILayout.Space(foldoutBottomSpace);

            if (showHorSelector)
            {
                GUILayout.BeginHorizontal(EditorStyles.helpBox);

                EditorGUILayout.LabelField(new GUIContent("Font"), customSkin.FindStyle("Text"), GUILayout.Width(120));
                EditorGUILayout.PropertyField(hSelectorFontSize, new GUIContent(""), GUILayout.Width(40));
                EditorGUILayout.PropertyField(selectorFont, new GUIContent(""));

                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal(EditorStyles.helpBox);

                EditorGUILayout.LabelField(new GUIContent("Color"), customSkin.FindStyle("Text"), GUILayout.Width(120));
                EditorGUILayout.PropertyField(selectorColor, new GUIContent(""));

                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal(EditorStyles.helpBox);

                EditorGUILayout.LabelField(new GUIContent("Highlighted Color"), customSkin.FindStyle("Text"), GUILayout.Width(120));
                EditorGUILayout.PropertyField(selectorHighlightedColor, new GUIContent(""));

                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal(EditorStyles.helpBox);

                EditorGUILayout.LabelField(new GUIContent("Invert Animation"), customSkin.FindStyle("Text"), GUILayout.Width(120));
                hSelectorInvertAnimation.boolValue = GUILayout.Toggle(hSelectorInvertAnimation.boolValue, new GUIContent(""), customSkin.FindStyle("Toggle"));
                hSelectorInvertAnimation.boolValue = GUILayout.Toggle(hSelectorInvertAnimation.boolValue, new GUIContent(""), customSkin.FindStyle("Toggle Helper"));

                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal(EditorStyles.helpBox);

                EditorGUILayout.LabelField(new GUIContent("Loop Selection"), customSkin.FindStyle("Text"), GUILayout.Width(120));
                hSelectorLoopSelection.boolValue = GUILayout.Toggle(hSelectorLoopSelection.boolValue, new GUIContent(""), customSkin.FindStyle("Toggle"));
                hSelectorLoopSelection.boolValue = GUILayout.Toggle(hSelectorLoopSelection.boolValue, new GUIContent(""), customSkin.FindStyle("Toggle Helper"));

                GUILayout.EndHorizontal();
            }

            GUILayout.EndVertical();
            GUILayout.Space(2);
            GUILayout.BeginVertical(EditorStyles.helpBox);

            // Input Field
            var inputFieldFont = serializedObject.FindProperty("inputFieldFont");
            var inputFieldFontSize = serializedObject.FindProperty("inputFieldFontSize");
            var inputFieldColor = serializedObject.FindProperty("inputFieldColor");

            GUILayout.Space(foldoutTopSpace);
            GUILayout.BeginHorizontal();
            showInputField = EditorGUILayout.Foldout(showInputField, "Input Field", true, foldoutStyle);
            showInputField = GUILayout.Toggle(showInputField, new GUIContent(""), customSkin.FindStyle("Toggle Helper"));
            GUILayout.EndHorizontal();
            GUILayout.Space(foldoutBottomSpace);

            if (showInputField)
            {
                GUILayout.BeginHorizontal(EditorStyles.helpBox);

                EditorGUILayout.LabelField(new GUIContent("Font"), customSkin.FindStyle("Text"), GUILayout.Width(120));
                EditorGUILayout.PropertyField(inputFieldFontSize, new GUIContent(""), GUILayout.Width(40));
                EditorGUILayout.PropertyField(inputFieldFont, new GUIContent(""));

                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal(EditorStyles.helpBox);

                EditorGUILayout.LabelField(new GUIContent("Color"), customSkin.FindStyle("Text"), GUILayout.Width(120));
                EditorGUILayout.PropertyField(inputFieldColor, new GUIContent(""));

                GUILayout.EndHorizontal();
            }

            GUILayout.EndVertical();
            GUILayout.Space(foldoutItemSpace);
            GUILayout.BeginVertical(EditorStyles.helpBox);

            // Modal Window
            var modalWindowTitleFont = serializedObject.FindProperty("modalWindowTitleFont");
            var modalWindowContentFont = serializedObject.FindProperty("modalWindowContentFont");
            var modalWindowTitleColor = serializedObject.FindProperty("modalWindowTitleColor");
            var modalWindowDescriptionColor = serializedObject.FindProperty("modalWindowDescriptionColor");
            var modalWindowIconColor = serializedObject.FindProperty("modalWindowIconColor");
            var modalWindowBackgroundColor = serializedObject.FindProperty("modalWindowBackgroundColor");
            var modalWindowContentPanelColor = serializedObject.FindProperty("modalWindowContentPanelColor");

            GUILayout.Space(foldoutTopSpace);
            GUILayout.BeginHorizontal();
            showModalWindow = EditorGUILayout.Foldout(showModalWindow, "Modal Window", true, foldoutStyle);
            showModalWindow = GUILayout.Toggle(showModalWindow, new GUIContent(""), customSkin.FindStyle("Toggle Helper"));
            GUILayout.EndHorizontal();
            GUILayout.Space(foldoutBottomSpace);

            if (showModalWindow)
            {
                GUILayout.BeginHorizontal(EditorStyles.helpBox);

                EditorGUILayout.LabelField(new GUIContent("Title Font"), customSkin.FindStyle("Text"), GUILayout.Width(120));
                EditorGUILayout.PropertyField(modalWindowTitleFont, new GUIContent(""));

                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal(EditorStyles.helpBox);

                EditorGUILayout.LabelField(new GUIContent("Content Font"), customSkin.FindStyle("Text"), GUILayout.Width(120));
                EditorGUILayout.PropertyField(modalWindowContentFont, new GUIContent(""));

                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal(EditorStyles.helpBox);

                EditorGUILayout.LabelField(new GUIContent("Title Color"), customSkin.FindStyle("Text"), GUILayout.Width(120));
                EditorGUILayout.PropertyField(modalWindowTitleColor, new GUIContent(""));

                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal(EditorStyles.helpBox);

                EditorGUILayout.LabelField(new GUIContent("Description Color"), customSkin.FindStyle("Text"), GUILayout.Width(120));
                EditorGUILayout.PropertyField(modalWindowDescriptionColor, new GUIContent(""));

                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal(EditorStyles.helpBox);

                EditorGUILayout.LabelField(new GUIContent("Icon Color"), customSkin.FindStyle("Text"), GUILayout.Width(120));
                EditorGUILayout.PropertyField(modalWindowIconColor, new GUIContent(""));

                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal(EditorStyles.helpBox);

                EditorGUILayout.LabelField(new GUIContent("Background Color"), customSkin.FindStyle("Text"), GUILayout.Width(120));
                EditorGUILayout.PropertyField(modalWindowBackgroundColor, new GUIContent(""));

                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal(EditorStyles.helpBox);

                EditorGUILayout.LabelField(new GUIContent("Content Panel Color"), customSkin.FindStyle("Text"), GUILayout.Width(120));
                EditorGUILayout.PropertyField(modalWindowContentPanelColor, new GUIContent(""));

                GUILayout.EndHorizontal();
                EditorGUILayout.HelpBox("These values will only affect 'Style 1 - Standard' window.", MessageType.Info);
            }

            GUILayout.EndVertical();
            GUILayout.Space(foldoutItemSpace);
            GUILayout.BeginVertical(EditorStyles.helpBox);

            // Notification
            var notificationTitleFont = serializedObject.FindProperty("notificationTitleFont");
            var notificationTitleFontSize = serializedObject.FindProperty("notificationTitleFontSize");
            var notificationDescriptionFont = serializedObject.FindProperty("notificationDescriptionFont");
            var notificationDescriptionFontSize = serializedObject.FindProperty("notificationDescriptionFontSize");
            var notificationBackgroundColor = serializedObject.FindProperty("notificationBackgroundColor");
            var notificationTitleColor = serializedObject.FindProperty("notificationTitleColor");
            var notificationDescriptionColor = serializedObject.FindProperty("notificationDescriptionColor");
            var notificationIconColor = serializedObject.FindProperty("notificationIconColor");

            GUILayout.Space(foldoutTopSpace);
            GUILayout.BeginHorizontal();
            showNotification = EditorGUILayout.Foldout(showNotification, "Notification", true, foldoutStyle);
            showNotification = GUILayout.Toggle(showNotification, new GUIContent(""), customSkin.FindStyle("Toggle Helper"));
            GUILayout.EndHorizontal();
            GUILayout.Space(foldoutBottomSpace);

            if (showNotification)
            {
                GUILayout.BeginHorizontal(EditorStyles.helpBox);

                EditorGUILayout.LabelField(new GUIContent("Title Font"), customSkin.FindStyle("Text"), GUILayout.Width(120));
                EditorGUILayout.PropertyField(notificationTitleFontSize, new GUIContent(""), GUILayout.Width(40));
                EditorGUILayout.PropertyField(notificationTitleFont, new GUIContent(""));

                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal(EditorStyles.helpBox);

                EditorGUILayout.LabelField(new GUIContent("Description Font"), customSkin.FindStyle("Text"), GUILayout.Width(120));
                EditorGUILayout.PropertyField(notificationDescriptionFontSize, new GUIContent(""), GUILayout.Width(40));
                EditorGUILayout.PropertyField(notificationDescriptionFont, new GUIContent(""));

                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal(EditorStyles.helpBox);

                EditorGUILayout.LabelField(new GUIContent("Background Color"), customSkin.FindStyle("Text"), GUILayout.Width(120));
                EditorGUILayout.PropertyField(notificationBackgroundColor, new GUIContent(""));

                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal(EditorStyles.helpBox);

                EditorGUILayout.LabelField(new GUIContent("Title Color"), customSkin.FindStyle("Text"), GUILayout.Width(120));
                EditorGUILayout.PropertyField(notificationTitleColor, new GUIContent(""));

                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal(EditorStyles.helpBox);

                EditorGUILayout.LabelField(new GUIContent("Description Color"), customSkin.FindStyle("Text"), GUILayout.Width(120));
                EditorGUILayout.PropertyField(notificationDescriptionColor, new GUIContent(""));

                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal(EditorStyles.helpBox);

                EditorGUILayout.LabelField(new GUIContent("Icon Color"), customSkin.FindStyle("Text"), GUILayout.Width(120));
                EditorGUILayout.PropertyField(notificationIconColor, new GUIContent(""));

                GUILayout.EndHorizontal();
            }

            GUILayout.EndVertical();
            GUILayout.Space(foldoutItemSpace);
            GUILayout.BeginVertical(EditorStyles.helpBox);

            // Progress Bar
            var progressBarLabelFont = serializedObject.FindProperty("progressBarLabelFont");
            var progressBarLabelFontSize = serializedObject.FindProperty("progressBarLabelFontSize");
            var progressBarColor = serializedObject.FindProperty("progressBarColor");
            var progressBarBackgroundColor = serializedObject.FindProperty("progressBarBackgroundColor");
            var progressBarLoopBackgroundColor = serializedObject.FindProperty("progressBarLoopBackgroundColor");
            var progressBarLabelColor = serializedObject.FindProperty("progressBarLabelColor");

            GUILayout.Space(foldoutTopSpace);
            GUILayout.BeginHorizontal();
            showProgressBar = EditorGUILayout.Foldout(showProgressBar, "Progress Bar", true, foldoutStyle);
            showProgressBar = GUILayout.Toggle(showProgressBar, new GUIContent(""), customSkin.FindStyle("Toggle Helper"));
            GUILayout.EndHorizontal();
            GUILayout.Space(foldoutBottomSpace);

            if (showProgressBar)
            {
                GUILayout.BeginHorizontal(EditorStyles.helpBox);

                EditorGUILayout.LabelField(new GUIContent("Label Font"), customSkin.FindStyle("Text"), GUILayout.Width(120));
                EditorGUILayout.PropertyField(progressBarLabelFontSize, new GUIContent(""), GUILayout.Width(40));
                EditorGUILayout.PropertyField(progressBarLabelFont, new GUIContent(""));

                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal(EditorStyles.helpBox);

                EditorGUILayout.LabelField(new GUIContent("Color"), customSkin.FindStyle("Text"), GUILayout.Width(120));
                EditorGUILayout.PropertyField(progressBarColor, new GUIContent(""));

                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal(EditorStyles.helpBox);

                EditorGUILayout.LabelField(new GUIContent("Label Color"), customSkin.FindStyle("Text"), GUILayout.Width(120));
                EditorGUILayout.PropertyField(progressBarLabelColor, new GUIContent(""));

                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal(EditorStyles.helpBox);

                EditorGUILayout.LabelField(new GUIContent("Background Color"), customSkin.FindStyle("Text"), GUILayout.Width(120));
                EditorGUILayout.PropertyField(progressBarBackgroundColor, new GUIContent(""));

                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal(EditorStyles.helpBox);

                EditorGUILayout.LabelField(new GUIContent("Loop BG Color"), customSkin.FindStyle("Text"), GUILayout.Width(120));
                EditorGUILayout.PropertyField(progressBarLoopBackgroundColor, new GUIContent(""));

                GUILayout.EndHorizontal();
            }

            GUILayout.EndVertical();
            GUILayout.Space(foldoutItemSpace);
            GUILayout.BeginVertical(EditorStyles.helpBox);

            // Scrollbar
            var scrollbarColor = serializedObject.FindProperty("scrollbarColor");
            var scrollbarBackgroundColor = serializedObject.FindProperty("scrollbarBackgroundColor");

            GUILayout.Space(foldoutTopSpace);
            GUILayout.BeginHorizontal();
            showScrollbar = EditorGUILayout.Foldout(showScrollbar, "Scrollbar", true, foldoutStyle);
            showScrollbar = GUILayout.Toggle(showScrollbar, new GUIContent(""), customSkin.FindStyle("Toggle Helper"));
            GUILayout.EndHorizontal();
            GUILayout.Space(foldoutBottomSpace);

            if (showScrollbar)
            {
                GUILayout.BeginHorizontal(EditorStyles.helpBox);

                EditorGUILayout.LabelField(new GUIContent("Bar Color"), customSkin.FindStyle("Text"), GUILayout.Width(120));
                EditorGUILayout.PropertyField(scrollbarColor, new GUIContent(""));

                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal(EditorStyles.helpBox);

                EditorGUILayout.LabelField(new GUIContent("Background Color"), customSkin.FindStyle("Text"), GUILayout.Width(120));
                EditorGUILayout.PropertyField(scrollbarBackgroundColor, new GUIContent(""));

                GUILayout.EndHorizontal();
            }

            GUILayout.EndVertical();
            GUILayout.Space(foldoutItemSpace);
            GUILayout.BeginVertical(EditorStyles.helpBox);

            // Slider
            var sliderThemeType = serializedObject.FindProperty("sliderThemeType");
            var sliderLabelFont = serializedObject.FindProperty("sliderLabelFont");
            var sliderLabelFontSize = serializedObject.FindProperty("sliderLabelFontSize");
            var sliderColor = serializedObject.FindProperty("sliderColor");
            var sliderLabelColor = serializedObject.FindProperty("sliderLabelColor");
            var sliderPopupLabelColor = serializedObject.FindProperty("sliderPopupLabelColor");
            var sliderHandleColor = serializedObject.FindProperty("sliderHandleColor");
            var sliderBackgroundColor = serializedObject.FindProperty("sliderBackgroundColor");

            GUILayout.Space(foldoutTopSpace);
            GUILayout.BeginHorizontal();
            showSlider = EditorGUILayout.Foldout(showSlider, "Slider", true, foldoutStyle);
            showSlider = GUILayout.Toggle(showSlider, new GUIContent(""), customSkin.FindStyle("Toggle Helper"));
            GUILayout.EndHorizontal();
            GUILayout.Space(foldoutBottomSpace);

            if (showSlider && sliderThemeType.enumValueIndex == 0)
            {
                GUILayout.BeginHorizontal(EditorStyles.helpBox);

                EditorGUILayout.LabelField(new GUIContent("Theme Type"), customSkin.FindStyle("Text"), GUILayout.Width(120));
                EditorGUILayout.PropertyField(sliderThemeType, new GUIContent(""));

                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal(EditorStyles.helpBox);

                EditorGUILayout.LabelField(new GUIContent("Label Font"), customSkin.FindStyle("Text"), GUILayout.Width(120));
                EditorGUILayout.PropertyField(sliderLabelFontSize, new GUIContent(""), GUILayout.Width(40));
                EditorGUILayout.PropertyField(sliderLabelFont, new GUIContent(""));

                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal(EditorStyles.helpBox);

                EditorGUILayout.LabelField(new GUIContent("Primary Color"), customSkin.FindStyle("Text"), GUILayout.Width(120));
                EditorGUILayout.PropertyField(sliderColor, new GUIContent(""));

                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal(EditorStyles.helpBox);

                EditorGUILayout.LabelField(new GUIContent("Secondary Color"), customSkin.FindStyle("Text"), GUILayout.Width(120));
                EditorGUILayout.PropertyField(sliderBackgroundColor, new GUIContent(""));

                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal(EditorStyles.helpBox);

                EditorGUILayout.LabelField(new GUIContent("Label Popup Color"), customSkin.FindStyle("Text"), GUILayout.Width(120));
                EditorGUILayout.PropertyField(sliderLabelColor, new GUIContent(""));

                GUILayout.EndHorizontal();
            }

            if (showSlider && sliderThemeType.enumValueIndex == 1)
            {
                GUILayout.BeginHorizontal(EditorStyles.helpBox);

                EditorGUILayout.LabelField(new GUIContent("Theme Type"), customSkin.FindStyle("Text"), GUILayout.Width(120));
                EditorGUILayout.PropertyField(sliderThemeType, new GUIContent(""));

                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal(EditorStyles.helpBox);

                EditorGUILayout.LabelField(new GUIContent("Label Font"), customSkin.FindStyle("Text"), GUILayout.Width(120));
                EditorGUILayout.PropertyField(sliderLabelFontSize, new GUIContent(""), GUILayout.Width(40));
                EditorGUILayout.PropertyField(sliderLabelFont, new GUIContent(""));

                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal(EditorStyles.helpBox);

                EditorGUILayout.LabelField(new GUIContent("Color"), customSkin.FindStyle("Text"), GUILayout.Width(120));
                EditorGUILayout.PropertyField(sliderColor, new GUIContent(""));

                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal(EditorStyles.helpBox);

                EditorGUILayout.LabelField(new GUIContent("Label Color"), customSkin.FindStyle("Text"), GUILayout.Width(120));
                EditorGUILayout.PropertyField(sliderLabelColor, new GUIContent(""));

                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal(EditorStyles.helpBox);

                EditorGUILayout.LabelField(new GUIContent("Label Popup Color"), customSkin.FindStyle("Text"), GUILayout.Width(120));
                EditorGUILayout.PropertyField(sliderPopupLabelColor, new GUIContent(""));

                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal(EditorStyles.helpBox);

                EditorGUILayout.LabelField(new GUIContent("Handle Color"), customSkin.FindStyle("Text"), GUILayout.Width(120));
                EditorGUILayout.PropertyField(sliderHandleColor, new GUIContent(""));

                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal(EditorStyles.helpBox);

                EditorGUILayout.LabelField(new GUIContent("Background Color"), customSkin.FindStyle("Text"), GUILayout.Width(120));
                EditorGUILayout.PropertyField(sliderBackgroundColor, new GUIContent(""));

                GUILayout.EndHorizontal();
            }

            GUILayout.EndVertical();
            GUILayout.Space(foldoutItemSpace);
            GUILayout.BeginVertical(EditorStyles.helpBox);

            // Switch
            var switchBorderColor = serializedObject.FindProperty("switchBorderColor");
            var switchBackgroundColor = serializedObject.FindProperty("switchBackgroundColor");
            var switchHandleOnColor = serializedObject.FindProperty("switchHandleOnColor");
            var switchHandleOffColor = serializedObject.FindProperty("switchHandleOffColor");

            GUILayout.Space(foldoutTopSpace);
            GUILayout.BeginHorizontal();
            showSwitch = EditorGUILayout.Foldout(showSwitch, "Switch", true, foldoutStyle);
            showSwitch = GUILayout.Toggle(showSwitch, new GUIContent(""), customSkin.FindStyle("Toggle Helper"));
            GUILayout.EndHorizontal();
            GUILayout.Space(foldoutBottomSpace);

            if (showSwitch)
            {
                GUILayout.BeginHorizontal(EditorStyles.helpBox);

                EditorGUILayout.LabelField(new GUIContent("Border Color"), customSkin.FindStyle("Text"), GUILayout.Width(120));
                EditorGUILayout.PropertyField(switchBorderColor, new GUIContent(""));

                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal(EditorStyles.helpBox);

                EditorGUILayout.LabelField(new GUIContent("Background Color"), customSkin.FindStyle("Text"), GUILayout.Width(120));
                EditorGUILayout.PropertyField(switchBackgroundColor, new GUIContent(""));

                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal(EditorStyles.helpBox);

                EditorGUILayout.LabelField(new GUIContent("Handle On Color"), customSkin.FindStyle("Text"), GUILayout.Width(120));
                EditorGUILayout.PropertyField(switchHandleOnColor, new GUIContent(""));

                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal(EditorStyles.helpBox);

                EditorGUILayout.LabelField(new GUIContent("Handle Off Color"), customSkin.FindStyle("Text"), GUILayout.Width(120));
                EditorGUILayout.PropertyField(switchHandleOffColor, new GUIContent(""));

                GUILayout.EndHorizontal();
            }

            GUILayout.EndVertical();
            GUILayout.Space(foldoutItemSpace);
            GUILayout.BeginVertical(EditorStyles.helpBox);

            // Toggle
            var toggleFont = serializedObject.FindProperty("toggleFont");
            var toggleFontSize = serializedObject.FindProperty("toggleFontSize");
            var toggleTextColor = serializedObject.FindProperty("toggleTextColor");
            var toggleBorderColor = serializedObject.FindProperty("toggleBorderColor");
            var toggleBackgroundColor = serializedObject.FindProperty("toggleBackgroundColor");
            var toggleCheckColor = serializedObject.FindProperty("toggleCheckColor");

            GUILayout.Space(foldoutTopSpace);
            GUILayout.BeginHorizontal();
            showToggle = EditorGUILayout.Foldout(showToggle, "Toggle", true, foldoutStyle);
            showToggle = GUILayout.Toggle(showToggle, new GUIContent(""), customSkin.FindStyle("Toggle Helper"));
            GUILayout.EndHorizontal();
            GUILayout.Space(foldoutBottomSpace);

            if (showToggle)
            {
                GUILayout.BeginHorizontal(EditorStyles.helpBox);

                EditorGUILayout.LabelField(new GUIContent("Font"), customSkin.FindStyle("Text"), GUILayout.Width(120));
                EditorGUILayout.PropertyField(toggleFontSize, new GUIContent(""), GUILayout.Width(40));
                EditorGUILayout.PropertyField(toggleFont, new GUIContent(""));

                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal(EditorStyles.helpBox);

                EditorGUILayout.LabelField(new GUIContent("Text Color"), customSkin.FindStyle("Text"), GUILayout.Width(120));
                EditorGUILayout.PropertyField(toggleTextColor, new GUIContent(""));

                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal(EditorStyles.helpBox);

                EditorGUILayout.LabelField(new GUIContent("Border Color"), customSkin.FindStyle("Text"), GUILayout.Width(120));
                EditorGUILayout.PropertyField(toggleBorderColor, new GUIContent(""));

                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal(EditorStyles.helpBox);

                EditorGUILayout.LabelField(new GUIContent("Background Color"), customSkin.FindStyle("Text"), GUILayout.Width(120));
                EditorGUILayout.PropertyField(toggleBackgroundColor, new GUIContent(""));

                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal(EditorStyles.helpBox);

                EditorGUILayout.LabelField(new GUIContent("Check Color"), customSkin.FindStyle("Text"), GUILayout.Width(120));
                EditorGUILayout.PropertyField(toggleCheckColor, new GUIContent(""));

                GUILayout.EndHorizontal();
            }

            GUILayout.EndVertical();
            GUILayout.Space(foldoutItemSpace);
            GUILayout.BeginVertical(EditorStyles.helpBox);

            // Tooltip
            var tooltipFont = serializedObject.FindProperty("tooltipFont");
            var tooltipFontSize = serializedObject.FindProperty("tooltipFontSize");
            var tooltipTextColor = serializedObject.FindProperty("tooltipTextColor");
            var tooltipBackgroundColor = serializedObject.FindProperty("tooltipBackgroundColor");

            GUILayout.Space(foldoutTopSpace);
            GUILayout.BeginHorizontal();
            showTooltip = EditorGUILayout.Foldout(showTooltip, "Tooltip", true, foldoutStyle);
            showTooltip = GUILayout.Toggle(showTooltip, new GUIContent(""), customSkin.FindStyle("Toggle Helper"));
            GUILayout.EndHorizontal();
            GUILayout.Space(foldoutBottomSpace);

            if (showTooltip)
            {
                GUILayout.BeginHorizontal(EditorStyles.helpBox);

                EditorGUILayout.LabelField(new GUIContent("Font"), customSkin.FindStyle("Text"), GUILayout.Width(120));
                EditorGUILayout.PropertyField(tooltipFontSize, new GUIContent(""), GUILayout.Width(40));
                EditorGUILayout.PropertyField(tooltipFont, new GUIContent(""));

                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal(EditorStyles.helpBox);

                EditorGUILayout.LabelField(new GUIContent("Text Color"), customSkin.FindStyle("Text"), GUILayout.Width(120));
                EditorGUILayout.PropertyField(tooltipTextColor, new GUIContent(""));

                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal(EditorStyles.helpBox);

                EditorGUILayout.LabelField(new GUIContent("Background Color"), customSkin.FindStyle("Text"), GUILayout.Width(120));
                EditorGUILayout.PropertyField(tooltipBackgroundColor, new GUIContent(""));

                GUILayout.EndHorizontal();
            }

            // Settings
            GUILayout.EndVertical();
            GUILayout.Space(14);
            GUILayout.Box(new GUIContent(""), customSkin.FindStyle("Options Header"));

            var enableDynamicUpdate = serializedObject.FindProperty("enableDynamicUpdate");

            GUILayout.BeginHorizontal(EditorStyles.helpBox);

            enableDynamicUpdate.boolValue = GUILayout.Toggle(enableDynamicUpdate.boolValue, new GUIContent("Update Values"), customSkin.FindStyle("Toggle"));
            enableDynamicUpdate.boolValue = GUILayout.Toggle(enableDynamicUpdate.boolValue, new GUIContent(""), customSkin.FindStyle("Toggle Helper"));

            GUILayout.EndHorizontal();

            var enableExtendedColorPicker = serializedObject.FindProperty("enableExtendedColorPicker");

            GUILayout.BeginHorizontal(EditorStyles.helpBox);

            enableExtendedColorPicker.boolValue = GUILayout.Toggle(enableExtendedColorPicker.boolValue, new GUIContent("Extended Color Picker"), customSkin.FindStyle("Toggle"));
            enableExtendedColorPicker.boolValue = GUILayout.Toggle(enableExtendedColorPicker.boolValue, new GUIContent(""), customSkin.FindStyle("Toggle Helper"));

            GUILayout.EndHorizontal();

            if (enableExtendedColorPicker.boolValue == true) { EditorPrefs.SetInt("UIManager.EnableExtendedColorPicker", 1); }
            else { EditorPrefs.SetInt("UIManager.EnableExtendedColorPicker", 0); }

            var editorHints = serializedObject.FindProperty("editorHints");

            GUILayout.BeginVertical(EditorStyles.helpBox);

            editorHints.boolValue = GUILayout.Toggle(editorHints.boolValue, new GUIContent("UI Manager Hints"), customSkin.FindStyle("Toggle"), GUILayout.Width(500));

            if (editorHints.boolValue == true)
            {
                EditorGUILayout.HelpBox("These values are universal and affect all objects containing 'UI Manager' component.", MessageType.Info);
                EditorGUILayout.HelpBox("If want to assign unique values, remove 'UI Manager' component from the object ", MessageType.Info);
				EditorGUILayout.HelpBox("You can press 'Control/Command + Shift + M' to open the manager quickly.", MessageType.Info);
            }

            GUILayout.EndVertical();

            serializedObject.ApplyModifiedProperties();
            Repaint();

            GUILayout.Space(12);
            GUILayout.BeginHorizontal();

            if (GUILayout.Button("Reset to defaults", customSkin.button))
                ResetToDefaults();

            GUILayout.EndHorizontal();

            // Support
            GUILayout.Space(14);
            GUILayout.Box(new GUIContent(""), customSkin.FindStyle("Support Header"));
            GUILayout.BeginVertical();
            GUILayout.BeginHorizontal();
            GUILayout.Label("Need help? Contact me via:", customSkin.FindStyle("Text"));
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();

            if (GUILayout.Button("Discord", customSkin.button)) { Discord(); }
            if (GUILayout.Button("Twitter", customSkin.button)) { Twitter(); }

            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();

            if (GUILayout.Button("E-mail", customSkin.button)) { Email(); }

            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
            GUILayout.Space(6);
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("ID: " + buildID);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.Space(6);
        }

        void Discord() { Application.OpenURL("https://discord.gg/VXpHyUt"); }
        void Email() { Application.OpenURL("https://www.michsky.com/contact/"); }
        void Twitter() { Application.OpenURL("https://www.twitter.com/michskyHQ"); }

        void ResetToDefaults()
        {
            if (EditorUtility.DisplayDialog("Reset to defaults", "Are you sure you want to reset UI Manager values to default?", "Yes", "Cancel"))
            {
                try
                {
                    Preset defaultPreset = Resources.Load<Preset>("UI Manager Presets/Default");
                    defaultPreset.ApplyTo(Resources.Load("MUIP Manager"));
                    Selection.activeObject = null;
                    Debug.Log("<b>[UI Manager]</b> Resetting successful.");
                }

                catch { Debug.LogWarning("<b>[UI Manager]</b> Resetting failed. Default preset seems to be missing"); }
            }    
        }
    }
}