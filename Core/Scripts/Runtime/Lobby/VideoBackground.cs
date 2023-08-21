using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Video;

namespace Agate.Starcade
{
    public class VideoBackground : MonoBehaviour
    {
        [SerializeField] private VideoPlayer _videoPlayer;

        private void Start()
        {
            _videoPlayer.playOnAwake = false;
            _videoPlayer.url = Path.Combine(Application.streamingAssetsPath, "Background.mp4");
            _videoPlayer.isLooping = true;
            _videoPlayer.Play();
        }
    }
}
