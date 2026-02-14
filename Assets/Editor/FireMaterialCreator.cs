using System.IO;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public static class FireMaterialCreator
{
    private const string MaterialsFolder = "Assets/Materials";
    private const string TexturePath = "Assets/Materials/FireGradient.png";
    private const string MaterialPath = "Assets/Materials/FireMaterial.mat";

    static FireMaterialCreator()
    {
        EditorApplication.delayCall += EnsureFireMaterial;
    }

    private static void EnsureFireMaterial()
    {
        if (!AssetDatabase.IsValidFolder(MaterialsFolder))
        {
            AssetDatabase.CreateFolder("Assets", "Materials");
        }

        Texture2D fireTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(TexturePath);
        if (fireTexture == null)
        {
            fireTexture = CreateFireGradientTexture();
            SaveTextureAsset(fireTexture, TexturePath);
            ConfigureTextureImporter(TexturePath);
        }

        Material fireMaterial = AssetDatabase.LoadAssetAtPath<Material>(MaterialPath);
        if (fireMaterial == null)
        {
            Shader shader = Shader.Find("Universal Render Pipeline/Particles/Unlit");
            if (shader == null)
            {
                shader = Shader.Find("Particles/Standard Unlit");
            }

            if (shader == null)
            {
                Debug.LogWarning("FireMaterialCreator: Could not find a particle shader.");
                return;
            }

            fireMaterial = new Material(shader);
            fireMaterial.SetColor("_BaseColor", new Color(1f, 0.5f, 0.15f, 1f));
            fireMaterial.SetColor("_Color", new Color(1f, 0.5f, 0.15f, 1f));
            if (fireTexture != null)
            {
                fireMaterial.SetTexture("_BaseMap", fireTexture);
                fireMaterial.SetTexture("_MainTex", fireTexture);
            }

            AssetDatabase.CreateAsset(fireMaterial, MaterialPath);
            AssetDatabase.SaveAssets();
        }
    }

    private static Texture2D CreateFireGradientTexture()
    {
        int size = 128;
        Texture2D texture = new Texture2D(size, size, TextureFormat.RGBA32, false, false);
        texture.name = "FireGradient";

        Vector2 center = new Vector2(size * 0.5f, size * 0.2f);
        float maxDist = size * 0.55f;

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float dist = Vector2.Distance(new Vector2(x, y), center);
                float t = Mathf.Clamp01(dist / maxDist);

                Color core = new Color(1f, 0.95f, 0.7f, 1f);
                Color mid = new Color(1f, 0.55f, 0.2f, 0.9f);
                Color edge = new Color(0.6f, 0.1f, 0.05f, 0f);

                Color color = Color.Lerp(core, mid, t);
                color = Color.Lerp(color, edge, Mathf.Pow(t, 1.8f));
                texture.SetPixel(x, y, color);
            }
        }

        texture.Apply();
        return texture;
    }

    private static void SaveTextureAsset(Texture2D texture, string assetPath)
    {
        byte[] pngData = texture.EncodeToPNG();
        File.WriteAllBytes(assetPath, pngData);
        AssetDatabase.ImportAsset(assetPath, ImportAssetOptions.ForceUpdate);
    }

    private static void ConfigureTextureImporter(string assetPath)
    {
        TextureImporter importer = AssetImporter.GetAtPath(assetPath) as TextureImporter;
        if (importer == null)
        {
            return;
        }

        importer.textureType = TextureImporterType.Default;
        importer.sRGBTexture = true;
        importer.alphaIsTransparency = true;
        importer.mipmapEnabled = false;
        importer.wrapMode = TextureWrapMode.Clamp;
        importer.filterMode = FilterMode.Bilinear;
        importer.SaveAndReimport();
    }
}
