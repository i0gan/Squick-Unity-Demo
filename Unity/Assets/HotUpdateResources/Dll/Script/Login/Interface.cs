using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace TFlash.Login
{
    public class Interface : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        // 用户回调热更工程里的代码
        public static bool LoginCallbackCall = false;    // ture为调用
        public static string LoginCallbackParm_1 = "";   // 函数参数1
        public void LoginCallback(string res)
        {
            Debug.Log("登录反馈： LoginCallback: " + res);
            LoginCallbackParm_1 = res;
            LoginCallbackCall = true;
        }


    }

}