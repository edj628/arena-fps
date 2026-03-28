using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

/// <summary>
/// Controls the main menu: panel switching, settings sliders, save/load stubs.
/// </summary>
public class MainMenuUI : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject mainPanel;
    [SerializeField] private GameObject settingsPanel;

    [Header("Settings Sliders")]
    [SerializeField] private Slider mouseSensSlider;
    [SerializeField] private Slider moveSensSlider;

    [Header("Slider Labels")]
    [SerializeField] private TextMeshProUGUI mouseSensLabel;
    [SerializeField] private TextMeshProUGUI moveSensLabel;

    [Header("Scene Names")]
    [SerializeField] private string gameSceneName = "TestScene";

    private void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible   = true;

        ShowMain();
        InitSliders();
    }

    // ── Panel navigation ──────────────────────────────────────────

    public void ShowMain()
    {
        mainPanel.SetActive(true);
        settingsPanel.SetActive(false);
    }

    public void ShowSettings()
    {
        mainPanel.SetActive(false);
        settingsPanel.SetActive(true);
    }

    // ── Main menu buttons ─────────────────────────────────────────

    public void OnNewGame()     => SceneManager.LoadScene(gameSceneName);
    public void OnLoadGame()    => Debug.Log("Load Game — not yet implemented");
    public void OnSettings()    => ShowSettings();
    public void OnQuit()        => Application.Quit();

    // ── Settings ──────────────────────────────────────────────────

    private void InitSliders()
    {
        var s = SettingsManager.Instance;

        mouseSensSlider.minValue = 0.05f;
        mouseSensSlider.maxValue = 2f;
        mouseSensSlider.value    = s != null ? s.MouseSensitivity : SettingsManager.DefaultMouseSensitivity;
        mouseSensSlider.onValueChanged.AddListener(OnMouseSensChanged);
        UpdateMouseLabel(mouseSensSlider.value);

        moveSensSlider.minValue = 4f;
        moveSensSlider.maxValue = 20f;
        moveSensSlider.value    = s != null ? s.MovementSensitivity : SettingsManager.DefaultMovementSensitivity;
        moveSensSlider.onValueChanged.AddListener(OnMoveSensChanged);
        UpdateMoveLabel(moveSensSlider.value);
    }

    private void OnMouseSensChanged(float value)
    {
        SettingsManager.Instance?.SetMouseSensitivity(value);
        UpdateMouseLabel(value);
    }

    private void OnMoveSensChanged(float value)
    {
        SettingsManager.Instance?.SetMovementSensitivity(value);
        UpdateMoveLabel(value);
    }

    public void OnSettingsBack()
    {
        SettingsManager.Instance?.Save();
        ShowMain();
    }

    private void UpdateMouseLabel(float v) =>
        mouseSensLabel.text = $"Mouse Sensitivity: {v:F2}";

    private void UpdateMoveLabel(float v) =>
        moveSensLabel.text = $"Movement Speed: {v:F1}";
}
