using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TheFrozenDesert.AI;
using TheFrozenDesert.GamePlayObjects.Equipment;
using TheFrozenDesert.GamePlayObjects.GUI;
using TheFrozenDesert.Pathfinding;
using TheFrozenDesert.Sound;
using TheFrozenDesert.States;
using TheFrozenDesert.Storage.Models;

namespace TheFrozenDesert.GamePlayObjects
{
    public enum EFaction
    {
        Player,
        Neutral,
        Enemy
    }

    public abstract class Human : AbstractMoveableObject
    {
        public readonly RoutineHandler mRoutineHandler;

        private readonly GamePlayObjectInfoBar mHealthBar;
        private readonly GamePlayObjectInfoBar mHeatBar;

        internal int Health { get; set; }

        internal bool HasArmor { get; set; }

        protected int Heat { get; private set; }
        protected int Saturation { get; private set; }
        protected int Attack { get; set; }
        internal EFaction Faction { get; private set; }
        protected bool IsDead { get; private set; }

        protected readonly int mRadiusCampfire = 1;

        protected static Point mGridPositionCampfire; // postion of Campfire

        public const int CostCampfire = 4;

        protected const int CostBridge = 10;

        private Generator mGenerator;
        internal readonly GameState mGameState;
        private CookedFood mCookedFood;
        private float mTimeForNextHealing;

        private float mTimeNotification;

        public AbstractEquipment Tool { get; set; }
        public AbstractEquipment Armor { get; set; }
        public Guid Guid { get; } = Guid.NewGuid();
        
        // Timing for heat and saturation
        private double mSaturationTime;    // counts Time since last Saturation Decrease
        private double mHeatTime;          // counts Time since last Heat Decrease 

        private const int HeatDecreaseTime = 5; // defines after how much seconds heat will be decreased
        private const int SaturationDecreaseTime = 5; // defines after how much seconds saturation will be decreased

        protected Human(Texture2D texture,
            int gridPosX,
            int gridPosY,
            float movementSpeed,
            int attack,
            int health,
            int heat,
            Grid grid,
            GameState gameState,
            EFaction eFaction,
            Routine routine = Routine.Nothing) : base(texture, gridPosX, gridPosY, movementSpeed, grid)
        {
           
            mGameState = gameState;
            Health = health;
            Heat = heat;
            Attack = attack;
            Faction = eFaction;
            Saturation = 100;
            mGui = new HumanGui(gameState, this);

            mHealthBar = new GamePlayObjectInfoBar(this, 0, Color.White);
            UpdateHealthbarColor();

            mHeatBar = new GamePlayObjectInfoBar(this, -5, Color.LightBlue);
            mRoutineHandler = new RoutineHandler(this, mGrid, routine);
            if (Faction == EFaction.Player)
            {
                gameState.AddPlayerHuman(this);
            }
            if (Faction == EFaction.Enemy)
            {
                gameState.AddEnemyHuman(this);
            }

        }

        protected Human(Texture2D texture,
            Grid grid,
            GameState gameState,
            HumanModel model,
            Routine routine = Routine.Nothing) : base(texture,
            model.GridCurrentPositionX,
            model.GridCurrentPositionY,
            100,
            grid)
        {

            mGameState = gameState;
            Health = model.Health;
            Heat = model.Heat;
            Attack = model.Attack;
            Saturation = model.Saturation;
            switch (model.Faction)
            {
                case "Player":
                    Faction = EFaction.Player;
                    break;
                case "Neutral":
                    Faction = EFaction.Neutral;
                    break;
                case "Enemy":
                    Faction = EFaction.Enemy;
                    break;
            }

            mGui = new HumanGui(gameState, this);
            mHealthBar = new GamePlayObjectInfoBar(this, 0, Color.White);
            UpdateHealthbarColor();

            mHeatBar = new GamePlayObjectInfoBar(this, -5, Color.LightBlue);
            mRoutineHandler = new RoutineHandler(this, mGrid, routine);
            if (Faction == EFaction.Player)
            {
                gameState.AddPlayerHuman(this);
            }
            if (Faction == EFaction.Enemy)
            {
                gameState.AddEnemyHuman(this);
            }

            if (model.HasArmor)
            {
                Armor = new AbstractEquipment(gameState.mWeapontexture, AbstractEquipment.ItemType.Ruestung);
            }
        }

