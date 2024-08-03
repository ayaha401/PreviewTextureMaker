using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class MaterialPreviewWindow : EditorWindow
{
    /// <summary>
    /// ��������e�N�X�`���̃T�C�Y�ݒ�
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
        window.titleContent = new GUIContent("�e�N�X�`������");
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
                        if (GUILayout.Button("�ۑ�"))
                        {
                            DrawSaveTexture(previewTexture);
                        }

                        DrawMaterialInspecter();
                    }
                }
            }
            else
            {
                EditorGUILayout.LabelField("�e�N�X�`���̃v���r���[�Ɏ��s");
            }
        }
        else
        {
            EditorGUILayout.LabelField("�}�e���A�����I������Ă��܂���");
        }
    }

    /// <summary>
    /// �I�������}�e���A������Texture�v���r���[�̃e�N�X�`�����쐬����
    /// </summary>
    /// <param name="material">�I�������}�e���A��</param>
    /// <param name="width">��������e�N�X�`���̃T�C�Y</param>
    /// <param name="height">��������e�N�X�`���̃T�C�Y</param>
    private Texture2D GetPreviewTexture(Material material, int width, int height)
    {
        width = Mathf.Max(width, 2);
        height = Mathf.Max(height, 2);

        // RenderTexture�̍쐬
        RenderTexture renderTexture = RenderTexture.GetTemporary(width, height);

        // �������_�����O�^�[�Q�b�g�̕ۑ�
        RenderTexture previous = RenderTexture.active;
        RenderTexture.active = renderTexture;

        // RenderTexture�Ƀe�N�X�`����`��
        GL.PushMatrix();
        GL.LoadPixelMatrix(0, width, height, 0);
        Graphics.Blit(Texture2D.whiteTexture, renderTexture, material);
        GL.PopMatrix();

        // Texture2D�ɃR�s�[
        Texture2D previewTexture = new Texture2D(width, height, TextureFormat.RGBA32, false);
        previewTexture.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        previewTexture.Apply();

        // ���̃����_�����O�^�[�Q�b�g�ɖ߂�
        RenderTexture.active = previous;
        RenderTexture.ReleaseTemporary(renderTexture);

        return previewTexture;
    }

    /// <summary>
    /// �e�N�X�`���̐����T�C�Y
    /// </summary>
    private void DrawGenerateTextureSize()
    {
        sizeKind = (GenerateSizeKinds)EditorGUILayout.EnumPopup("�e�N�X�`���T�C�Y", sizeKind);

        // �J�X�^���T�C�Y��I�����͐�p�̓��̓G���A���o��
        if (sizeKind == GenerateSizeKinds.Custom)
        {
            textureSize = EditorGUILayout.Vector2IntField("�J�X�^���T�C�Y", textureSize);
            textureSize = Vector2Int.Max(textureSize, new Vector2Int(2, 2));
        }
        else
        {
            textureSize.x = (int)sizeKind;
            textureSize.y = (int)sizeKind;
        }
    }

    /// <summary>
    /// �e�N�X�`�����v���r���[�ŕ\������
    /// </summary>
    /// <param name="texture">���������e�N�X�`��</param>
    private void PreviewTexture(Texture2D texture)
    {
        Rect windowRect = position;
        float windowWidth = windowRect.width;
        float windowHeight = windowRect.height;

        // �Z���ق��̕ӂ��擾����
        float getShorter = Mathf.Min(windowWidth, windowHeight);

        float textureWidth = getShorter;
        float textureHeight = getShorter;
        Rect textureRect = GUILayoutUtility.GetRect(300, 300, GUILayout.ExpandWidth(false), GUILayout.ExpandHeight(false));
        // �e�N�X�`����`��
        GUI.DrawTexture(textureRect, texture);
    }

    /// <summary>
    /// �}�e���A���̃C���X�y�N�^�[��\������
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
    /// �e�N�X�`����ۑ�����
    /// </summary>
    /// <param name="texture">���������e�N�X�`��</param>
    private void DrawSaveTexture(Texture2D texture)
    {
        var path = EditorUtility.SaveFilePanel("���O��t���ĕۑ�", "", "Texture", "png");
        if (string.IsNullOrEmpty(path))
        {
            // ���[�U�[���L�����Z�������ꍇ
            return;
        }

        // �e�N�X�`���f�[�^��PNG�ɃG���R�[�h
        byte[] pngData = texture.EncodeToPNG();

        if (pngData != null)
        {
            File.WriteAllBytes(path, pngData);
            Debug.Log("�e�N�X�`����ۑ�: " + path);
        }
        else
        {
            Debug.LogError("PNG�ւ̃G���R�[�h�Ɏ��s");
        }
    }
}
