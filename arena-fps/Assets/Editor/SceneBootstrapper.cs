using UnityEngine;
using UnityEditor;
using UnityEngine.InputSystem;
using Unity.Netcode;

/// <summary>
/// Menu: Arena FPS > Build Test Scene
/// Programmatically creates a playtest scene with ground, platforms, and a wired-up player.
/// </summary>
public static class SceneBootstrapper
{
    [MenuItem("Arena FPS/Build Test Scene")]
    public static void BuildTestScene()
    {
        // Clear existing objects (except cameras handled below)
        foreach (var go in Object.FindObjectsByType<GameObject>(FindObjectsSortMode.None))
            Object.DestroyImmediate(go);

        // ── Lighting ──────────────────────────────────────────────
        var sun = new GameObject("Directional Light");
        var light = sun.AddComponent<Light>();
        light.type = LightType.Directional;
        light.intensity = 1f;
        sun.transform.rotation = Quaternion.Euler(50f, -30f, 0f);

        // ── Ground ────────────────────────────────────────────────
        CreateBox("Ground", Vector3.zero, new Vector3(60f, 1f, 60f), new Color(0.25f, 0.25f, 0.25f));

        // ── Platforms ─────────────────────────────────────────────
        CreateBox("Platform_A", new Vector3(8f,  2f,  0f),  new Vector3(6f, 0.5f, 6f), new Color(0.4f, 0.35f, 0.3f));
        CreateBox("Platform_B", new Vector3(-8f, 4f,  8f),  new Vector3(6f, 0.5f, 6f), new Color(0.4f, 0.35f, 0.3f));
        CreateBox("Platform_C", new Vector3(0f,  6f, -10f), new Vector3(6f, 0.5f, 6f), new Color(0.4f, 0.35f, 0.3f));

        // Wall-run walls
        CreateBox("Wall_Left",  new Vector3(-4f, 3f, 0f), new Vector3(0.5f, 6f, 10f), new Color(0.3f, 0.3f, 0.5f));
        CreateBox("Wall_Right", new Vector3( 4f, 3f, 0f), new Vector3(0.5f, 6f, 10f), new Color(0.3f, 0.3f, 0.5f));

        // ── Player ────────────────────────────────────────────────
        var player = BuildPlayer();
        player.transform.position = new Vector3(0f, 2f, 0f);

        // ── Network Manager ───────────────────────────────────────
        var nmGo = new GameObject("NetworkManager");
        var nm   = nmGo.AddComponent<NetworkManager>();
        var transport = nmGo.AddComponent<Unity.Netcode.Transports.UTP.UnityTransport>();
        nm.NetworkConfig = new NetworkConfig { NetworkTransport = transport };

        // ── Save scene ────────────────────────────────────────────
        string scenePath = "Assets/_Project/Scenes/TestScene.unity";
        UnityEditor.SceneManagement.EditorSceneManager.SaveScene(
            UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene(), scenePath);

        Debug.Log("Test scene built and saved to " + scenePath);
    }

    // ── Helpers ───────────────────────────────────────────────────

    private static GameObject BuildPlayer()
    {
        // Root
        var root = new GameObject("Player");
        var cc = root.AddComponent<CharacterController>();
        cc.height  = 1.8f;
        cc.radius  = 0.4f;
        cc.center  = new Vector3(0f, 0.9f, 0f);
        cc.skinWidth = 0.02f;

        // Camera holder (child)
        var camHolder = new GameObject("CameraHolder");
        camHolder.transform.SetParent(root.transform);
        camHolder.transform.localPosition = new Vector3(0f, 1.6f, 0f);

        // Camera (grandchild)
        var camGo = new GameObject("MainCamera");
        camGo.tag = "MainCamera";
        camGo.transform.SetParent(camHolder.transform);
        camGo.transform.localPosition = Vector3.zero;
        var cam = camGo.AddComponent<Camera>();
        cam.fieldOfView = 100f;
        cam.nearClipPlane = 0.05f;
        camGo.AddComponent<AudioListener>();

        // PlayerMovement — assign camera holder via reflection-safe SerializedObject
        var movement = root.AddComponent<PlayerMovement>();
        var so = new SerializedObject(movement);
        so.FindProperty("cameraHolder").objectReferenceValue = camHolder.transform;
        so.ApplyModifiedPropertiesWithoutUndo();

        // PlayerInput
        var inputActions = AssetDatabase.LoadAssetAtPath<InputActionAsset>(
            "Assets/_Project/Input/ArenaInputActions.inputactions");
        var playerInput = root.AddComponent<PlayerInput>();
        playerInput.actions = inputActions;
        playerInput.defaultActionMap = "Player";
        playerInput.notificationBehavior = PlayerNotifications.SendMessages;

        // PlayerSetup
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
        go.transform.position = pos;
        go.transform.localScale = scale;

        var mat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        if (mat.shader.name == "Hidden/InternalErrorShader")
            mat = new Material(Shader.Find("Standard")); // fallback if URP not active
        mat.color = color;
        go.GetComponent<Renderer>().material = mat;

        return go;
    }
}
