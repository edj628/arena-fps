using UnityEngine;

/// <summary>
/// Generates procedural textures for weapons at runtime.
/// No external texture files required.
/// </summary>
public static class TextureGenerator
{
    public static Texture2D MachineGun()  => MetalTexture(new Color(0.25f, 0.25f, 0.28f), new Color(0.15f, 0.15f, 0.17f), 64);
    public static Texture2D Shotgun()     => WoodMetalTexture(new Color(0.45f, 0.28f, 0.10f), new Color(0.30f, 0.30f, 0.32f), 64);
    public static Texture2D Railgun()     => SciFiTexture(new Color(0.05f, 0.30f, 0.70f), new Color(0.60f, 0.85f, 1.00f), 64);
    public static Texture2D RocketLauncher() => CamoTexture(new Color(0.25f, 0.30f, 0.15f), new Color(0.40f, 0.35f, 0.20f), 64);

    // ── Dark scratched metal ──────────────────────────────────────────

    private static Texture2D MetalTexture(Color baseColor, Color darkColor, int size)
    {
        var tex = new Texture2D(size, size);
        var pixels = new Color[size * size];
        var rng = new System.Random(1);

        for (int y = 0; y < size; y++)
        for (int x = 0; x < size; x++)
        {
            float noise = (float)rng.NextDouble() * 0.12f;
            // Horizontal scratch lines
            float scratch = (y % 7 == 0) ? -0.08f : 0f;
            pixels[y * size + x] = Color.Lerp(darkColor, baseColor, noise) + new Color(scratch, scratch, scratch);
        }

        tex.SetPixels(pixels);
        tex.Apply();
        return tex;
    }

    // ── Wood stock + metal barrel (alternating bands) ─────────────────

    private static Texture2D WoodMetalTexture(Color wood, Color metal, int size)
    {
        var tex = new Texture2D(size, size);
        var pixels = new Color[size * size];
        var rng = new System.Random(2);

        for (int y = 0; y < size; y++)
        for (int x = 0; x < size; x++)
        {
            float noise = (float)rng.NextDouble() * 0.10f;
            // Wood grain lines
            float grain = Mathf.Sin(y * 0.4f + (float)rng.NextDouble() * 0.5f) * 0.08f;
            bool isWood = y < size * 0.55f;
            pixels[y * size + x] = isWood
                ? wood  + new Color(grain, grain * 0.5f, 0f)
                : metal + new Color(noise, noise, noise);
        }

        tex.SetPixels(pixels);
        tex.Apply();
        return tex;
    }

    // ── Sci-fi glowing panel lines ────────────────────────────────────

    private static Texture2D SciFiTexture(Color baseColor, Color glowColor, int size)
    {
        var tex = new Texture2D(size, size);
        var pixels = new Color[size * size];
        var rng = new System.Random(3);

        for (int y = 0; y < size; y++)
        for (int x = 0; x < size; x++)
        {
            float noise = (float)rng.NextDouble() * 0.08f;
            // Panel lines every 8 pixels
            bool hLine = (y % 8 == 0) || (y % 8 == 1);
            bool vLine = (x % 16 == 0);
            float glow = (hLine || vLine) ? 0.6f : 0f;
            pixels[y * size + x] = Color.Lerp(baseColor + new Color(noise, noise, noise), glowColor, glow);
        }

        tex.SetPixels(pixels);
        tex.Apply();
        return tex;
    }

    // ── Military camo blotches ────────────────────────────────────────

    private static Texture2D CamoTexture(Color dark, Color light, int size)
    {
        var tex = new Texture2D(size, size);
        var pixels = new Color[size * size];
        var rng = new System.Random(4);

        // Seed camo blotch centers
        var centers = new Vector2[12];
        for (int i = 0; i < centers.Length; i++)
            centers[i] = new Vector2((float)rng.NextDouble() * size, (float)rng.NextDouble() * size);

        for (int y = 0; y < size; y++)
        for (int x = 0; x < size; x++)
        {
            float minDist = float.MaxValue;
            int nearest = 0;
            for (int i = 0; i < centers.Length; i++)
            {
                float d = Vector2.Distance(new Vector2(x, y), centers[i]);
                if (d < minDist) { minDist = d; nearest = i; }
            }
            float noise = (float)rng.NextDouble() * 0.06f;
            pixels[y * size + x] = (nearest % 2 == 0 ? dark : light) + new Color(noise, noise, noise);
        }

        tex.SetPixels(pixels);
        tex.Apply();
        return tex;
    }
}
