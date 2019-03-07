using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : SingletonTemplate<UIManager>
{
    private Dictionary<UIWindowKey, UGUIBase> m_UGUIMap = new Dictionary<UIWindowKey, UGUIBase>();

    public void RegistUGUIWindows()
    {
        
    }

    private void RegistUGUIWindow(UIWindowKey uGuiWindowKey)
    {
        if (m_UGUIMap.ContainsKey(uGuiWindowKey))
        {

        }
    }
}
