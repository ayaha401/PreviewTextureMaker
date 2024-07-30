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

            // �Z���ق��̕ӂ��擾����
            float getShorter = Mathf.Min(windowWidth, windowHeight);

            float textureWidth = getShorter;
            float textureHeight = getShorter;

            // �e�N�X�`���̕\���ʒu���v�Z
            float x = (windowWidth - textureWidth) / 2;
            float y = (windowHeight - textureHeight) / 2;


            EditorGUI.DrawPreviewTexture(new Rect(x, y, textureWidth, textureHeight), Texture2D.whiteTexture, material);

            if (GUI.Button(new Rect(0, 0, 100, 20), "����"))
            {
                Texture2D previewTexture = GetPreviewTexture(material, 256, 256);
                if (previewTexture != null)
                {
                    var path = EditorUtility.SaveFilePanel("���O��t���ĕۑ�", "", "Texture", "png");
                    if (string.IsNullOrEmpty(path))
                    {
                        // ���[�U�[���L�����Z�������ꍇ
                        return;
                    }

                    // �e�N�X�`���f�[�^��PNG�ɃG���R�[�h
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
}
