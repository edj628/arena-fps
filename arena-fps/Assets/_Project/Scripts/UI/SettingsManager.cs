using UnityEngine;

/// <summary>
/// Singleton that holds runtime settings and persists them via PlayerPrefs.
/// </summary>
public class SettingsManager : MonoBehaviour
{
    public static SettingsManager Instance { get; private set; }

    // Defaults
    public const float DefaultMouseSensitivity     = 0.5f;
    public const float DefaultMovementSensitivity  = 10f;

    public float MouseSensitivity    { get; private set; }
    public float MovementSensitivity { get; private set; }

    private const string KeyMouse    = "settings_mouse_sens";
    private const string KeyMovement = "settings_move_sens";

    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        Load();
    }

    public void SetMouseSensitivity(float value)
    {
        MouseSensitivity = value;
        PlayerPrefs.SetFloat(KeyMouse, value);
    }

    public void SetMovementSensitivity(float value)
    {
        MovementSensitivity = value;
        PlayerPrefs.SetFloat(KeyMovement, value);
    }

    public void Save() => PlayerPrefs.Save();

    private void Load()
    {
        MouseSensitivity    = PlayerPrefs.GetFloat(KeyMouse,    DefaultMouseSensitivity);
        MovementSensitivity = PlayerPrefs.GetFloat(KeyMovement, DefaultMovementSensitivity);
    }
}
