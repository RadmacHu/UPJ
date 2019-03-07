using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum UIWindowKey
{
    UI_TEST_MAIN,
}

public class UIPrefabsDefine
{
    private static Dictionary<UIWindowKey, string> m_UIPrefabsPath = new Dictionary<UIWindowKey, string>()
    {
        { UIWindowKey.UI_TEST_MAIN, "Prefabs/UIPrefabs/UITestMain"},
    };

    public static string GetUIPrefabsPath(UIWindowKey uiWindowKey)
    {
        if (m_UIPrefabsPath.ContainsKey(uiWindowKey))
        {
            return m_UIPrefabsPath[uiWindowKey];
        }

        return string.Empty;
    }
}
