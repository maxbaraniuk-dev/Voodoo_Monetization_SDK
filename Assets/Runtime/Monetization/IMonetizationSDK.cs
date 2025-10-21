using System;
using System.Threading.Tasks;
using Runtime.Core;

namespace Runtime.Monetization
{
    interface IMonetizationSDK
    {
        void Initialize(string appId, string userId, Action onCompleted, Action onFailed);
        Task<Result> InitializeAsync(string appId, string userId);
        bool IsInitialized();
        void LoadAds(Action onCompleted, Action onFailed);
        Task<Result> LoadAdsAsync();
        bool IsAdsReady();
        void ShowRewardedAds(Action onCompleted, Action onFailed, Action onRewarded);
    }
}