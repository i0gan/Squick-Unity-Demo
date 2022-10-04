


using UnityEngine;

namespace HotScripts
{

    public static class Program
    {
        public static void SetupGame()
        {

            Debug.Log("Start Hot Script dll");
            //Debug.Log("<color=cyan>[SetupGame] 这个周期在ClassBind初始化之前，可以对游戏数据进行一些初始化</color>");
            //防止Task内的报错找不到堆栈，不建议删下面的代码
            System.Threading.Tasks.TaskScheduler.UnobservedTaskException += (sender, e) =>
            {
                foreach (var innerEx in e.Exception.InnerExceptions)
                {
                    Debug.LogError($"{innerEx.Message}\n" +
                    $"ILRuntime StackTrace: {innerEx.Data["StackTrace"]}\n\n" +
                    $"Full Stacktrace: {innerEx.StackTrace}");
                }
            };

            
        }

        public static void RunGame()
        {
            Debug.Log("Run in Hot Script dll");
            //Debug.Log("<color=yellow>[RunGame] 这个周期在ClassBind初始化后，可以激活游戏相关逻辑</color>");
            //如果生成热更解决方案跳过，参考https://xgamedev.uoyou.com/guide-v0-6.html#hash-516317491的方法一，把生成的平台改成Any CPU（默认是小写的，windows下无法生成）

            
        }
        
    }



}
