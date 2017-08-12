using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LClient;

namespace LMiniGame
{
    public struct MiniGameOverInfo
    {
        public bool IsWin;
    }
    public delegate void MiniGameOverHandler(object sender,MiniGameOverInfo info);
    public class MiniGameManager
    {
        protected static MiniGameManager mInstance;
        public static MiniGameManager Instance
        {
            get
            {
                if(mInstance == null)
                {
                    mInstance = new MiniGameManager();
                }
                return mInstance;
            }
        }

        private MiniGameOverHandler mOnGameOver;
        public MiniGameOverHandler OnGameOver
        {
            get { return mOnGameOver; }
        }

        private bool mIsGaming;
        public bool IsGaming
        {
            get { return mIsGaming; }
        }

        public virtual void OnCreate()
        {
            mIsGaming = false;
        }

        public virtual void OnAwake()
        {
            mIsGaming = true;
        }

        public void OnUpdate(float deltaTime)
        {
            if (mIsGaming)
            {
                UpdateKnowledge(deltaTime);
                UpdateStratedgy(deltaTime);
                UpdateRequest(deltaTime);
                UpdateBehavior(deltaTime);
                MiniGameOverInfo info;
                if (CheckGameOver(out info))
                {
                    if (mOnGameOver != null)
                    {
                        mOnGameOver.Invoke(this, info);
                    }
                }
            }
        }

        public virtual void OnClear()
        {
            mIsGaming = false;
        }

        public virtual void OnShutDown()
        {
            OnClear();
            mOnGameOver = null;
        }

        protected virtual void UpdateKnowledge(float deltaTime)
        {

        }

        protected virtual void UpdateStratedgy(float deltaTime)
        {

        }

        protected virtual void UpdateRequest(float deltaTime)
        {

        }

        protected virtual void UpdateBehavior(float deltaTime)
        {

        }

        protected virtual bool CheckGameOver(out MiniGameOverInfo info)
        {
            info = new MiniGameOverInfo();
            info.IsWin = false;
            return true;
        }
    }
}
