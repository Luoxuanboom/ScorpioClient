using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LClient {
    public delegate void WindowLoadFinishHandle(BaseWindow window);
    public class WindowManager {

        class WindowLoadInfo
        {
            public ResourceRequest mResourceRequest;
            public WindowInfo info;
            public bool isBackground;
            public WindowLoadFinishHandle callback;
        }

        private static WindowManager mInstance;
        public static WindowManager Instance
        {
            get
            {
                if (mInstance == null)
                    mInstance = new WindowManager();
                return mInstance;
            }
        }

        private List<WindowLoadInfo> mWindowLoadInfoList;

        private Dictionary<int, Transform> mCanvas;

        private Dictionary<string, BaseWindow> mBackgroundWindows;

        private Dictionary<string, BaseWindow> mFrontWindows;

        private Transform mUIRoot;

        public void Initial(Transform uiRoot)
        {
            mCanvas = new Dictionary<int, Transform>();
            mWindowLoadInfoList = new List<WindowLoadInfo>();
            mBackgroundWindows = new Dictionary<string, BaseWindow>();
            mFrontWindows = new Dictionary<string, BaseWindow>();
            mUIRoot = uiRoot;
            WindowInfoTable.Instance.Initial();
            for(int i=0;i<mUIRoot.childCount;i++)
            {
                Transform child = mUIRoot.GetChild(i);
                string name = child.name;
                if (name.Substring(0, 5) == "Layer")
                {
                    int layer = -1;
                    if(int.TryParse(name.Substring(5,name.Length - 5),out layer))
                    {
                        mCanvas.Add(layer, child);
                    }
                }
            }
        }

        public void Start()
        {

        }

        public void Clear()
        {
            mCanvas.Clear();
            mWindowLoadInfoList.Clear();
            foreach(KeyValuePair<string,BaseWindow> pair in mBackgroundWindows)
            {
                pair.Value.Del();
            }
            foreach (KeyValuePair<string, BaseWindow> pair in mFrontWindows)
            {
                pair.Value.Del();
            }
            mBackgroundWindows.Clear();
            mFrontWindows.Clear();
        }

        public void ShutDown()
        {
            Clear();
            mCanvas = null;
            mWindowLoadInfoList = null;
            mBackgroundWindows = null;
            mFrontWindows = null;
        }

        public void Update(float deltaTime)
        {
            for (int i = mWindowLoadInfoList.Count - 1; i >= 0; i--)
            {
                WindowLoadInfo loadInfo = mWindowLoadInfoList[i];
                ResourceRequest request = loadInfo.mResourceRequest;
                if (request.isDone)
                {
                    GameObject prefab = request.asset as GameObject;
                    WindowInfo info = loadInfo.info;
                    System.Type windowType = System.Type.GetType("LClient." + info.className);
                    if (windowType != null) {
                        BaseWindow window = (BaseWindow)System.Activator.CreateInstance(windowType);
                        GameObject instance = GameObject.Instantiate(prefab);
                        if (mCanvas.ContainsKey(info.layer))
                        {
                            RectTransform rect = instance.GetComponent<RectTransform>();
                            rect.SetParent(mCanvas[info.layer],false);
                            if (!loadInfo.isBackground)
                            {
                                window.Create(instance, true);
                                mFrontWindows.Add(info.name, window);
                            }
                            else
                            {
                                window.Create(instance, false);
                                mBackgroundWindows.Add(info.name, window);
                            }
                            WindowLoadFinishHandle callback = loadInfo.callback;
                            if (callback != null)
                            {
                                callback.Invoke(window);
                            }
                        }
                    }
                    mWindowLoadInfoList.RemoveAt(i);
                }
            }
        }

        public void AddLayer(int layer)
        {
            if (mCanvas.ContainsKey(layer))
            {
                return;
            }
            Transform child = mUIRoot.Find("Layer" + layer.ToString());
            if (child != null)
            {
                mCanvas.Add(layer, child);
                return;
            }
            else
            {
                GameObject ga = new GameObject();
                ga.name = "Layer" + layer.ToString();
                Transform canvas = ga.transform;
                canvas.transform.parent = mUIRoot;
                mCanvas.Add(layer, canvas);
            }
        }

        public void RemoveLayer(int layer)
        {
            if (!mCanvas.ContainsKey(layer))
            {
                return;
            }
            else
            {
                Transform canvas = mCanvas[layer];
                GameObject.Destroy(canvas.gameObject);
                mCanvas.Remove(layer);
            }
        }

        public void LoadWindow(string windowName,bool isBackground = false,WindowLoadFinishHandle callback = null)
        {
            WindowInfo windowInfo = WindowInfoTable.Instance.GetWindowInfo(windowName);
            if(windowInfo!=null)
            {
                ResourceRequest request = Resources.LoadAsync(windowInfo.path);
                WindowLoadInfo info = new WindowLoadInfo();
                info.mResourceRequest = request;
                info.info = windowInfo;
                info.isBackground = isBackground;
                info.callback = callback;
                mWindowLoadInfoList.Add(info);
            }
        }
        
        public void ShowWindow(string windowName)
        {
            if(mBackgroundWindows.ContainsKey(windowName))
            {
                BaseWindow window = mBackgroundWindows[windowName];
                window.Awake();
                mBackgroundWindows.Remove(windowName);
                mFrontWindows.Add(windowName, window);
            }
        }

        public void HideWindow(string windowName)
        {
            if(mFrontWindows.ContainsKey(windowName))
            {
                BaseWindow window = mFrontWindows[windowName];
                window.Close();
                mFrontWindows.Remove(windowName);
                mBackgroundWindows.Add(windowName, window);
            }
        }

        public void DelWindow(string windowName)
        {
            if(mFrontWindows.ContainsKey(windowName))
            {
                mFrontWindows[windowName].Del();
                mFrontWindows.Remove(windowName);
                return;
            }
            if (mBackgroundWindows.ContainsKey(windowName))
            {
                mBackgroundWindows[windowName].Del();
                mBackgroundWindows.Remove(windowName);
                return;
            }

        }
    }
}
