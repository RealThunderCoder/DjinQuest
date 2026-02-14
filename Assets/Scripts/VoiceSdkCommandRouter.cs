using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;

public class VoiceSdkCommandRouter : MonoBehaviour
{
    [SerializeField] private VoiceMenuSpawner targetSpawner;
    [SerializeField] private bool autoFindVoiceExperience = true;
    [SerializeField] private bool autoActivateOnEnable = true;
    [SerializeField] private string appVoiceExperienceTypeName = "Oculus.Voice.AppVoiceExperience";

    private Component appVoiceExperience;
    private bool isHooked;

    private void Awake()
    {
        if (targetSpawner == null)
        {
            targetSpawner = FindFirstObjectByType<VoiceMenuSpawner>();
        }
    }

    private void OnEnable()
    {
        TryHookVoiceSdk();
        if (autoActivateOnEnable)
        {
            TryActivateVoice();
        }
    }

    private void OnDisable()
    {
        isHooked = false;
    }

    private void TryHookVoiceSdk()
    {
        if (isHooked)
        {
            return;
        }

        if (appVoiceExperience == null && autoFindVoiceExperience)
        {
            appVoiceExperience = FindVoiceExperienceComponent();
        }

        if (appVoiceExperience == null)
        {
            return;
        }

        PropertyInfo voiceEventsProp = appVoiceExperience.GetType().GetProperty("VoiceEvents");
        if (voiceEventsProp == null)
        {
            return;
        }

        object voiceEvents = voiceEventsProp.GetValue(appVoiceExperience);
        if (voiceEvents == null)
        {
            return;
        }

        PropertyInfo fullTranscriptionProp = voiceEvents.GetType().GetProperty("OnFullTranscription");
        if (fullTranscriptionProp == null)
        {
            return;
        }

        object unityEventObj = fullTranscriptionProp.GetValue(voiceEvents);
        if (unityEventObj == null)
        {
            return;
        }

        MethodInfo addListener = unityEventObj.GetType().GetMethod("AddListener", new[] { typeof(UnityAction<string>) });
        if (addListener == null)
        {
            return;
        }

        UnityAction<string> action = HandleTranscript;
        addListener.Invoke(unityEventObj, new object[] { action });
        isHooked = true;
    }

    private void HandleTranscript(string text)
    {
        if (targetSpawner == null)
        {
            return;
        }

        targetSpawner.HandleCommandText(text);
    }

    private void TryActivateVoice()
    {
        if (appVoiceExperience == null)
        {
            return;
        }

        MethodInfo activate = appVoiceExperience.GetType().GetMethod("Activate");
        if (activate != null)
        {
            activate.Invoke(appVoiceExperience, null);
        }
    }

    private Component FindVoiceExperienceComponent()
    {
        Type voiceType = FindTypeByName(appVoiceExperienceTypeName);
        if (voiceType == null)
        {
            return null;
        }

        UnityEngine.Object obj = FindFirstObjectByType(voiceType);
        return obj as Component;
    }

    private Type FindTypeByName(string typeName)
    {
        foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            Type type = assembly.GetType(typeName);
            if (type != null)
            {
                return type;
            }
        }

        return null;
    }
}
