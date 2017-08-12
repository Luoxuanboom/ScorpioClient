using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public delegate void UIEventHandler();
namespace LClient
{
    public class EventListener : MonoBehaviour, IPointerClickHandler
    {
        public static EventListener GetEventListener(GameObject gobj)
        {
            if(null == gobj)
            {
                return null;
            }
            EventListener listener = null;
            listener = gobj.GetComponent<EventListener>();
            if(listener == null)
            {
                listener = gobj.AddComponent<EventListener>();
            }
            return listener;
        }
        public UIEventHandler OnClick;
        public void OnPointerClick(PointerEventData eventData)
        {
            if(OnClick!=null)
            {
                OnClick.Invoke();
            }
        }
    }
}
