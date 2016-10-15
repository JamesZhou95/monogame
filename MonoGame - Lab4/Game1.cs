﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Audio;
using System;

namespace MonoGame___Lab4
{
   public class Game1 : Game
   {
      #region Static Fields
      public static Random random = new Random();
      public static Model car, obs, rock;
      public static Song sFX;
      public static SoundEffect accelerateSFX;
      public static SoundEffect hit;
      public static SoundEffect stop;
      public static Matrix rotation = Matrix.CreateRotationY(MathHelper.ToRadians(180));
      #endregion
      #region Private Fields
      private GraphicsDeviceManager graphics;
      private SpriteBatch spriteBatch;
      private Camera cam;
      private Character main;
      private TexturePlane plane;
      private Texture2D ground;
      private SpriteFont font;
      private Song bgm;
      private MapData map;
      private bool win = false;
      private string text;
      #endregion
      #region Public Property
      public bool GameOver { get; set; }
      public Point startP { get; set; }
      public Point endP { get; set; }
      #endregion

      public Game1()
      {
         graphics = new GraphicsDeviceManager(this);
         Content.RootDirectory = "Content";
      }

      protected override void Initialize()
      {
         GameOver = true;
         base.Initialize();
      }

      protected override void LoadContent()
      {
         spriteBatch = new SpriteBatch(GraphicsDevice);
         ground = Content.Load<Texture2D>("Textures/snow");
         car = Content.Load<Model>("Models/car");
         obs = Content.Load<Model>("Models/bullet");
         rock = Content.Load<Model>("Models/rock");
         sFX = Content.Load<Song>("Sounds/carExplodeSFX");
         accelerateSFX = Content.Load<SoundEffect>("Sounds/carAccelerateSFX");
         hit = Content.Load<SoundEffect>("Sounds/Hit");
         stop = Content.Load<SoundEffect>("Sounds/stop");
         bgm = Content.Load<Song>("Sounds/BGM");
         font = Content.Load<SpriteFont>("Fonts/Arial");
      }

      protected override void UnloadContent()
      {

      }

      protected override void Update(GameTime gameTime)
      {
         if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

         if (Keyboard.GetState().IsKeyDown(Keys.Enter) && GameOver)
            ResetGame();

         if (!GameOver)
         {
            main.Update(gameTime);
            map.Update(gameTime);
            cam.Update(main.Position);
         }

         if(main !=null && main.Position.Z > MapData.MAPSIZE.Y - 10f)
         {
            GameOver = true;
            win = true;
         }

         if (main != null && main.Position == Vector3.Zero)
            ResetGame();

         base.Update(gameTime);
      }

      private void ResetGame()
      {
         main = new Character(this, car, new Vector3(0, 0, 5), 8f, 0.02f, rotation);
         plane = new TexturePlane(GraphicsDevice, ground, MapData.MAPSIZE, Matrix.Identity, 1);
         map = new MapData(this,"MapData", main);
         cam = new Camera(this, new Vector3(0f, 15f, 12f), Vector3.Zero, 10);
         GameOver = false;
         win = false;
         MediaPlayer.Volume = 0.6f;
         MediaPlayer.Play(bgm);
         MediaPlayer.IsRepeating = true;
         SoundEffect.MasterVolume = 0.05f;
      }

      protected override void Draw(GameTime gameTime)
      {
         GraphicsDevice.Clear(Color.WhiteSmoke);

         if (GameOver != true)
         {
            map.Draw(cam);
            main.Draw(cam);
            plane.Draw(cam);

            spriteBatch.Begin();
            spriteBatch.DrawString(font, "Distance: " + main.Position.Z.ToString().Split('.')[0] + "M", new Vector2(10, 10), Color.Black, 0f, Vector2.Zero, 0.5f, SpriteEffects.None, 0f);
            spriteBatch.DrawString(font, "Durability: " + main.Life.ToString().Split('.')[0] + "%", new Vector2(650, 10), Color.Black, 0f, Vector2.Zero, 0.5f, SpriteEffects.None, 0f);
            spriteBatch.End();

            GraphicsDevice.BlendState = BlendState.Opaque;
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            GraphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;
         }
         else
         {
            if (win == false)
               text = "     [Press 'Enter' to start the game]\n[Reach to the end to finish the game]";
            else
               text = "              Congratulations!\n[Press 'Enter' to restart the game]";
            spriteBatch.Begin();
            spriteBatch.DrawString(font, text, new Vector2(90, 200), Color.Black, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
            spriteBatch.End();
         }

         base.Draw(gameTime);
      }
   }
}
