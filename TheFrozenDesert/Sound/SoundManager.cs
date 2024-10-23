using Microsoft.Xna.Framework.Audio;
using TheFrozenDesert.Content;

namespace TheFrozenDesert.Sound
{
    public sealed class SoundManager
    {
        private readonly SoundEffect mBuildCampfireSound;
        private readonly SoundEffect mCutTreeSound;
        private readonly SoundEffect mDyingSound;
        private readonly SoundEffect mMineRockSound;
        private readonly SoundEffect mShootBowSound;
        private readonly SoundEffect mWalkingSound;
        private readonly SoundEffect mHumanHitSound;
        private readonly SoundEffect mGameOverSound;
        private readonly SoundEffect mWinSound;
        private readonly SoundEffect mBuildSound;
        private readonly SoundEffect mRadioSound;
        private readonly SoundEffect mSledgeSound;
        private readonly SoundEffect mSegmentSound;
        private readonly SoundEffect mEatSound;
        private readonly SoundEffect mCutBigTreeSound;
        private SoundEffectInstance mBuildCampfireSoundInstance;
        private SoundEffectInstance mCutTreeSoundInstance;
        private SoundEffectInstance mDyingSoundInstance;
        private SoundEffectInstance mMineRockSoundInstance;
        private SoundEffectInstance mShootBowSoundInstance;
        private SoundEffectInstance mWalkingSoundInstance;
        private SoundEffectInstance mHumanHitSoundInstance;
        private SoundEffectInstance mGameOverSoundInstance;
        private SoundEffectInstance mWinSoundInstance;
        private SoundEffectInstance mBuildSoundInstance;
        private SoundEffectInstance mRadioSoundInstance;
        private SoundEffectInstance mSledgeInstance;
        private SoundEffectInstance mSegmentInstance;
        private SoundEffectInstance mEatInstance;
        private SoundEffectInstance mCutBigTreeInstance;
        private Music.CurrentSong mCurrentSong;
        
        
        private float mSoundEffectVolume;
        private bool mSoundEffectPlaying = true;

        internal SoundManager(FrozenDesertContentManager content)
        {
            mWalkingSound = content.GetSoundEffect("Music/laufen");
            mDyingSound = content.GetSoundEffect("Music/dead");
            mShootBowSound = content.GetSoundEffect("Music/bogen_schießen");
            mCutTreeSound = content.GetSoundEffect("Music/baum_abbauen");
            mMineRockSound = content.GetSoundEffect("Music/fels_abbauen");
            mBuildCampfireSound = content.GetSoundEffect("Music/lager_feuer_füllen");
            mHumanHitSound = content.GetSoundEffect("Music/human_hit");
            mGameOverSound = content.GetSoundEffect("Music/GameOver!");
            mWinSound = content.GetSoundEffect("Music/win1");
            mBuildSound = content.GetSoundEffect("Music/build");
            mRadioSound = content.GetSoundEffect("Music/Radio_rauschen");
            mSledgeSound = content.GetSoundEffect("Music/SledgeMoving");
            mSegmentSound = content.GetSoundEffect("Music/SegmentFinish");
            mEatSound = content.GetSoundEffect("Music/EatSound");
            mCutBigTreeSound = content.GetSoundEffect("Music/BigTreeFalling");
            mCurrentSong = Music.CurrentSong.Nothing;

            //BackgroundMusic = new Music(content);

            // ContentManager musicContent = new ContentManager(content.ServiceProvider, content.RootDirectory); 
            BackgroundMusic = new Music(content, 0.2f);
            mSoundEffectVolume = 0.2f;   // set Sound Effect Volume to Start Value 
        }

        private Music BackgroundMusic { get; }

        public void VolumeUp()
        {
           BackgroundMusic.VolumeUp(0.2f); 
           VolumeSoundEffectUp(0.2f);
        }

        public void VolumeDown()
        {
            BackgroundMusic.VolumeDown(0.2f);
            VolumeSoundEffectDown(0.2f);
        }
        public void ToggleSound()
        {
            BackgroundMusic.ToggleSound();
            ToggleSoundEffectSound();
        }

        private void VolumeSoundEffectUp(float vol)
        {
            var newVolume = mSoundEffectVolume + vol;
            if (newVolume <= 1)
            {
                mSoundEffectVolume = newVolume;
            }
            else
            {
                mSoundEffectVolume = 1;
            }

        }

        private void VolumeSoundEffectDown(float vol)
        {
            var newVolume = mSoundEffectVolume - vol;
            if (newVolume >= 0)
            {
                mSoundEffectVolume = newVolume;
            }
            else
            {
                mSoundEffectVolume = 0;
            }
        }

        private void ToggleSoundEffectSound()
        {
            mSoundEffectPlaying = !mSoundEffectPlaying;
        }

