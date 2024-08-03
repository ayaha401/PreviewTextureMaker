using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class PreviewTextureMaker
{
    /// <summary>
    /// �I���������̂��}�e���A�������ׂ�
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
        Material originalMaterial = Selection.activeObject as Material;

        if (originalMaterial != null)
        {
            Material materialInstance = new Material(originalMaterial);
            MaterialPreviewWindow.ShowWindow(materialInstance);
        }
    }
}
