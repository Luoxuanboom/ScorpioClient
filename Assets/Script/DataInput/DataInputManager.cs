using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;
using UnityEngine;
using System;
namespace LCore
{
    public enum EInputDataType
    {
        Gyroscope,
        Acceleration
    }
    [Serializable]
    public struct SenssorDataListStruct
    {
        public List<SenssorDataStruct> dataList;
    }
    [Serializable]
    public struct SenssorDataStruct
    {
        public int dataType;
        public string data;
    }
    public class DataInputManager
    {
        private static DataInputManager mInstance;
        public static DataInputManager Instance
        {
            get
            {
                if (mInstance == null)
                {
                    mInstance = new DataInputManager();
                }
                return mInstance;
            }
        }

        private ConcurrentDictionary<int, object> mInputDataDic;
        public ConcurrentDictionary<int,object> InputDataDic
        {
            get { return mInputDataDic; }
        }

        private Gyroscope mGyro;

        public void Initial()
        {
            mInputDataDic = new ConcurrentDictionary<int, object>();
#if UNITY_IPHONE || UNITY_ANDROID
            mGyro = Input.gyro;
#endif
            OnAwake();
        }

        public void OnAwake()
        {
#if UNITY_EDITOR || UNITY_STANDALONE
            NetworkManager.Instance.RegistSocketEventHandler(ESocketEventType.SenssorData, value =>
            {
                try
                {
                    SenssorDataListStruct dataList = JsonUtility.FromJson<SenssorDataListStruct>(value);
                    for (int i = 0; i < dataList.dataList.Count; i++)
                    {
                        SenssorDataStruct data = dataList.dataList[i];
                        if (data.dataType == (int)EInputDataType.Gyroscope)
                        {
                            Quaternion qua = JsonUtility.FromJson<Quaternion>(data.data);
                            AddInputData(EInputDataType.Gyroscope, qua);
                        }
                        else if(data.dataType == (int)EInputDataType.Acceleration)
                        {
                            Vector3 acce = JsonUtility.FromJson<Vector3>(data.data);
                            AddInputData(EInputDataType.Acceleration, acce);
                        }
                    }
                }
                catch (Exception e)
                {
                    Debug.Log("解析传感器数据出错   error: " + e.Message);
                }
            }
            );

#elif UNITY_IPHONE || UNITY_ANDROID
             if (mGyro != null)
            {
                mGyro.enabled = true;
            }
#endif
        }

        public void OnUpdate(float deltaTime)
        {

#if UNITY_IPHONE || UNITY_ANDROID && !UNITY_EDITOR
            AddInputData(EInputDataType.Acceleration, Input.acceleration);
            AddInputData(EInputDataType.Gyroscope, mGyro.attitude);
            List<SenssorDataStruct> dataList = new List<SenssorDataStruct>();
            foreach(KeyValuePair<int,object> pair in mInputDataDic)
            {
                SenssorDataStruct data = new SenssorDataStruct();
                data.dataType = pair.Key;
                data.data = JsonUtility.ToJson(pair.Value);
                dataList.Add(data);
            }
            SenssorDataListStruct list = new SenssorDataListStruct();
            list.dataList = dataList;
            NetworkManager.Instance.SendSocketMessage(ESocketEventType.SenssorData, JsonUtility.ToJson(list));
            list.dataList.Clear();
            list.dataList = null;
#endif
        }

        public void OnClear()
        {
            if (mInputDataDic != null)
                mInputDataDic.Clear();
            if(mGyro != null)
            {
                mGyro.enabled = false;
            }
        }

        public void OnShutDown()
        {
            OnClear();
            mInputDataDic = null;
            mGyro = null;
        }
        
        private void AddInputData(EInputDataType type,object value)
        {
            mInputDataDic[(int)type] = value;
        }

        private void RemoveInputData(EInputDataType type)
        {
            if(mInputDataDic.ContainsKey((int)type))
            {
                object value;
                mInputDataDic.TryRemove((int)type,out value);
            }
        }
    }
}
