using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Agate.Starcade.Runtime.Arcade.General.UI;
using Agate.Starcade.Runtime.Audio;
using Agate.Starcade.Scripts.Runtime.Arcade.Monstamatch.Jackpots;
using Agate.Starcade.Scripts.Runtime.Utilities;
using UnityEngine;
using UnityEngine.UI;

namespace Agate.Starcade.Scripts.Runtime.Arcade.Monstamatch.VFX
{
    public class VFXController : MonoBehaviour
    {
        [Header("VFX Configs")]
        [SerializeField] private float _animationFactorTime;

        [Header("Trails")]
        [SerializeField] private SpriteRenderer _puzzleVFX;

        [SerializeField] private ParticleSystem _puzzleParticle;
        [SerializeField] private Transform _coinVFX;

        [SerializeField] private Transform _coinPools;
        [SerializeField] private ParticleSystem _goldCoin;
        [SerializeField] private ParticleSystem _backgroundVFX;
        [Header("Animators")]
        [SerializeField] private Animator _curtainAnimator;

        [Header("Jackpot Notifications VFX")]
        // Puzzle Jackpot
        [SerializeField] private GameObject _puzzleJackpotVFX;
        [SerializeField] private PuzzleVFXManager _puzzleVFXManager;
        // Progress Jackpot
        [SerializeField] private Animator _progressJackpotVFX;
        // Instant Jackpot
        [SerializeField] private GameObject _instantJackpotVFX;

        [Header("Instant Jackpot VFX")] 
        [SerializeField] private Animator _miniVFX;
        [SerializeField] private Animator _minorVFX;
        [SerializeField] private Animator _majorVFX;
        
        [Header("Input VFX")]
        [SerializeField] private TouchVFX _touchVFX;
        private AudioController _audioController;
        private static readonly int Play = Animator.StringToHash("Play");
        
        private List<Transform> _coinTrailPool;
        private List<ParticleSystem> _goldCoinPool;

        
        public async Task Init()
        {
            _touchVFX.Init();
        }

        public void PlayTouchVFX()
        {
            _touchVFX.TouchRaycast();
        }
      
        public void PlayCoinTrailVFX(Image image, Vector3 from, Vector3 to)
        {
            if (_coinTrailPool == null)
            {
                _coinTrailPool = new List<Transform>();
                _goldCoinPool = new List<ParticleSystem>();
            }

            try
            {
                Transform coinTrail = null;
                ParticleSystem goldCoin = null;

                if (_coinTrailPool.Count > 0)
                {
                    coinTrail = _coinTrailPool[0];
                    goldCoin = _goldCoinPool[0];

                    _coinTrailPool.RemoveAt(0);
                    _goldCoinPool.RemoveAt(0);

                    coinTrail.position = from;
                    goldCoin.transform.position = to;
                }
                else
                {
                    coinTrail = Instantiate(_coinVFX, from, Quaternion.identity, _coinPools);
                    goldCoin = Instantiate(_goldCoin, to, Quaternion.identity, _coinPools);
                }

                coinTrail.gameObject.SetActive(true);

                LeanTween.move(coinTrail.gameObject, to, _animationFactorTime).setOnComplete(() =>
                {
                    image.enabled = true;

                    coinTrail.gameObject.SetActive(false);
                    goldCoin.Play();
                    ActivateVFX();

                    _coinTrailPool.Add(coinTrail);
                    _goldCoinPool.Add(goldCoin);
                });

            }
            catch (Exception error)
            {
                Debug.Log(error.Message);
            }
           
            
            // goldCoin.gameObject.SetActive(false);
        }
        
        private void ActivateVFX()
        {
            _backgroundVFX.gameObject.SetActive(true);
            if (_backgroundVFX.gameObject.activeSelf)
            {
                _backgroundVFX.Play();
            }
        }

        public async Task PlayPuzzleVFX(int index)
        {
            _puzzleVFXManager.PlayVFX(index);
            await Task.Yield();

        }

        public async Task PlayPuzzleTrailVFX(Image image, Vector3 from, Vector3 to)
        {
            var isAnimationComplete = false;
            _puzzleParticle.gameObject.SetActive(true);

            _puzzleVFX.sprite = image.sprite;
            var imageVFX = Instantiate(_puzzleVFX, from, Quaternion.identity, transform);

            _puzzleParticle.gameObject.transform.position = from;
            _puzzleParticle.gameObject.transform.SetParent(imageVFX.transform);

            LeanTween.move(imageVFX.gameObject, to, _animationFactorTime).setOnComplete(() =>
            {
                isAnimationComplete = true;
                image.enabled = true;
                _puzzleParticle.gameObject.transform.SetParent(transform);
            });

            await AsyncUtility.WaitUntil(() => isAnimationComplete);
            Destroy(imageVFX.gameObject);
            _puzzleParticle.gameObject.SetActive(false);
        }
        
        public async Task PlayArtifactJackpotVFX()
        {
            _progressJackpotVFX.gameObject.SetActive(true);
            
            _progressJackpotVFX.SetTrigger(Play);
            
            await AsyncUtility.WaitUntil(() => IsFinishPlaying(_progressJackpotVFX, "In", .83f));
            FinishArtifactJackpotVFX();
        }

        private async void FinishArtifactJackpotVFX()
        {
            await AsyncUtility.WaitUntil(() => IsFinishPlaying(_progressJackpotVFX, "Out", .2f));
            _progressJackpotVFX.gameObject.SetActive(false);
        }


        public async Task PlayPuzzleVFX()
        {
            _puzzleJackpotVFX.SetActive(true);
            await Task.Delay(2000);
            _puzzleJackpotVFX.SetActive(false);
        }

        public async Task PlayInstantJackpotVFX(InstantJackpotType type)
        {
            Animator instant;
            
            switch (type)
            {
                case InstantJackpotType.Minor:
                    instant = _minorVFX;
                    break;
                case InstantJackpotType.Mayor:
                    instant = _majorVFX;
                    break;
                default:
                    instant = _miniVFX;
                    break;
            }

            var vfxGameObject = instant.transform.parent.gameObject;
            vfxGameObject.SetActive(true);
            instant.SetTrigger(Play);
            
            await AsyncUtility.WaitUntil(() => IsFinishPlaying(instant, "Out", .8f));
            vfxGameObject.SetActive(false);
        }

        public async Task OpenCurtain(int delay = 800)
        {
			//TODO: HOTFIX CURTAIN TIMING
			await Task.Delay(delay);
			_curtainAnimator.SetTrigger("TrOpen"); 
            await Task.Delay(1500);
        }
        
        public async Task CloseCurtain()
        {
            _curtainAnimator.SetTrigger("TrClose");
			await AsyncUtility.WaitUntil(() => IsFinishPlaying(_curtainAnimator, "Close"));
        }
        
        private static bool IsFinishPlaying(Animator anim, string stateName, float atProgress = .99f)
        {
            AnimatorStateInfo currentAnimatorState = anim.GetCurrentAnimatorStateInfo(0);
            return currentAnimatorState.IsName(stateName) && currentAnimatorState.normalizedTime >= atProgress;
        }
    }
}
