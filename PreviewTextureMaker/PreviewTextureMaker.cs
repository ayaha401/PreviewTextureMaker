using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class PreviewTextureMaker
{
    /// <summary>
    /// 選択したものがマテリアルか調べる
    /// </summary>
    /// <returns></returns>
    [MenuItem("Assets/Create/MaterialPreviewTexture", true)]
    private static bool CheckSelectionObjIsMaterial()
    {
        return Selection.activeObject is Material;
    }

    [MenuItem("Assets/Create/MaterialPreviewTexture", priority = 301)]
    private static void MaterialPreviewTexture()
    {
        Material material = Selection.activeObject as Material;

        if (material != null)
        {
            Debug.Log("Selected Material: " + material.name);
            MaterialPreviewWindow.ShowWindow(material);
        }
    }
}
