using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using TheFrozenDesert.Input;
using TheFrozenDesert.MainMenu;

namespace TheFrozenDesert.States
{
    public sealed class StateManager
    {
        private readonly Game1 mGame;
        private GraphicsDevice mGraphicsDevice;
        private readonly ContentManager mContentManager;
        private readonly InputHandler mInputHandler;
        private readonly GameWindow mGameWindow;

        private StateType mPrevOptions = StateType.OptionsMenu;
        
        // Define State types
        public enum StateType
        {
            Game,
            NewGame,
            Menu,
            Options,
            OptionsMenu,
            PauseMenu,
            PauseMenuOptions,
            SoundMenu,
            VolumeMenu,
            Techdemo,
            AchievementsMenu,
            InGameStatisticsMenu,
            GesamtStatisticsMenu,
            LoadGame,
            WinState,
            GameOverState
        }
        
        // States which need to be saved
        public GameState GameState { get; private set;}

        private State mActiveState;

        internal StateManager(Game1 game, 
            GraphicsDevice graphicsDevice,
            ContentManager contentManager,
            InputHandler inputHandler,
            GameWindow gameWindow)
        {
            mGame = game;
            mGraphicsDevice = graphicsDevice;
            mContentManager = contentManager;
            mInputHandler = inputHandler;
            mGameWindow = gameWindow;

        }

        internal void Initialize(GraphicsDevice graphicsDevice)
        {
            mGraphicsDevice = graphicsDevice;
            // start with a MenuState
            ChangeToMenu();
        }

        internal void UpdateCurrentState(GameTime gameTime, Game1.Managers managers)
        {
            mActiveState.Update(gameTime, managers);
            mActiveState.PostUpdate(gameTime);
        }

        internal void DrawCurrentState(GameTime gameTime, SpriteBatch spriteBatch)
        {
            mActiveState.Draw(gameTime, spriteBatch); 
        }
        public void ChangeState(StateType stateType)
        {
            // save current State if nessesary
            if (mActiveState is GameState state)
            {
                // save GameState
                GameState = state;
            }
            // switch to new State
            switch (stateType)
            {
                case StateType.Game:
                    mGame.mManagers.mSoundManager.GameSound();
                    ChangeToGame(false, false);
                    break;
                case StateType.NewGame:
                    ChangeToGame(false, true);
                    break;
                case StateType.Menu:
                    ChangeToMenu();
                    break;
                case StateType.Options:
                    ChangeState(mPrevOptions);
                    break;
                case StateType.OptionsMenu:
                    mPrevOptions = StateType.OptionsMenu;
                    ChangeToOptionsMenu();
                    break;
                case StateType.PauseMenu:
                    ChangeToPauseMenu();
                    break;
                case StateType.PauseMenuOptions:
                    mPrevOptions = StateType.PauseMenuOptions;
                    ChangeToPauseMenuOptions();
                    break;
                case StateType.SoundMenu:
                    ChangeToSoundMenu();
                    break;
                case StateType.VolumeMenu:
                    ChangeToVolumeMenu();
                    break;
                case StateType.Techdemo:
                    ChangeToGame(true, true);
                    break;
                case StateType.AchievementsMenu:
                    ChangeToAchievementsMenu();
                    break;
                case StateType.LoadGame:
                    ChangeToLoadGame();
                    break;
                case StateType.InGameStatisticsMenu:
                    ChangeToInGameStatistics();
                    break;
                case StateType.GesamtStatisticsMenu:
                    ChangeToGesamtStatistics();
                    break;
                case StateType.GameOverState:
                    ChangeToGameOverState();
                    break;
                case StateType.WinState:
                    ChangeToWinState();
                    break;
            }
        }

        /* private methods to change to State ******** */
        private void ChangeToGame(bool isTechDemo, bool isNewGame)
        {
            if (GameState != null && !isNewGame)
            {
                mActiveState = GameState;
            }
            else
            {
                mActiveState = new GameState(mGame, mGraphicsDevice, mContentManager, mInputHandler, mGameWindow, new Random().Next(), isTechDemo);
                if (isTechDemo)
                {
                    ((GameState)mActiveState).MakeTechDemo();
                }
            }
        }

        private void ChangeToGameOverState()
        {
            mActiveState = new GameOverState(mGame, mGraphicsDevice, mContentManager, mInputHandler);
        }
        public void LoadGame(int id)
        {
            if (mActiveState is GameState state)
            {
                // save GameState
                GameState = state;
            }
            if (GameState != null && GameState.mGameStateId == id)
            {
                mActiveState = GameState;
            }
            else
            {
                mActiveState = new GameState(mGame, mGraphicsDevice, mContentManager, mInputHandler, mGameWindow, id, false);
            }
        }

        private void ChangeToMenu()
        {
            mActiveState = new MenuState(mGame, mGraphicsDevice, mContentManager, mInputHandler);
        }
        private void ChangeToLoadGame()
        {
            mActiveState = new Saves(mGame, mGraphicsDevice, mContentManager, mInputHandler);
        }
        private void ChangeToWinState()
        {
            mActiveState = new WinState(mGame, mGraphicsDevice, mContentManager, mInputHandler);
        }
        private void ChangeToOptionsMenu()
        {
            mActiveState = new Options(mGame, mGraphicsDevice, mContentManager, mInputHandler);
        }

        private void ChangeToAchievementsMenu()
        {
            mActiveState = new Achievements(mGame, mGraphicsDevice, mContentManager, mInputHandler);
        }

        private void ChangeToPauseMenu()
        { 
            mActiveState = new PauseState(mGame, mGraphicsDevice, mContentManager, mInputHandler);
        }
        private void ChangeToInGameStatistics()
        {
            mActiveState = new InGameStatistics(mGame, mGraphicsDevice, mContentManager, mInputHandler);
        }
        private void ChangeToGesamtStatistics()
        {
            mActiveState = new GesamtStatistics(mGame, mGraphicsDevice, mContentManager, mInputHandler);
        }
        private void ChangeToPauseMenuOptions()
        { 
            mActiveState = new PauseMenuOptions(mGame, mGraphicsDevice, mContentManager, mInputHandler);
        }
        private void ChangeToSoundMenu()
        {
            mActiveState = new MainMenu.Sound(mGame, mGraphicsDevice, mContentManager, mInputHandler);
        }
        private void ChangeToVolumeMenu()
        {
            mActiveState = new Volume(mGame, mGraphicsDevice, mContentManager, mInputHandler);
        }

    }
}