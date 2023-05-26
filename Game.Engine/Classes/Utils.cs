using System;
using System.IO;

namespace Ira.Game {
    public static class Utils {
        static Random rnd;
        private static System.Media.SoundPlayer player = new System.Media.SoundPlayer();
        private static WMPLib.WindowsMediaPlayer bgPlayer;

        static Utils() {
            rnd = new Random();
        }

        public static int GetRandome(int min, int max) {
            return rnd.Next(min, max);
        }

        public static void PlayMainTheme() {
            var mainThemeFilePath = ".\\media\\main_theme.mp3";
            if (bgPlayer == null && File.Exists(mainThemeFilePath)) {
                bgPlayer = new WMPLib.WindowsMediaPlayer();
                bgPlayer.PlayStateChange += state => {
                    if (state == (int) WMPLib.WMPPlayState.wmppsStopped)
                        bgPlayer.controls.play();
                };
                bgPlayer.URL = mainThemeFilePath;
            }

            bgPlayer?.controls.play();
        }

        public static void StopMainTheme() {
            bgPlayer?.controls.pause();
        }

        public static void PlaySoundMove() {
            player.SoundLocation = ".\\media\\move.wav";
            player.Play();
        }

        public static void PlaySoundExit() {
            player.SoundLocation = ".\\media\\exit.wav";
            player.Play();
        }

        public static void PlaySoundKill() {
            player.SoundLocation = ".\\media\\kill.wav";
            player.Play();
        }
        
        public static void PlaySoundError() {
            player.SoundLocation = ".\\media\\error.wav";
            player.Play();
        }
    }
}