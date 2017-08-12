using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindowInfo {

    public string name;

    public string className;

    public string path;

    public int layer;
}

public class WindowInfoTable
{
    private static WindowInfoTable mInstance;
    public static WindowInfoTable Instance
    {
        get
        {
            if(mInstance == null)
            {
                mInstance = new WindowInfoTable();
            }
            return mInstance;
        }
    }

    private Dictionary<string, WindowInfo> mWindowInfos;

    public void Initial()
    {
        mWindowInfos = new Dictionary<string, WindowInfo>();
        WindowInfo startWindow = new WindowInfo();
        startWindow.name = "StartWindow";
        startWindow.className = "StartWindow";
        startWindow.layer = 0;
        startWindow.path = "Prefabs/UI/StartWindow";
        mWindowInfos.Add(startWindow.name, startWindow);
    }

    public void Clear()
    {
        mWindowInfos.Clear();
    }

    public void ShutDown()
    {
        Clear();
        mWindowInfos = null;
    }

    public WindowInfo GetWindowInfo(string windowName)
    {
        if(mWindowInfos.ContainsKey(windowName))
        {
            return mWindowInfos[windowName];
        }
        else
        {
            return null;
        }
    }
}
