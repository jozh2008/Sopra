using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using MonoGame.Extended.Content;

namespace TheFrozenDesert.Content
{
    public sealed class FrozenDesertContentManager
    {
        private readonly ContentManager mContent;
        private readonly Dictionary<string, Song> mSongs;
        private readonly Dictionary<string, SoundEffect> mSoundEffects;
        private readonly Dictionary<string, Texture2D> mTextures;
        private SpriteFont mFont;

        public FrozenDesertContentManager(ContentManager contentManager)
        {
            mContent = contentManager;
            mTextures = new Dictionary<string, Texture2D>();
            mSongs = new Dictionary<string, Song>();
            mSoundEffects = new Dictionary<string, SoundEffect>();
        }

        public void LoadAllContent()
        {
            LoadFont();
        }

        private void LoadFont()
        {
            mFont = mContent.Load<SpriteFont>("Fonts/Font");
        }

        /* Textures ************************************************************************** */
        private void LoadTexture(string textureLocation)
        {
            mTextures.Add(textureLocation, mContent.Load<Texture2D>(textureLocation));
        }

        public Texture2D GetTexture(string textureLocation)
        {
            try
            {
                return mTextures[textureLocation];
            }
            catch (KeyNotFoundException)
            {
                try
                {
                    LoadTexture(textureLocation);
                    return mTextures[textureLocation];
                }
                catch (ContentLoadException)
                {
                    return mTextures["GameplayObjects/missingTexture"];
                }
            }
        }

        public Texture2D GetBlankTexture()
        {
            return new Texture2D(mContent.GetGraphicsDevice(), 1, 1);
        }

        /* Sounds ************************************************************************** */
        //Songs
        private void LoadSong(string songLocation)
        {
            mSongs.Add(songLocation, mContent.Load<Song>(songLocation));
        }

        public Song GetSong(string songLocation)
        {
            try
            {
                return mSongs[songLocation];
            }
            catch (KeyNotFoundException)
            {
                try
                {
                    LoadSong(songLocation);
                    return mSongs[songLocation];
                }
                catch (ContentLoadException)
                {
                    throw new SystemException("The given Song File can't be loaded!");
                }
            }
        }

        //Sound Effects
        private void LoadSoundEffect(string soundEffectLocation)
        {
            mSoundEffects.Add(soundEffectLocation, mContent.Load<SoundEffect>(soundEffectLocation));
        }

        public SoundEffect GetSoundEffect(string soundEffectLocation)
        {
            try
            {
                return mSoundEffects[soundEffectLocation];
            }
            catch (KeyNotFoundException)
            {
                try
                {
                    LoadSoundEffect(soundEffectLocation);
                    return mSoundEffects[soundEffectLocation];
                }
                catch (ContentLoadException)
                {
                    return mSoundEffects["Music/laufen"];
                }
            }
        }

        /* Fonts ************************************************************************** */
        public SpriteFont GetFont()
        {
            return mFont;
        }
    }
}