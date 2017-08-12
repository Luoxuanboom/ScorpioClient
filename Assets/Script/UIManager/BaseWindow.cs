using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LClient
{
    public class BaseWindow
    {
        private GameObject mInstance;
        public GameObject Instance
        {
            get { return mInstance; }
        }

        private bool mIsAwake;
        public bool IsAwake
        {
            get { return mIsAwake; }
        }

        protected virtual void OnCreate()
        {

        }

        protected virtual void OnAwake()
        {

        }

        protected virtual void OnUpdate(float deltaTime)
        {

        }

        protected virtual void OnClose()
        {

        }

        protected virtual void OnDel()
        {

        }

        public void Create(GameObject instance,bool isAwake = true)
        {
            if (instance != null)
            {
                mInstance = instance;
                mInstance.SetActive(false);
                mIsAwake = false;
                OnCreate();
                if(isAwake)
                {
                    Awake();
                }
            }
        }

        public void Awake()
        {
            if(mInstance != null && !mIsAwake)
            {
                mIsAwake = true;
                mInstance.SetActive(true);
                OnAwake();
            }
        }

        public void Close()
        {
            if(mInstance != null && mIsAwake)
            {
                mIsAwake = false;
                mInstance.SetActive(false);
                OnClose();
            }
        }

        public void Del()
        {
            if(mInstance != null)
            {
                if(mIsAwake)
                {
                    Close();
                }
                GameObject.Destroy(mInstance);
                OnDel();
            }
        }
    }
}
