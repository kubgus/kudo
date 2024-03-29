﻿using KudoEngine;

// This file's code is effortless and is
// just for testing purposes
// Don't copy this to your own game
namespace KudoGame
{
    internal class OldDemo_Platformer : Kudo
    {
        public OldDemo_Platformer() : base(new Vector2(1080, 607), "Kudo Test Demo") { }

        bool left;
        bool right;
        bool up;
        bool down;

        static Vector2 gridSize = new(16, 9);

        public float DefaultMovementSpeed = 8f;

        string[,] Map = new string[10, 60];

        Sprite2D player;
        BoxCollider2D playerCollider;
        BoxCollider2D playerGroundCollider;
        Physics2D playerPhysics;
        Sprite2D eoc;
        BoxCollider2D eocCollider;

        float playerScaleY;

        Camera cam1;

        List<Sprite2D> coins = new();

        SpriteSheet s = new(BitmapFromFile("Sprites/2T"));
        SpriteSheet ad = new(BitmapFromFile("Sprites/ad"));


        int eocVelocity = -1;
        int eocSpeed = 1;

        public bool isGrounded(BoxCollider2D playerGroundCollider)
        {
            if (playerGroundCollider.IsColliding(new List<string>() { "tiles" }))
            {
                return true;
            }
            return false;
        }

        public void MapGen()
        {
            Random rnd = new Random();
            int height = rnd.Next(2, 6);
            int lastTree = 0;
            for (int i = 0; i < Map.GetLength(1); i++)
            {
                for (int j = 0; j <= height; j++)
                {
                    if (0 <= Map.GetLength(0) - 1 - j)
                    {
                        Map[Map.GetLength(0) - 1 - j, i] = "g";
                    }
                }

                if (rnd.Next(0, 9) == 0 && Map.GetLength(0) - 2 - height >= 0)
                {
                    Map[Map.GetLength(0) - 2 - height, i] = "b";
                }
                if (rnd.Next(0, 9) == 0 && Map.GetLength(0) - 2 - height >= 0)
                {
                    Map[Map.GetLength(0) - 2 - height, i] = "c";
                }
                if (rnd.Next(0, 4) == 0
                    && lastTree - i < -3
                    && Map.GetLength(0) - 5 - height >= 0
                    && Map.GetLength(1) - 1 > i + 1
                    && 0 <= i - 1)
                {
                    Map[Map.GetLength(0) - 2 - height, i] = "p";
                    Map[Map.GetLength(0) - 3 - height, i] = "p";
                    Map[Map.GetLength(0) - 4 - height, i + 1] = "g";
                    Map[Map.GetLength(0) - 4 - height, i - 1] = "g";
                    Map[Map.GetLength(0) - 4 - height, i] = "g";
                    Map[Map.GetLength(0) - 5 - height, i] = "g";

                    lastTree = i;
                }
                if (height > 0)
                {
                    if (rnd.Next(0, 2) == 0)
                    {
                        height += rnd.Next(-1, 2);
                    }
                }
                else
                {
                    height++;
                }

            }
        }

        static readonly Vector2 tileSize = new(ScreenSize.X / gridSize.X, ScreenSize.Y / gridSize.Y);

        public void MapRender()
        {
            for (int i = 0; i < Map.GetLength(1); i++)
            {
                for (int j = 0; j < Map.GetLength(0); j++)
                {
                    if (Map[j, i] == "g")
                    {
                        _ = new BoxCollider2D(new Shape2D(new((i * tileSize.X),(j * tileSize.Y)), tileSize, Color.DarkGreen, default, "Ground"), "tiles");
                    }
                    else if (Map[j, i] == "p")
                    {
                        _ = new BoxCollider2D(new Sprite2D(new((i * tileSize.X),(j * tileSize.Y)), tileSize, BitmapFromFile("Sprites/plank"), default, "Wood"), "tiles");
                    }
                    else if (Map[j, i] == "c")
                    {
                        _ = new BoxCollider2D(new Sprite2D(new((i * tileSize.X),(j * tileSize.Y)), tileSize, s.GetSprite("coin"), default, "Coin"), "collectible");
                    }
                    else if (Map[j, i] == "b")
                    {
                        new BoxCollider2D(new Sprite2D(new(i * ScreenSize.X / gridSize.X, j * ScreenSize.Y / gridSize.Y), new(ScreenSize.X / gridSize.X, ScreenSize.Y / gridSize.Y), BitmapFromFile("Sprites/bush"), default, "Bush", 1), "bushes");
                    }
                }
            }
        }

