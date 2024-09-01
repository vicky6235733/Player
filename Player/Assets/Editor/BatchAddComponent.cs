using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

public class BatchAddComponent : EditorWindow
{
    private string searchString = ""; // 用於搜尋的字串
    private List<string> filteredScriptNames = new List<string>(); // 搜尋結果列表
    private Type selectedScriptType; // 選中的腳本類型
    private List<GameObject> selectedObjects = new List<GameObject>(); // 被選中的物件列表

    [MenuItem("Tools/批量添加腳本")]
    public static void ShowWindow()
    {
        GetWindow<BatchAddComponent>("批量添加腳本");
    }

    private void OnEnable()
    {
        // 初始化搜尋結果
        UpdateFilteredScriptList();
    }

    private void OnGUI()
    {
        // 搜尋框
        EditorGUILayout.LabelField("搜尋腳本:");
        searchString = EditorGUILayout.TextField(searchString);
        UpdateFilteredScriptList();

        // 顯示搜尋結果的下拉式選單
        if (filteredScriptNames.Count > 0)
        {
            int selectedIndex = EditorGUILayout.Popup(0, filteredScriptNames.ToArray());
            selectedScriptType = GetScriptType(filteredScriptNames[selectedIndex]);
        }
        else
        {
            EditorGUILayout.LabelField("找不到符合條件的腳本");
        }

        if (GUILayout.Button("選擇物件"))
        {
            SelectObjects();
        }

        if (GUILayout.Button("添加腳本到選擇物件"))
        {
            ApplyScriptToObjects();
        }

        // 顯示選擇的物件名稱
        if (selectedObjects.Count > 0)
        {
            EditorGUILayout.LabelField("選擇的物件:");
            foreach (var obj in selectedObjects)
            {
                EditorGUILayout.LabelField(obj.name);
            }
        }
    }

    private void UpdateFilteredScriptList()
    {
        // 根據搜尋字串過濾腳本名稱
        var monoBehaviours = TypeCache.GetTypesDerivedFrom<MonoBehaviour>();
        filteredScriptNames = monoBehaviours
            .Where(type => type.FullName.IndexOf(searchString, StringComparison.OrdinalIgnoreCase) >= 0)
            .Select(type => type.FullName)
            .ToList();
    }

    private Type GetScriptType(string typeName)
    {
        // 從所有已加載的程序集中查找腳本類型
        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            var type = assembly.GetType(typeName);
            if (type != null)
            {
                return type;
            }
        }
        return null;
    }

    private void SelectObjects()
    {
        selectedObjects.Clear();
        selectedObjects.AddRange(Selection.gameObjects);
    }

    private void ApplyScriptToObjects()
    {
        if (selectedScriptType == null)
        {
            Debug.LogError("請選擇要添加的腳本");
            return;
        }

        foreach (GameObject obj in selectedObjects)
        {
            if (obj != null && !obj.GetComponent(selectedScriptType))
            {
                obj.AddComponent(selectedScriptType);
            }
        }

        Debug.Log("腳本已成功添加到選擇的物件上");
    }
}
