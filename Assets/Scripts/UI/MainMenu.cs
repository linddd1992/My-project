using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public InputField sceneInputField;
    public Button loadSceneButton;

    private void Start()
    {
        // 确保已经分配了InputField和Button
        if (sceneInputField == null || loadSceneButton == null)
        {
            Debug.LogError("请在Inspector中分配InputField和Button组件");
            return;
        }

        // 为按钮添加点击事件监听器
        loadSceneButton.onClick.AddListener(LoadInputScene);
    }

    private void LoadInputScene()
    {
        string sceneName = sceneInputField.text.Trim();
        
        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.LogWarning("场景名称不能为空");
            return;
        }

        GameManager.Instance.LoadCustomScene(sceneName);
        // 检查场景是否存在
        // if (Application.CanStreamedLevelBeLoaded(sceneName))
        // {
        //     // 使用GameManager加载场景
        //     GameManager.Instance.LoadCustomScene(sceneName);
        // }
        // else
        // {
        //     Debug.LogWarning($"场景 '{sceneName}' 不存在");
        // }
    }
}