        public bool IsCold()
        {
            return Heat <= 50;
        }

        public bool IsHungry()
        {
            return Saturation <= 50;
        }

        public override void Draw(SpriteBatch spriteBatch, Camera camera)
        {
            var color = mSelected ? Color.Green : Color.White;
            var rect = new Rectangle(Rectangle.X - (int) camera.PositionPixels.X,
                Rectangle.Y - (int) camera.PositionPixels.Y,
                mFieldSize,
                mFieldSize);
            spriteBatch.Draw(mTexture, rect, mTextureRegion, color);
            mHealthBar.Draw(spriteBatch, camera);
            mHeatBar.Draw(spriteBatch, camera);
        }

        public override void Update(GameTime gameTime,
            AbstractGameObject[,] grid,
            GameState gameState,
            SoundManager soundManager)
        {
            base.Update(gameTime, grid, gameState, soundManager);
            
            UpdateHeatSaturation(gameTime);

            mRoutineHandler.Update(gameTime, gameState);

            mHealthBar.SetPercent(Health);
            mHeatBar.SetPercent(Heat);
            if (IsinsideCampfireRange(new Point(mGridPos.X, mGridPos.Y), mGameState.mGrid, mRadiusCampfire))
            {
                Heat = 100;
            }


            if (gameState.mInputHandler.KeyboardRIsDown)
            {
                mTimeNotification = 5f;
                if (Faction == EFaction.Player && gameState.mCapacityHumanInSledge > gameState.mPlayerAliveHumans.Count && HasNeighbourToRecruit())
                { 
                    gameState.mNotificationForRecruting = "Neutraler Mensch rekrutiert, neu " +
                                                        gameState.mPlayerAliveHumans.Count + " Menschen";
                }
                else if (gameState.mCapacityHumanInSledge < gameState.mPlayerAliveHumans.Count && Faction == EFaction.Player)
                {
                    gameState.mNotificationForRecruting =
                        "Nicht genug Kapazität, " + gameState.mPlayerAliveHumans.Count + " Menschen";
                }
            }

            TimerForNotification(gameTime, gameState);
            if (Faction == EFaction.Player && JustSelected())
            {
                mRoutineHandler.SetRoutine(Routine.Nothing);
            }


        }
        /* ************************************************
         * Heat and Saturation managment
         * ************************************************/
        private void UpdateHeatSaturation(GameTime gameTime)
        {
            if (Faction == EFaction.Neutral)
            {
                return;
            }

            mSaturationTime += gameTime.ElapsedGameTime.TotalSeconds;
            mHeatTime += gameTime.ElapsedGameTime.TotalSeconds;

            if (mSaturationTime >= SaturationDecreaseTime)
            {
                mSaturationTime = 0;
                DecreaseSaturation();
            }

            if (mHeatTime >= HeatDecreaseTime)
            {
                mHeatTime = 0;
                DecreaseHeat();
            }
        }
        void DecreaseSaturation()
        {
            DecreaseSaturation(1);
            if (Saturation <= 0)
            {
                DamageHuman(3, mGameState, true);
            }
        }
        void DecreaseHeat()
        {
            AddCold(HasArmor ? 1 : 2);
            if (Heat <= 0)
            {
                DamageHuman(3, mGameState, true);
            }
        }
        private void TimerForNotification(GameTime gameTime, GameState gameState)
        {
            mTimeNotification -= (float) gameTime.ElapsedGameTime.TotalSeconds;

            if (mTimeNotification < 0)
            {
                gameState.mNotificationForRecruting = "";
            }
        }

        public override void Select(List<AbstractGameObject> previouslySelected)
        {
            if (Faction == EFaction.Player)
            {
                base.Select(previouslySelected);
            }
        }

        public bool GetCookedFood(CookedFood food)
        {
            mCookedFood = food;
            if (!(mCookedFood is {Ate: false}))
            {
                return false;
            }

            var saturation = Saturation + mCookedFood.AddSaturation;
            if (saturation <= 100)
            {
                Saturation = saturation;
                mCookedFood.Ate = true;
                return true;
            }

            return false;
        }

