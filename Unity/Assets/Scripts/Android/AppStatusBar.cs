using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_ANDROID
public class AppStatusBar : MonoBehaviour
{

    [Tooltip("״̬���Ƿ���ʾ״̬��֪ͨ")]
    public bool statusBar;
    [Tooltip("״̬����ʽ")]
    public AndroidStatusBar.States states = AndroidStatusBar.States.TranslucentOverContent;
    //  public Button[] button;
    // Use this for initialization
    void Awake()
    {
        statusBar = true;
        if (Application.platform == RuntimePlatform.Android)
        {
            AndroidStatusBar.dimmed = !statusBar;
            //��AndroidStatusBar.dimmed=falseʱ��״̬����ʾ����״̬��֪ͨͼ��
            //��AndroidStatusBar.dimmed=trueʱ��״̬����������ʱ�䣬����ʾ����״̬��֪ͨ

            //��ʾ״̬����ռ����Ļ���Ϸ���һ��������
            //AndroidStatusBar.statusBarState = AndroidStatusBar.States.Visible;

            //������ʾ״̬������ռ����Ļ����
            //AndroidStatusBar.statusBarState = AndroidStatusBar.States.VisibleOverContent;

            //͸��������ʾ״̬������ռ����Ļ����
            AndroidStatusBar.statusBarState = AndroidStatusBar.States.TranslucentOverContent;

            //����״̬��
            //AndroidStatusBar.statusBarState = AndroidStatusBar.States.Hidden;

            //AndroidStatusBar.statusBarState = states;
        }
    }
    private void Start()
    {
        /*
        for(int i = 0;i<4;i++)
        {
            if(i==0)
            {
                button[i].onClick.AddListener(
                    delegate
                    {
                        AndroidStatusBar.statusBarState = AndroidStatusBar.States.Visible;
                        //AndroidStatusBar.statusBarState = states;
                    });
            }
            if (i == 1)
            {
                button[i].onClick.AddListener(
                    delegate
                    {
                        AndroidStatusBar.statusBarState = AndroidStatusBar.States.VisibleOverContent;
                        //AndroidStatusBar.statusBarState = states;
                    });
            }
            if (i == 2)
            {
                button[i].onClick.AddListener(
                    delegate
                    {
                        AndroidStatusBar.statusBarState = AndroidStatusBar.States.TranslucentOverContent;
                        //AndroidStatusBar.statusBarState = states;
                    });
            }
            if (i == 3)
            {
                button[i].onClick.AddListener(
                    delegate
                    {
                        AndroidStatusBar.statusBarState = AndroidStatusBar.States.Hidden;
                        //AndroidStatusBar.statusBarState = states;
                    });
            }



        }

    */
        AndroidStatusBar.statusBarState = AndroidStatusBar.States.TranslucentOverContent;
        //AndroidStatusBar.statusBarState = states;


    }

}

#endif