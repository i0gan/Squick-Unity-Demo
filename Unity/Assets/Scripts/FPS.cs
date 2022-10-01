using UnityEngine;
 
public class FPS : MonoBehaviour
{
    private Rect labelRect = new Rect(30, 30, 100, 30);
    private float _Interval = 0.5f;
    private int _FrameCount = 0;
    private float _TimeCount = 0;
    private float _FrameRate = 0;
    public int targetFrameRate = 120;
    void Start()
    {
        Application.targetFrameRate = targetFrameRate;
    }
    void Update()
    {
        _FrameCount++;
        _TimeCount += Time.unscaledDeltaTime;
        if (_TimeCount >= _Interval)
        {
            _FrameRate = _FrameCount / _TimeCount;
            _FrameCount = 0;
            _TimeCount -= _Interval;
        }
    }
 
    void OnGUI()
    {
        GUI.Label(labelRect, string.Format("FPS: {0:F1}", _FrameRate));
    }
}