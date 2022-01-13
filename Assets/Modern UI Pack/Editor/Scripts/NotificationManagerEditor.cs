﻿using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace Michsky.UI.ModernUIPack
{
    [CustomEditor(typeof(NotificationManager))]
    public class NotificationManagerEditor : Editor
    {
        private GUISkin customSkin;
        private NotificationManager ntfTarget;
        private UIManagerNotification tempUIM;
        private int currentTab;

        private void OnEnable()
        {
            ntfTarget = (NotificationManager)target;

            try { tempUIM = ntfTarget.GetComponent<UIManagerNotification>(); }
            catch { }

            if (EditorGUIUtility.isProSkin == true) { customSkin = (GUISkin)Resources.Load("Editor\\MUI Skin Dark"); }
            else { customSkin = (GUISkin)Resources.Load("Editor\\MUI Skin Light"); }
        }

        public override void OnInspectorGUI()
        {
            MUIPEditorHandler.DrawComponentHeader(customSkin, "Notification Top Header");

            GUIContent[] toolbarTabs = new GUIContent[3];
            toolbarTabs[0] = new GUIContent("Content");
            toolbarTabs[1] = new GUIContent("Resources");
            toolbarTabs[2] = new GUIContent("Settings");

            currentTab = MUIPEditorHandler.DrawTabs(currentTab, toolbarTabs, customSkin);

            if (GUILayout.Button(new GUIContent("Content", "Content"), customSkin.FindStyle("Tab Content")))
                currentTab = 0;
            if (GUILayout.Button(new GUIContent("Resources", "Resources"), customSkin.FindStyle("Tab Resources")))
                currentTab = 1;
            if (GUILayout.Button(new GUIContent("Settings", "Settings"), customSkin.FindStyle("Tab Settings")))
                currentTab = 2;

            GUILayout.EndHorizontal();

            var icon = serializedObject.FindProperty("icon");
            var title = serializedObject.FindProperty("title");
            var description = serializedObject.FindProperty("description");
            var notificationAnimator = serializedObject.FindProperty("notificationAnimator");
            var iconObj = serializedObject.FindProperty("iconObj");
            var titleObj = serializedObject.FindProperty("titleObj");
            var descriptionObj = serializedObject.FindProperty("descriptionObj");
            var enableTimer = serializedObject.FindProperty("enableTimer");
            var timer = serializedObject.FindProperty("timer");
            var notificationStyle = serializedObject.FindProperty("notificationStyle");
            var useCustomContent = serializedObject.FindProperty("useCustomContent");
            var useStacking = serializedObject.FindProperty("useStacking");
            var destroyAfterPlaying = serializedObject.FindProperty("destroyAfterPlaying");

            switch (currentTab)
            {
                case 0:
                    MUIPEditorHandler.DrawHeader(customSkin, "Content Header", 6);
                    MUIPEditorHandler.DrawProperty(icon, customSkin, "Icon");

                    if (ntfTarget.iconObj != null)
                        ntfTarget.iconObj.sprite = ntfTarget.icon;

                    else
                    {
                        if (ntfTarget.iconObj == null)
                        {
                            GUILayout.BeginHorizontal();
                            EditorGUILayout.HelpBox("'Icon Object' is not assigned. Go to Resources tab and assign the correct variable.", MessageType.Error);
                            GUILayout.EndHorizontal();
                        }
                    }

                    MUIPEditorHandler.DrawProperty(title, customSkin, "Title");

                    if (ntfTarget.titleObj != null)
                        ntfTarget.titleObj.text = title.stringValue;

                    else
                    {
                        if (ntfTarget.titleObj == null)
                        {
                            GUILayout.BeginHorizontal();
                            EditorGUILayout.HelpBox("'Title Object' is not assigned. Go to Resources tab and assign the correct variable.", MessageType.Error);
                            GUILayout.EndHorizontal();
                        }
                    }

                    GUILayout.BeginHorizontal(EditorStyles.helpBox);

                    EditorGUILayout.LabelField(new GUIContent("Description"), customSkin.FindStyle("Text"), GUILayout.Width(-3));
                    EditorGUILayout.PropertyField(description, new GUIContent(""), GUILayout.Height(50));

                    GUILayout.EndHorizontal();

                    if (ntfTarget.descriptionObj != null)
                        ntfTarget.descriptionObj.text = description.stringValue;

                    else
                    {
                        if (ntfTarget.descriptionObj == null)
                        {
                            GUILayout.BeginHorizontal();
                            EditorGUILayout.HelpBox("'Description Object' is not assigned. Go to Resources tab and assign the correct variable.", MessageType.Error);
                            GUILayout.EndHorizontal();
                        }
                    }

                    if (ntfTarget.GetComponent<CanvasGroup>().alpha == 0)
                    {
                        if (GUILayout.Button("Make It Visible", customSkin.button))
                        {
                            ntfTarget.GetComponent<CanvasGroup>().alpha = 1;
                            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
                        }
                    }

                    else
                    {
                        if (GUILayout.Button("Make It Invisible", customSkin.button))
                        {
                            ntfTarget.GetComponent<CanvasGroup>().alpha = 0;
                            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
                        }
                    }

                    break;

                case 1:
                    MUIPEditorHandler.DrawHeader(customSkin, "Core Header", 6);
                    MUIPEditorHandler.DrawProperty(notificationAnimator, customSkin, "Animator");
                    MUIPEditorHandler.DrawProperty(iconObj, customSkin, "Icon Object");
                    MUIPEditorHandler.DrawProperty(titleObj, customSkin, "Title Object");
                    MUIPEditorHandler.DrawProperty(descriptionObj, customSkin, "Description Object");
                    break;

                case 2:
                    MUIPEditorHandler.DrawHeader(customSkin, "Options Header", 6);
                    useCustomContent.boolValue = MUIPEditorHandler.DrawToggle(useCustomContent.boolValue, customSkin, "Use Custom Content");
                    useStacking.boolValue = MUIPEditorHandler.DrawToggle(useStacking.boolValue, customSkin, "Use Stacking");
                    destroyAfterPlaying.boolValue = MUIPEditorHandler.DrawToggle(destroyAfterPlaying.boolValue, customSkin, "Destroy After Playing");
                    enableTimer.boolValue = MUIPEditorHandler.DrawToggle(enableTimer.boolValue, customSkin, "Enable Timer");

                    if (enableTimer.boolValue == true)
                        MUIPEditorHandler.DrawProperty(timer, customSkin, "Timer");

                    MUIPEditorHandler.DrawProperty(notificationStyle, customSkin, "Notification Style");

                    MUIPEditorHandler.DrawHeader(customSkin, "UIM Header", 10);

                    if (tempUIM != null)
                    {
                        EditorGUILayout.HelpBox("This object is connected with UI Manager. Some parameters (such as colors, " +
                            "fonts or booleans) are managed by the manager.", MessageType.Info);

                        if (GUILayout.Button("Open UI Manager", customSkin.button))
                            EditorApplication.ExecuteMenuItem("Tools/Modern UI Pack/Show UI Manager");

                        if (GUILayout.Button("Disable UI Manager Connection", customSkin.button))
                        {
                            if (EditorUtility.DisplayDialog("Modern UI Pack", "Are you sure you want to disable UI Manager connection with the object? " +
                                "This operation cannot be undone.", "Yes", "Cancel"))
                            {
                                try { DestroyImmediate(tempUIM); }
                                catch { Debug.LogError("<b>[Notification Manager]</b> Failed to delete UI Manager connection.", this); }
                            }
                        }
                    }

                    else if (tempUIM == null)
                        EditorGUILayout.HelpBox("This object does not have any connection with UI Manager.", MessageType.Info);

                    break;
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}