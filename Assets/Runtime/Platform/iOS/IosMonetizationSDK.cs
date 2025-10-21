using System;
using System.Threading.Tasks;
using Runtime.Core;
using Runtime.Monetization;

namespace Runtime.Platform.iOS
{
    sealed class IosMonetizationSDK : IMonetizationSDK
    {
        public void Initialize(string appId, string userId, Action onCompleted, Action onFailed)
        {
            throw new NotImplementedException();
        }

        public Task<Result> InitializeAsync(string appId, string userId)
        {
            throw new NotImplementedException();
        }

        public bool IsInitialized()
        {
            throw new NotImplementedException();
        }

        public void LoadAds(Action onCompleted, Action onFailed)
        {
            throw new NotImplementedException();
        }

        public Task<Result> LoadAdsAsync()
        {
            throw new NotImplementedException();
        }

        public bool IsAdsReady()
        {
            throw new NotImplementedException();
        }

        public void ShowRewardedAds(Action onCompleted, Action onFailed, Action onRewarded)
        {
            throw new NotImplementedException();
        }
    }
}