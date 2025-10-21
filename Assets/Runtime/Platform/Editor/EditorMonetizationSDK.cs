using System;
using System.Collections;
using System.Net;
using System.Threading.Tasks;
using Runtime.Assets.Scripts;
using Runtime.Core;
using Runtime.Monetization;
using UnityEngine;
using UnityEngine.Networking;
using Object = UnityEngine.Object;

namespace Runtime.Platform.Editor
{
    sealed class EditorMonetizationSDK  : IMonetizationSDK
    {
        private bool _isInitialized;
        private AdsConfig _config;
        private bool _adsReady;
        public void Initialize(string appId, string userId, Action onCompleted, Action onFailed)
        {
            CoroutineRunner.Run(Initialize_coroutine(appId, userId, onCompleted, onFailed));
        }

        public Task<Result> InitializeAsync(string appId, string userId)
        {
            return InitializeInternal(appId, userId);
        }

        public bool IsInitialized()
        {
            return _isInitialized;
        }
        
        public void LoadAds(Action onCompleted, Action onFailed)
        {
            CoroutineRunner.Run(LoadAds_coroutine(onCompleted, onFailed));
        }

        public Task<Result> LoadAdsAsync()
        {
            return LoadAdsInternal();
        }

        public bool IsAdsReady()
        {
            return _adsReady;
        }

        public void ShowRewardedAds(Action onCompleted, Action onFailed, Action onRewarded)
        {
            throw new NotImplementedException();
        }
        
        IEnumerator Initialize_coroutine(string appId, string userId, Action onCompleted, Action onFailed)
        {
            _config = Resources.Load<AdsConfig>("AdsConfig");

            if (_config == null)
            {
                Debug.LogError("AdsConfig not found.");
                onFailed?.Invoke();
                yield break;           
            }

            var request = new UnityWebRequest(_config.initializationUrl, WebRequestMethods.Http.Post);
            request.SetRequestHeader("Content-Type", "application/json");
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

            var player = Object.Instantiate(_config.playerPrefab);
            player.Prepare(_config.adsUrl, onCompleted);
        }
        
        async Task<Result> LoadAdsInternal()
        {
            if (string.IsNullOrEmpty(_config.adsUrl))
            {
                Debug.LogError("Ads url is empty.");
                return Result.FailedResult("Ads url is empty.");
            }
        
            var player = Object.Instantiate(_config.playerPrefab);
            var isReady = false;
            player.Prepare(_config.adsUrl, () => isReady = true);

            while (!isReady)
            {
                await Task.Delay(10);
            }
            return Result.SuccessResult;
        }
    }
}