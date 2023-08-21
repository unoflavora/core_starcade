using System.Threading.Tasks;
using Agate.Starcade.Boot;
using Agate.Starcade.Runtime.Audio;
using Agate.Starcade.Runtime.Main;
using Agate.Starcade.Scripts.Runtime.Arcade.Monstamatch.Data;

namespace Agate.Starcade.Scripts.Runtime.Arcade.Monstamatch
{
    public static class ArcadeAudioHelper
    {
        private static AudioController _audioController;

        public static async Task Init()
        {
            _audioController = MainSceneController.Instance.Audio;
            await _audioController.LoadAudioData("monstarmatch_audio");
        }

        public static async void Unload()
        {
            _audioController.UnloadAudioData("monstarmatch_audio");
        }

        //TODO Complete Audio Method
        public static void PlayMainBGM()
        {
            _audioController.PlayBgm(AudioKey.MainBGM);
        }

        public static void StopBGM()
        {
            _audioController.StopBgm();
        }

        public static void PlayMatchNotes(int pitch)
        {
            switch (pitch)
            {
                case 1:
                    _audioController.PlaySfx(AudioKey.Match01);
                    break;
                case 2:
                    _audioController.PlaySfx(AudioKey.Match02);
                    break;
                case 3:
                    _audioController.PlaySfx(AudioKey.Match03);
                    break;
                case 4:
                    _audioController.PlaySfx(AudioKey.Match04);
                    break;
                case 5:
                    _audioController.PlaySfx(AudioKey.Match05);
                    break;
                case 6:
                    _audioController.PlaySfx(AudioKey.Match06);
                    break;
                case 7:
                    _audioController.PlaySfx(AudioKey.Match07);
                    break;
                case 8:
                    _audioController.PlaySfx(AudioKey.Match08);
                    break;
            }
        }

        public static void PlaySpinButton()
        {
            _audioController.PlaySfx(AudioKey.SpinButton);
        }

        public static void PlayMatchPop()
        {
            _audioController.PlaySfx(AudioKey.MatchPop);
        }

        public static void PlayPuzzleJackpot()
        {
            _audioController.PlayFocusSfx(AudioKey.PuzzleJackpot);
        }

        public static void PlayPuzzlePieces()
        {
            _audioController.PlaySfx(AudioKey.PuzzlePieces);
        }

        public static void PlayCoinJackpot()
        {
            _audioController.PlayFocusSfx(AudioKey.CoinJackpot);
        }

        public static void PlayCoinObtained()
        {
            _audioController.PlaySfx(AudioKey.CoinObtained);
        }

        public static void PlayArtifactJackpot()
        {
            _audioController.PlayFocusSfx(AudioKey.ArtifactJackpot);
        }
    }
}