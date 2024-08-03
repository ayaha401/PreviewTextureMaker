using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class MaterialPreviewWindow : EditorWindow
{
    /// <summary>
    /// 生成するテクスチャのサイズ設定
    /// </summary>
    private enum GenerateSizeKinds
    {
        Half = 512,
        Normal = 1024,
        Large = 2048,
        Custom = -1,
    }

    private Material _material;
    private GenerateSizeKinds sizeKind = GenerateSizeKinds.Normal;
    private Vector2Int textureSize = new Vector2Int(1024, 1024);
    private MaterialEditor materialEditor;
    private Vector2 scrollPosition;

    public static void ShowWindow(Material material)
    {
        MaterialPreviewWindow window = (MaterialPreviewWindow)EditorWindow.GetWindow(typeof(MaterialPreviewWindow));
        window.titleContent = new GUIContent("テクスチャ生成");
        window._material = material;
        window.Show();
    }

    void OnGUI()
    {
        if (_material != null)
        {
            Texture2D previewTexture = GetPreviewTexture(_material, textureSize.x, textureSize.y);
            if (previewTexture != null)
            {
                using (new GUILayout.VerticalScope())
                {
                    PreviewTexture(previewTexture);

                    using (new GUILayout.VerticalScope())
                    {
                        DrawGenerateTextureSize();
                        if (GUILayout.Button("保存"))
                        {
                            DrawSaveTexture(previewTexture);
                        }

                        DrawMaterialInspecter();
                    }
                }
            }
            else
            {
                EditorGUILayout.LabelField("テクスチャのプレビューに失敗");
            }
        }
        else
        {
            EditorGUILayout.LabelField("マテリアルが選択されていません");
        }
    }

    /// <summary>
    /// 選択したマテリアルからTextureプレビューのテクスチャを作成する
    /// </summary>
    /// <param name="material">選択したマテリアル</param>
    /// <param name="width">生成するテクスチャのサイズ</param>
    /// <param name="height">生成するテクスチャのサイズ</param>
    private Texture2D GetPreviewTexture(Material material, int width, int height)
    {
        width = Mathf.Max(width, 2);
        height = Mathf.Max(height, 2);

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

    /// <summary>
    /// テクスチャの生成サイズ
    /// </summary>
    private void DrawGenerateTextureSize()
    {
        sizeKind = (GenerateSizeKinds)EditorGUILayout.EnumPopup("テクスチャサイズ", sizeKind);

        // カスタムサイズを選択時は専用の入力エリアを出す
        if (sizeKind == GenerateSizeKinds.Custom)
        {
            textureSize = EditorGUILayout.Vector2IntField("カスタムサイズ", textureSize);
            textureSize = Vector2Int.Max(textureSize, new Vector2Int(2, 2));
        }
        else
        {
            textureSize.x = (int)sizeKind;
            textureSize.y = (int)sizeKind;
        }
    }

    /// <summary>
    /// テクスチャをプレビューで表示する
    /// </summary>
    /// <param name="texture">生成したテクスチャ</param>
    private void PreviewTexture(Texture2D texture)
    {
        Rect windowRect = position;
        float windowWidth = windowRect.width;
        float windowHeight = windowRect.height;

        // 短いほうの辺を取得する
        float getShorter = Mathf.Min(windowWidth, windowHeight);

        float textureWidth = getShorter;
        float textureHeight = getShorter;
        Rect textureRect = GUILayoutUtility.GetRect(300, 300, GUILayout.ExpandWidth(false), GUILayout.ExpandHeight(false));
        // テクスチャを描画
        GUI.DrawTexture(textureRect, texture);
    }

    /// <summary>
    /// マテリアルのインスペクターを表示する
    /// </summary>
    private void DrawMaterialInspecter()
    {
        EditorGUI.BeginChangeCheck();
        if (_material != null && EditorGUI.EndChangeCheck())
        {
            if (materialEditor != null) DestroyImmediate(materialEditor);
        }

        materialEditor = (MaterialEditor)Editor.CreateEditor(_material, typeof(MaterialEditor));
        if (materialEditor != null)
        {
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
            using (new GUILayout.VerticalScope("Box"))
            {
                materialEditor.DrawHeader();
                materialEditor.OnInspectorGUI();
            }
            EditorGUILayout.EndScrollView();
        }
    }

    /// <summary>
    /// テクスチャを保存する
    /// </summary>
    /// <param name="texture">生成したテクスチャ</param>
    private void DrawSaveTexture(Texture2D texture)
    {
        var path = EditorUtility.SaveFilePanel("名前を付けて保存", "", "Texture", "png");
        if (string.IsNullOrEmpty(path))
        {
            // ユーザーがキャンセルした場合
            return;
        }

        // テクスチャデータをPNGにエンコード
        byte[] pngData = texture.EncodeToPNG();

        if (pngData != null)
        {
            File.WriteAllBytes(path, pngData);
            Debug.Log("テクスチャを保存: " + path);
        }
        else
        {
            Debug.LogError("PNGへのエンコードに失敗");
        }
    }
}
