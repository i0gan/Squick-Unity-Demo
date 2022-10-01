using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TFlash.UI
{
    public class ScaleScrollViewImageSlider : ScrollViewImageSlider
    {
        // ����ҳ�Ķ���
        public GameObject[] items;
        public float currentScale = 1.0f;
        public float otherScale = 0.6f;
        public int lastPage;
        public int nextPage;

        // Start is called before the first frame update
        protected void Start()
        {
            base.Start();
            items = new GameObject[pageCount];
            for (int i = 0; i < pageCount; i++)
            {
                items[i] = transform.Find("Viewport/Content").GetChild(i).gameObject;
                if (i != 0)
                {
                    items[i].transform.localScale = Vector3.one * otherScale;
                }
            }
        }

        // Update is called once per frame
        protected void Update()
        {
            base.Update();
            ListenScale();
        }

        public void ListenScale()
        {
            if (!isDraging && !isMoveing) return;
            // �ҵ���һҳ
            // ���ҵ���ǰҳ�����һҳ
            for (int i = 0; i < items.Length; i++)
            {
                if (pages[i] <= rect.horizontalNormalizedPosition) // ��һҳ
                {
                    lastPage = i;
                    //break;
                }
            }

            for (int i = 0; i < items.Length; i++)
            {
                if (pages[i] > rect.horizontalNormalizedPosition) // ��һҳ
                {
                    nextPage = i;
                    break;
                }
            }
            if (lastPage == nextPage) return;
            //Debug.Log("set...ddd.�� " + currentPage);
            // ����ʱ��������С��ͨ��percent�������ƴ�С
            float percent = (rect.horizontalNormalizedPosition - pages[lastPage]) / (pages[nextPage] - pages[lastPage]);
            items[lastPage].transform.localScale = Vector3.Lerp(Vector3.one * currentScale, Vector3.one * otherScale, percent); // ��һҳ����С
            items[nextPage].transform.localScale = Vector3.Lerp(Vector3.one * currentScale, Vector3.one * otherScale, 1 - percent); // ������һҳ�����

            /*
            for(int i = 1; i < items.Length; i++)
            {
                // ���õ�ǰҳ
                if(i != lastPage && i != nextPage && currentPage != 0)
                {
                    Debug.Log("set....�� " + currentPage);
                    items[i].transform.localScale = Vector3.one * otherScale;
                }
            }*/
        }
    }

}