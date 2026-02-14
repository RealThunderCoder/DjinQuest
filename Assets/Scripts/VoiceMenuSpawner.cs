using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System;
using UnityEngine.EventSystems;
using UnityEngine.UI;
#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
using UnityEngine.Windows.Speech;
#endif
#if UNITY_ANDROID && !UNITY_EDITOR
using UnityEngine.Android;
#endif

public class VoiceMenuSpawner : MonoBehaviour
{
    [SerializeField] private Transform headTransform;
    [SerializeField] private bool enableLocalKeywordRecognition = true;
    [SerializeField] private float spawnDistance = 1.5f;
    [SerializeField] private GameObject menuCubePrefab;
    [SerializeField] private string keyword = "menu";
    [SerializeField] private string keywordAlt = "menue";
    [SerializeField] private string keywordOff = "menu off";
    [SerializeField] private string keywordOffAlt = "menue off";
    [SerializeField] private GameObject menuUIPrefab;
    [SerializeField] private Vector2 menuUISize = new Vector2(700f, 420f);
    [SerializeField] private Vector3 menuUIOffset = new Vector3(0f, 0.1f, 0f);
    [SerializeField] private float menuUIScale = 0.01f;
    [SerializeField] private bool pauseTimeWhileMenuOpen = true;
    [SerializeField] private bool enableFallbackPointer = true;
    [SerializeField] private Transform fallbackRayOrigin;
    [SerializeField] private LayerMask fallbackUiLayerMask = ~0;
    [SerializeField] private MonoBehaviour[] pauseBehaviours;
    [SerializeField] private Transform pauseRoot;
    [SerializeField] private Transform[] pauseKeepRoots;
    [SerializeField] private bool autoDisableOnMenu = false;
    [SerializeField] private string[] autoDisableKeepTypeNames =
    {
        "Meta.XR.BuildingBlocks.BuildingBlock",
        "Oculus.Interaction.HandGrabInteractor",
        "Oculus.Interaction.RayInteractor",
        "Oculus.Interaction.ActiveStateSelector",
        "Oculus.Interaction.ActiveStateGroup",
        "Oculus.Interaction.SelectorUnityEventWrapper",
        "Oculus.Interaction.PoseDetection.TransformRecognizerActiveState",
        "Oculus.Interaction.PoseDetection.ShapeRecognizerActiveState",
        "Oculus.Interaction.HandGrabInteractable",
        "Oculus.Interaction.RayInteractable",
        "OVRCameraRig",
        "OVRManager"
    };
    [SerializeField] private string fireKeyword = "fire";
    [SerializeField] private string fireOffKeyword = "fire off";
    [SerializeField] private FireHandEffect[] fireEffects;

    private GameObject spawnedMenuCube;
    private GameObject spawnedMenuUI;
    private bool isMenuOpen;
    private readonly List<MonoBehaviour> autoDisabledBehaviours = new List<MonoBehaviour>();
    private MenuPointerFallback fallbackPointer;

#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
    private KeywordRecognizer keywordRecognizer;
#endif
#if UNITY_ANDROID && !UNITY_EDITOR
    private AndroidJavaObject speechRecognizer;
    private AndroidJavaObject recognizerIntent;
    private RecognitionListener recognitionListener;
    private bool isListening;
    private bool permissionRequested;
#endif

    private void Awake()
    {
        if (headTransform == null && Camera.main != null)
        {
            headTransform = Camera.main.transform;
        }
    }

    private void OnEnable()
    {
#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
        if (enableLocalKeywordRecognition)
        {
            if (keywordRecognizer == null)
            {
                keywordRecognizer = new KeywordRecognizer(new[]
                {
                    keyword, keywordAlt, keywordOff, keywordOffAlt, fireKeyword, fireOffKeyword
                });
                keywordRecognizer.OnPhraseRecognized += OnPhraseRecognized;
            }

            if (!keywordRecognizer.IsRunning)
            {
                keywordRecognizer.Start();
            }
        }
#endif
#if UNITY_ANDROID && !UNITY_EDITOR
        if (enableLocalKeywordRecognition)
        {
            RequestMicrophonePermission();
            SetupAndroidSpeechRecognizer();
            TryStartAndroidListening();
        }
#endif
    }

