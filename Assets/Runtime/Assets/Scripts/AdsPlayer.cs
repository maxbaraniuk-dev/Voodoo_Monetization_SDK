using System;
using UnityEngine;
using UnityEngine.Video;

namespace Runtime.Assets.Scripts
{
    internal class AdsPlayer : MonoBehaviour
    {
        [SerializeField] VideoPlayer videoPlayer;

        private Action _onPrepared;
        internal bool IsReady { get; private set; }

        void Start()
        {
            videoPlayer.prepareCompleted += OnPrepared;
        }

        public void Prepare(string url, Action onCompleted)
        {
            videoPlayer.url = url;
            _onPrepared = onCompleted;
            videoPlayer.Prepare();
        }

        public void Play()
        {
            videoPlayer.Play();
        }

        private void OnDestroy()
        {
            videoPlayer.prepareCompleted -= OnPrepared;
        }

        private void OnPrepared(VideoPlayer vp)
        {
            IsReady = true;
            _onPrepared.Invoke();
        }
    }
}
