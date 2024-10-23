using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TheFrozenDesert.GamePlayObjects.Equipment;
using TheFrozenDesert.States;

namespace TheFrozenDesert.GamePlayObjects.GUI
{
    internal class AddNewSledgeButton : AbstractGameObjectGuiButton
    {
        private readonly GameState mGameState;
 
        public AddNewSledgeButton(GameState gameState,
            AbstractGameObject abstractGameObject,
            SpriteFont spriteFont,
            string text,
            Point size,
            Point relativePos,
            Texture2D texture) : base(gameState,
            abstractGameObject,
            size,// size of button
            relativePos,// position of button
            text,
            spriteFont,
            texture)
        {
            mGameState = gameState;

        }

        protected override void Click(GameState gameState)
        {

            //Debug.WriteLine(mText);
            if (mText == "Neues Schlittensegment")
            {
                NeuesSchlittenSegment(gameState);
            }
            else if (mText =="Kochen/Essen")
            {
                KochenEssen(gameState);
            }
            else if (mText == "Kamin aktivieren/deaktivieren")
            {
                KaminAktivierenDeaktivieren(gameState);
                //Debug.WriteLine(gameState.mKaminIsActivated);
            }
            else if (mText == "Objekt herstellen")
            {
                ObjektHerstellen(gameState);
            }
            else if (mText == "Lager ")
            {
                Lager(gameState);
            }
        }

