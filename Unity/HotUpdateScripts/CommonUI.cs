using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CommonUI : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LoadMainScene()
    {
        AsyncOperation op = SceneManager.LoadSceneAsync("Game/Scenes/Main");
        op.allowSceneActivation = true;
    }
}
