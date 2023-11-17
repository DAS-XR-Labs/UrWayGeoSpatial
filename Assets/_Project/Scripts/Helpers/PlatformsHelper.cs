namespace DAS.Urway
{
    public static class PlatformsHelper
    {
//         public static bool IsWebGL =>
// #if UNITY_WEBGL
//       true;
// #else
//             false;
// #endif

        public static bool IsEditor =>
#if UNITY_EDITOR
            true;
#else
            false;
#endif

        public static bool IsPC =>
#if UNITY_STANDALONE
            true;
#else
            false;
#endif

        public static bool IsIOS =>
#if UNITY_IOS
            true;
#else
            false;
#endif

        public static bool IsAndroid =>
#if UNITY_ANDROID
             true;
#else
            false;
#endif
    }
}