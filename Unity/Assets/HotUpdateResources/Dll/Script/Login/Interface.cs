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

        // �û��ص��ȸ�������Ĵ���
        public static bool LoginCallbackCall = false;    // tureΪ����
        public static string LoginCallbackParm_1 = "";   // ��������1
        public void LoginCallback(string res)
        {
            Debug.Log("��¼������ LoginCallback: " + res);
            LoginCallbackParm_1 = res;
            LoginCallbackCall = true;
        }


    }

}