        public void SetGeneratorAsTarget(Generator generator)
        {
            mRoutineHandler.SetRoutine(Routine.MoveToGenerator);
            mGenerator = generator;
        }

        protected int GetAttack()
        {
            return Attack;
        }

        public bool IsAlive()
        {
            return !IsDead;
        }

        public void AddHeat(int heat)
        {
            if (Heat + heat <= 100)
            {
                Heat += heat;

            }

        }

        private void AddCold(int cold)
        {
            if (Heat >= cold)
            {
                Heat -= cold;
            }
            else
            {
                Heat = 0;
            }
        }

        public void AddHealth(int health, GameTime gameTime)
        {
            mTimeForNextHealing -= (float) gameTime.ElapsedGameTime.TotalSeconds;
            if (mTimeForNextHealing < 0)
            {
                if (Health + health <= 100)
                {
                    Health += health;
                    mTimeForNextHealing = 1;

                }
                else
                {
                    Health = 100;
                    mTimeForNextHealing = 1;
                }

            }

        }

        internal abstract void TryToCraftToolEnemy();

        public string ReturnHumanType(Human human)
        {
            if (human is Gatherers)
            {
                return "Gatherer";
            }
            else if (human is Archer)
            {
                return "Archer";
            }
            else if (human is Fighter)
            {
                return "Fighter";
            }

            return "";
        }

        private string GetSpriteSheetHumanType(Human human)
        {
            if (human is Gatherers)
            {
                return "GameplayObjects/AxeSpritesheet";
            }
            else if (human is Archer)
            {
                return "GameplayObjects/ArcherSpritesheet";
            }
            else if (human is Fighter)
            {
                return "GameplayObjects/SwordSpritesheet";
            }

            return "";
        }

        private void DecreaseSaturation(int saturation)
        {
            if (Saturation >= saturation)
            {
                Saturation -= saturation;
            }
            else
            {
                Saturation = 0;
            }
        }

        protected override void UpdateMovementPath(PathFinder pathFinder)
        {
            if (Faction == EFaction.Player)
            {
                pathFinder.SetFindKeys(true);
                base.UpdateMovementPath(pathFinder);
                pathFinder.SetFindKeys(false);
            }
            else
            {
                base.UpdateMovementPath(pathFinder);
            }
        }

        public void DamageHuman(int damage, GameState gameState, bool selfDamage = false)
        {
            // added locker as it might happen that two resources are trying to access the same obj/variable
            lock (gameState)
            {
                if (HasArmor)
                {
                    damage = (int) (damage * 0.67f);
                }

                if (Health >= damage)
                {
                    Health -= damage;
                    gameState.mManagers.mSoundManager.Human_hit_sound();
                }
                else
                {
                    Health = 0;
                    IsDead = true;
                    gameState.mManagers.mSoundManager.Dead_sound();
                    if (Faction != EFaction.Player && !selfDamage)
                    {
                        gameState.AddKill();
                        gameState.mResources.Increase(ResourceType.Meat, 5);
                    }
                    else
                    {
                        gameState.mEnemyResources.Increase(ResourceType.Meat, 5);
                        gameState.RemovePlayerHuman(this);
                    }

                    mGrid.RemoveObjectFromGridPosition(new Point(mGridPos.X, mGridPos.Y));
                }
            }
        }

        //check if two object are neighbour on the grid
        public bool IsNeighbor(GridPosition gridPosition1, GridPosition gridPosition2)
        {
            return (Math.Abs(gridPosition1.X - gridPosition2.X) <= 1 &&
                    Math.Abs(gridPosition1.Y - gridPosition2.Y) <= 1);
        }