        protected override void ClickNewSledgeStation(GameState gameState, string sledeStation)
        {
            gameState.mHasEnoughResourceForBuildingSledgeStation = "";
            if (sledeStation == "Werkbank" && ((Sledge)mAbstractGameObject).CanBuildSledgeType(gameState))

            {
                gameState.mManagers.mSoundManager.Segment_New();
                gameState.mHasEnoughResourceForBuildingSledgeStation = sledeStation + " wurde gebaut";
               ((Sledge)mAbstractGameObject).BuildSledgeType(gameState);
                ((Sledge)mAbstractGameObject).TryAttachSledgeType(gameState, gameState.GetCurrentGrid(), "Werkbank");
             
            }
            else if ((sledeStation == "Schmiede")&& ((Sledge)mAbstractGameObject).CanBuildSledgeType(gameState)) 
            {
                gameState.mManagers.mSoundManager.Segment_New();
                gameState.mHasEnoughResourceForBuildingSledgeStation = sledeStation + " wurde gebaut";
                ((Sledge)mAbstractGameObject).BuildSledgeType(gameState);
                ((Sledge)mAbstractGameObject).TryAttachSledgeType(gameState, gameState.GetCurrentGrid(), "Schmiede");
                gameState.mHasSchmiede = true;
            }
            else if ((sledeStation == "Unterkunft")&& ((Sledge)mAbstractGameObject).CanBuildSledgeType(gameState))
            {
                gameState.mManagers.mSoundManager.Segment_New();
                gameState.mHasEnoughResourceForBuildingSledgeStation = sledeStation + " wurde gebaut";
                ((Sledge)mAbstractGameObject).BuildSledgeType(gameState);
                ((Sledge)mAbstractGameObject).TryAttachSledgeType(gameState, gameState.GetCurrentGrid(), "Unterkunft");
                

            }
            else if ((sledeStation == "Küche")&& ((Sledge)mAbstractGameObject).CanBuildSledgeType(gameState))
            {
                gameState.mManagers.mSoundManager.Segment_New();
                gameState.mHasEnoughResourceForBuildingSledgeStation = sledeStation + " wurde gebaut";
                ((Sledge)mAbstractGameObject).BuildSledgeType(gameState);
                ((Sledge)mAbstractGameObject).TryAttachSledgeType(gameState, gameState.GetCurrentGrid(), "Küche");
                gameState.mHasKueche = true;

            }
            else if ((sledeStation == "Hospiz")&& ((Sledge)mAbstractGameObject).CanBuildSledgeType(gameState))
            {
                gameState.mManagers.mSoundManager.Segment_New();
                gameState.mHasEnoughResourceForBuildingSledgeStation = sledeStation + " wurde gebaut";
                ((Sledge)mAbstractGameObject).BuildSledgeType(gameState);
                ((Sledge)mAbstractGameObject).TryAttachSledgeType(gameState, gameState.GetCurrentGrid(), "Hospiz");
            }
            else if ((sledeStation == "Kamin")&& ((Sledge)mAbstractGameObject).CanBuildSledgeType(gameState)) 
            {
                gameState.mManagers.mSoundManager.Segment_New();
                gameState.mHasEnoughResourceForBuildingSledgeStation = sledeStation + " wurde gebaut";
                ((Sledge)mAbstractGameObject).BuildSledgeType(gameState);
                ((Sledge)mAbstractGameObject).TryAttachSledgeType(gameState, gameState.GetCurrentGrid(), sledeStation);
            }
            else if ((sledeStation == "Lager")&& ((Sledge)mAbstractGameObject).CanBuildSledgeType(gameState)) 
            {
                gameState.mManagers.mSoundManager.Segment_New();
                gameState.mHasEnoughResourceForBuildingSledgeStation = sledeStation + " wurde gebaut";
                ((Sledge)mAbstractGameObject).BuildSledgeType(gameState);
                ((Sledge)mAbstractGameObject).TryAttachSledgeType(gameState, gameState.GetCurrentGrid(), "Lager");

            }
            else if ((sledeStation == "Dampf\n-maschine")&&((Sledge)mAbstractGameObject).CanBuildDampfmaschine(gameState)) // can just build one dampfmaschine
            {
                gameState.mManagers.mSoundManager.Segment_New();
                //gameState.mHasEnoughResourceForBuildingSledgeStation = sledeStation + " wurde gebaut";
                ((Sledge)mAbstractGameObject).BuildDampfmaschine(gameState);
               ((Sledge)mAbstractGameObject).AttachSledgeFront(gameState, gameState.GetCurrentGrid());
               
               // Radio
               mGameState.mRadio = mGameState.GamePlayObjectFactory.CreateRadio();
               mGameState.mManagers.mSoundManager.Radio_sound();
               mGameState.mGuiOpen = null; // Schlitten gui verschwinden lassen
            }
            else if(IsSledgeTypeOtherThanDampfmaschine(sledeStation))
            {
                if (((Sledge)mAbstractGameObject).TryAttachSledge(gameState.GetCurrentGrid())){
                    gameState.mHasEnoughResourceForBuildingSledgeStation = "Braucht " + ((Sledge)mAbstractGameObject).WoodForSledgeTypeOtherThanDampfmaschine + " Holz und " +
                        ((Sledge)mAbstractGameObject).RockForSedgeTypeOtherThanDampfmaschine + " Metall für" + sledeStation;
                }
                else
                {
                    gameState.mHasEnoughResourceForBuildingSledgeStation = "Kein Platz für neues Schlittensegment";
                }
            }

        }
        protected override void ClickObjektHerstellen(GameState gameState, string equipment)
        {
         
            if (equipment == "Holzaxt")
            {
                if (gameState.AbleToBuildHolzaxt(false))// check if can build holzaxt
                {
                    BuildHolzaxt(gameState);//build it
                    gameState.mEquipment["Holzaxt"] += 1; // has one holzaxt in lager
                    gameState.mHasEnoughResourceForBuildingItems = equipment + "wurde gebaut";
                }
                else
                {
                    gameState.mHasEnoughResourceForBuildingItems = "Braucht " + (GameState.QuantityOfWoodNeededForHolzaxt) + " Holz für " + equipment;
                }
            }
            else if (equipment == "Metallaxt")
            {
                if (gameState.AbleToBuildMetallaxt(false))
                {
                    gameState.mEquipment["Metallaxt"] += 1;
                    //Debug.WriteLine(gameState.mEquipment["Metallaxt"]);
                    BuildMetallaxt(gameState);

                    gameState.mHasEnoughResourceForBuildingItems = equipment + "wurde gebaut";
                }
                else
                {
                    if (gameState.mHasSchmiede)
                    {
                        gameState.mHasEnoughResourceForBuildingItems = "Braucht " + (GameState.QuantityOfMetallNeededForMetallaxt) + " Metall und " + (GameState.QuantityHolzaxtForMetallaxt) + " Holzaxte für " + equipment;
                    }
                    else
                    {
                        gameState.mHasEnoughResourceForBuildingItems = "Braucht Schmiede für Metallequipment";
                    }
                }
            }
            else if (equipment == "Holzschwert")
            {
                if (gameState.AbleToBuildHolzschwert(false))
                {
                    
                    BuildHolzschwert(gameState);
                    gameState.mEquipment["Holzschwert"] += 1;
                    gameState.mHasEnoughResourceForBuildingItems = equipment + "wurde gebaut";
                }
                else
                {
                    gameState.mHasEnoughResourceForBuildingItems = "Braucht " + (GameState.QuantityOfWoodNeededForHolzschwert) + " Holz für " + equipment;
                }
            }
            else if (equipment == "Metallschwert")
            {
                if (gameState.AbleToBuildMetallschwert(false))
                {
                    BuildMetallschwert(gameState);
                    gameState.mEquipment["Metallschwert"] += 1;
                    gameState.mHasEnoughResourceForBuildingItems = equipment + "wurde gebaut";
                }
                else
                {
                    
                    if (gameState.mHasSchmiede)
                    {
                        gameState.mHasEnoughResourceForBuildingItems = "Braucht " + (GameState.QuantityOfMetallNeededForMetallschwert) + " Metall und " + (GameState.QuantityHolzschwertForMetallschwert) + " Holzschwert für " + equipment;
                    }
                    else
                    {
                        gameState.mHasEnoughResourceForBuildingItems = "Braucht Schmiede für Metallequipment";
                    }


                }
            }
            else if (equipment == "Holzbogen")
            {
                if (gameState.AbleToBuildHolzaxt(false))
                {
                    BuildHolzbogen(gameState);
                    gameState.mEquipment["Holzbogen"] += 1;
                    gameState.mHasEnoughResourceForBuildingItems = equipment + "wurde gebaut";
                }
                else
                {
                    gameState.mHasEnoughResourceForBuildingItems = "Braucht " + (GameState.QuantityOfWoodNeededForHolzbogen) + " Holz für " + equipment;
                }
            }
            else if (equipment == "Metallbogen")
            {
                if (gameState.AbleToBuildMetallbogen(false))
                {
                    BuildMetallbogen(gameState);
                    gameState.mEquipment["Metallbogen"] += 1;
                    gameState.mHasEnoughResourceForBuildingItems = equipment + "wurde gebaut";
                }
                else
                {
                    if (gameState.mHasSchmiede)
                    {

                        gameState.mHasEnoughResourceForBuildingItems = "Braucht " + (GameState.QuantityOfMetallNeededForMetallbogen) + " Metall und " + (GameState.QuantityHolzbogenForMetallbogen) + " Holzbogen für " + equipment;
                    }
                    else
                    {
                        gameState.mHasEnoughResourceForBuildingItems = "Braucht Schmiede für Metallequipment";
                    }
                }
            }
            else if (equipment == "Metallrüstung")
            {
                if (gameState.AbleToBuildMetallruestung(false))
                {
                    BuildMetallruestung(gameState);
                    gameState.mEquipment["Metallrüstung"] += 1;
                    gameState.mHasEnoughResourceForBuildingItems = equipment + "wurde gebaut";
                }
                else
                {
                    if (gameState.mHasSchmiede)
                    {

                        gameState.mHasEnoughResourceForBuildingItems = "Braucht " + (GameState.QuantityOfMetallNeededForMetallruestung) + " Metall für " + equipment;
                    }
                    else
                    {
                        gameState.mHasEnoughResourceForBuildingItems = "Braucht Schmiede für Metallequipment";
                    }
                }
            }
        }
        protected override string ClickLager(GameState gameState, string equipment)
        {
            if (equipment == "Holzaxt: " + gameState.mEquipment["Holzaxt"])
            {
                var gameObject = gameState.mGrid.GetAbstractGameObjectAt(gameState.mHumanPosition);
                if (gameObject is Gatherers gatherers && gameState.mEquipment["Holzaxt"] > 0)// check if human next to sledge is gatherer,
                    // if so gatherer gets a tool
                {
                    gameState.mEquipment["Holzaxt"] -= 1;//one holzaxt less in lager
                    
                    
                    Axe axe = new Axe(Axe.AxeType.Holz, false);// new axe is produced
                    gatherers.GetEquipment(axe);//
                    gatherers.Tool = new AbstractEquipment(gameState.mWeapontexture, AbstractEquipment.ItemType.Holzaxt);
                    equipment = "Holzaxt: " + gameState.mEquipment["Holzaxt"];// update for drawstring methode


                }
               

            }
            else if (equipment == "Metallaxt: " + gameState.mEquipment["Metallaxt"])
            {
                
                var gameObject = gameState.mGrid.GetAbstractGameObjectAt(gameState.mHumanPosition);
                if (gameObject is Gatherers gatherers && gameState.mEquipment["Metallaxt"] > 0)
                {
                    gameState.mEquipment["Metallaxt"] -= 1;
                    Axe axe = new Axe(Axe.AxeType.Metall, false);// new axe is produced
                    gatherers.GetEquipment(axe);//
                    gatherers.Tool = new AbstractEquipment(gameState.mWeapontexture, AbstractEquipment.ItemType.Metallaxt);
                    equipment = "Metallaxt: " + gameState.mEquipment["Metallaxt"];
                }

            }
            else if (equipment == "Holzschwert: " + gameState.mEquipment["Holzschwert"])
            {

                var gameObject = gameState.mGrid.GetAbstractGameObjectAt(gameState.mHumanPosition);
                if (gameObject is Fighter fighter && gameState.mEquipment["Holzschwert"] > 0)
                {
                    gameState.mEquipment["Holzschwert"] -= 1;
                    Sword sword = new Sword( Sword.SwordType.Holz, false);
                    fighter.GetEquipment(sword);
                    fighter.Tool = new AbstractEquipment(gameState.mWeapontexture, AbstractEquipment.ItemType.Holzschwert);
                    equipment = "Holzschwert: " + gameState.mEquipment["Holzschwert"];
                }
            }
            else if (equipment == "Metallschwert: " + gameState.mEquipment["Metallschwert"])
            {

                var gameObject = gameState.mGrid.GetAbstractGameObjectAt(gameState.mHumanPosition);
                if (gameObject is Fighter fighter && gameState.mEquipment["Metallschwert"] > 0)
                {
                    gameState.mEquipment["Metallschwert"] -= 1;
                    Sword sword = new Sword(Sword.SwordType.Metall, false);
                  
                    fighter.GetEquipment(sword);
                    fighter.Tool = new AbstractEquipment(gameState.mWeapontexture, AbstractEquipment.ItemType.Metallschwert);
                    equipment = "Metallschwert: " + gameState.mEquipment["Metallschwert"];
                }

            }
            else if (equipment == "Holzbogen: " + gameState.mEquipment["Holzbogen"])
            {
               
                   
                var gameObject = gameState.mGrid.GetAbstractGameObjectAt(gameState.mHumanPosition);
                if(gameObject is Archer archer && gameState.mEquipment["Holzbogen"]>0)
                {
                    gameState.mEquipment["Holzbogen"] -= 1;
                    Bow b = new Bow( Bow.BowType.Holz, false);
                    archer.GetEquipment(b);
                    archer.Tool = new AbstractEquipment(gameState.mWeapontexture, AbstractEquipment.ItemType.Holzbogen);
                    equipment = "Holzbogen: " + gameState.mEquipment["Holzbogen"];

                }


            }
            else if (equipment == "Metallbogen: " + gameState.mEquipment["Metallbogen"])
            {

                var gameObject = gameState.mGrid.GetAbstractGameObjectAt(gameState.mHumanPosition);
                if (gameObject is Archer archer && gameState.mEquipment["Metallbogen"] > 0)
                {
                    gameState.mEquipment["Metallbogen"] -= 1;
                    Bow b = new Bow( Bow.BowType.Metall, false);
                    archer.GetEquipment(b);
                    archer.Tool = new AbstractEquipment(gameState.mWeapontexture, AbstractEquipment.ItemType.Metallbogen);
                    equipment = "Metallbogen: " + gameState.mEquipment["Metallbogen"];
                }
            }
            else if (equipment == "Metallrüstung: " + gameState.mEquipment["Metallrüstung"])
            {

                var gameObject = gameState.mGrid.GetAbstractGameObjectAt(gameState.mHumanPosition);
                if (gameObject is Human && gameState.mEquipment["Metallrüstung"] > 0)
                {
                    Human human = (Human)gameObject;
                    gameState.mEquipment["Metallrüstung"] -= 1;
                    human.HasArmor = true;
                    human.Armor= new AbstractEquipment(gameState.mWeapontexture, AbstractEquipment.ItemType.Ruestung);
                    equipment = "Metallrüstung: " + gameState.mEquipment["Metallrüstung"];// new string for printing-> update auotmatically
                }

            }
            return equipment;
        }
        private bool IsSledgeTypeOtherThanDampfmaschine(string sledgeType)
        {
            if (sledgeType == "Werkbank"||
               sledgeType == "Schmiede" ||
               sledgeType == "Küche" ||
                sledgeType=="Lager" ||
                sledgeType == "Hospiz" ||
                sledgeType == "Kamin" ||
                sledgeType == "Unterkunft"
                )
            {
                return true;
            }
            return false;
        }

