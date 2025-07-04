using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SpaceDefence;

internal class BulletPool
{
    private Queue<Bullet> availableBullets = new Queue<Bullet>();
    private List<Bullet> activeBullets = new List<Bullet>();
    
    public void ShootBullet(Vector2 location, Vector2 direction, float speed, CollisionType collisionType)
    {
        Bullet bullet;
        if (availableBullets.Count > 0)
        {
            bullet = availableBullets.Dequeue();
            bullet.Reset(location, direction, speed, collisionType);
        }
        else
        {
            bullet = new Bullet(location, direction, speed, collisionType);
            GameManager.GetGameManager().AddGameObject(bullet);
        }
        
        activeBullets.Add(bullet);
    }
    
    public void ReturnBullet(Bullet bullet)
    {
        bullet.Active = false;
        activeBullets.Remove(bullet);
        availableBullets.Enqueue(bullet);
    }
}