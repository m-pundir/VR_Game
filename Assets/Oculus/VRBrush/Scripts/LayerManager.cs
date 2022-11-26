using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif


public class LayerManager : SingleTon<LayerManager>
{
    private void Awake()
    {
#if UNITY_EDITOR
        SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);

        SerializedProperty layers = tagManager.FindProperty("layers");

        if (layers == null || !layers.isArray)
        {
            Debug.LogWarning("Can't set up the layers. It's possible the format of the layers and tags data has@@!#!@$%#");
            Debug.LogWarning("Layers is null");

            return;
        }
        int arrSize = layers.arraySize;
        bool isLayerValue = false;
        for (int i = 9; i < arrSize; i++)
        {
            SerializedProperty layerSP = layers.GetArrayElementAtIndex(i);
            if (layerSP.stringValue == "VRBrushUI")
            {
                isLayerValue = true;
            }
        }

        if (!isLayerValue)
        {
            for (int i = 9; i < arrSize; i++)
            {
                SerializedProperty layerSP = layers.GetArrayElementAtIndex(i);
                if (layerSP.stringValue == "")
                {
                    layerSP.stringValue = "VRBrushUI";
                    break;
                }
            }
        }

        tagManager.ApplyModifiedProperties();

        this.gameObject.layer = LayerMask.NameToLayer("VRBrushUI");

        Transform[] child = this.gameObject.GetComponentsInChildren<Transform>();
        for (int i = 0; i < child.Length; i++)
        {
            child[i].gameObject.layer = LayerMask.NameToLayer("VRBrushUI");
        }
#endif
    }

    public void AddLayerMask(string name)
    {
#if UNITY_EDITOR
        SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);

        SerializedProperty layers = tagManager.FindProperty("layers");

        if (layers == null || !layers.isArray)
        {
            Debug.LogWarning("Can't set up the layers. It's possible the format of the layers and tags data has@@!#!@$%#");
            Debug.LogWarning("Layers is null");

            return;
        }
        int arrSize = layers.arraySize;
        bool isLayerValue = false;
        for (int i = 9; i < arrSize; i++)
        {
            SerializedProperty layerSP = layers.GetArrayElementAtIndex(i);
            if (layerSP.stringValue == name)
            {
                isLayerValue = true;
            }
        }

        if (!isLayerValue)
        {
            for (int i = 9; i < arrSize; i++)
            {
                SerializedProperty layerSP = layers.GetArrayElementAtIndex(i);
                if (layerSP.stringValue == "")
                {
                    layerSP.stringValue = name;
                    break;
                }
            }
        }

        tagManager.ApplyModifiedProperties();
#endif
    }
}
