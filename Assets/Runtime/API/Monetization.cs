using System;
using System.Threading.Tasks;
using Runtime.Core;
using Runtime.Monetization;
using UnityEngine;

namespace Runtime.API
{
    public class Monetization
    {
        private static IMonetizationSDK _sdk;
        
        public static void Initialize(string appId, string userId, Action onCompleted, Action onFailed)
        {
            _sdk = SDKProvider.Create();
            Debug.Log("Monetization Init");
            _sdk.Initialize(appId, userId, onCompleted, onFailed);
        }
        
        public static async Task<Result> InitializeAsync(string appId, string userId)
        {
            Debug.Log("Monetization Init");
            return await _sdk.InitializeAsync(appId, userId);
        }

        public static void LoadAds(Action onCompleted, Action onFailed)
        {
            if (_sdk == null)
            {
                Debug.LogError("SDK not initialized.");
                return;
            }
            
            _sdk.LoadAds(onCompleted, onFailed);
        }

        public static Task<Result> LoadAdsAsync()
        {
            if (_sdk == null)
            {
                Debug.LogError("SDK is not initialized.");
                return Task.FromResult(Result.FailedResult("SDK is not initialized"));
            }
            
            return _sdk.LoadAdsAsync();
        }
        
        public static bool IsAdsReady()
        {
            if (_sdk == null)
            {
                Debug.LogError("SDK not initialized.");
                return false;
            }
            
            return _sdk.IsAdsReady();
        }

        public static void ShowRewardedAds(Action onCompleted, Action onFailed, Action onRewarded)
        {
            if (_sdk == null)
            {
                Debug.LogError("SDK not initialized.");
                return;
            }

            if (!IsAdsReady())
            {
                Debug.LogError("Ads not ready.");
                return;           
            }

            _sdk.ShowRewardedAds(onCompleted, onFailed, onRewarded);
        }
    }
}