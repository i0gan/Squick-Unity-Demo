using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Squick;

public abstract class NFUIDialog : MonoBehaviour 
{
    public DataList mUserData = null;

    // Use this for initialization
    public abstract void Init();
    //public void OnEnable();
    //public void OnDisable();
}