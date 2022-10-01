using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// 点击播放特效
public class ClickEffect : MonoBehaviour
{
    public GameObject effectGo;
    private Vector3 point;

    void Start()
    {
        //effectGo = Resources.Load<GameObject>("Prefabs/EffectClick");
    }
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("ok");
            point = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0.0f);//获得鼠标点击点
            point = Camera.main.ScreenToWorldPoint(point);//从屏幕空间转换到世界空间
            GameObject go = Instantiate(effectGo);//生成特效
            go.transform.position = point;
            Destroy(go, 3.0f);
        }
    }
}