using UnityEngine;

/// <summary>
/// Builds simple geometry and applies a procedural texture for a held weapon.
/// Attach to the weapon GameObject alongside the weapon script.
/// </summary>
public class WeaponVisuals : MonoBehaviour
{
    public enum WeaponStyle { MachineGun, Shotgun, Railgun, RocketLauncher }

    [SerializeField] private WeaponStyle style;

    private void Start() => Build();

    public void Init(WeaponStyle s) { style = s; Build(); }

    private void Build()
    {
        // Clear any existing children
        foreach (Transform child in transform)
            Destroy(child.gameObject);

        Texture2D tex = style switch
        {
            WeaponStyle.MachineGun     => TextureGenerator.MachineGun(),
            WeaponStyle.Shotgun        => TextureGenerator.Shotgun(),
            WeaponStyle.Railgun        => TextureGenerator.Railgun(),
            WeaponStyle.RocketLauncher => TextureGenerator.RocketLauncher(),
            _                          => TextureGenerator.MachineGun()
        };

        Material mat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        if (mat.shader.name == "Hidden/InternalErrorShader")
            mat = new Material(Shader.Find("Standard"));
        mat.mainTexture = tex;

        switch (style)
        {
            case WeaponStyle.MachineGun:     BuildMachineGun(mat);     break;
            case WeaponStyle.Shotgun:        BuildShotgun(mat);        break;
            case WeaponStyle.Railgun:        BuildRailgun(mat);        break;
            case WeaponStyle.RocketLauncher: BuildRocketLauncher(mat); break;
        }
    }

    // ── Weapon shapes ─────────────────────────────────────────────────

    private void BuildMachineGun(Material mat)
    {
        // Body
        AddBox("Body",   new Vector3(0f,    0f,    0.15f), new Vector3(0.06f, 0.06f, 0.30f), mat);
        // Barrel
        AddBox("Barrel", new Vector3(0f,    0.01f, 0.42f), new Vector3(0.025f,0.025f,0.18f), mat);
        // Stock
        AddBox("Stock",  new Vector3(0f,   -0.02f,-0.04f), new Vector3(0.05f, 0.04f, 0.14f), mat);
        // Magazine
        AddBox("Mag",    new Vector3(0f,   -0.05f, 0.12f), new Vector3(0.03f, 0.08f, 0.04f), mat);
    }

    private void BuildShotgun(Material mat)
    {
        // Body
        AddBox("Body",    new Vector3(0f,  0f,    0.15f), new Vector3(0.065f, 0.055f, 0.28f), mat);
        // Long barrel
        AddBox("Barrel",  new Vector3(0f,  0.015f,0.46f), new Vector3(0.03f,  0.03f,  0.22f), mat);
        // Pump
        AddBox("Pump",    new Vector3(0f, -0.01f, 0.36f), new Vector3(0.04f,  0.025f, 0.08f), mat);
        // Stock (wider wood)
        AddBox("Stock",   new Vector3(0f, -0.015f,-0.05f),new Vector3(0.055f, 0.05f,  0.16f), mat);
    }

    private void BuildRailgun(Material mat)
    {
        // Sleek body
        AddBox("Body",   new Vector3(0f,    0f,    0.18f), new Vector3(0.055f, 0.055f, 0.36f), mat);
        // Thin barrel with slight upward angle
        AddBox("Barrel", new Vector3(0f,    0.005f,0.52f), new Vector3(0.018f, 0.018f, 0.20f), mat);
        // Side fins
        AddBox("FinL",   new Vector3(-0.05f,0f,    0.18f), new Vector3(0.02f,  0.06f,  0.22f), mat);
        AddBox("FinR",   new Vector3( 0.05f,0f,    0.18f), new Vector3(0.02f,  0.06f,  0.22f), mat);
        // Power cell
        AddBox("Cell",   new Vector3(0f,   -0.05f, 0.10f), new Vector3(0.03f,  0.06f,  0.05f), mat);
    }

    private void BuildRocketLauncher(Material mat)
    {
        // Wide tube
        AddBox("Tube",   new Vector3(0f,    0f,    0.20f), new Vector3(0.09f, 0.09f, 0.40f), mat);
        // Muzzle bell
        AddBox("Muzzle", new Vector3(0f,    0f,    0.44f), new Vector3(0.11f, 0.11f, 0.06f), mat);
        // Grip
        AddBox("Grip",   new Vector3(0f,   -0.08f, 0.12f), new Vector3(0.04f, 0.10f, 0.04f), mat);
        // Sight
        AddBox("Sight",  new Vector3(0f,    0.07f, 0.18f), new Vector3(0.015f,0.03f,  0.04f), mat);
    }

    // ── Helper ────────────────────────────────────────────────────────

    private GameObject AddBox(string partName, Vector3 localPos, Vector3 size, Material mat)
    {
        var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
        go.name = partName;
        go.transform.SetParent(transform);
        go.transform.localPosition = localPos;
        go.transform.localRotation = Quaternion.identity;
        go.transform.localScale    = size;
        go.GetComponent<Renderer>().material = mat;

        // No colliders on weapon parts
        Destroy(go.GetComponent<BoxCollider>());

        // Don't cast shadows on the view model
        go.GetComponent<Renderer>().shadowCastingMode =
            UnityEngine.Rendering.ShadowCastingMode.Off;

        return go;
    }
}
