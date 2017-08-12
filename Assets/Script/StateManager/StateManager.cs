using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LClient {
    public class StateManager {

        protected static StateManager mInstance;

        public static StateManager Instance
        {
            get
            {
                if (mInstance == null)
                {
                    mInstance = new StateManager();
                }
                return mInstance;
            }
        }

        protected Dictionary<string, BaseState> mStateDic;

        protected BaseState mCurrentState;
        public BaseState CurrentState
        {
            get { return mCurrentState; }
        }

        public virtual void OnCreate()
        {
            mStateDic = new Dictionary<string, BaseState>();
        }

        public virtual void OnAwake()
        {

        } 

        public virtual void OnClear()
        {
            foreach(KeyValuePair<string,BaseState> pair in mStateDic)
            {
                pair.Value.OnClear();
            }
            mStateDic.Clear();
        }

        public virtual void OnShutDown()
        {
            foreach (KeyValuePair<string, BaseState> pair in mStateDic)
            {
                pair.Value.OnShutDown();
            }
            OnClear();
            mStateDic = null;
        }

        public virtual void OnUpdate(float deltaTime)
        {
            if(mCurrentState != null)
            {
                mCurrentState.OnUpdate(deltaTime);
            }
        }

        public void GoNextState(string stateName,bool isHang = false)
        {
            if(mStateDic.ContainsKey(stateName) && mCurrentState != null)
            {
                BaseState state = mStateDic[stateName];
                if(isHang)
                {
                    mCurrentState.OnHang(stateName);
                    
                }
                else
                {
                    mCurrentState.OnExit(stateName);
                }
                if (state.IsHang)
                {
                    state.OnResume(mCurrentState.Name);
                }
                else
                {
                    state.OnEnter(mCurrentState.Name);
                }
                mCurrentState = state;
            }
        }

        public void GoPreviousState(bool isHang = false)
        {
            if (mStateDic.ContainsKey(mCurrentState.PreviousStateName) && mCurrentState != null)
            {
                BaseState state = mStateDic[mCurrentState.PreviousStateName];
                if (isHang)
                {
                    mCurrentState.OnHang(state.Name);

                }
                else
                {
                    mCurrentState.OnExit(state.Name);
                }
                if (state.IsHang)
                {
                    state.OnResume(mCurrentState.Name);
                }
                else
                {
                    state.OnEnter(mCurrentState.Name);
                }
                mCurrentState = state;
            }
        }
    }
}