        private void NeuesSchlittenSegment(GameState gameState)
        {
            gameState.mButtonsAbstractGameObjectGui.Clear();
            gameState.mHasEnoughResourceForBuildingItems = "";
            gameState.mHasEnoughResourceForCookingFood = "";
            gameState.mButtonsAbstractGameObjectGui.Add(new AddNewSledgeButton(gameState, mAbstractGameObject, mFont, "Werkbank", new Point(126, 64), new Point(2 * gameState.mGameObjectGuiTexture.Width, 64), gameState.mGameObjectGuiTexture));
            gameState.mButtonsAbstractGameObjectGui.Add(new AddNewSledgeButton(gameState, mAbstractGameObject, mFont, "Schmiede", new Point(126, 64), new Point(2 * gameState.mGameObjectGuiTexture.Width, 128), gameState.mGameObjectGuiTexture));
            gameState.mButtonsAbstractGameObjectGui.Add(new AddNewSledgeButton(gameState, mAbstractGameObject, mFont, "Unterkunft", new Point(126, 64), new Point(2 * gameState.mGameObjectGuiTexture.Width, 192), gameState.mGameObjectGuiTexture));
            gameState.mButtonsAbstractGameObjectGui.Add(new AddNewSledgeButton(gameState, mAbstractGameObject, mFont, "Küche", new Point(126, 64), new Point(2 * gameState.mGameObjectGuiTexture.Width, 256), gameState.mGameObjectGuiTexture));
            gameState.mButtonsAbstractGameObjectGui.Add(new AddNewSledgeButton(gameState, mAbstractGameObject, mFont, "Hospiz", new Point(126, 64), new Point(2 * gameState.mGameObjectGuiTexture.Width, 320), gameState.mGameObjectGuiTexture));
            gameState.mButtonsAbstractGameObjectGui.Add(new AddNewSledgeButton(gameState, mAbstractGameObject, mFont, "Kamin", new Point(126, 64), new Point(2 * gameState.mGameObjectGuiTexture.Width, 384), gameState.mGameObjectGuiTexture));
            gameState.mButtonsAbstractGameObjectGui.Add(new AddNewSledgeButton(gameState, mAbstractGameObject, mFont, "Lager", new Point(126, 64), new Point(2 * gameState.mGameObjectGuiTexture.Width, 448), gameState.mGameObjectGuiTexture));
            gameState.mButtonsAbstractGameObjectGui.Add(new AddNewSledgeButton(gameState, mAbstractGameObject, mFont, "Dampf\n-maschine", new Point(126, 64), new Point(2 * gameState.mGameObjectGuiTexture.Width, 512), gameState.mGameObjectGuiTexture));
            
        }
        private void KochenEssen(GameState gameState)
        {
            gameState.mButtonsAbstractGameObjectGui.Clear();
            gameState.mHasEnoughResourceForBuildingItems = "";
            gameState.mHasEnoughResourceForBuildingSledgeStation = "";
            var gameObject = gameState.mGrid.GetAbstractGameObjectAt(gameState.mHumanPosition);
            if (gameObject is Human human && gameState.mResources.Get(ResourceType.Meat) >= 1 && gameState.mResources.Get(ResourceType.Wood) >= 1&& gameState.mHasKueche)
            {
                CookedFood cookedFood = new CookedFood(5, false);
                if (human.GetCookedFood(cookedFood))
                {
                    gameState.mResources.Decrease(ResourceType.Meat, 1);
                    gameState.mResources.Decrease(ResourceType.Wood, 1);
                    gameState.mHasEnoughResourceForCookingFood = "Essen wurde gekocht und gegessen";
                    gameState.mManagers.mSoundManager.Food();
                }

            }
            else
            {
                if (gameState.mHasKueche)
                {
                    gameState.mHasEnoughResourceForCookingFood = "Braucht 1 Holz und 1 Fleisch um zu kochen/essen";
                }
                else
                {
                    gameState.mHasEnoughResourceForCookingFood = "Braucht Küche um zu kochen/essen";
                }
            }
        }

