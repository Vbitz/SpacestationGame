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

namespace Vbitz
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class MainGame : Microsoft.Xna.Framework.Game
    {
        public const int WindowWidth = 1280;
        public const int WindowHeight = 800;

        GraphicsDeviceManager graphics;
        private SpriteBatch _Batch;

        protected EntityContainer Container;

        public Rectangle WindowSize;

        public MainGame()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            this.graphics.PreferredBackBufferWidth = WindowWidth;
            this.graphics.PreferredBackBufferHeight = WindowHeight;
            this.IsMouseVisible = true;
            //this.IsFixedTimeStep = false;

            WindowSize = new Rectangle(0, 0, WindowWidth, WindowHeight);
        }

        #region Input

        /// <summary>
        /// Enum for MouseButtons, just for convience and a diferent way of doing it compared to XNA
        /// None is for MouseIsSellecting
        /// </summary>
        public enum MouseButtons
        {
            Left,
            Middle,
            Right,
            None
        }

        // Current and last keyboard and mouse states
        private KeyboardState lastKeyboatdState;
        private MouseState lastMouseState;

        private KeyboardState currentKeyboardState;
        private MouseState currentMouseState;

        /// <summary>
        /// You should never need to call this, this is called in update to make sure input is ready to be used
        /// </summary>
        /// <param name="keyb">Basicly Keyboard.GetState()</param>
        /// <param name="mouse">Basicly Mouse.GetState()</param>
        private void UpdateInput(KeyboardState keyb, MouseState mouse)
        {
            lastKeyboatdState = this.currentKeyboardState;
            lastMouseState = this.currentMouseState;

            currentKeyboardState = keyb;
            currentMouseState = mouse;
        }

        /// <summary>
        /// Good for UI's and maybe for somekind of mouse related game. Checks if the mouse and optionly a button is being used in a rectangle.
        /// </summary>
        /// <param name="rect">The Rectangle the user should be clicking on to trigger this event</param>
        /// <param name="button">The mosue button being pressed or None for no mouse click</param>
        /// <param name="repeat">Should this repeat between frames, if this is true (which it is by default) then this will only trigger on the frame the mouse was used</param>
        /// <returns>Is the mouse cursor inside rect</returns>
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
                case MouseButtons.None:
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

        /// <summary>
        /// Gets the Mouse's location in Point form
        /// </summary>
        /// <returns>The Mouse's location in Point form</returns>
        public Point GetMouseLocation()
        {
            return new Point(currentMouseState.X, currentMouseState.Y);
        }

        /// <summary>
        /// Self explanitory, though if repeat is true (which it will be unless you specify otherwise) this will only trigger for one frame
        /// </summary>
        /// <param name="key">The Key to check</param>
        /// <param name="repeat">Should this repeat every frame while the key is being pressed or just one</param>
        /// <returns>Is key pressed</returns>
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

        /// <summary>
        /// This is pretty much around for compatabily and making offsetting slightly easier
        /// </summary>
        public Point Camera;

        private Vector2 tempCam = new Vector2(0, 0);

        /// <summary>
        /// Vector2 format for the camera, setting this will also update Camera
        /// </summary>
        public Vector2 CameraVec
        {
            get
            {
                return tempCam;
            }
            protected set
            {
                tempCam = value;
                Camera = new Point((int)tempCam.X, (int)tempCam.Y);
            }
        }

        private bool _UseCameraInput = true;

        /// <summary>
        /// Should the game allow the player to pan the camera with wsad and the arrow keys
        /// </summary>
        public bool UseCameraInput
        {
            get { return _UseCameraInput; }
            protected set { _UseCameraInput = value; }
        }


        private Rectangle _cameraBounds;

        /// <summary>
        /// This shows you what section of the screen is visible at this momment
        /// </summary>
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

        /// <summary>
        /// Can the object rect be seen right now, if this is false then you might as well not draw it
        /// </summary>
        /// <param name="rect">The object to look for</param>
        /// <returns>Can it be seen</returns>
        public bool IsSeen(Rectangle rect)
        {
            return WindowSize.Intersects(rect);
        }

        /// <summary>
        /// Offset's a point to find it's location in the world relative to the camera
        /// </summary>
        /// <param name="src">The point to offset</param>
        /// <returns>A new offset point</returns>
        public Point ScreenToWorld(Point src)
        {
            return new Point(src.X - Camera.X, src.Y - Camera.Y);
        }

        /// <summary>
        /// Moves the camera, the location is relative to the camera's current position
        /// </summary>
        public void MoveCamera(float x, float y)
        {
            CameraVec = Vector2.Add(CameraVec, new Vector2(x, y));
        }

        /// <summary>
        /// You should not need to call this function, if you want to disable moving the camera you should set UseCameraInput to false
        /// </summary>
        /// <param name="time">The gameTime passed from Update</param>
        private void UpdateCamera(GameTime time)
        {
            if (!UseCameraInput)
            {
                return;
            }

            if (IsKeyDown(Keys.W) || IsKeyDown(Keys.Up))
            {
                CameraVec = Vector2.Add(CameraVec, new Vector2(0, (float)time.ElapsedGameTime.Milliseconds / CameraSpeed));
            }

            if (IsKeyDown(Keys.S) || IsKeyDown(Keys.Down))
            {
                CameraVec = Vector2.Subtract(CameraVec, new Vector2(0, (float)time.ElapsedGameTime.Milliseconds / CameraSpeed));
            }

            if (IsKeyDown(Keys.A) || IsKeyDown(Keys.Left))
            {
                CameraVec = Vector2.Add(CameraVec, new Vector2((float)time.ElapsedGameTime.Milliseconds / CameraSpeed, 0));
            }

            if (IsKeyDown(Keys.D) || IsKeyDown(Keys.Right))
            {
                CameraVec = Vector2.Subtract(CameraVec, new Vector2((float)time.ElapsedGameTime.Milliseconds / CameraSpeed, 0));
            }
        }

        #endregion

        #region Content Loader

        private Texture2D _pixel;

        /// <summary>
        /// Just one white Pixel
        /// </summary>
        public Texture2D Pixel
        {
            get { return _pixel; }
            private set { _pixel = value; }
        }

        private Dictionary<string, Texture2D> Textures = new Dictionary<string, Texture2D>();
        private Dictionary<string, SpriteFont> Fonts = new Dictionary<string, SpriteFont>();

        /// <summary>
        /// Called by LoadAllContent, this creates Pixel
        /// </summary>
        private void CreatePlaceholderContent()
        {
            this.Pixel = new Texture2D(this.GraphicsDevice, 1, 1);
            this.Pixel.SetData<Color>(new Color[] { Color.White });
        }

        /// <summary>
        /// Loads all content from the folders "Content\Textures" and "Content\Fonts", if these don't exist or are empty this method will fail
        /// </summary>
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

        /// <summary>
        /// Get's a texture by name, if it can not be found then this will return Pixel
        /// </summary>
        /// <param name="name">The name of the texture to load without the file extention, same as Content.Load</param>
        /// <returns>A valid texture to draw with</returns>
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

        private int drawCallsThisFrame = 0;

        /// <summary>
        /// Draw's a Image at a location, it will move with the camera by default
        /// </summary>
        /// <param name="imageName">The filename for the image without an extention</param>
        /// <param name="location">The location to draw the image at, this is NOT centered in the middle of the image</param>
        /// <param name="fix">Should this image fix in one spot to not move with the camera</param>
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
            drawCallsThisFrame++;
        }

        /// <summary>
        /// Draws a rectagle using the Pixel with a color
        /// </summary>
        /// <param name="rect">The rectangle to draw</param>
        /// <param name="col">The color to draw it with</param>
        /// <param name="fix">Should this image fix in one spot to not move with the camera</param>
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
            drawCallsThisFrame++;
        }

        /// <summary>
        /// Draw's a image scaled to fit rect
        /// </summary>
        /// <param name="imageName">The filename for the image without a extention</param>
        /// <param name="rect">The rectangle to fit the image in</param>
        /// <param name="fix">Should this image fix in one spot to not move with the camera</param>
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
            drawCallsThisFrame++;
        }

        /// <summary>
        /// Draw's a image scaled to fit rect
        /// </summary>
        /// <param name="imageName">The filename for the image without a extention</param>
        /// <param name="rect">The rectangle to fit the image in</param>
        /// <param name="col">The color to tint it with</param>
        /// <param name="fix">Should this image fix in one spot to not move with the camera</param>
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
            drawCallsThisFrame++;
        }

        /// <summary>
        /// Draw's text at a location with a color, by default text will not move with the camera
        /// </summary>
        /// <param name="str">The text to draw</param>
        /// <param name="location">The location to draw it at</param>
        /// <param name="col">The color to draw the text with</param>
        /// <param name="fix">Should this text fix in one spot to not move with the camera</param>
        public void DrawString(string str, Vector2 location, Color col, bool fix = true)
        {
            DrawString("Placeholder", str, location, col, fix);
        }

        /// <summary>
        /// Draw's text at a location with a color, by default text will not move with the camera
        /// </summary>
        /// <param name="fontName">The Font to draw the text with</param>
        /// <param name="str">The text to draw</param>
        /// <param name="location">The location to draw it at</param>
        /// <param name="fix">Should this text fix in one spot to not move with the camera</param>
        public void DrawString(string fontName, string str, Vector2 location, bool fix = true)
        {
            DrawString(fontName, str, location, Color.White, false);
        }

        /// <summary>
        /// Draw's text at a location with a color, by default text will not move with the camera
        /// </summary>
        /// <param name="fontName">The Font to draw the text with</param>
        /// <param name="str">The text to draw</param>
        /// <param name="location">The location to draw it at</param>
        /// <param name="col">The color to draw the text with</param>
        /// <param name="fix">Should this text fix in one spot to not move with the camera</param>
        public void DrawString(string fontName, string str, Vector2 location, Color col, bool fix = true)
        {
            Vector2 locationFixed = !fix ? new Vector2(location.X + CameraVec.X, location.Y + CameraVec.Y) : location;
            if (Fonts.ContainsKey(fontName) == false)
            {
                _Batch.DrawString(this.Fonts["Placeholder"], str, locationFixed, col);
                drawCallsThisFrame++;
            }
            else
            {
                _Batch.DrawString(this.Fonts[fontName], str, locationFixed, col);
                drawCallsThisFrame++;
            }
        }

        #endregion

        #region FPS Counting

        private bool _ShowFPSCounter = true;

        /// <summary>
        /// Should the FPS counter be visable
        /// </summary>
        public bool ShowFPSCounter
        {
            get { return _ShowFPSCounter; }
            protected set { _ShowFPSCounter = value; }
        }

        public float CurrentFrameTime = 0;

        private int currentMS = 0;
        private int currentFrames = 0;

        private int lastFPS = 0;

        private void UpdateFPSCounter(GameTime time)
        {
            if (!ShowFPSCounter)
            {
                return;
            }

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
            if (!ShowFPSCounter)
            {
                return;
            }
            DrawImage(new Rectangle(5, 5, 200, 55), new Color(200, 200, 200, 55), true);
            DrawString("Placeholder", this.CurrentFrameTime.ToString() + "ms", new Vector2(20, 15), Color.Black, true);
            DrawString("Placeholder", ":  " + this.lastFPS + "fps", new Vector2(110, 15), Color.Black, true);
            DrawString("Placeholder", "draw calls: " + drawCallsThisFrame.ToString(), new Vector2(20, 35), Color.Black, true);
        }

        #endregion

        #region Core XNA

        private Color _drawColor = Color.HotPink;

        public Color DrawColor
        {
            get { return _drawColor; }
            protected set { _drawColor = value; }
        }


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

            Container = new EntityContainer();

            LoadAllContent();

            OnInit();

            Container.Update();

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

            Container.Update(this, null, gameTime);

            OnUpdate(gameTime);

            Container.Update();

            UpdateFPSCounter(gameTime);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(DrawColor);

            drawCallsThisFrame = 0;

            _Batch.Begin();
            
            Container.Draw(this, null);

            OnDraw(gameTime);

            Container.Update();

            DrawFPSCounter();

            _Batch.End();

            base.Draw(gameTime);
        }

        #endregion

        #region Virtual Methods

        protected virtual void OnDraw(GameTime gameTime)
        {

        }

        protected virtual void OnUpdate(GameTime gameTime)
        {

        }

        protected virtual void OnInit()
        {

        }

        #endregion
    }
}
