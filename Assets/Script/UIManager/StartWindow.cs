using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace LClient
{
    public class StartWindow : BaseWindow
    {
        private void OnStartButtonClick()
        {

        }

        protected override void OnAwake()
        {
            base.OnAwake();
            Transform startButton = Instance.transform.Find("StartButton");
            if(startButton != null)
            {
                startButton.GetComponent<Button>().onClick.AddListener(OnStartButtonClick);
            }
        }
    }
}