        private void KaminAktivierenDeaktivieren(GameState gameState)
        {
            //Debug.WriteLine(324234234234234);
            gameState.mButtonsAbstractGameObjectGui.Clear();//for sledge menu
            gameState.mHasEnoughResourceForBuildingItems = "";//for notification field
            gameState.mHasEnoughResourceForBuildingSledgeStation = "";//for notification field
           gameState.mHasEnoughResourceForCookingFood = "";
            if (gameState.mHasKamin && gameState.mResources.Get(ResourceType.Wood) >= 1)
            {
                gameState.mKaminIsActivated = !gameState.mKaminIsActivated;
            }
        }
        private void ObjektHerstellen(GameState gameState)
        {
            gameState.mButtonsAbstractGameObjectGui.Clear();
            gameState.mHasEnoughResourceForBuildingSledgeStation = "";
            gameState.mHasEnoughResourceForCookingFood = "";
            var buttonSize = new Point(136,64);
            var buttonStarty = 64;
            gameState.mButtonsAbstractGameObjectGui.Add(new AddNewSledgeButton(gameState, mAbstractGameObject, mFont, "Holzaxt", buttonSize, new Point(2 * gameState.mGameObjectGuiTexture.Width, buttonStarty), gameState.mGameObjectGuiTexture));
            gameState.mButtonsAbstractGameObjectGui.Add(new AddNewSledgeButton(gameState, mAbstractGameObject, mFont, "Metallaxt", buttonSize, new Point(2 * gameState.mGameObjectGuiTexture.Width, buttonStarty + buttonSize.Y), gameState.mGameObjectGuiTexture));
            gameState.mButtonsAbstractGameObjectGui.Add(new AddNewSledgeButton(gameState, mAbstractGameObject, mFont, "Holzschwert", buttonSize, new Point(2 * gameState.mGameObjectGuiTexture.Width,buttonStarty + buttonSize.Y * 2), gameState.mGameObjectGuiTexture));
            gameState.mButtonsAbstractGameObjectGui.Add(new AddNewSledgeButton(gameState, mAbstractGameObject, mFont, "Metallschwert", buttonSize, new Point(2 * gameState.mGameObjectGuiTexture.Width,buttonStarty + buttonSize.Y *3), gameState.mGameObjectGuiTexture));
            gameState.mButtonsAbstractGameObjectGui.Add(new AddNewSledgeButton(gameState, mAbstractGameObject, mFont, "Holzbogen", buttonSize, new Point(2 * gameState.mGameObjectGuiTexture.Width,buttonStarty + buttonSize.Y * 4), gameState.mGameObjectGuiTexture));
            gameState.mButtonsAbstractGameObjectGui.Add(new AddNewSledgeButton(gameState, mAbstractGameObject, mFont, "Metallbogen", buttonSize, new Point(2 * gameState.mGameObjectGuiTexture.Width,buttonStarty + buttonSize.Y * 5), gameState.mGameObjectGuiTexture));
            gameState.mButtonsAbstractGameObjectGui.Add(new AddNewSledgeButton(gameState, mAbstractGameObject, mFont, "Metallrüstung", buttonSize, new Point(2 * gameState.mGameObjectGuiTexture.Width,buttonStarty + buttonSize.Y * 6), gameState.mGameObjectGuiTexture));
        }

