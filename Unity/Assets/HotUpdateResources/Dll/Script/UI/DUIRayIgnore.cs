using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Collections;

public class DUIRayIgnore : MonoBehaviour, ICanvasRaycastFilter

{
   // µã»÷ÊÂ¼þ´©Í¸UI
    public bool IsRaycastLocationValid(Vector2 screenPoint, Camera eventCamera)
    {
        return false;
    }

}