        private void PlaySafeSound(SoundEffectInstance soundEffectInstance)
        {
            if (soundEffectInstance != null && soundEffectInstance.State != SoundState.Playing)
            {
                soundEffectInstance.Play();
            }
        }
        public void MainMenuSound()
        {
            if (mCurrentSong == Music.CurrentSong.MainMenuMusic)
            {
                return;
            }

            mCurrentSong = Music.CurrentSong.MainMenuMusic;
            BackgroundMusic.NextSong("Music/MainMenuMusic");
            BackgroundMusic.PlaySong();
        }

        // unterscheide zwischen mainmenu  und andere Listen in den letzzen 2 Zeilen 

        public void PauseMenuSound()
        {
            if (mCurrentSong == Music.CurrentSong.PauseMenuMusic)
            {
                return;
            }

            BackgroundMusic.NextSong("Music/Pause_Menu");
            mCurrentSong = Music.CurrentSong.PauseMenuMusic;
            BackgroundMusic.PlaySong();
        }


        public void GameSound()
        {
            if (mCurrentSong == Music.CurrentSong.NewGameMusic)
            {
                return;
            }


            BackgroundMusic.NextSong("Music/newGame");
            mCurrentSong = Music.CurrentSong.NewGameMusic;
            BackgroundMusic.PlaySong();
        }

        public void WinSound()
        {
            BackgroundMusic.StopSong();

            if (mSoundEffectPlaying)
            {
                if (mWinSoundInstance == null)
                {
                    mWinSoundInstance = mWinSound.CreateInstance();
                    mWinSoundInstance.IsLooped = false;
                    mWinSoundInstance.Volume = mSoundEffectVolume;
                    mWinSoundInstance.Play();
                }
                else
                {
                    mWinSoundInstance.Volume = mSoundEffectVolume;
                    PlaySafeSound(mWinSoundInstance);
                }
            }


        }

        public void GameOverSound()
        
        {

            BackgroundMusic.StopSong();
            if (mSoundEffectPlaying)
            {
                if (mGameOverSoundInstance == null)
                {
                    mGameOverSoundInstance = mGameOverSound.CreateInstance();
                    mGameOverSoundInstance.IsLooped = false;
                    mGameOverSoundInstance.Volume = mSoundEffectVolume;
                    mGameOverSoundInstance.Play();
                }
                else
                {
                    mGameOverSoundInstance.Volume = mSoundEffectVolume;
                    PlaySafeSound(mGameOverSoundInstance);
                }
            }
        }

        public void Dead_sound()
        {
            if (mSoundEffectPlaying)
            {
                if (mDyingSoundInstance == null)
                {
                    mDyingSoundInstance = mDyingSound.CreateInstance();
                    mDyingSoundInstance.IsLooped = false;
                    mDyingSoundInstance.Volume = mSoundEffectVolume;
                    mDyingSoundInstance.Play();
                }
                else
                {
                    mDyingSoundInstance.Volume = mSoundEffectVolume;
                    PlaySafeSound(mDyingSoundInstance);
                }
            }
        }
        public void Build_sound()
        {
            if (mSoundEffectPlaying)
            {
                if (mBuildSoundInstance == null)
                {
                    mBuildSoundInstance = mBuildSound.CreateInstance();
                    mBuildSoundInstance.IsLooped = false;
                    mBuildSoundInstance.Volume = mSoundEffectVolume;
                    mBuildSoundInstance.Play();
                }
                else
                {
                    mBuildSoundInstance.Volume = mSoundEffectVolume;
                    PlaySafeSound(mBuildSoundInstance);
                }
            }
        }
        public void Radio_sound()
        {
            if (mSoundEffectPlaying)
            {
                if (mRadioSoundInstance == null)
                {
                    mRadioSoundInstance = mRadioSound.CreateInstance();
                    mRadioSoundInstance.IsLooped = false;
                    mRadioSoundInstance.Volume = mSoundEffectVolume;
                    mRadioSoundInstance.Play();
                }
                else
                {
                    mRadioSoundInstance.Volume = mSoundEffectVolume;

                    PlaySafeSound(mRadioSoundInstance);
                }
            }
        }

        public void Baum_abbauen_sound()
        {
            if (mSoundEffectPlaying)
            {
                if (mCutTreeSoundInstance == null)
                {
                    mCutTreeSoundInstance = mCutTreeSound.CreateInstance();
                    mCutTreeSoundInstance.IsLooped = false;
                    mCutTreeSoundInstance.Volume = mSoundEffectVolume;
                    mCutTreeSoundInstance.Play();
                }

                else
                {
                    mCutTreeSoundInstance.Volume = mSoundEffectVolume;
                    PlaySafeSound(mCutTreeSoundInstance);
                }
            }
        }