        private void Lager(GameState gameState)
        {
            gameState.mButtonsAbstractGameObjectGui.Clear();
            gameState.mHasEnoughResourceForBuildingItems = "";
            gameState.mHasEnoughResourceForBuildingSledgeStation = "";
            gameState.mHasEnoughResourceForCookingFood = "";
            gameState.mButtonsAbstractGameObjectGui.Add(new AddNewSledgeButton(gameState, mAbstractGameObject, mFont, "Holzaxt: " + gameState.mEquipment["Holzaxt"], new Point(166, 64), new Point(2 * gameState.mGameObjectGuiTexture.Width, 64), gameState.mGameObjectGuiTexture));
            gameState.mButtonsAbstractGameObjectGui.Add(new AddNewSledgeButton(gameState, mAbstractGameObject, mFont, "Metallaxt: " + gameState.mEquipment["Metallaxt"], new Point(166, 64), new Point(2 * gameState.mGameObjectGuiTexture.Width, 128), gameState.mGameObjectGuiTexture));
            gameState.mButtonsAbstractGameObjectGui.Add(new AddNewSledgeButton(gameState, mAbstractGameObject, mFont, "Holzschwert: " + gameState.mEquipment["Holzschwert"], new Point(166, 64), new Point(2 * gameState.mGameObjectGuiTexture.Width, 192), gameState.mGameObjectGuiTexture));
            gameState.mButtonsAbstractGameObjectGui.Add(new AddNewSledgeButton(gameState, mAbstractGameObject, mFont, "Metallschwert: " + gameState.mEquipment["Metallschwert"], new Point(166, 64), new Point(2 * gameState.mGameObjectGuiTexture.Width, 256), gameState.mGameObjectGuiTexture));
            gameState.mButtonsAbstractGameObjectGui.Add(new AddNewSledgeButton(gameState, mAbstractGameObject, mFont, "Holzbogen: " + gameState.mEquipment["Holzbogen"], new Point(166, 64), new Point(2 * gameState.mGameObjectGuiTexture.Width, 320), gameState.mGameObjectGuiTexture));
            gameState.mButtonsAbstractGameObjectGui.Add(new AddNewSledgeButton(gameState, mAbstractGameObject, mFont, "Metallbogen: " + gameState.mEquipment["Metallbogen"], new Point(166, 64), new Point(2 * gameState.mGameObjectGuiTexture.Width, 384), gameState.mGameObjectGuiTexture));
            gameState.mButtonsAbstractGameObjectGui.Add(new AddNewSledgeButton(gameState, mAbstractGameObject, mFont, "Metallrüstung: " + gameState.mEquipment["Metallrüstung"], new Point(166, 64), new Point(2 * gameState.mGameObjectGuiTexture.Width, 448), gameState.mGameObjectGuiTexture));
        }
    }
}