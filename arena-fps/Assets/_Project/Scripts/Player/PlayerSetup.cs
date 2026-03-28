using UnityEngine;

/// <summary>
/// Bootstraps the local player prefab: positions the camera, sets layers.
/// Attach to the Player prefab root.
/// </summary>
public class PlayerSetup : MonoBehaviour
{
    [SerializeField] private GameObject cameraRig;
    [SerializeField] private GameObject[] localOnlyObjects;  // e.g. arms, HUD canvas
    [SerializeField] private GameObject[] remoteOnlyObjects; // e.g. full body mesh

    /// <summary>Call this after spawning to configure local vs remote visuals.</summary>
    public void SetupLocal()
    {
        cameraRig.SetActive(true);
        foreach (var obj in localOnlyObjects)  obj.SetActive(true);
        foreach (var obj in remoteOnlyObjects) obj.SetActive(false);
    }

    public void SetupRemote()
    {
        cameraRig.SetActive(false);
        foreach (var obj in localOnlyObjects)  obj.SetActive(false);
        foreach (var obj in remoteOnlyObjects) obj.SetActive(true);
    }
}
