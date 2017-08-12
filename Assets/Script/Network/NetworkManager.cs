using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fleck;
using WebSocket4Net;
using System;

namespace LCore
{
    [Serializable]
    public struct SocketMessageStruct
    {
        public int eventType;
        public string data;
    }
    public enum ESocketEventType
    {
        SenssorData,
    }
    public delegate void SocketEventHandler(string data);
    public class NetworkManager
    {
        private static NetworkManager mInstance;
        public static NetworkManager Instance
        {
            get
            {
                if (mInstance == null)
                {
                    mInstance = new NetworkManager();
                }
                return mInstance;
            }
        }

        private int serverPort = 3000;
        private WebSocketServer mLocalServer;

        

        private List<IWebSocketConnection> mWebSocketConnectionList;

        private WebSocket mClientSocket;

        private Dictionary<int, SocketEventHandler> mSocketEventDic;

        private void InvokeSocketEvent(int eventType,string value)
        {
            if(mSocketEventDic != null && mSocketEventDic.ContainsKey(eventType))
            {
                SocketEventHandler callback = mSocketEventDic[eventType];
                if(callback != null)
                {
                    callback.Invoke(value);
                }
            }
        }

        public void RegistSocketEventHandler(ESocketEventType eventType, SocketEventHandler callback)
        {
            if(mSocketEventDic != null)
            {
                if (mSocketEventDic.ContainsKey((int)eventType))
                {
                    mSocketEventDic[(int)eventType] += callback;
                }
                else
                {
                    mSocketEventDic.Add((int)eventType, callback);
                }
            }
        }

        public void UnRegistEventHandler(ESocketEventType eventType, SocketEventHandler callback)
        {
            if(mSocketEventDic != null && mSocketEventDic.ContainsKey((int)eventType))
            {
                mSocketEventDic[(int)eventType] -= callback;
                if(mSocketEventDic[(int)eventType] == null)
                {
                    mSocketEventDic.Remove((int)eventType);
                }
            }
        }

        public void SendSocketMessage(ESocketEventType eventType, string data)
        {
            SocketMessageStruct mes = new SocketMessageStruct();
            mes.eventType = (int)eventType;
            mes.data = data;
            try
            {
                string json = JsonUtility.ToJson(mes);
#if UNITY_EDITOR || UNITY_STANDALONE
                if (mWebSocketConnectionList != null)
                {
                    for (int i = 0; i < mWebSocketConnectionList.Count; i++)
                    {
                        if (mWebSocketConnectionList[i] != null && mWebSocketConnectionList[i].IsAvailable)
                        {
                            mWebSocketConnectionList[i].Send(json);
                        }
                    }
                }
#elif UNITY_ANDROID || UNITY_IPHONE
                if(mClientSocket != null)
                {
                    mClientSocket.Send(json);
                }
#endif
            }
            catch (Exception e)
            {
                Debug.Log("发送消息解析出错     error:" + e.Message);
            }
        }

        private void ConnectToServer(string path,int port)
        {
            mClientSocket = new WebSocket("ws://" + path + ":" + port.ToString());
            mClientSocket.Opened += (sender,e) =>
            {
                Debug.Log("Connect to " + path);
            };
            mClientSocket.Closed += (sender, e) =>
            {
                Debug.Log("Disconnect from " + path);
            };
            mClientSocket.MessageReceived += (sender, e) =>
            {
                //接受消息
                if (e != null)
                {
                    try
                    {
                        SocketMessageStruct message = JsonUtility.FromJson<SocketMessageStruct>(e.Message);
                        InvokeSocketEvent(message.eventType, message.data);
                    }
                    catch(Exception err)
                    {
                        Debug.Log("Socket消息解析出错     error:" + err.Message);
                    }
                }
            };
            mClientSocket.Open();
        }

        private void CreateLocalServer()
        {
            if (mLocalServer != null)
            {
                mLocalServer.RestartAfterListenError = true;
                mLocalServer.Start(socket =>
                {
                    socket.OnOpen = () => 
                    {

                        if (mWebSocketConnectionList != null)
                        {
                            mWebSocketConnectionList.Add(socket);
                        }
                        Debug.Log(socket.ConnectionInfo.Id + "Open");
                    } ;
                    socket.OnClose = () => 
                    {

                        if (mWebSocketConnectionList != null)
                        {
                            mWebSocketConnectionList.Remove(socket);
                        }
                        Debug.Log(socket.ConnectionInfo.Id + "Close");
                    };
                    socket.OnMessage = Message => 
                    {
                        //接受消息
                            try
                            {
                                SocketMessageStruct message = JsonUtility.FromJson<SocketMessageStruct>(Message);
                                InvokeSocketEvent(message.eventType, message.data);
                            }
                            catch (Exception err)
                            {
                                Debug.Log("Socket消息解析出错     error:" + err.Message);
                            }
                        
                    };
                });
                Debug.Log("WebSocketServer On Listen");
            }
        }

        public void Initial()
        {
            mLocalServer = new WebSocketServer("ws://0.0.0.0:" + serverPort.ToString());
            mWebSocketConnectionList = new List<IWebSocketConnection>();
            mSocketEventDic = new Dictionary<int, SocketEventHandler>();
            OnAwake();
        }

        public void OnAwake()
        {
#if UNITY_EDITOR || UNITY_STANDALONE
            CreateLocalServer();
#elif UNITY_ANDROID || UNITY_IPHONE
            ConnectToServer("192.168.199.113",3000);
#endif
        }

        public void OnClear()
        {
            if(mSocketEventDic != null)
            {
                mSocketEventDic.Clear();
            }
            if (mWebSocketConnectionList != null)
            {
                for(int i=0;i<mWebSocketConnectionList.Count;i++)
                {
                    if(mWebSocketConnectionList[i] != null)
                        mWebSocketConnectionList[i].Close();
                }
                mWebSocketConnectionList.Clear();
            }
            if(mLocalServer != null)
            {
                mLocalServer.Dispose();
                Debug.Log("Stop listen");
            }
            if(mClientSocket != null)
            {
                mClientSocket.Close();
                mClientSocket = null;
            }
        }

        public void OnShutDown()
        {
            OnClear();
            mLocalServer = null;
            mWebSocketConnectionList = null;
            mSocketEventDic = null;
        }
    }
}
