using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
namespace SpaceDefence
{
    public class SpaceDefence : Game
    {
        public static int XSpacing = 70;
        public static int YSpacing = 200;
        public static int ShipRows = 7;
        public static int ShipColumns = 16;
        private SpriteBatch _spriteBatch;
        private GraphicsDeviceManager _graphics;
        private GameManager _gameManager;
        
        private List<float> _fpsHistory = new List<float>();
        private List<float> _frameTimeHistory = new List<float>();
        private List<int> _entityCountHistory = new List<int>();

        public SpaceDefence()
        {
            _graphics = new GraphicsDeviceManager(this);
            _graphics.IsFullScreen = false;

            // Set the size of the screen
            _graphics.PreferredBackBufferWidth = 2000;
            _graphics.PreferredBackBufferHeight = 1200;
            
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            Exiting += Game_Exiting;
        }
        
        private void Game_Exiting(object sender, System.EventArgs e)
        {
            // !
            Console.WriteLine("Ship amount: " + _gameManager.GetGameObjects().Count(x => x is Ship));
            if (_fpsHistory.Count > 0)
            {
                float avgFps = _fpsHistory.Average();
                float minFps = _fpsHistory.Min();
                float maxFps = _fpsHistory.Max();

                System.Console.WriteLine("--- FPS Stats ---");
                System.Console.WriteLine($"Average: {avgFps:F2}");
                System.Console.WriteLine($"Low: {minFps:F2}");
                System.Console.WriteLine($"High: {maxFps:F2}");
            }

            if (_frameTimeHistory.Count > 0)
            {
                float avgFrameTime = _frameTimeHistory.Average();
                float minFrameTime = _frameTimeHistory.Min();
                float maxFrameTime = _frameTimeHistory.Max();

                System.Console.WriteLine("--- Frametime (ms) Stats ---");
                System.Console.WriteLine($"Mid: {avgFrameTime:F2}");
                System.Console.WriteLine($"Low: {minFrameTime:F2}");
                System.Console.WriteLine($"High: {maxFrameTime:F2}");
            }

            if (_entityCountHistory.Count > 0)
            {
                int totalEntities = _entityCountHistory.Max();
                System.Console.WriteLine("--- Entity Stats ---");
                System.Console.WriteLine($"Total amount of entities: {totalEntities}");
            }
        }

        protected override void Initialize()
        {
            //Initialize the GameManager
            _gameManager = GameManager.GetGameManager();
            base.Initialize();
            Random r = new Random(7);
            // Place the player at the center of the screen
            for(int i = 0; i < ShipRows; i++)
            {
                for(int j = 0;  j < ShipColumns; j++)
                {
                    Point team1Pos =  new Point(r.Next(20) + 200 + j * XSpacing * ShipRows + i * XSpacing, r.Next(20) + 200 + i * YSpacing);
                    Point team2Pos =  new Point(r.Next(20) + 200 + j * XSpacing * ShipRows + i * XSpacing, 2000 + r.Next(20) + 200 + i * YSpacing);
                    Ship player = new Ship(team1Pos, CollisionType.Team1, Color.Red);
                    Ship player2 = new Ship(team2Pos, CollisionType.Team2, Color.Blue);
                    _gameManager.AddGameObject(player);
                    _gameManager.AddGameObject(player2);
                }
            }
            // Add the starting objects to the GameManager
            _gameManager.Initialize(Content, this);
            
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _gameManager.Load(Content);
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            _gameManager.Update(gameTime);
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            _gameManager.Draw(gameTime, _spriteBatch);

            float frameTime = (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            _frameTimeHistory.Add(frameTime);

            float fps = 1.0f / (float)gameTime.ElapsedGameTime.TotalSeconds;
            _fpsHistory.Add(fps);

            int entityCount = _gameManager.GetObjCount();
            _entityCountHistory.Add(entityCount);
    
            base.Draw(gameTime);
        }



    }
}
