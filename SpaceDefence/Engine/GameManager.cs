using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace SpaceDefence
{
    public class GameManager
    {
        private static GameManager gameManager;

        private List<GameObject> _gameObjects;
        private List<GameObject> _toBeRemoved;
        private List<GameObject> _toBeAdded;
        private ContentManager _content;
        private Effect _teamColorEffect;
        internal readonly ParticlePool ParticlePool = new ParticlePool();
        internal readonly BulletPool BulletPool = new BulletPool();
        private SpatialGrid _spatialGrid;
        
        public Matrix WorldMatrix { get; set; }

        public Random RNG { get; private set; }
        public InputManager InputManager { get; private set; }
        public Game Game { get; private set; }

        public static GameManager GetGameManager()
        {
            if(gameManager == null)
                gameManager = new GameManager();
            return gameManager;
        }
        public GameManager()
        {
            _gameObjects = new List<GameObject>();
            _toBeRemoved = new List<GameObject>();
            _toBeAdded = new List<GameObject>();
            InputManager = new InputManager();
            RNG = new Random();
            WorldMatrix = Matrix.CreateScale(.3f);
            //WorldMatrix = Matrix.CreateScale(1f) * Matrix.CreateTranslation(0, -800, 0);
        }

        public int GetObjCount() => _gameObjects.Count;

        public void Initialize(ContentManager content, Game game)
        {
            Game = game;
            _content = content;
            
            var worldBounds = new Rectangle(0, 0, 
                (int)(game.GraphicsDevice.Viewport.Width / .3f), // .3f hardcoded because of above scale used
                (int)(game.GraphicsDevice.Viewport.Height / .3f));
            _spatialGrid = new SpatialGrid(worldBounds);
        }

        public void Load(ContentManager content)
        {
            _teamColorEffect = content.Load<Effect>("TeamColors");
            foreach (GameObject gameObject in _gameObjects)
            {
                gameObject.Load(content);
            }
        }

        public void HandleInput(InputManager inputManager)
        {
            foreach (GameObject gameObject in _gameObjects)
            {
                gameObject.HandleInput(this.InputManager);
            }
        }

        public void CheckCollision()
        {
            _spatialGrid.Clear();
            
            foreach (var obj in _gameObjects)
            {
                if (obj is Bullet bullet && !bullet.Active)
                    continue;

                _spatialGrid.Insert(obj);
            }
            
            var checkedPairs = new HashSet<(GameObject, GameObject)>();
            foreach (var obj in _gameObjects)
            {
                if (obj is Bullet bullet && !bullet.Active)
                    continue;

                var nearbyObjects = _spatialGrid.GetNearbyObjects(obj);
                foreach (var other in nearbyObjects)
                {
                    var pair1 = (obj, other);
                    var pair2 = (other, obj);
                    if (checkedPairs.Contains(pair1) || checkedPairs.Contains(pair2))
                        continue;

                    checkedPairs.Add(pair1);
                    if (!ShouldCheckCollision(obj, other))
                        continue;

                    if (obj.CheckCollision(other))
                    {
                        obj.OnCollision(other);
                        other.OnCollision(obj);
                    }
                }
            }
        }

        private bool ShouldCheckCollision(GameObject obj1, GameObject obj2)
        {
            var type1 = obj1.CollisionType;
            var type2 = obj2.CollisionType;
            if ((type1 & CollisionType.Team1) != 0 && (type2 & CollisionType.Team2) != 0) return true;
            if ((type1 & CollisionType.Team2) != 0 && (type2 & CollisionType.Team1) != 0) return true;
            if ((type1 & CollisionType.Solid) != 0 || (type2 & CollisionType.Solid) != 0) return true;
            
            return false;
        }
        
        public void Update(GameTime gameTime) 
        {
            InputManager.Update();

            // Handle input
            HandleInput(InputManager);


            // Update
            ParticlePool.Update(gameTime);
            foreach (GameObject gameObject in _gameObjects)
            {
                gameObject.Update(gameTime);
            }

            // Check Collission
            CheckCollision();

            foreach (GameObject gameObject in _toBeAdded)
            {
                gameObject.Load(_content);
                _gameObjects.Add(gameObject);
            }
            _toBeAdded.Clear();

            foreach (GameObject gameObject in _toBeRemoved)
            {
                gameObject.Destroy();
                _gameObjects.Remove(gameObject);
            }
            _toBeRemoved.Clear();
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch) 
        {
            spriteBatch.Begin(transformMatrix: WorldMatrix,effect: _teamColorEffect);
            ParticlePool.Draw(gameTime, spriteBatch);
            foreach (GameObject gameObject in _gameObjects)
            {
                gameObject.Draw(gameTime, spriteBatch);
            }
            spriteBatch.End();
        }

        /// <summary>
        /// Add a new GameObject to the GameManager. 
        /// The GameObject will be added at the start of the next Update step. 
        /// Once it is added, the GameManager will ensure all steps of the game loop will be called on the object automatically. 
        /// </summary>
        /// <param name="gameObject"> The GameObject to add. </param>
        public void AddGameObject(GameObject gameObject)
        {
            _toBeAdded.Add(gameObject);
        }

        /// <summary>
        /// Remove GameObject from the GameManager. 
        /// The GameObject will be removed at the start of the next Update step and its Destroy() mehtod will be called.
        /// After that the object will no longer receive any updates.
        /// </summary>
        /// <param name="gameObject"> The GameObject to Remove. </param>
        public void RemoveGameObject(GameObject gameObject)
        {
            _toBeRemoved.Add(gameObject);
        }

        public List<GameObject> GetGameObjects()
        {
            return _gameObjects;
        }

        /// <summary>
        /// Get a random location on the screen.
        /// </summary>
        public Vector2 RandomScreenLocation()
        {
            return new Vector2(
                RNG.Next(0, Game.GraphicsDevice.Viewport.Width),
                RNG.Next(0, Game.GraphicsDevice.Viewport.Height));
        }
    }
}
