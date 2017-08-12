using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LCore;
using System;

public class testGyro : MonoBehaviour {

    Quaternion turnToStandard = Quaternion.AngleAxis(0, Vector3.up);
	// Use this for initialization
	void Start () {
        
    }
	
	// Update is called once per frame
	void Update () {
        if(DataInputManager.Instance.InputDataDic.ContainsKey((int)EInputDataType.Gyroscope))
        {
            Quaternion qua = (Quaternion)DataInputManager.Instance.InputDataDic[(int)EInputDataType.Gyroscope];
            
            transform.localRotation = turnToStandard * (new Quaternion(qua.x,qua.y,-qua.z,-qua.w));
        }
	}
    
    public Quaternion StartCheckPos(Vector3 currentPhoneUp,Vector3 standardUp)
    {
        return Quaternion.FromToRotation(currentPhoneUp, standardUp);
    }
}
