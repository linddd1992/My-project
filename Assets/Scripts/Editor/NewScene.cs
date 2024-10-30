using UnityEngine;
using UnityEditor;
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor.SceneManagement;

public class SceneCreatorWindow : EditorWindow
{
    private string newSceneName = "";
    private string defaultScenePath = "Assets/Scenes/SampleScene.scene"; // 默认场景的路径

    [MenuItem("Tools/新建场景")]
    public static void ShowWindow()
    {
        GetWindow<SceneCreatorWindow>("Scene Creator");
    }

    private void OnGUI()
    {
        // 输入框用于输入新场景的名称
        newSceneName = EditorGUILayout.TextField("New Scene Name", newSceneName);

        // 按钮用于创建新场景
        if (GUILayout.Button("Create Scene"))
        {
            CreateScene();
        }
    }

    private void CreateScene()
    {
        // 验证场景名称是否有效
        if (string.IsNullOrEmpty(newSceneName))
        {
            EditorUtility.DisplayDialog("Error", "场景名无效.", "OK");
            return;
        }

        if (!Regex.IsMatch(newSceneName, @"^[a-zA-Z0-9_]+$"))
        {
            EditorUtility.DisplayDialog("Error", "Scene name must be alphanumeric or underscores only.", "OK");
            return;
        }

        // 确保默认场景存在
        if (!File.Exists(defaultScenePath))
        {
            EditorUtility.DisplayDialog("Error", "Default scene not found.", "OK");
            return;
        }

        // 创建新场景的路径
        string newScenePath = "Assets/Scenes/" + newSceneName + ".scene";
        if (File.Exists(newScenePath))
        {
            EditorUtility.DisplayDialog("Error", "A scene with the same name already exists.", "OK");
            return;
        }

        // 复制默认场景到新场景
        File.Copy(defaultScenePath, newScenePath, true);

        // 加载新场景
        EditorSceneManager.OpenScene(newScenePath);

        // 提示用户场景已创建
        EditorUtility.DisplayDialog("Success", "创建场景成功.", "OK");
    }
}