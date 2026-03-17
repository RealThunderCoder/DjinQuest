using System.IO;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public static class KingdomHeartsCubeMaterialCreator
{
    private const string MaterialsFolder = "Assets/Materials";
    private const string TexturePath = "Assets/Materials/KingdomHeartsSkyGradient.png";
    private const string MaterialPath = "Assets/Materials/KingdomHeartsCubeSky.mat";

    static KingdomHeartsCubeMaterialCreator()
    {
        EditorApplication.delayCall += EnsureMaterial;
    }

    private static void EnsureMaterial()
    {
        if (!AssetDatabase.IsValidFolder(MaterialsFolder))
        {
            AssetDatabase.CreateFolder("Assets", "Materials");
        }

        Texture2D texture = AssetDatabase.LoadAssetAtPath<Texture2D>(TexturePath);
        if (texture == null)
        {
            texture = CreateGradientTexture();
            SaveTexture(texture, TexturePath);
            ConfigureTextureImporter(TexturePath);
        }

        Material material = AssetDatabase.LoadAssetAtPath<Material>(MaterialPath);
        if (material == null)
        {
            Shader shader = Shader.Find("Universal Render Pipeline/Unlit");
            if (shader == null)
            {
                shader = Shader.Find("Unlit/Texture");
            }

            if (shader == null)
            {
                Debug.LogWarning("KingdomHeartsCubeMaterialCreator: No unlit shader found.");
                return;
            }

            material = new Material(shader);
            material.SetTexture("_BaseMap", texture);
            material.SetTexture("_MainTex", texture);
            AssetDatabase.CreateAsset(material, MaterialPath);
            AssetDatabase.SaveAssets();
        }
    }

    private static Texture2D CreateGradientTexture()
    {
        int width = 256;
        int height = 256;
        Texture2D tex = new Texture2D(width, height, TextureFormat.RGBA32, false, false);
        tex.name = "KingdomHeartsSkyGradient";

        Color top = new Color(0.4f, 0.72f, 1f, 1f);
        Color mid = new Color(0.85f, 0.78f, 1f, 1f);
        Color bottom = new Color(0.98f, 0.86f, 0.95f, 1f);

        for (int y = 0; y < height; y++)
        {
            float t = y / (height - 1f);
            Color color = t < 0.6f
                ? Color.Lerp(bottom, mid, t / 0.6f)
                : Color.Lerp(mid, top, (t - 0.6f) / 0.4f);

            for (int x = 0; x < width; x++)
            {
                tex.SetPixel(x, y, color);
            }
        }

        tex.Apply();
        return tex;
    }

    private static void SaveTexture(Texture2D tex, string path)
    {
        byte[] png = tex.EncodeToPNG();
        File.WriteAllBytes(path, png);
        AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
    }

    private static void ConfigureTextureImporter(string path)
    {
        TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
        if (importer == null)
        {
            return;
        }

        importer.textureType = TextureImporterType.Default;
        importer.sRGBTexture = true;
        importer.alphaIsTransparency = false;
        importer.mipmapEnabled = false;
        importer.wrapMode = TextureWrapMode.Clamp;
        importer.filterMode = FilterMode.Bilinear;
        importer.SaveAndReimport();
    }
}
