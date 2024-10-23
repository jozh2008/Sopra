using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TheFrozenDesert.Controls;
using TheFrozenDesert.Manager;
using TheFrozenDesert.Storage.Models;

namespace TheFrozenDesert.States
{
    public class LoadState : State


    {
        private Button _button;
        private Button _buttonSaveButton;

        private SpriteFont _font;
        private bool _isSaving;

        private int _score;

        private ScoreManager _scoreManager;

        private float _timer;

        public static Random Random;
        public LoadState(Game1 game, GraphicsDevice graphicsDevice, ContentManager content) : base(game, graphicsDevice, content)
        {
            
            Random = new Random();
            _scoreManager =ScoreManager.Load();
            var buttonTexture = _content.Load<Texture2D>("Controls/knopf");
            _font = _content.Load<SpriteFont>("Fonts/Font");
            _buttonSaveButton = new Button(buttonTexture, _font)
            {
                Position = new Vector2(400, 250),
                Text = "Save"

            };

            _button = new Button(buttonTexture, _font)
            {
                Text = "Click on me"
            };
            _button.Click += Button_Click;
          
            _isSaving = false;
            _buttonSaveButton.Click += Button_Click_Save;

          

            _timer = 10;

        }

        private void Button_Click_Save(object sender, EventArgs e)
        {
            _isSaving = true;
        }

        private void Button_Click(object sender, EventArgs e)
        {
            SetButtonPosition((Button)sender);

            _score++;
        }
        private void SetButtonPosition(Button button)
        {
            var x = Random.Next(0,1000-button.Rectangle.Width);
            var y = Random.Next(0,1000-button.Rectangle.Height);
            button.Position = new Vector2(x, y);

        }
        public override void Draw(GameTime gameTime, SpriteBatch spritebatch)
        {
            //spritebatch.Begin();
            _button.Draw(gameTime,spritebatch);
            _buttonSaveButton.Draw(gameTime,spritebatch);
            spritebatch.DrawString(_font, "Score: " + _score, new Vector2(10,10), Color.Red);
            spritebatch.DrawString(_font, "Time: " + _timer.ToString("N2"), new Vector2(10, 30), Color.Red);
            spritebatch.DrawString(_font, "Highscores:\n " + string.Join("\n",_scoreManager.HighScores.Select(c => c.PlayerName + ": "+ c.Highscore).ToArray()), new Vector2(10, 60), Color.Red);


            //spritebatch.End();
        }

        public override void PostUpdate(GameTime gameTime)
        {
           
        }

        public override void Update(GameTime gameTime)
        {
            _timer -= (float) gameTime.ElapsedGameTime.TotalSeconds;

            if (_timer <= 0)
            {
                SetButtonPosition(_button);
                _scoreManager.Add(new Storage.Models.Score()
                    {
                    PlayerName = "Josh",
                    Highscore = _score
                    }
                );
                ScoreManager.Save(_scoreManager);
                _timer = 10;
                _score = 0;
            }

            if (_isSaving)
            {
                _scoreManager.Add(new Storage.Models.Score()
                    {
                        PlayerName = "Piet",
                        Highscore = _score
                    }
                );
                ScoreManager.Save(_scoreManager);
                _timer = 10;
                _score = 0;
                _isSaving = false;
            }

            _buttonSaveButton.Update(gameTime);
            _button.Update(gameTime);
        }
    }
}
