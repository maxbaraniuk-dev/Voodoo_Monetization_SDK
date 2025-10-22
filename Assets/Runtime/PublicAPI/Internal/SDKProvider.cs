using Runtime.Internal.Platform.Editor;
using Runtime.PublicAPI.Internal;

namespace PublicAPI.Internal
{
    internal static class SDKProvider
    {
        public static IMonetizationSDK Create()
        {
#if UNITY_EDITOR
            return new EditorMonetizationSDK();
#elif UNITY_ANDROID
            return new AndroidMonetizationSDK();
#elif UNITY_IOS
            return new IosMonetizationSDK();
#endif            
        }
    }
}