        public void Fels_abbauen_sound()
        {
            if (mSoundEffectPlaying)
            {
                if (mMineRockSoundInstance == null)
                {
                    mMineRockSoundInstance = mMineRockSound.CreateInstance();
                    mMineRockSoundInstance.IsLooped = false;
                    mMineRockSoundInstance.Volume = mSoundEffectVolume;
                    mMineRockSoundInstance.Play();
                }

                else
                {
                    mMineRockSoundInstance.Volume = mSoundEffectVolume;
                    PlaySafeSound(mMineRockSoundInstance);
                }
            }
        }

        public void Laufen_sound()
        {
            if (mSoundEffectPlaying)
            {
                if (mWalkingSoundInstance == null)
                {
                    mWalkingSoundInstance = mWalkingSound.CreateInstance();
                    mWalkingSoundInstance.IsLooped = false;
                    mWalkingSoundInstance.Volume = mSoundEffectVolume;
                    mWalkingSoundInstance.Play();
                }
                else
                {
                    mWalkingSoundInstance.Volume = mSoundEffectVolume;
                    PlaySafeSound(mWalkingSoundInstance);
                }
            }
        }

        public void Sledge_Sliding()
        {
            if (mSoundEffectPlaying)
            {
                if (mSledgeInstance == null)
                {
                    mSledgeInstance = mSledgeSound.CreateInstance();
                    mSledgeInstance.IsLooped = false;
                    mSledgeInstance.Volume = mSoundEffectVolume;
                    mSledgeInstance.Play();
                }
                else
                {
                    mSledgeInstance.Volume = mSoundEffectVolume;
                    PlaySafeSound(mSledgeInstance);
                }
            }
        }

        public void Segment_New()
        {
            if (mSoundEffectPlaying)
            {
                if (mSegmentInstance == null)
                {
                    mSegmentInstance = mSegmentSound.CreateInstance();
                    mSegmentInstance.IsLooped = false;
                    mSegmentInstance.Volume = mSoundEffectVolume;
                    mSegmentInstance.Play();
                }
                else
                {
                    mSegmentInstance.Volume = mSoundEffectVolume;
                    PlaySafeSound(mSegmentInstance);
                }
            }
        }

        public void Food()
        {
            if (mSoundEffectPlaying)
            {
                if (mEatInstance == null)
                {
                    mEatInstance = mEatSound.CreateInstance();
                    mEatInstance.IsLooped = false;
                    mEatInstance.Volume = mSoundEffectVolume;
                    mEatInstance.Play();
                }
                else
                {
                    mEatInstance.Volume = mSoundEffectVolume;
                    PlaySafeSound(mEatInstance);
                }
            }

        }

        public void BigTree()
        {
            if (mSoundEffectPlaying)
            {
                if (mCutBigTreeInstance == null)
                {
                    mCutBigTreeInstance = mCutBigTreeSound.CreateInstance();
                    mCutBigTreeInstance.IsLooped = false;
                    mCutBigTreeInstance.Volume = mSoundEffectVolume;
                    mCutBigTreeInstance.Play();
                }
                else
                {
                    mCutBigTreeInstance.Volume = mSoundEffectVolume;
                    PlaySafeSound(mCutBigTreeInstance);
                }
            }
        }

        public void Lager_feuer_füllen_sound()
        {
            if (mSoundEffectPlaying)
            {
                if (mBuildCampfireSoundInstance == null)
                {
                    mBuildCampfireSoundInstance = mBuildCampfireSound.CreateInstance();
                    mBuildCampfireSoundInstance.IsLooped = false;
                    mBuildCampfireSoundInstance.Volume = mSoundEffectVolume;
                    mBuildCampfireSoundInstance.Play();
                }

                else
                {
                    mBuildCampfireSoundInstance.Volume = mSoundEffectVolume;
                    PlaySafeSound(mBuildCampfireSoundInstance);
                }
            }
        }

        public void Bogen_schießen_sound()
        {
            if (mSoundEffectPlaying)
            {
                if (mShootBowSoundInstance == null)
                {
                    mShootBowSoundInstance = mShootBowSound.CreateInstance();
                    mShootBowSoundInstance.IsLooped = false;
                    mShootBowSoundInstance.Volume = mSoundEffectVolume;
                    mShootBowSoundInstance.Play();
                }

                else
                {
                    mShootBowSoundInstance.Volume = mSoundEffectVolume;
                    PlaySafeSound(mShootBowSoundInstance);
                }
            }
        }
        public void Human_hit_sound()
        {
            if (mSoundEffectPlaying)
            {
                if (mHumanHitSoundInstance == null)
                {
                    mHumanHitSoundInstance = mHumanHitSound.CreateInstance();
                    mHumanHitSoundInstance.IsLooped = false;
                    mHumanHitSoundInstance.Volume = mSoundEffectVolume;
                    mHumanHitSoundInstance.Play();
                }

                else
                {
                    mHumanHitSoundInstance.Volume = mSoundEffectVolume;
                    PlaySafeSound(mHumanHitSoundInstance);
                }
            }
        }
    }
}