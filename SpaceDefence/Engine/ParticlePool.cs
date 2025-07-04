using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using SpaceDefence;

internal class ParticlePool
    {
        private Queue<Particle> availableParticles = new Queue<Particle>();
        private List<Particle> activeParticles = new List<Particle>();
        private Random random = new Random();
        
        private void SetParticleData(Particle particle, ParticleData data, Vector2 location)
        {
            float direction = MathHelper.Lerp(data.minDirection, data.maxDirection, (float)random.NextDouble());
            particle.velocity = new Vector2((float)Math.Cos(direction), (float)Math.Sin(direction));
            particle.velocity *= MathHelper.Lerp(data.minSpeed, data.maxSpeed, (float)random.NextDouble());
            particle.scale = MathHelper.Lerp(data.minScale, data.maxScale, (float)random.NextDouble());

            particle.location = location;
            particle.acceleration = data.acceleration;
            particle.lifespan = data.lifespan;
            particle.fade = data.fade;
            particle.color = new Color(200 + random.Next(55), 40 + random.Next(180), 40 + random.Next(80), 255);
            particle.Active = true;
        }

        public void Update(GameTime gameTime)
        {
            for (int i = activeParticles.Count - 1; i >= 0; i--)
            {
                var particle = activeParticles[i];
                particle.Update(gameTime);
                
                if (particle.lifespan <= 0)
                {
                    ReturnParticle(particle);
                }
            }
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            foreach (var particle in activeParticles)
            {
                particle.Draw(gameTime, spriteBatch);
            }
        }

        public void SpawnParticle(Vector2 location, ParticleData data)
        {
            for (int i = 0; i < data.particleCount; i++)
            {
                Particle particle;
                if (availableParticles.Count > 0)
                {
                    particle = availableParticles.Dequeue();
                }
                else
                {
                    particle = new Particle();
                    particle.Load(GameManager.GetGameManager().Game.Content);
                }
            
                SetParticleData(particle, data, location);
                activeParticles.Add(particle);
            }
        }

        private void ReturnParticle(Particle particle)
        {
            particle.Active = false;
            activeParticles.Remove(particle);
            availableParticles.Enqueue(particle);
        }
    }