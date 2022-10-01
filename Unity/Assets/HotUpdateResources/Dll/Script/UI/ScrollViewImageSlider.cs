using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
// ref: https://ke.qq.com/webcourse/index.html?r=1657093710902#cid=2380362&term_id=102484166&taid=8778341924753994&type=3072&source=PC_COURSE_DETAIL&vid=5285890802240604284


namespace TFlash.UI
{
    public class ScrollViewImageSlider : MonoBehaviour, IBeginDragHandler, IEndDragHandler
    {
        #region 字段
        public ScrollRect rect;
        public int pageCount;
        RectTransform content;
        public float[] pages;
        public float moveTime = 0.3f;
        private float timer = 0;
        private float startMovePos = 0.0f;
        public int currentPage = 0;
        public bool isMoveing = false;
        public bool isDraging = false;
        public bool isAutoScroll = true;
        public float autoScrollTime = 3.0f; // 自动滚动时间
        public float autoScrollTimer = 0.0f;
        private float dragTimer = 0.0f;
        private Vector2 clickLastPos;//上一个位置
        private Vector2 clickCurrentPos;//下一个位置
        #endregion
        // Start is called before the first frame update

        protected void Start()
        {
            rect = transform.GetComponent<ScrollRect>();
            content = transform.Find("Viewport/Content").GetComponent<RectTransform>();
            pageCount = content.childCount;
            pages = new float[pageCount];
            for (int i = 0; i < pageCount; i++)
            {
                pages[i] = i * (1.0f / (pageCount - 1));
            }
        }

        // Update is called once per frame
        protected void Update()
        {
            if (pageCount > 1)
            {
                MoveListener();
                AutoMoveListener();
            }

        }
        void AutoMoveListener()
        {
            if (isAutoScroll && isDraging == false)
            {
                autoScrollTimer += Time.deltaTime;
                if (autoScrollTimer >= autoScrollTime) // 触发
                {
                    currentPage++;
                    currentPage %= pageCount;
                    ScrollToPage(currentPage);
                    autoScrollTimer = 0;
                }
            }
        }
        void MoveListener()
        {
            if (isMoveing)
            {
                timer += Time.deltaTime * (1 / moveTime);
                rect.horizontalNormalizedPosition = Mathf.Lerp(startMovePos, pages[currentPage], timer);
                if (timer >= 1) // 移动结束
                {
                    isMoveing = false;
                }
            }
        }

        public void ScrollToPage(int page)
        {
            currentPage = page;
            timer = 0;
            startMovePos = rect.horizontalNormalizedPosition;
            isMoveing = true;
        }

        // 拖拽事件
        public void OnEndDrag(PointerEventData eventData)
        {

            //Debug.Log("OnEndDrag: " + eventData.position);
            
            clickCurrentPos = eventData.position;
            float dragTimerOffset = 0.3f;
            if (Time.time - dragTimer < dragTimerOffset)
            {
                //Debug.Log("时间满足");
                if (clickCurrentPos.x - clickLastPos.x < -50.0f )
                {
                    //Debug.Log("左");
                    if (currentPage < (pageCount - 1))
                    {
                        currentPage++;
                        ScrollToPage(currentPage);
                        isDraging = false;
                        return;
                    }
                }

                if ((clickCurrentPos.x - clickLastPos.x) > 50.0f )
                {
                    //Debug.Log("右");
                    if (currentPage > 0)
                    {
                        currentPage--;
                        ScrollToPage(currentPage);
                        isDraging = false;
                        return;
                    }
                }
            }

            // 计算出目前离最近的页面

            int minPage = 0;
            for (int i = 1; i < pages.Length; i++)
            {
                if (Mathf.Abs(pages[i] - rect.horizontalNormalizedPosition) < Mathf.Abs(pages[minPage] - rect.horizontalNormalizedPosition))
                {
                    minPage = i;
                }
            }

            ScrollToPage(minPage);
            isDraging = false;
            
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            isDraging = true;
            //Debug.Log("OnBeginDrag: " + eventData.position);
            clickLastPos = eventData.position;
            clickCurrentPos = eventData.position;
            dragTimer = Time.time;
            
        }
    }

}