        private bool HasNeighbourToRecruit()
        {
            for (var i = -1; i <= 1; i++)
            {
                for (var j = -1; j <= 1; j++)
                {
                    var gridDestination = new Point(mGridPos.X + i, mGridPos.Y + j);
                    if (gridDestination != new Point(mGridPos.X, mGridPos.Y))
                    {
                        var gameObject = mGrid.GetAbstractGameObjectAt(gridDestination);
                        if (gameObject is Human {Faction: EFaction.Neutral} human)
                        {
                            human.SetFactionPlayer();
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        private void SetFactionPlayer()
        {
            Faction = EFaction.Player;
            mRoutineHandler.SetRoutine(Routine.Nothing);
            UpdateHealthbarColor();
            mTexture = mGameState.mGame.GetContentManager()
                .GetTexture(GetSpriteSheetHumanType(this));
            mGameState.AddPlayerHuman(this);
        }

        private void UpdateHealthbarColor()
        {
            // diffrent Healtbarcolors for diffrent Faction
            if (Faction == EFaction.Player)
            {
                mHealthBar.mForegroundColor = Color.LightGreen;
            }
            else if (Faction == EFaction.Neutral)
            {
                mHealthBar.mForegroundColor = Color.LightYellow;
            }
            else
            {
                mHealthBar.mForegroundColor = Color.Red;
            }
            
        }

        private bool IsinsideCampfireRange(Point p1, Grid grid, int range)
        {
            for (var i = -range; i <= range; i++)
            {
                for (var j = -range; j <= range; j++)
                {
                    Point p = new Point(p1.X + i, p1.Y + j);
                    if (grid.IsInGrid(p) && grid.GetAbstractGameObjectAtNewGridPosition(p.ToVector2()) is Campfire)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public Human FindClosestOpponent()
        {
            Human closestOpponent = null;
            foreach (var human in mGrid.Humans)
            {
                if (IsOpponent(human) && (closestOpponent == null || DistanceTo(human) < DistanceTo(closestOpponent)))
                {
                    closestOpponent = human;
                }
            }

            return closestOpponent;
        }

        protected ResourceObject FindClosestResource()
        {
            ResourceObject closestResource = null;
            foreach (var resource in mGrid.ResourceObjects)
            {
                if (closestResource == null || DistanceTo(resource) < DistanceTo(closestResource))
                {
                    closestResource = resource;
                }
            }

            return closestResource;
        }

        public Campfire FindClosestCampfire()
        {
            Campfire closestCampfire = null;
            foreach (var campfire in mGrid.Campfires)
            {
                if (closestCampfire == null || DistanceTo(campfire) < DistanceTo(closestCampfire))
                {
                    closestCampfire = campfire;
                }
            }

            return closestCampfire;
        }

        public Wolf FindClosestWolf()
        {
            Wolf closestWolf = null;
            foreach (var wolf in mGrid.Wolfs)
            {
                if (closestWolf == null || DistanceTo(wolf) < DistanceTo(closestWolf))
                {
                    closestWolf = wolf;
                }
            }

            return closestWolf;
        }

        private bool IsOpponent(Human human)
        {
            return (Faction == EFaction.Player && human.Faction == EFaction.Enemy) ||
                   (Faction == EFaction.Enemy && human.Faction == EFaction.Player);
        }



        public HumanInfo GetHumanInfo()
        {
            return new HumanInfo(Saturation, Heat, Health, Faction);
        }

        public AbstractEquipment GetEquipment(AbstractEquipment.EquipmentSlotType type)
        {
            switch (type)
            {
                case AbstractEquipment.EquipmentSlotType.Tool:
                    return Tool;
                case AbstractEquipment.EquipmentSlotType.Armor:
                    return Armor;
            }
            return null;
        }

        public struct HumanInfo
        {
            public readonly int mHealth;
            public readonly int mHeat;
            public readonly int mSaturation;
            public readonly string mFaction;

            public HumanInfo(int saturation, int heat, int health, EFaction faction)
            {
                mSaturation = saturation;
                mHeat = heat;
                mHealth = health;
                mFaction = faction.ToString();
            }
        }

        public void ClaimGenerator(GameTime gameTime)
        {
            if (mGameState.mKeyCounter >= 3)
            {
                mGenerator.AddProgress((float)gameTime.ElapsedGameTime.TotalMilliseconds/100.0f);
            }
        }

        public void BuildCampfire()
        {
            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    Point pos = new Point(mGridPos.X + i, mGridPos.Y + j);
                    if (mGrid.IsInGrid(pos) && mGrid.GetAbstractGameObjectAt(pos) is EmptyObject)
                    {
                        mGameState.GamePlayObjectFactory.CreateCampfire(pos.X, pos.Y, mRadiusCampfire);
                        mGameState.mEnemyResources.Decrease(ResourceType.Wood, CostCampfire);
                        return;
                    }
                }
            }
        }

        public abstract bool MissingEquipment();
    }
}


