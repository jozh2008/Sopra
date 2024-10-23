using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TheFrozenDesert.Content;
using TheFrozenDesert.Input;
using TheFrozenDesert.Sound;
using TheFrozenDesert.States;
using TheFrozenDesert.Storage;
using TheFrozenDesert.Storage.Models;


namespace TheFrozenDesert
{
    public sealed class Game1 : Game
    {
        private readonly FrozenDesertContentManager mFrozenDesertContentManager;
        private readonly GraphicsDeviceManager mGraphics;
        private readonly InputHandler mInputHandler;
        public readonly Managers mManagers;
        private readonly SafeLoadManager mSafeLoadManager;
        private readonly SoundManager mSoundManager;
        private readonly StateManager mStateManager;
        private readonly SelectionManager mSelectionManager;
        private Texture2D mBackgroundTexture;
        private SpriteBatch mSpriteBatch;
        public AchievementsModel AchievementsModel { get; private set; }
        public StatisticsModel StatisticsModel { get; private set; }
        public static Random mGlobalRandomNumberGenerator;
        public const int MapWidth = 575;
        public const int MapHeight = 177;
        public Game1()
        {
            mGraphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            
            // initialize random number generator
            mGlobalRandomNumberGenerator = new Random();

            // initalizing managers
            mInputHandler = new InputHandler();
            var windowManager = new WindowManager(mGraphics, Window);
            mSafeLoadManager = new SafeLoadManager();
            mFrozenDesertContentManager = new FrozenDesertContentManager(Content);
            mSoundManager = new SoundManager(mFrozenDesertContentManager);
            mSelectionManager = new SelectionManager();
            mStateManager = new StateManager(this, GraphicsDevice, Content, mInputHandler, Window); // StateManager needs to be the laste one initialized
            
            // defining manager struct
            mManagers = new Managers(mSoundManager, mInputHandler, mSelectionManager);
            mInputHandler.ToggleFullscreen += windowManager.OnToggleFullscreen;
        }

        protected override void Initialize()
        {
            IsMouseVisible = true;
            mGraphics.PreferredBackBufferWidth = 1280; // format is 16:9 pixels are equal to virtual Camera width/height
            mGraphics.PreferredBackBufferHeight = 720;
            mGraphics.ApplyChanges();
            mGraphics.ApplyChanges();

            base.Initialize();
            mStateManager.Initialize(GraphicsDevice); // initalize State handler and start startmenu
            AchievementsModel = mSafeLoadManager.LoadAchievements();
            StatisticsModel = mSafeLoadManager.LoadStatisticsModel();
        }

        protected override void LoadContent()
        {
            mSpriteBatch = new SpriteBatch(GraphicsDevice);
            mFrozenDesertContentManager.LoadAllContent();
            mBackgroundTexture = Content.Load<Texture2D>("Controls/Background");
        }

        protected override void Update(GameTime gameTime)
        {
            mInputHandler.Update();
            mStateManager.UpdateCurrentState(gameTime, mManagers);
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            mSpriteBatch.Begin();

            mSpriteBatch.Draw(mBackgroundTexture,
                new Rectangle(0,
                    0,
                    GraphicsDevice.Viewport.Width,
                    GraphicsDevice.Viewport.Height),
                Color.AliceBlue);


            mStateManager.DrawCurrentState(gameTime, mSpriteBatch);
            mSpriteBatch.End();


            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }

        public SafeLoadManager GetSafeLoadManager()
        {
            return mSafeLoadManager;
        }

        public FrozenDesertContentManager GetContentManager()
        {
            return mFrozenDesertContentManager;
        }

        public SoundManager GetSoundManager()
        {
            return mSoundManager;
        }

        public StateManager GetStateManager()
        {
            return mStateManager;
        }

        public SelectionManager GetSelectionManager()
        {
            return mSelectionManager;
        }

        public readonly struct Managers
        {
            public readonly SoundManager mSoundManager;
            public readonly InputHandler mInputHandler;
            public readonly SelectionManager mSelectionManager;

            public Managers(SoundManager soundManager,
                InputHandler inputHandler,
                SelectionManager selectionManager)
            {
                mSoundManager = soundManager;
                mInputHandler = inputHandler;
                mSelectionManager = selectionManager;
            }
        }
    }
}