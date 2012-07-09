using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

using System.IO;

namespace SpacestationGame
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class MainGame : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch _Batch;

        EntityContainer container;

        public Rectangle WindowSize;

        public MainGame()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            this.graphics.PreferredBackBufferWidth = 1280;
            this.graphics.PreferredBackBufferHeight = 800;
            this.IsMouseVisible = true;
            this.IsFixedTimeStep = false;

            WindowSize = new Rectangle(0, 0, 1280, 800);
        }

        #region Input

        public enum MouseButtons
        {
            Left,
            Middle,
            Right
        }

        KeyboardState lastKeyboatdState;
        MouseState lastMouseState;

        KeyboardState currentKeyboardState;
        MouseState currentMouseState;

        public void UpdateInput(KeyboardState keyb, MouseState mouse)
        {
            lastKeyboatdState = this.currentKeyboardState;
            lastMouseState = this.currentMouseState;

            currentKeyboardState = keyb;
            currentMouseState = mouse;
        }

        public bool MouseIsSellecting(Rectangle rect, MouseButtons button, bool repeat = true)
        {
            switch (button)
            {
                case MouseButtons.Left:
                    if (currentMouseState.LeftButton != ButtonState.Pressed)
                    {
                        return false;
                    }
                    break;
                case MouseButtons.Middle:
                    if (currentMouseState.MiddleButton != ButtonState.Pressed)
                    {
                        return false;
                    }
                    break;
                case MouseButtons.Right:
                    if (currentMouseState.RightButton != ButtonState.Pressed)
                    {
                        return false;
                    }
                    break;
            }
            if (!repeat)
            {
                if (currentMouseState.X == lastMouseState.X)
                {
                    return false;
                }
                if (currentMouseState.Y == lastMouseState.Y)
                {
                    return false;
                }
            }
            return rect.Contains(new Point(currentMouseState.X, currentMouseState.Y));
        }

        public Point GetMouseLocation()
        {
            return new Point(currentMouseState.X, currentMouseState.Y);
        }

        public bool IsKeyDown(Keys key, bool repeat = true)
        {
            if (!repeat)
            {
                if (currentKeyboardState.IsKeyDown(key) == lastKeyboatdState.IsKeyDown(key))
                {
                    return false;
                }
            }
            return currentKeyboardState.IsKeyDown(key);
        }

        #endregion

        #region Camera

        public Point Camera;

        private Vector2 tempCam = new Vector2(0, 0);

        public Vector2 CameraVec
        {
            get
            {
                return tempCam;
            }
            set
            {
                tempCam = value;
                Camera = new Point((int)tempCam.X, (int)tempCam.Y);
            }
        }

        private Rectangle _cameraBounds;

        public Rectangle CameraBounds
        {
            get
            {
                if (_cameraBounds.X != Camera.X)
                {
                    _cameraBounds = WindowSize;
                    _cameraBounds.Offset(-Camera.X, -Camera.Y);
                }
                return _cameraBounds;
            }
        }

        const int CameraSpeed = 5;

        public bool IsSeen(Rectangle rect)
        {
            return WindowSize.Contains(rect);
        }

        public Point ScreenToWorld(Point src)
        {
            return new Point(src.X - Camera.X, src.Y - Camera.Y);
        }

        public void MoveCamera(float x, float y)
        {
            CameraVec = Vector2.Add(CameraVec, new Vector2(x, y));
        }

        private void UpdateCamera(GameTime time)
        {
            if (IsKeyDown(Keys.W))
            {
                CameraVec = Vector2.Add(CameraVec, new Vector2(0, (float)time.ElapsedGameTime.Milliseconds / CameraSpeed));
            }

            if (IsKeyDown(Keys.S))
            {
                CameraVec = Vector2.Subtract(CameraVec, new Vector2(0, (float)time.ElapsedGameTime.Milliseconds / CameraSpeed));
            }

            if (IsKeyDown(Keys.A))
            {
                CameraVec = Vector2.Add(CameraVec, new Vector2((float)time.ElapsedGameTime.Milliseconds / CameraSpeed, 0));
            }

            if (IsKeyDown(Keys.D))
            {
                CameraVec = Vector2.Subtract(CameraVec, new Vector2((float)time.ElapsedGameTime.Milliseconds / CameraSpeed, 0));
            }
        }

        #endregion

        #region ContentLoader

        private Texture2D _pixel;

        public Texture2D Pixel
        {
            get { return _pixel; }
            private set { _pixel = value; }
        }

        private Dictionary<string, Texture2D> Textures = new Dictionary<string, Texture2D>();
        private Dictionary<string, SpriteFont> Fonts = new Dictionary<string, SpriteFont>();

        private void CreatePlaceholderContent()
        {
            this.Pixel = new Texture2D(this.GraphicsDevice, 1, 1);
            this.Pixel.SetData<Color>(new Color[] { Color.White });
        }

        private void LoadAllContent()
        {
            CreatePlaceholderContent();

            foreach (string item in Directory.GetFiles(this.Content.RootDirectory + "/Textures"))
            {
                string name = new FileInfo(item).Name.Split('.')[0];
                this.Textures.Add(name, this.Content.Load<Texture2D>("Textures\\" + name));
            }

            foreach (string item in Directory.GetFiles(this.Content.RootDirectory + "/Fonts"))
            {
                string name = new FileInfo(item).Name.Split('.')[0];
                this.Fonts.Add(name, this.Content.Load<SpriteFont>("Fonts\\" + name));
            }
        }

        public Texture2D GetTexture(string name)
        {
            if (this.Textures.ContainsKey(name))
            {
                return this.Textures[name];
            }
            else
            {
                return this.Pixel;
            }
        }

        #endregion

        #region Draw Calls

        public void DrawImage(string imageName, Vector2 location, bool fix = false)
        {
            Texture2D texture = GetTexture(imageName);
            Rectangle rect = new Rectangle((int)location.X, (int)location.Y, texture.Width, texture.Height);
            if (!fix)
            {
                rect.Offset(Camera);
            }
            if (!IsSeen(rect))
            {
                return;
            }
            _Batch.Draw(texture, rect, Color.White);
        }

        public void DrawImage(Rectangle rect, Color col, bool fix = false)
        {
            if (!fix)
            {
                rect.Offset(Camera);
            }
            if (!IsSeen(rect))
            {
                return;
            }
            _Batch.Draw(this.Pixel, rect, col);
        }

        public void DrawImage(string imageName, Rectangle rect, bool fix = false)
        {
            if (!fix)
            {
                rect.Offset(Camera);
            }
            if (!IsSeen(rect))
            {
                return;
            }
            _Batch.Draw(GetTexture(imageName), rect, Color.White);
        }

        public void DrawImage(string imageName, Rectangle rect, Color col, bool fix = false)
        {
            if (!fix)
            {
                rect.Offset(Camera);
            }
            if (!IsSeen(rect))
            {
                return;
            }
            _Batch.Draw(GetTexture(imageName), rect, col);
        }

        public void DrawString(string fontName, string str, Vector2 location, bool fix = false)
        {
            if (Fonts.ContainsKey(fontName) == false)
            {
                _Batch.DrawString(this.Fonts["Placeholder"], str, location, Color.White);
            }
            else
            {
                _Batch.DrawString(this.Fonts[fontName], str, location, Color.White);
            }
        }

        public void DrawString(string fontName, string str, Vector2 location, Color col, bool fix = false)
        {
            if (Fonts.ContainsKey(fontName) == false)
            {
                _Batch.DrawString(this.Fonts["Placeholder"], str, location, col);
            }
            else
            {
                _Batch.DrawString(this.Fonts[fontName], str, location, col);
            }
        }

        #endregion

        #region FPS Counting

        public float CurrentFrameTime = 0;

        public int currentMS = 0;
        public int currentFrames = 0;

        public int lastFPS = 0;

        private void UpdateFPSCounter(GameTime time)
        {
            this.CurrentFrameTime = time.ElapsedGameTime.Milliseconds / 1000.0f;

            this.currentFrames++;
            this.currentMS += (int)time.ElapsedGameTime.Milliseconds;

            if (currentMS > 1000)
            {
                lastFPS = currentFrames;
                currentFrames = 0;
                currentMS = 0;
            }
        }

        private void DrawFPSCounter()
        {
            DrawImage(new Rectangle(5, 5, 190, 25), new Color(0, 0, 0, 55));
            DrawString("Placeholder", this.CurrentFrameTime.ToString() + "ms", new Vector2(10, 10), Color.Black, true);
            DrawString("Placeholder", ":  " + this.lastFPS + "fps", new Vector2(100, 10), Color.Black, true);
        }

        #endregion

        #region Core XNA

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            _Batch = new SpriteBatch(GraphicsDevice);

            container = new EntityContainer();

            LoadAllContent();

            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            UpdateInput(Keyboard.GetState(), Mouse.GetState());

            UpdateCamera(gameTime);

            container.Update(this, null, gameTime);

            container.Update();

            UpdateFPSCounter(gameTime);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.HotPink);

            _Batch.Begin();
            
            container.Draw(this, null);

            container.Update();

            DrawFPSCounter();

            _Batch.End();

            base.Draw(gameTime);
        }

        #endregion
    }
}
