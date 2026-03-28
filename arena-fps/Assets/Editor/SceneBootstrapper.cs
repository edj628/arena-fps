using UnityEngine;
using UnityEditor;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Unity.Netcode;
using TMPro;

/// <summary>
/// Arena FPS > Build Test Scene  — playtest level with player
/// Arena FPS > Build Main Menu   — main menu with settings
/// </summary>
public static class SceneBootstrapper
{
    // ================================================================
    //  Test Scene
    // ================================================================

    [MenuItem("Arena FPS/Build Test Scene")]
    public static void BuildTestScene()
    {
        foreach (var go in Object.FindObjectsByType<GameObject>(FindObjectsSortMode.None))
            Object.DestroyImmediate(go);

        // Lighting
        var sun = new GameObject("Directional Light");
        var light = sun.AddComponent<Light>();
        light.type = LightType.Directional;
        light.intensity = 1f;
        sun.transform.rotation = Quaternion.Euler(50f, -30f, 0f);

        // Ground + platforms
        CreateBox("Ground",     Vector3.zero,                new Vector3(60f, 1f, 60f),   new Color(0.25f, 0.25f, 0.25f));
        CreateBox("Platform_A", new Vector3(8f,  2f,  0f),  new Vector3(6f, 0.5f, 6f),   new Color(0.4f, 0.35f, 0.3f));
        CreateBox("Platform_B", new Vector3(-8f, 4f,  8f),  new Vector3(6f, 0.5f, 6f),   new Color(0.4f, 0.35f, 0.3f));
        CreateBox("Platform_C", new Vector3(0f,  6f, -10f), new Vector3(6f, 0.5f, 6f),   new Color(0.4f, 0.35f, 0.3f));
        CreateBox("Wall_Left",  new Vector3(-4f, 3f, 0f),   new Vector3(0.5f, 6f, 10f),  new Color(0.3f, 0.3f, 0.5f));
        CreateBox("Wall_Right", new Vector3( 4f, 3f, 0f),   new Vector3(0.5f, 6f, 10f),  new Color(0.3f, 0.3f, 0.5f));

        // Player
        var player = BuildPlayer();
        player.transform.position = new Vector3(0f, 2f, 0f);

        // Settings Manager
        var smGo = new GameObject("SettingsManager");
        smGo.AddComponent<SettingsManager>();

        // Network Manager
        var nmGo = new GameObject("NetworkManager");
        var nm   = nmGo.AddComponent<NetworkManager>();
        var transport = nmGo.AddComponent<Unity.Netcode.Transports.UTP.UnityTransport>();
        nm.NetworkConfig = new NetworkConfig { NetworkTransport = transport };

        SaveScene("Assets/_Project/Scenes/TestScene.unity");
    }

    // ================================================================
    //  Main Menu Scene
    // ================================================================

