using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class UIManager : MonoBehaviour
{
    [SerializeField] private string uiPrefabPath = "UI/"; // 预制体所在的路径

    // 单例模式优化
    private static UIManager _instance;
    public static UIManager Instance
    {
        get
        {
            if (_instance == null)
            {
                Debug.Log("UIManager Instance is null");
                GameObject go = new GameObject("UIManager");
                _instance = go.AddComponent<UIManager>();
                DontDestroyOnLoad(go);
            }
            return _instance;
        }
    }

    /// <summary>
    /// 打开UI窗口
    /// </summary>
    public GameObject OpenWindow(string name)
    {
        string assetPath = System.IO.Path.Combine(uiPrefabPath, name);
        GameObject prefab = Resources.Load<GameObject>(assetPath);
        
        if (prefab != null)
        {
            GameObject canvas = GameObject.Find("Canvas");
            if (canvas == null)
            {
                Debug.LogError("场景中没有找到Canvas对象");
                return null;
            }

            GameObject window = Instantiate(prefab, canvas.transform);
            window.name = name;
            return window;
        }
        else
        {
            Debug.LogWarning($"未能加载预制体 {assetPath}");
            return null;
        }
    }

    /// <summary>
    /// 关闭UI窗口
    /// </summary>
    public void CloseWindow(string name)
    {
        GameObject window = GameObject.Find(name);
        if (window != null)
        {
            Destroy(window);
        }
        else
        {
            Debug.LogWarning($"未找到UI窗口 {name}");
        }
    }

    /// <summary>
    /// 检查UI窗口是否已打开
    /// </summary>
    public bool IsWindowOpen(string name)
    {
        return GameObject.Find(name) != null;
    }
}