        public override void Load()
        {
            Skybox = Color.Aqua;

            cam1 = new(new());

            ActiveCamera = cam1;

            Shape2D someButton = new(new(10, 10), new(50, 50), Color.Red, default, "button", 1000);

            s.AddSprite("coin", new(new(225, 385), new(35, 30)));

            ad.AddSprite("idle", new(new(5, 11), new(18, 20)));
            for (int i = 0; i < 8; i++)
            {
                ad.AddSprite("walk" + i, new(new(5 + 32 * i, 41), new(18, 20)));
            }

            MapGen();
            MapRender();

            eoc = new(new(550, 200), new(400, 200), BitmapFromFile("Sprites/eoc"), default, "Boss");
            eocCollider = new(eoc, "bosses", new(-30f, -30f));
            player = new(new(150, 150), new(70, 100), ad.GetSprite("idle"), default, "Player");
            playerCollider = new(player, "player", new(-17.5f, -3f), new(-1f,0f));
            playerPhysics = new Physics2D(playerCollider, new List<string>() { "tiles" });
            playerPhysics.Weight = 10f;
            playerGroundCollider = new(player, "Ground Check", new(5f, 0f), new(0f, 1f));

            playerScaleY = player.Scale.Y;
        }

        public override void Draw()
        {
            playerPhysics.Update();
        }

        int animation = 0;

        public override void Update()
        {
            float MovementSpeed = DefaultMovementSpeed;

            cam1.Position = player.Center();

            if (playerCollider.IsColliding(new List<string>() { "bushes" }))
            {
                MovementSpeed /= 5f;
            }

            if (Input.IsKeyDown(Keys.Up) && isGrounded(playerGroundCollider))
            {
                playerPhysics.Velocity.Y = -MovementSpeed * 2;
            }
            if (down)
            {
                player.Position.Y += MovementSpeed;
            }
            if (left)
            {
                if (Frame % 5 == 0)
                {
                    animation++;
                    if (animation > 7)
                    {
                        animation = 0;
                    }
                    player.Sprite = ad.GetSprite("walk" + animation);
                }
                playerPhysics.Velocity.X = -MovementSpeed;
            }
            if (right)
            {
                if (Frame % 5 == 0)
                {
                    animation++;
                    if (animation > 7)
                    {
                        animation = 0;
                    }
                    player.Sprite = ad.GetSprite("walk" + animation);
                }
                playerPhysics.Velocity.X = MovementSpeed;
            }

            if (eoc.Position.X <= 300)
            {
                eocVelocity = 1;
            }
            else if (eoc.Position.X >= 600)
            {
                eocVelocity = -1;
            }

            var a = playerCollider.GetCollisions("collectible");
            foreach (var o in a)
            {
                if (o.Rendered.IsAlive)
                {
                    o.Rendered.Kill();
                }
            }

            eoc.Position.X += eocSpeed * eocVelocity;

            if (playerCollider.IsColliding(eocCollider))
            {
                playerCollider.Rendered.Kill();
                Skybox = Color.DarkRed;
            }
        }

        public override void KeyDown(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Up)
            {
                up = true;
            }
            if (e.KeyCode == Keys.Left)
            {
                left = true;
            }
            if (e.KeyCode == Keys.Right)
            {
                right = true;
            }
            if (e.KeyCode == Keys.V)
            {
                ActiveCamera.Rotation += 1;
            }
            if (e.KeyCode == Keys.C)
            {
                ActiveCamera.Rotation -= 1;
            }
            if (e.KeyCode == Keys.G)
            {
                ActiveCamera.Zoom += 0.01f;
            }
            if (e.KeyCode == Keys.F)
            {
                ActiveCamera.Zoom -= 0.01f;
            }
            if (e.KeyCode == Keys.LControlKey)
            {
                playerCollider.ScaleModifier.Y = player.Scale.Y / 2f;
                playerCollider.PositionModifier.Y -= player.Scale.Y / 2f;

            }
        }

        public override void KeyUp(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Up)
            {
                up = false;
            }
            if (e.KeyCode == Keys.Down)
            {
                down = false;
            }
            if (e.KeyCode == Keys.Left)
            {
                animation = 0;
                player.Sprite = ad.GetSprite("idle");
                left = false;
            }
            if (e.KeyCode == Keys.Right)
            {
                animation = 0;
                player.Sprite = ad.GetSprite("idle");
                right = false;
            }
            if (e.KeyCode == Keys.LControlKey)
            {
                playerCollider.PositionModifier.Y += player.Scale.Y / 2f;
                playerCollider.ScaleModifier.Y = playerScaleY;

            }
        }
    }
}
