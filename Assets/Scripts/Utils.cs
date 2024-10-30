using UnityEngine;

public static class GameObjectExtensions
{
    /// <summary>
    /// 获取或添加指定类型的组件。
    /// </summary>
    /// <typeparam name="T">组件类型</typeparam>
    /// <param name="gameObject">游戏对象</param>
    /// <returns>组件实例</returns>
    public static T GetOrAddComponent<T>(this GameObject gameObject) where T : Component
    {
        T component = gameObject.GetComponent<T>();
        if (component == null)
        {
            component = gameObject.AddComponent<T>();
        }
        return component;
    }
}

