using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

public class CopyComponent : Editor
{

#if UNITY_EDITOR
    static Component[] compoentArr;

    [MenuItem("Tools/Component/Copy All %&A")]
    static void DoCopyComponent()
    {
        Component[] allCompoents = Selection.activeGameObject.GetComponents<Component>();
        if (allCompoents == null) return;
    }

    [MenuItem("Tools/Component/Copy All Execpt Transform %&E")]
    static void DoCopyComponent_Except_1()
    {
        Component[] allCompoents = Selection.activeGameObject.GetComponents<Component>();
        if (allCompoents == null) return;

        int length = 0;
        for (int i = 0; i < allCompoents.Length; i++)
        {
            if (allCompoents[i].GetType().Name == "RectTransform" || allCompoents[i].GetType().Name == "Transform") continue;
            length++;
        }

        if (length == 0) return;

        compoentArr = new Component[length];
        for (int i = 0,j = 0; i < allCompoents.Length; i++)
        {
            if (allCompoents[i].GetType().Name == "RectTransform" || allCompoents[i].GetType().Name == "Transform") continue;
            compoentArr[j] = allCompoents[i];
            j++;
        }

    }

    [MenuItem("Tools/Component/Paste %&V")]
    static void DoPasteComponent()
    {
        if (compoentArr == null)
        {
            return;
        }

        GameObject targetObject = Selection.activeGameObject;
        if (targetObject == null)
        {
            return;
        }

        for (int i = 0; i < compoentArr.Length; i++)
        {
            Component newComponent = compoentArr[i];
            if (newComponent == null)
            {
                continue;
            }
            UnityEditorInternal.ComponentUtility.CopyComponent(newComponent);
            Component oldComponent = targetObject.GetComponent(newComponent.GetType());
            if (oldComponent != null) // 存在旧的组件
            {
                if (UnityEditorInternal.ComponentUtility.PasteComponentValues(oldComponent))
                {
                    Debug.Log("Paste Component" + newComponent.GetType().ToString() + " Success");
                }
                else
                {
                    Debug.Log("Paste Component " + newComponent.GetType().ToString() + " Failed");
                }
            }
            else // 存在旧的组件
            {
                if (UnityEditorInternal.ComponentUtility.PasteComponentAsNew(targetObject))
                {
                    Debug.Log("Paste Component Overwrited" + newComponent.GetType().ToString() + " Success");
                }
                else
                {
                    Debug.Log("Paste Component Overwrited" + newComponent.GetType().ToString() + " Failed");
                }
            }
        }
    }
#endif
}