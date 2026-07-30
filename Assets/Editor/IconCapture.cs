using UnityEngine;
using UnityEditor;
using System.IO;

public class IconCapture
{
    [MenuItem("Tools/Capture Icon %#i")]
    static void Capture()
    {
        Camera cam = Camera.main;
        var oldFlags = cam.clearFlags;
        var oldBg = cam.backgroundColor;

        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = new Color(0, 0, 0, 0);

        RenderTexture rt = new RenderTexture(512, 512, 24, RenderTextureFormat.ARGB32);
        cam.targetTexture = rt;
        cam.Render();

        RenderTexture.active = rt;
        Texture2D tex = new Texture2D(512, 512, TextureFormat.ARGB32, false);
        tex.ReadPixels(new Rect(0, 0, 512, 512), 0, 0);
        tex.Apply();

        byte[] bytes = tex.EncodeToPNG();
        string path = EditorUtility.SaveFilePanel("Save Icon", Application.dataPath, "Icon", "png");
        if (!string.IsNullOrEmpty(path))
        {
            File.WriteAllBytes(path, bytes);
            AssetDatabase.Refresh();
        }

        cam.targetTexture = null;
        cam.clearFlags = oldFlags;
        cam.backgroundColor = oldBg;
        RenderTexture.active = null;
        Object.DestroyImmediate(rt);
        Object.DestroyImmediate(tex);
    }
}