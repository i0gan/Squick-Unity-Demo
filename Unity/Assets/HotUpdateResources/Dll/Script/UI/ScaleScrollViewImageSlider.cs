using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TFlash.UI
{
    public class ScaleScrollViewImageSlider : ScrollViewImageSlider
    {
        // 所有页的对象
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
            // 找到上一页
            // 先找到当前页面的上一页
            for (int i = 0; i < items.Length; i++)
            {
                if (pages[i] <= rect.horizontalNormalizedPosition) // 上一页
                {
                    lastPage = i;
                    //break;
                }
            }

            for (int i = 0; i < items.Length; i++)
            {
                if (pages[i] > rect.horizontalNormalizedPosition) // 上一页
                {
                    nextPage = i;
                    break;
                }
            }
            if (lastPage == nextPage) return;
            //Debug.Log("set...ddd.： " + currentPage);
            // 滑动时，慢慢变小，通过percent变量控制大小
            float percent = (rect.horizontalNormalizedPosition - pages[lastPage]) / (pages[nextPage] - pages[lastPage]);
            items[lastPage].transform.localScale = Vector3.Lerp(Vector3.one * currentScale, Vector3.one * otherScale, percent); // 上一页，边小
            items[nextPage].transform.localScale = Vector3.Lerp(Vector3.one * currentScale, Vector3.one * otherScale, 1 - percent); // 设置下一页，变大

            /*
            for(int i = 1; i < items.Length; i++)
            {
                // 设置当前页
                if(i != lastPage && i != nextPage && currentPage != 0)
                {
                    Debug.Log("set....： " + currentPage);
                    items[i].transform.localScale = Vector3.one * otherScale;
                }
            }*/
        }
    }

}