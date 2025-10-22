using UnityEngine;

namespace Runtime.PublicAPI.Internal
{
    [CreateAssetMenu(fileName = "AdsConfig", menuName = "Scriptable Objects/AdsConfig")]
    internal class AdsConfig : ScriptableObject
    {
        [Header("SDK url to get advertisement")]
        public string adsUrl;
        
        [Header("SDK url to initialize")]
        public string initializationUrl;
        
        public AdsPlayer playerPrefab;
    }
}
