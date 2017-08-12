using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LClient;
using LCore;

public class GameStart : MonoBehaviour {

	// Use this for initialization
	void Start () {
        GameObject mUIroot = GameObject.Find("UIRoot");
        WindowManager.Instance.Initial(mUIroot.transform);
        WindowManager.Instance.LoadWindow("StartWindow",false);
        NetworkManager.Instance.Initial();
        DataInputManager.Instance.Initial();
    }
	
	// Update is called once per frame
	void Update () {
        WindowManager.Instance.Update(Time.deltaTime);
        DataInputManager.Instance.OnUpdate(Time.deltaTime);
	}

    private void OnApplicationQuit()
    {
        WindowManager.Instance.ShutDown();
        DataInputManager.Instance.OnShutDown();
        NetworkManager.Instance.OnShutDown();
    }
}
