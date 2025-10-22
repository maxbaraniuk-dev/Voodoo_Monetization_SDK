using System;
using System.Collections;
using System.Net;
using System.Threading.Tasks;
using Runtime.Core;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using Object = UnityEngine.Object;

namespace Runtime.Internal.Platform.Editor
{
    /// <summary>
    /// Editor implementation of the monetization SDK used while running inside the Unity Editor.
    /// Simulates network initialization and ad loading to enable testing of the API without a device build.
    /// </summary>
    sealed class EditorMonetizationSDK  : IMonetizationSDK
    {
        private const string ConfigPath = "Packages/com.voodoo.sdk/Runtime/Assets/AdsInternalConfig.asset";
        
        private bool _isInitialized;
        private AdsConfig _config;
        private bool _adsReady;
        private AdsPlayer _adsPlayer;
        /// <summary>
        /// Initializes the editor monetization SDK with the provided application id and user id.
        /// </summary>
        /// <param name="appId">Application identifier used for server calls.</param>
        /// <param name="userId">Unique identifier for the current user/session.</param>
        /// <param name="onCompleted">Invoked when initialization succeeds.</param>
        /// <param name="onFailed">Invoked when initialization fails.</param>
        public void Initialize(string appId, string userId, Action onCompleted, Action onFailed)
        {
            CoroutineRunner.Run(Initialize_coroutine(appId, userId, onCompleted, onFailed));
        }

        /// <summary>
        /// Asynchronously initializes the editor monetization SDK.
        /// </summary>
        /// <param name="appId">Application identifier used for server calls.</param>
        /// <param name="userId">Unique identifier for the current user/session.</param>
        /// <returns>A <see cref="Result"/> indicating success or failure.</returns>
        public Task<Result> InitializeAsync(string appId, string userId)
        {
            return InitializeInternal(appId, userId);
        }

        /// <summary>
        /// Indicates whether the editor monetization SDK has completed initialization.
        /// </summary>
        /// <returns>True if initialized; otherwise, false.</returns>
        public bool IsInitialized()
        {
            return _isInitialized;
        }
        
        /// <summary>
        /// Preloads ad content in the editor so it can be displayed without delay.
        /// </summary>
        /// <param name="onCompleted">Invoked when ads are successfully loaded and ready.</param>
        /// <param name="onFailed">Invoked if loading fails.</param>
        public void LoadAds(Action onCompleted, Action onFailed)
        {
            CoroutineRunner.Run(LoadAds_coroutine(onCompleted, onFailed));
        }

        /// <summary>
        /// Asynchronously preloads ad content in the editor.
        /// </summary>
        /// <returns>A <see cref="Result"/> that indicates whether ad loading succeeded.</returns>
        public Task<Result> LoadAdsAsync()
        {
            return LoadAdsInternal();
        }

        /// <summary>
        /// Returns whether ad content is currently prepared and ready to be shown in the editor.
        /// </summary>
        /// <returns>True if ad content is ready; otherwise, false.</returns>
        public bool IsAdsReady()
        {
            return _adsReady;
        }

        /// <summary>
        /// Displays a rewarded ad in the editor if ads are ready.
        /// </summary>
        /// <param name="onRewarded">Invoked when the ad finishes and the reward should be granted.</param>
        /// <param name="onFailed">Invoked if showing the ad fails.</param>
        /// <param name="onSkipped">Invoked if the ad is skipped and no reward should be granted.</param>
        public void ShowRewardedAds(Action onRewarded, Action onFailed, Action onSkipped)
        {
            if (!_adsReady)
            {
                Debug.LogError("Ads are not ready.");
                onFailed?.Invoke();
                return;
            }

            if (_adsPlayer == null)
            {
                Debug.LogError("Video player is null.");
                onFailed.Invoke();
                return;
            }
            
            _adsPlayer.Play(onRewarded, onSkipped);
        }

        /// <summary>
        /// Disposes editor monetization resources and clears loaded ad content.
        /// </summary>
        /// <remarks>
        /// After calling this method, the internal player and config references are released. You must re-initialize before further use.
        /// </remarks>
        public void Dispose()
        {
            Object.Destroy(_adsPlayer.gameObject);
            _adsPlayer = null;
            _config = null;
        }

        IEnumerator Initialize_coroutine(string appId, string userId, Action onCompleted, Action onFailed)
        {
            _config = AssetDatabase.LoadAssetAtPath<AdsConfig>(ConfigPath);
            if (_config == null)
            {
                Debug.LogError("AdsConfig not found.");
                onFailed?.Invoke();
                yield break;           
            }

            var request = new UnityWebRequest(_config.initializationUrl, WebRequestMethods.Http.Post);
            request.SetRequestHeader("Content-Type", "application/json");
            var data = new InitData
            {
                appId = appId,
                userId = userId
            };
            var json = JsonUtility.ToJson(data);
            request.uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(json));
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Failed to initialize ads.");
                onFailed?.Invoke();
                request.Dispose();
                yield break;           
            }
            
            request.Dispose();
            _isInitialized = true;
            onCompleted.Invoke();
        }

        async Task<Result> InitializeInternal(string appId, string userId)
        {
            _config = Resources.Load<AdsConfig>("AdsConfig");

            if (_config == null)
            {
                Debug.LogError("AdsConfig not found.");
                return Result.FailedResult("AdsConfig not found.");
            }

            var request = new UnityWebRequest(_config.initializationUrl, WebRequestMethods.Http.Post);
            request.SetRequestHeader("Content-Type", "application/json");
            
            var data = new InitData
            {
                appId = appId,
                userId = userId
            };
            var json = JsonUtility.ToJson(data);
            request.uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(json));
            
            await request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                request.Dispose();
                Debug.LogError("Failed to initialize ads.");
                return Result.FailedResult("Failed to initialize ads.");   
            }
            
            _isInitialized = true;
            request.Dispose();
            return Result.SuccessResult;
        }

        IEnumerator LoadAds_coroutine(Action onCompleted, Action onFailed)
        {
            if (string.IsNullOrEmpty(_config.adsUrl))
            {
                Debug.LogError("Ads url is empty.");
                onFailed?.Invoke();
                yield break;
            }

            var request = UnityWebRequest.Get(_config.adsUrl);
            request.SetRequestHeader("Content-Type", "application/json");
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Failed to initialize ads.");
                onFailed?.Invoke();
                request.Dispose();
                yield break;           
            }
            var adsData = JsonUtility.FromJson<AdResponseData>(request.downloadHandler.text);
            request.Dispose();
            _adsPlayer = Object.Instantiate(_config.playerPrefab);
            onCompleted += () => _adsReady = true;
            _adsPlayer.Prepare(adsData.url, onCompleted);
        }
        
        async Task<Result> LoadAdsInternal()
        {
            if (string.IsNullOrEmpty(_config.adsUrl))
            {
                Debug.LogError("Ads url is empty.");
                return Result.FailedResult("Ads url is empty.");
            }
        
            var request = UnityWebRequest.Get(_config.adsUrl);
            request.SetRequestHeader("Content-Type", "application/json");
            await request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Failed to load ads");
                request.Dispose();
                return Result.FailedResult("Failed to load ads");      
            }
            var adsData = JsonUtility.FromJson<AdResponseData>(request.downloadHandler.text);
            request.Dispose();
            
            _adsPlayer = Object.Instantiate(_config.playerPrefab);

            _adsPlayer.Prepare(adsData.url, () => _adsReady = true);

            while (!_adsReady)
                await Task.Delay(10);
            
            return Result.SuccessResult;
        }
    }
}