using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

namespace Runtime.PublicAPI.Internal
{
    public class AdsPlayer : MonoBehaviour
    {
        [SerializeField] VideoPlayer videoPlayer;
        [SerializeField] RawImage renderImage;
        [SerializeField] CloseButtonDrawer skipButton;

        private Action _onPrepared;
        private Action _onSkipped;
        private Action _onCompleted;
        public bool IsReady { get; private set; }

        void Start()
        {
            videoPlayer.prepareCompleted += OnPrepared;
            videoPlayer.loopPointReached += OnVideoEnded;
            skipButton.onClose += Stop;
            skipButton.enabled = false;
        }

        public void Prepare(string url, Action onCompleted)
        {
            videoPlayer.url = url;
            _onPrepared = onCompleted;
            videoPlayer.Prepare();
        }

        public void Play(Action onCompleted, Action onSkipped)
        {
            _onCompleted = onCompleted;
            _onSkipped = onSkipped;
            renderImage.enabled = true;
            skipButton.enabled = true;
            videoPlayer.Play();
        }

        private void Stop()
        {
            _onSkipped?.Invoke();
            videoPlayer.Stop();
            renderImage.enabled = false;
            skipButton.enabled = false;
        }

        private void OnVideoEnded(VideoPlayer source)
        {
            _onCompleted?.Invoke();
            videoPlayer.Stop();
            renderImage.enabled = false;
            skipButton.enabled = false;
        }

        private void OnPrepared(VideoPlayer vp)
        {
            IsReady = true;
            _onPrepared?.Invoke();
        }
        
        private void OnDestroy()
        {
            videoPlayer.prepareCompleted -= OnPrepared;
            skipButton.onClose -= Stop;
        }
    }
}