    private void OnDisable()
    {
#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
        if (keywordRecognizer != null && keywordRecognizer.IsRunning)
        {
            keywordRecognizer.Stop();
        }
#endif
#if UNITY_ANDROID && !UNITY_EDITOR
        StopAndroidListening();
        DisposeAndroidSpeechRecognizer();
#endif
    }

#if UNITY_ANDROID && !UNITY_EDITOR
    private void Update()
    {
        if (!enableLocalKeywordRecognition)
        {
            return;
        }

        if (!isListening)
        {
            TryStartAndroidListening();
        }
    }
#endif

    public void HandleCommandText(string text)
    {
        if (IsKeywordOffMatch(text))
        {
            CloseMenu();
        }
        else if (IsKeywordMatch(text))
        {
            OpenMenu();
        }
        else if (IsFireOffMatch(text))
        {
            SetFireActive(false);
        }
        else if (IsFireMatch(text))
        {
            SetFireActive(true);
        }
    }

#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
    private void OnPhraseRecognized(PhraseRecognizedEventArgs args)
    {
        HandleCommandText(args.text);
    }
#endif

    private bool IsKeywordMatch(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return false;
        }

        string lower = text.ToLowerInvariant();
        bool isMenu = lower.Contains(keyword) || lower.Contains(keywordAlt);
        bool isOff = lower.Contains("off");
        return isMenu && !isOff;
    }

    private bool IsKeywordOffMatch(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return false;
        }

        string lower = text.ToLowerInvariant();
        bool isMenu = lower.Contains(keyword) || lower.Contains(keywordAlt);
        return (lower.Contains(keywordOff) || lower.Contains(keywordOffAlt)) || (isMenu && lower.Contains("off"));
    }

    private bool IsFireMatch(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return false;
        }

        string lower = text.ToLowerInvariant();
        return lower.Contains(fireKeyword) && !lower.Contains("off");
    }

    private bool IsFireOffMatch(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return false;
        }

        string lower = text.ToLowerInvariant();
        return lower.Contains(fireOffKeyword) || (lower.Contains(fireKeyword) && lower.Contains("off"));
    }

    private void SpawnMenuCube()
    {
        if (headTransform == null)
        {
            return;
        }

        Vector3 forward = headTransform.forward;
        forward.y = 0f;
        forward = forward.sqrMagnitude > 0.0001f ? forward.normalized : headTransform.forward;

        Vector3 spawnPos = headTransform.position + forward * spawnDistance;

        if (spawnedMenuCube != null)
        {
            Destroy(spawnedMenuCube);
        }

        spawnedMenuCube = menuCubePrefab != null
            ? Instantiate(menuCubePrefab, spawnPos, Quaternion.identity)
            : GameObject.CreatePrimitive(PrimitiveType.Cube);

        spawnedMenuCube.transform.position = spawnPos;
        spawnedMenuCube.transform.rotation = Quaternion.identity;
    }

    private void DespawnMenuCube()
    {
        if (spawnedMenuCube != null)
        {
            Destroy(spawnedMenuCube);
            spawnedMenuCube = null;
        }
    }

    private void OpenMenu()
    {
        if (isMenuOpen)
        {
            return;
        }

        isMenuOpen = true;
        SpawnMenuUI();
        SetFallbackPointerActive(true);
        if (pauseTimeWhileMenuOpen)
        {
            Time.timeScale = 0f;
            AudioListener.pause = true;
        }

        SetPausedBehaviours(false);
        AutoDisableBehaviours();
    }

    public void CloseMenu()
    {
        if (!isMenuOpen)
        {
            return;
        }

        isMenuOpen = false;
        SetFallbackPointerActive(false);
        DespawnMenuUI();
        DespawnMenuCube();
        if (pauseTimeWhileMenuOpen)
        {
            Time.timeScale = 1f;
            AudioListener.pause = false;
        }

        SetPausedBehaviours(true);
        RestoreAutoDisabledBehaviours();
    }

    private void SpawnMenuUI()
    {
        if (headTransform == null)
        {
            return;
        }

        if (spawnedMenuUI != null)
        {
            return;
        }

        Vector3 forward = headTransform.forward;
        forward.y = 0f;
        forward = forward.sqrMagnitude > 0.0001f ? forward.normalized : headTransform.forward;
        Vector3 spawnPos = headTransform.position + forward * spawnDistance + menuUIOffset;

        EnsureEventSystem();

        spawnedMenuUI = menuUIPrefab != null
            ? Instantiate(menuUIPrefab, spawnPos, Quaternion.identity)
            : CreateFallbackMenuUI(spawnPos);

        spawnedMenuUI.transform.rotation = Quaternion.LookRotation(forward, Vector3.up);

        VoiceMenuUIController controller = spawnedMenuUI.GetComponentInChildren<VoiceMenuUIController>();
        if (controller != null)
        {
            controller.Initialize(this);
        }
    }

    private void DespawnMenuUI()
    {
        if (spawnedMenuUI != null)
        {
            Destroy(spawnedMenuUI);
            spawnedMenuUI = null;
        }
    }

    private GameObject CreateFallbackMenuUI(Vector3 position)
    {
        GameObject canvasGO = new GameObject("VoiceMenuUI");
        Canvas canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;
        canvas.worldCamera = Camera.main;
        canvas.sortingOrder = 100;
        CanvasScaler scaler = canvasGO.AddComponent<CanvasScaler>();
        scaler.dynamicPixelsPerUnit = 10f;
        GraphicRaycaster graphicRaycaster = canvasGO.AddComponent<GraphicRaycaster>();

        RectTransform canvasRect = canvas.GetComponent<RectTransform>();
        canvasRect.sizeDelta = menuUISize;
        canvasRect.position = position;
        canvasRect.localScale = Vector3.one * menuUIScale;

        GameObject panelGO = new GameObject("Panel");
        panelGO.transform.SetParent(canvasGO.transform, false);
        Image panelImage = panelGO.AddComponent<Image>();
        panelImage.color = new Color(0f, 0f, 0f, 0.6f);
        RectTransform panelRect = panelGO.GetComponent<RectTransform>();
        panelRect.anchorMin = Vector2.zero;
        panelRect.anchorMax = Vector2.one;
        panelRect.offsetMin = Vector2.zero;
        panelRect.offsetMax = Vector2.zero;

        GameObject titleGO = new GameObject("Title");
        titleGO.transform.SetParent(panelGO.transform, false);
        TextMeshProUGUI titleText = titleGO.AddComponent<TextMeshProUGUI>();
        titleText.text = "Paused";
        titleText.alignment = TextAlignmentOptions.Center;
        titleText.color = Color.white;
        titleText.fontSize = 48;
        RectTransform titleRect = titleGO.GetComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0f, 0.65f);
        titleRect.anchorMax = new Vector2(1f, 1f);
        titleRect.offsetMin = new Vector2(20f, 0f);
        titleRect.offsetMax = new Vector2(-20f, -20f);

        GameObject hintGO = new GameObject("Hint");
        hintGO.transform.SetParent(panelGO.transform, false);
        TextMeshProUGUI hintText = hintGO.AddComponent<TextMeshProUGUI>();
        hintText.text = "Say \"menu off\" to resume";
        hintText.alignment = TextAlignmentOptions.Center;
        hintText.color = new Color(1f, 1f, 1f, 0.85f);
        hintText.fontSize = 26;
        RectTransform hintRect = hintGO.GetComponent<RectTransform>();
        hintRect.anchorMin = new Vector2(0f, 0.5f);
        hintRect.anchorMax = new Vector2(1f, 0.7f);
        hintRect.offsetMin = new Vector2(20f, 0f);
        hintRect.offsetMax = new Vector2(-20f, 0f);

        GameObject startButton = CreateButton(panelGO.transform, "StartButton", "Start", new Vector2(0.1f, 0.12f), new Vector2(0.45f, 0.42f));
        GameObject quitButton = CreateButton(panelGO.transform, "QuitButton", "Quit", new Vector2(0.55f, 0.12f), new Vector2(0.9f, 0.42f));

        VoiceMenuUIController controller = canvasGO.AddComponent<VoiceMenuUIController>();
        controller.WireButtons(startButton.GetComponent<Button>(), quitButton.GetComponent<Button>());
        EnsureMenuButton(startButton, MenuButton.ActionType.Resume);
        EnsureMenuButton(quitButton, MenuButton.ActionType.Quit);

        SetLayerRecursive(canvasGO, LayerMask.NameToLayer("UI"));
        EnsureOVRRaycaster(canvasGO, graphicRaycaster);

        return canvasGO;
    }

    private GameObject CreateButton(Transform parent, string name, string label, Vector2 anchorMin, Vector2 anchorMax)
    {
        GameObject buttonGO = new GameObject(name);
        buttonGO.transform.SetParent(parent, false);
        Image buttonImage = buttonGO.AddComponent<Image>();
        buttonImage.color = new Color(1f, 1f, 1f, 0.12f);
        Button button = buttonGO.AddComponent<Button>();
        button.targetGraphic = buttonImage;

        RectTransform rect = buttonGO.GetComponent<RectTransform>();
        rect.anchorMin = anchorMin;
        rect.anchorMax = anchorMax;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;

        BoxCollider collider = buttonGO.AddComponent<BoxCollider>();
        collider.size = new Vector3(rect.rect.width, rect.rect.height, 1f);
        collider.center = Vector3.zero;

        GameObject textGO = new GameObject("Text");
        textGO.transform.SetParent(buttonGO.transform, false);
        TextMeshProUGUI text = textGO.AddComponent<TextMeshProUGUI>();
        text.text = label;
        text.alignment = TextAlignmentOptions.Center;
        text.color = Color.white;
        text.fontSize = 24;
        RectTransform textRect = textGO.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;

        return buttonGO;
    }

    private void EnsureEventSystem()
    {
        EventSystem eventSystem = FindFirstObjectByType<EventSystem>();
        if (eventSystem == null)
        {
            GameObject eventSystemGO = new GameObject("EventSystem");
            eventSystem = eventSystemGO.AddComponent<EventSystem>();
        }

        EnsureOVRInputModule(eventSystem.gameObject);
    }

    private void SetPausedBehaviours(bool enabled)
    {
        if (pauseBehaviours == null || pauseBehaviours.Length == 0)
        {
            return;
        }

        foreach (MonoBehaviour behaviour in pauseBehaviours)
        {
            if (behaviour != null)
            {
                behaviour.enabled = enabled;
            }
        }
    }

    private void AutoDisableBehaviours()
    {
        if (!autoDisableOnMenu || pauseRoot == null)
        {
            return;
        }

        autoDisabledBehaviours.Clear();
        MonoBehaviour[] behaviours = pauseRoot.GetComponentsInChildren<MonoBehaviour>(true);
        foreach (MonoBehaviour behaviour in behaviours)
        {
            if (behaviour == null || behaviour == this)
            {
                continue;
            }

            if (behaviour is EventSystem || behaviour is StandaloneInputModule || behaviour is VoiceMenuUIController)
            {
                continue;
            }

            if (IsUnderKeepRoot(behaviour.transform))
            {
                continue;
            }

            if (ShouldKeepEnabled(behaviour))
            {
                continue;
            }

            if (behaviour.enabled)
            {
                behaviour.enabled = false;
                autoDisabledBehaviours.Add(behaviour);
            }
        }
    }

    private void RestoreAutoDisabledBehaviours()
    {
        if (autoDisabledBehaviours.Count == 0)
        {
            return;
        }

        foreach (MonoBehaviour behaviour in autoDisabledBehaviours)
        {
            if (behaviour != null)
            {
                behaviour.enabled = true;
            }
        }

        autoDisabledBehaviours.Clear();
    }

    private bool ShouldKeepEnabled(MonoBehaviour behaviour)
    {
        if (behaviour == null || autoDisableKeepTypeNames == null)
        {
            return false;
        }

        string typeName = behaviour.GetType().FullName;
        foreach (string keepType in autoDisableKeepTypeNames)
        {
            if (!string.IsNullOrEmpty(keepType) && typeName == keepType)
            {
                return true;
            }
        }

        return false;
    }

    private void EnsureOVRInputModule(GameObject eventSystemGO)
    {
        if (eventSystemGO == null)
        {
            return;
        }

        Type ovrInputModuleType = FindTypeByName("OVRInputModule");
        if (ovrInputModuleType != null)
        {
            if (eventSystemGO.GetComponent(ovrInputModuleType) == null)
            {
                eventSystemGO.AddComponent(ovrInputModuleType);
            }
        }

        StandaloneInputModule standalone = eventSystemGO.GetComponent<StandaloneInputModule>();
        if (standalone != null)
        {
            Destroy(standalone);
        }

        Type inputSystemUIModuleType = FindTypeByName("UnityEngine.InputSystem.UI.InputSystemUIInputModule");
        if (inputSystemUIModuleType != null)
        {
            Component inputSystemUIModule = eventSystemGO.GetComponent(inputSystemUIModuleType);
            if (inputSystemUIModule != null)
            {
                Destroy(inputSystemUIModule);
            }
        }
    }

    private void EnsureOVRRaycaster(GameObject canvasGO, GraphicRaycaster graphicRaycaster)
    {
        if (canvasGO == null)
        {
            return;
        }

        Type ovrRaycasterType = FindTypeByName("OVRRaycaster");
        if (ovrRaycasterType != null && canvasGO.GetComponent(ovrRaycasterType) == null)
        {
            canvasGO.AddComponent(ovrRaycasterType);
        }

        if (graphicRaycaster != null && ovrRaycasterType != null)
        {
            Destroy(graphicRaycaster);
        }
    }

    private void EnsureMenuButton(GameObject buttonGO, MenuButton.ActionType action)
    {
        if (buttonGO == null)
        {
            return;
        }

        MenuButton menuButton = buttonGO.GetComponent<MenuButton>();
        if (menuButton == null)
        {
            menuButton = buttonGO.AddComponent<MenuButton>();
        }

        menuButton.Initialize(this, action);
    }

    private void SetFallbackPointerActive(bool isActive)
    {
        if (!enableFallbackPointer)
        {
            return;
        }

        if (fallbackPointer == null)
        {
            fallbackPointer = GetComponent<MenuPointerFallback>();
            if (fallbackPointer == null)
            {
                fallbackPointer = gameObject.AddComponent<MenuPointerFallback>();
            }

            Transform origin = fallbackRayOrigin != null ? fallbackRayOrigin : headTransform;
            fallbackPointer.Initialize(origin, fallbackUiLayerMask);
        }

        fallbackPointer.SetActive(isActive);
    }

    private void SetLayerRecursive(GameObject target, int layer)
    {
        if (target == null || layer < 0)
        {
            return;
        }

        target.layer = layer;
        foreach (Transform child in target.transform)
        {
            if (child != null)
            {
                SetLayerRecursive(child.gameObject, layer);
            }
        }
    }

    private Type FindTypeByName(string typeName)
    {
        foreach (System.Reflection.Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            Type type = assembly.GetType(typeName);
            if (type != null)
            {
                return type;
            }
        }

        return null;
    }

    private bool IsUnderKeepRoot(Transform target)
    {
        if (target == null || pauseKeepRoots == null || pauseKeepRoots.Length == 0)
        {
            return false;
        }

        foreach (Transform keepRoot in pauseKeepRoots)
        {
            if (keepRoot != null && target.IsChildOf(keepRoot))
            {
                return true;
            }
        }

        return false;
    }

    private void SetFireActive(bool isActive)
    {
        if (fireEffects == null || fireEffects.Length == 0)
        {
            return;
        }

        foreach (FireHandEffect effect in fireEffects)
        {
            if (effect == null)
            {
                continue;
            }

            if (isActive)
            {
                effect.ShowFire();
            }
            else
            {
                effect.HideFire();
            }
        }
    }

