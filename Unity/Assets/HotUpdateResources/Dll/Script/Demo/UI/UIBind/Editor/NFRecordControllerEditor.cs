﻿using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System.Xml;
using System.Collections;

[CanEditMultipleObjects]
[ExecuteInEditMode]
[CustomEditor(typeof(RecordController))]
public class RecordControllerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        //成员变量就不展示了
        base.OnInspectorGUI();
        Component xRectTransform = this.target as Component;
        
        MatchByColValue(xRectTransform.gameObject);
		MatchByColProperty(xRectTransform.gameObject);
    }

	void MatchByColValue(GameObject go)
    {
		RecordController xRecordController = go.GetComponent<RecordController>();
        
		xRecordController.ColValueCondition = GUILayout.Toggle(xRecordController.ColValueCondition, "MatchByColProperty");
		if (xRecordController.ColValueCondition)
        {
			xRecordController.ColPropertyCondition = false;

			xRecordController.ColConditionNum = EditorGUILayout.IntField("Col", xRecordController.ColConditionNum);
			xRecordController.ColConditionContent = EditorGUILayout.TextField("Value", xRecordController.ColConditionContent);
        }
    }

	void MatchByColProperty(GameObject go)
    {
		RecordController xRecordController = go.GetComponent<RecordController>();

		xRecordController.ColPropertyCondition = GUILayout.Toggle(xRecordController.ColPropertyCondition, "MatchByPropertyName");
		if (xRecordController.ColPropertyCondition)
        {  
			xRecordController.ColValueCondition = false;
            
			xRecordController.ColConditionNum = EditorGUILayout.IntField("Col", xRecordController.ColConditionNum);
			xRecordController.ColConditionPropertyName = EditorGUILayout.TextField("PropertyName", xRecordController.ColConditionPropertyName);
			xRecordController.ColConditionPropertyValue = EditorGUILayout.TextField("Value", xRecordController.ColConditionPropertyValue);
        }
    }
}

