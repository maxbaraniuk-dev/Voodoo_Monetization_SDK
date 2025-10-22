using System;
using System.Threading.Tasks;
using Runtime.Internal;

namespace Voodoo.Internal.Core
{
    public interface IMonetizationSDK
    {
        void Initialize(string appId, string userId, Action onCompleted, Action onFailed);
        Task<Result> InitializeAsync(string appId, string userId);
        bool IsInitialized();
        void LoadAds(Action onCompleted, Action onFailed);
        Task<Result> LoadAdsAsync();
        bool IsAdsReady();
        void ShowRewardedAds(Action onRewarded, Action onFailed, Action onSkipped);
        void Dispose();
    }
}