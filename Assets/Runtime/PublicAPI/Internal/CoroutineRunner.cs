using System.Collections;
using UnityEngine;

namespace Runtime.PublicAPI.Internal
{
    class CoroutineRunner : MonoBehaviour
    {
        private static CoroutineRunner _instance;

        private static CoroutineRunner Instance
        {
            get
            {
                if (_instance != null) 
                    return _instance;
                
                var runner = new GameObject("CoroutineRunner");
                DontDestroyOnLoad(runner);
                _instance = runner.AddComponent<CoroutineRunner>();
                return _instance;
            }
        }

        internal static Coroutine Run(IEnumerator routine)
        {
            return Instance.StartCoroutine(routine);
        }
    }
}