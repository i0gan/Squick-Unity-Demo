using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Uquick.Editor
{
    /// <summary>
    /// 跳转场景
    /// </summary>
    internal static class ChangeScene
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
        private static async void DoChange()
        {
            var prefix = $"Uquick.Editor.Setting.{Application.productName}.{SetData.GetPrefix()}.";
            var jump = PlayerPrefs.GetString($"{prefix}JumpStartUpScene", "1") == "1";
            if (!jump) return;
            var scene = SceneManager.GetActiveScene(); // 获取当前场景
            if (scene.path != Setting.StartUpScenePath)
            {
                //PlayerPrefs.DeleteAll();
                //Setting.StartUpScenePath = 
                Debug.Log("启动场景：" + Setting.StartUpScenePath);
                string name = Setting.StartUpScenePath
                    .Substring(Setting.StartUpScenePath.LastIndexOf('/') + 1)
                    .Replace(".unity", "");

                SceneManager.LoadScene(Setting.StartUpScenePath); // 设置初始化
                while (SceneManager.GetActiveScene().name != name)
                {
                    if (!Application.isPlaying) return;
                    await Task.Delay(10);
                }
                DynamicGI.UpdateEnvironment();
            }
            
            var key = Object.FindObjectOfType<InitUquick>().key;
            var k = PlayerPrefs.GetString($"{prefix}.EncryptPassword", "");
            if (string.IsNullOrEmpty(k))
            {
                PlayerPrefs.SetString($"{prefix}.EncryptPassword", key);
            }
            else
            {
                if (key != k)
                {
                    var res = EditorUtility.DisplayDialog(
                        string.Format(Setting.GetString(SettingString.MismatchDLLKey), k, key),
                        Setting.GetString(SettingString.Ok), Setting.GetString(SettingString.Ignore));
                    if (res)
                    {
                        Object.FindObjectOfType<InitUquick>().key = k;
                    }
                }
            }
        }
    }
}