    [MenuItem("Arena FPS/Build Main Menu")]
    public static void BuildMainMenuScene()
    {
        foreach (var go in Object.FindObjectsByType<GameObject>(FindObjectsSortMode.None))
            Object.DestroyImmediate(go);

        // Settings Manager (persists to game scene via DontDestroyOnLoad)
        var smGo = new GameObject("SettingsManager");
        smGo.AddComponent<SettingsManager>();

        // Camera
        var camGo = new GameObject("Camera");
        camGo.tag = "MainCamera";
        var cam = camGo.AddComponent<Camera>();
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = new Color(0.05f, 0.05f, 0.08f);
        camGo.AddComponent<AudioListener>();

        // Canvas
        var canvasGo = new GameObject("Canvas");
        var canvas = canvasGo.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasGo.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        canvasGo.AddComponent<GraphicRaycaster>();
        new GameObject("EventSystem").AddComponent<UnityEngine.EventSystems.EventSystem>()
            .gameObject.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();

        // ── Main Panel ──────────────────────────────────────────────
        var mainPanel = CreatePanel("MainPanel", canvasGo.transform, Color.clear);

        CreateTMPLabel("Title", mainPanel.transform,
            "ARENA FPS",
            new Vector2(0f, 120f), new Vector2(500f, 80f),
            fontSize: 52, bold: true, color: new Color(0.9f, 0.3f, 0.1f));

        var btnNewGame  = CreateButton("Btn_NewGame",  mainPanel.transform, "NEW GAME",    new Vector2(0f,  30f));
        var btnLoadGame = CreateButton("Btn_LoadGame", mainPanel.transform, "LOAD GAME",   new Vector2(0f, -30f));
        var btnSettings = CreateButton("Btn_Settings", mainPanel.transform, "SETTINGS",    new Vector2(0f, -90f));
        var btnQuit     = CreateButton("Btn_Quit",     mainPanel.transform, "QUIT",         new Vector2(0f,-150f));

        // ── Settings Panel ──────────────────────────────────────────
        var settingsPanel = CreatePanel("SettingsPanel", canvasGo.transform, Color.clear);
        settingsPanel.SetActive(false);

        CreateTMPLabel("Title", settingsPanel.transform,
            "SETTINGS",
            new Vector2(0f, 160f), new Vector2(400f, 60f),
            fontSize: 40, bold: true, color: Color.white);

        // Mouse sensitivity slider
        CreateTMPLabel("MouseSensLabel", settingsPanel.transform,
            "Mouse Sensitivity: 0.50", new Vector2(0f, 70f), new Vector2(380f, 36f),
            fontSize: 22, bold: false, color: Color.white);
        var mouseSlider = CreateSlider("MouseSensSlider", settingsPanel.transform, new Vector2(0f, 30f));

        // Movement sensitivity slider
        CreateTMPLabel("MoveSensLabel", settingsPanel.transform,
            "Movement Speed: 10.0", new Vector2(0f, -30f), new Vector2(380f, 36f),
            fontSize: 22, bold: false, color: Color.white);
        var moveSlider = CreateSlider("MoveSensSlider", settingsPanel.transform, new Vector2(0f, -70f));

        var btnBack = CreateButton("Btn_Back", settingsPanel.transform, "BACK", new Vector2(0f, -150f));

        // ── Wire up MainMenuUI ──────────────────────────────────────
        var menuUI = canvasGo.AddComponent<MainMenuUI>();
        var so = new SerializedObject(menuUI);
        so.FindProperty("mainPanel").objectReferenceValue     = mainPanel;
        so.FindProperty("settingsPanel").objectReferenceValue = settingsPanel;
        so.FindProperty("mouseSensSlider").objectReferenceValue = mouseSlider;
        so.FindProperty("moveSensSlider").objectReferenceValue  = moveSlider;
        so.FindProperty("mouseSensLabel").objectReferenceValue  =
            settingsPanel.transform.Find("MouseSensLabel").GetComponent<TextMeshProUGUI>();
        so.FindProperty("moveSensLabel").objectReferenceValue   =
            settingsPanel.transform.Find("MoveSensLabel").GetComponent<TextMeshProUGUI>();
        so.FindProperty("gameSceneName").stringValue = "TestScene";
        so.ApplyModifiedPropertiesWithoutUndo();

        // Button listeners
        btnNewGame .GetComponent<Button>().onClick.AddListener(menuUI.OnNewGame);
        btnLoadGame.GetComponent<Button>().onClick.AddListener(menuUI.OnLoadGame);
        btnSettings.GetComponent<Button>().onClick.AddListener(menuUI.OnSettings);
        btnQuit    .GetComponent<Button>().onClick.AddListener(menuUI.OnQuit);
        btnBack    .GetComponent<Button>().onClick.AddListener(menuUI.OnSettingsBack);

        SaveScene("Assets/_Project/Scenes/MainMenu.unity");
    }

    // ================================================================
    //  Shared helpers
    // ================================================================

    private static GameObject BuildPlayer()
    {
        var root = new GameObject("Player");
        var cc = root.AddComponent<CharacterController>();
        cc.height    = 1.8f;
        cc.radius    = 0.4f;
        cc.center    = new Vector3(0f, 0.9f, 0f);
        cc.skinWidth = 0.02f;

        var camHolder = new GameObject("CameraHolder");
        camHolder.transform.SetParent(root.transform);
        camHolder.transform.localPosition = new Vector3(0f, 1.6f, 0f);

        var camGo = new GameObject("MainCamera");
        camGo.tag = "MainCamera";
        camGo.transform.SetParent(camHolder.transform);
        camGo.transform.localPosition = Vector3.zero;
        var cam = camGo.AddComponent<Camera>();
        cam.fieldOfView    = 100f;
        cam.nearClipPlane  = 0.05f;
        camGo.AddComponent<AudioListener>();

        var movement = root.AddComponent<PlayerMovement>();
        var so = new SerializedObject(movement);
        so.FindProperty("cameraHolder").objectReferenceValue = camHolder.transform;
        so.ApplyModifiedPropertiesWithoutUndo();

        var inputActions = AssetDatabase.LoadAssetAtPath<InputActionAsset>(
            "Assets/_Project/Input/ArenaInputActions.inputactions");
        var playerInput = root.AddComponent<PlayerInput>();
        playerInput.actions            = inputActions;
        playerInput.defaultActionMap   = "Player";
        playerInput.notificationBehavior = PlayerNotifications.SendMessages;

        var setup = root.AddComponent<PlayerSetup>();
        var soSetup = new SerializedObject(setup);
        soSetup.FindProperty("cameraRig").objectReferenceValue = camHolder;
        soSetup.ApplyModifiedPropertiesWithoutUndo();

        return root;
    }

    private static GameObject CreateBox(string name, Vector3 pos, Vector3 scale, Color color)
    {
        var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
        go.name = name;
        go.transform.position   = pos;
        go.transform.localScale = scale;
        var mat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        if (mat.shader.name == "Hidden/InternalErrorShader")
            mat = new Material(Shader.Find("Standard"));
        mat.color = color;
        go.GetComponent<Renderer>().material = mat;
        return go;
    }

    private static GameObject CreatePanel(string name, Transform parent, Color bgColor)
    {
        var go  = new GameObject(name);
        go.transform.SetParent(parent, false);
        var img = go.AddComponent<Image>();
        img.color = bgColor;
        var rt  = go.GetComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
        return go;
    }

