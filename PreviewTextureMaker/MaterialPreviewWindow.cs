using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class MaterialPreviewWindow : EditorWindow
{
    private Material material;

    public static void ShowWindow(Material material)
    {
        MaterialPreviewWindow window = (MaterialPreviewWindow)EditorWindow.GetWindow(typeof(MaterialPreviewWindow));
        window.titleContent = new GUIContent("Material Preview");
        window.material = material;
        window.Show();
    }

    void OnGUI()
    {
        if (material != null)
        {
            Rect windowRect = position;
            float windowWidth = windowRect.width;
            float windowHeight = windowRect.height;

            // 短いほうの辺を取得する
            float getShorter = Mathf.Min(windowWidth, windowHeight);

            float textureWidth = getShorter;
            float textureHeight = getShorter;

            // テクスチャの表示位置を計算
            float x = (windowWidth - textureWidth) / 2;
            float y = (windowHeight - textureHeight) / 2;


            EditorGUI.DrawPreviewTexture(new Rect(0, 0, textureWidth, textureHeight), Texture2D.whiteTexture, material);

            // ボタンを描画（例：テクスチャの下にボタンを配置）
            if (y + textureHeight < windowHeight)
            {
                // テクスチャの下に空きスペースがある場合
                if (GUI.Button(new Rect(x, y + textureHeight, textureWidth, 30), "Below Button"))
                {
                    Debug.Log("Below Button clicked");
                }
            }
            else if (x + textureWidth < windowWidth)
            {
                // テクスチャの右に空きスペースがある場合
                if (GUI.Button(new Rect(x + textureWidth, y, 100, textureHeight), "Side Button"))
                {
                    Debug.Log("Side Button clicked");
                }
            }



            if (GUI.Button(new Rect(0, 0, 100, 20), "生成"))
            {
                Texture2D previewTexture = GetPreviewTexture(material, 256, 256);
                if (previewTexture != null)
                {
                    var path = EditorUtility.SaveFilePanel("名前を付けて保存", "", "Texture", "png");
                    if (string.IsNullOrEmpty(path))
                    {
                        // ユーザーがキャンセルした場合
                        return;
                    }

                    // テクスチャデータをPNGにエンコード
                    byte[] pngData = previewTexture.EncodeToPNG();

                    if (pngData != null)
                    {
                        File.WriteAllBytes(path, pngData);
                        Debug.Log("Texture saved to: " + path);
                    }
                    else
                    {
                        Debug.LogError("Failed to encode texture to PNG.");
                    }
                }
            }


        }
        else
        {
            EditorGUILayout.LabelField("No material selected.");
        }
    }

    Texture2D GetPreviewTexture(Material material, int width, int height)
    {
        // RenderTextureの作成
        RenderTexture renderTexture = RenderTexture.GetTemporary(width, height);
        // 旧レンダリングターゲットの保存
        RenderTexture previous = RenderTexture.active;
        RenderTexture.active = renderTexture;

        // RenderTextureにテクスチャを描画
        GL.PushMatrix();
        GL.LoadPixelMatrix(0, width, height, 0);
        Graphics.Blit(Texture2D.whiteTexture, renderTexture, material);
        GL.PopMatrix();

        // Texture2Dにコピー
        Texture2D previewTexture = new Texture2D(width, height, TextureFormat.RGBA32, false);
        previewTexture.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        previewTexture.Apply();

        // 元のレンダリングターゲットに戻す
        RenderTexture.active = previous;
        RenderTexture.ReleaseTemporary(renderTexture);

        return previewTexture;
    }
}
