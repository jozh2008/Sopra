using Microsoft.Xna.Framework.Media;
using TheFrozenDesert.Content;

//using System.Linq;
//using System.Text;

namespace TheFrozenDesert.Sound
{
    internal sealed class Music
    {
        // saves which song is currently being played

        private bool mPlayingState = true;

        public enum CurrentSong
        {
            NewGameMusic,
            MainMenuMusic,
            PauseMenuMusic,
            Nothing
        }

        private readonly FrozenDesertContentManager mFrozenDesertContent;
        private Song mSong;


        public Music(FrozenDesertContentManager content, float startVolume)
        {
            mFrozenDesertContent = content;
            IsPlaying = false;
            MediaPlayer.Volume = startVolume;
        }

        private bool IsPlaying { get; set; }


        public void PlaySong()
        {
            if (IsPlaying || mPlayingState == false)
            {
                return;
            }

            MediaPlayer.Play(mSong);
            MediaPlayer.IsRepeating = true;
            IsPlaying = true;
        }

        public void StopSong()
        {
            MediaPlayer.Stop();
            IsPlaying = false;
        }


        /// <param name="songName"></param>
        public void NextSong(string songName)
        {
            if (IsPlaying)
            {
                MediaPlayer.Stop();
                // mContent.UnloadSound();
            }

            if (mFrozenDesertContent != null)
            {
                mSong = mFrozenDesertContent.GetSong(songName);
            }

            IsPlaying = false;
        }


        public void VolumeDown(float volume)
        {
            MediaPlayer.Volume -= volume;
        }
        public void VolumeUp(float volume)
        {
            MediaPlayer.Volume += volume;
        }

        public void ToggleSound()
        {
            var state = MediaPlayer.State;
            if (state == MediaState.Playing)
            {
                // stop sound
                StopSong();
                mPlayingState = false;
            }
            else if (state == MediaState.Stopped)
            {
                // Start Playing sound
                PlaySong();
                mPlayingState = true;
            }
        }
    }
}