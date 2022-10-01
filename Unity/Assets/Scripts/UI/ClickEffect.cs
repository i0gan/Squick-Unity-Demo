using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// ���������Ч
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
            point = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0.0f);//����������
            point = Camera.main.ScreenToWorldPoint(point);//����Ļ�ռ�ת��������ռ�
            GameObject go = Instantiate(effectGo);//������Ч
            go.transform.position = point;
            Destroy(go, 3.0f);
        }
    }
}