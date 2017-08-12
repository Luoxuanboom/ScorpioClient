using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LClient
{
    public class BaseState
    {
        protected string mName;
        public string Name
        {
            get { return mName; }
        }

        protected string mPreviousStateName;
        public string PreviousStateName
        {
            get { return mPreviousStateName; }
        }

        protected string mNextStateName;
        public string NextStateName
        {
            get { return mNextStateName; }
        }

        protected bool mIsHang;
        public bool IsHang
        {
            get { return mIsHang; }
        }

        public virtual void OnEnter(string previousStateName)
        {
            mPreviousStateName = previousStateName;
            mIsHang = false;
        }

        public virtual void OnResume(string previousStateName)
        {
            mPreviousStateName = previousStateName;
            mIsHang = false;
        }

        public virtual void OnHang(string nextStateName)
        {
            mNextStateName = nextStateName;
            mIsHang = true;
        }

        public virtual void OnExit(string nextStateName)
        {
            mNextStateName = nextStateName;
            mIsHang = false;
        }

        public virtual void OnReset()
        {

        }

        public virtual void OnUpdate(float deltaTime)
        {

        }

        public virtual void OnClear()
        {

        }

        public virtual void OnShutDown()
        {
            OnClear();
        }
    }
}
