using UnityEditor;

namespace Uquick.Editor
{
    internal static class AllowUnsafe
    {
        public static bool hasAdded;
        
        public static void Update()
        {
            hasAdded = true;
            
            if (!PlayerSettings.allowUnsafeCode)
            {
                PlayerSettings.allowUnsafeCode = true;
            }
        }
    }
}