    private static GameObject CreateButton(string name, Transform parent, string label, Vector2 anchoredPos)
    {
        var go  = new GameObject(name);
        go.transform.SetParent(parent, false);
        var img = go.AddComponent<Image>();
        img.color = new Color(0.15f, 0.15f, 0.2f);
        var btn = go.AddComponent<Button>();
        var colors = btn.colors;
        colors.highlightedColor = new Color(0.9f, 0.3f, 0.1f);
        colors.pressedColor     = new Color(0.6f, 0.2f, 0.05f);
        btn.colors = colors;
        var rt  = go.GetComponent<RectTransform>();
        rt.sizeDelta     = new Vector2(260f, 50f);
        rt.anchoredPosition = anchoredPos;

        var textGo = new GameObject("Text");
        textGo.transform.SetParent(go.transform, false);
        var tmp = textGo.AddComponent<TextMeshProUGUI>();
        tmp.text      = label;
        tmp.fontSize  = 20;
        tmp.fontStyle = FontStyles.Bold;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color     = Color.white;
        var trt = textGo.GetComponent<RectTransform>();
        trt.anchorMin   = Vector2.zero;
        trt.anchorMax   = Vector2.one;
        trt.offsetMin   = Vector2.zero;
        trt.offsetMax   = Vector2.zero;

        return go;
    }

    private static TextMeshProUGUI CreateTMPLabel(string name, Transform parent, string text,
        Vector2 anchoredPos, Vector2 size, float fontSize, bool bold, Color color)
    {
        var go  = new GameObject(name);
        go.transform.SetParent(parent, false);
        var tmp = go.AddComponent<TextMeshProUGUI>();
        tmp.text      = text;
        tmp.fontSize  = fontSize;
        tmp.fontStyle = bold ? FontStyles.Bold : FontStyles.Normal;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color     = color;
        var rt = go.GetComponent<RectTransform>();
        rt.sizeDelta        = size;
        rt.anchoredPosition = anchoredPos;
        return tmp;
    }

    private static Slider CreateSlider(string name, Transform parent, Vector2 anchoredPos)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent, false);
        var rt = go.AddComponent<RectTransform>().gameObject.GetComponent<RectTransform>();
        rt = go.GetComponent<RectTransform>();
        rt.sizeDelta        = new Vector2(340f, 24f);
        rt.anchoredPosition = anchoredPos;

        // Background
        var bg = new GameObject("Background");
        bg.transform.SetParent(go.transform, false);
        var bgImg = bg.AddComponent<Image>();
        bgImg.color = new Color(0.2f, 0.2f, 0.2f);
        var bgRt = bg.GetComponent<RectTransform>();
        bgRt.anchorMin = Vector2.zero; bgRt.anchorMax = Vector2.one;
        bgRt.offsetMin = Vector2.zero; bgRt.offsetMax = Vector2.zero;

        // Fill area
        var fillArea = new GameObject("Fill Area");
        fillArea.transform.SetParent(go.transform, false);
        var faRt = fillArea.AddComponent<RectTransform>();
        faRt.anchorMin = Vector2.zero; faRt.anchorMax = Vector2.one;
        faRt.offsetMin = new Vector2(5, 0); faRt.offsetMax = new Vector2(-15, 0);

        var fill = new GameObject("Fill");
        fill.transform.SetParent(fillArea.transform, false);
        var fillImg = fill.AddComponent<Image>();
        fillImg.color = new Color(0.9f, 0.3f, 0.1f);
        var fillRt = fill.GetComponent<RectTransform>();
        fillRt.anchorMin = Vector2.zero; fillRt.anchorMax = Vector2.one;
        fillRt.offsetMin = Vector2.zero; fillRt.offsetMax = Vector2.zero;

        // Handle
        var handleArea = new GameObject("Handle Slide Area");
        handleArea.transform.SetParent(go.transform, false);
        var haRt = handleArea.AddComponent<RectTransform>();
        haRt.anchorMin = Vector2.zero; haRt.anchorMax = Vector2.one;
        haRt.offsetMin = new Vector2(10, 0); haRt.offsetMax = new Vector2(-10, 0);

        var handle = new GameObject("Handle");
        handle.transform.SetParent(handleArea.transform, false);
        var handleImg = handle.AddComponent<Image>();
        handleImg.color = Color.white;
        var handleRt = handle.GetComponent<RectTransform>();
        handleRt.sizeDelta = new Vector2(20f, 28f);

        var slider = go.AddComponent<Slider>();
        slider.fillRect        = fill.GetComponent<RectTransform>();
        slider.handleRect      = handle.GetComponent<RectTransform>();
        slider.targetGraphic   = handleImg;
        slider.direction       = Slider.Direction.LeftToRight;

        return slider;
    }

    private static void SaveScene(string path)
    {
        UnityEditor.SceneManagement.EditorSceneManager.SaveScene(
            UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene(), path);
        Debug.Log("Scene saved to " + path);
    }
}