#if UNITY_ANDROID && !UNITY_EDITOR
    private void RequestMicrophonePermission()
    {
        if (!Permission.HasUserAuthorizedPermission(Permission.Microphone))
        {
            if (!permissionRequested)
            {
                permissionRequested = true;
                Permission.RequestUserPermission(Permission.Microphone);
            }
        }
    }

    private void SetupAndroidSpeechRecognizer()
    {
        if (speechRecognizer != null)
        {
            return;
        }

        AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
        AndroidJavaClass speechRecognizerClass = new AndroidJavaClass("android.speech.SpeechRecognizer");

        speechRecognizer = speechRecognizerClass.CallStatic<AndroidJavaObject>("createSpeechRecognizer", activity);
        recognitionListener = new RecognitionListener(this);
        speechRecognizer.Call("setRecognitionListener", recognitionListener);

        recognizerIntent = new AndroidJavaObject("android.content.Intent",
            "android.speech.RecognizerIntent.ACTION_RECOGNIZE_SPEECH");
        recognizerIntent.Call<AndroidJavaObject>("putExtra",
            "android.speech.extra.LANGUAGE_MODEL",
            "android.speech.RecognizerIntent.LANGUAGE_MODEL_FREE_FORM");
        recognizerIntent.Call<AndroidJavaObject>("putExtra",
            "android.speech.extra.PARTIAL_RESULTS",
            true);
        recognizerIntent.Call<AndroidJavaObject>("putExtra",
            "android.speech.extra.MAX_RESULTS",
            3);
    }

    private void TryStartAndroidListening()
    {
        if (!Permission.HasUserAuthorizedPermission(Permission.Microphone))
        {
            RequestMicrophonePermission();
            return;
        }

        if (speechRecognizer == null || recognizerIntent == null)
        {
            SetupAndroidSpeechRecognizer();
        }

        if (speechRecognizer != null && !isListening)
        {
            isListening = true;
            speechRecognizer.Call("startListening", recognizerIntent);
        }
    }

    private void StopAndroidListening()
    {
        if (speechRecognizer != null && isListening)
        {
            speechRecognizer.Call("stopListening");
            isListening = false;
        }
    }

    private void DisposeAndroidSpeechRecognizer()
    {
        if (speechRecognizer != null)
        {
            try
            {
                speechRecognizer.Call("cancel");
            }
            catch
            {
            }

            speechRecognizer.Call("destroy");
            speechRecognizer = null;
        }

        recognizerIntent = null;
        recognitionListener = null;
        isListening = false;
    }

    internal void HandleAndroidResults(AndroidJavaObject resultsBundle)
    {
        isListening = false;
        EvaluateAndroidResults(resultsBundle);
        TryStartAndroidListening();
    }

    internal void HandleAndroidPartialResults(AndroidJavaObject resultsBundle)
    {
        EvaluateAndroidResults(resultsBundle);
    }

    internal void HandleAndroidError(int error)
    {
        isListening = false;
        TryStartAndroidListening();
    }

    private void EvaluateAndroidResults(AndroidJavaObject resultsBundle)
    {
        if (resultsBundle == null)
        {
            return;
        }

        AndroidJavaObject matches = resultsBundle.Call<AndroidJavaObject>("getStringArrayList", "results_recognition");
        if (matches == null)
        {
            return;
        }

        int size = matches.Call<int>("size");
        for (int i = 0; i < size; i++)
        {
            string text = matches.Call<string>("get", i);
            HandleCommandText(text);
            break;
        }
    }

    private class RecognitionListener : AndroidJavaProxy
    {
        private readonly VoiceMenuSpawner owner;

        public RecognitionListener(VoiceMenuSpawner owner)
            : base("android.speech.RecognitionListener")
        {
            this.owner = owner;
        }

        public void onReadyForSpeech(AndroidJavaObject @params) { }
        public void onBeginningOfSpeech() { }
        public void onRmsChanged(float rmsdB) { }
        public void onBufferReceived(byte[] buffer) { }
        public void onEndOfSpeech() { }
        public void onError(int error) => owner.HandleAndroidError(error);
        public void onResults(AndroidJavaObject results) => owner.HandleAndroidResults(results);
        public void onPartialResults(AndroidJavaObject partialResults) => owner.HandleAndroidPartialResults(partialResults);
        public void onEvent(int eventType, AndroidJavaObject @params) { }
    }
#endif
}
