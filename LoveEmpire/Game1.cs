using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace LoveEmpire
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        enum MouseStates
        {
            Up,
            Down,
            Hold
        }

        const int NUMBER_OF_BUTTONS = 4;

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        // My stuff
        Button[] buttons = new Button[NUMBER_OF_BUTTONS];
        Texture2D buttonBGTex;
        Texture2D progBarBGTex;
        Texture2D progBarColorsTex;
        Texture2D workButtonTex;
        Button manualWorkButton;

        string[] buttonNames = new string[NUMBER_OF_BUTTONS];
        int[] jobLevels = new int[NUMBER_OF_BUTTONS];
        JobLogic[] jobLogics = new JobLogic[NUMBER_OF_BUTTONS];
        int currencyOnHand = 0;

        // My own mouse state logic
        MouseStates ms = MouseStates.Up;
        System.Diagnostics.Stopwatch gameTime = new System.Diagnostics.Stopwatch();

        SpriteFont font;
        SpriteFont bigFont;

        ProgressBar[] progressBars = new ProgressBar[NUMBER_OF_BUTTONS];

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = 1280;
            graphics.PreferredBackBufferHeight = 720;
            Content.RootDirectory = "Content";
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
            this.IsMouseVisible = true;

            InitializeThemButtons();

            gameTime.Start();

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            font = Content.Load<SpriteFont>(@"font");
            bigFont = Content.Load<SpriteFont>(@"BigFont");

            // TODO: use this.Content to load your game content here
            buttonBGTex = Content.Load<Texture2D>(@"images/ButtonBG");
            buttons[0].AssignTexture(buttonBGTex);
            buttons[1].AssignTexture(buttonBGTex);
            buttons[2].AssignTexture(buttonBGTex);
            buttons[3].AssignTexture(buttonBGTex);

            progBarBGTex = Content.Load<Texture2D>(@"images/ProgressBarBackground");
            progBarColorsTex = Content.Load<Texture2D>(@"images/ProgressBarColor");
            progressBars[0].AssignTexture(progBarBGTex, progBarColorsTex);
            progressBars[1].AssignTexture(progBarBGTex, progBarColorsTex);
            progressBars[2].AssignTexture(progBarBGTex, progBarColorsTex);
            progressBars[3].AssignTexture(progBarBGTex, progBarColorsTex);

            workButtonTex = Content.Load<Texture2D>(@"images/WorkButton");
            manualWorkButton.AssignTexture(workButtonTex);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
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
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here

            // Update Buttons
            foreach (Button b in buttons)
            {
                if (b != null)
                {
                    b.Update();
                }
            }

            // Update Progress Bars
            foreach(ProgressBar p in progressBars)
            {
                if(p != null)
                {
                    p.Update();
                }
            }

            // Update Custom Mouse State with different states other than up and down
            // I should add a timed hold function eventually so it doesn't default to hold
            if (Mouse.GetState().LeftButton == ButtonState.Pressed && ms == MouseStates.Up)
            {
                ms = MouseStates.Down;
            }
            else if(Mouse.GetState().LeftButton == ButtonState.Pressed && (ms == MouseStates.Down || ms == MouseStates.Hold))
            {
                ms = MouseStates.Hold;
            }
            else
            {
                ms = MouseStates.Up;
            }

            // Check mouse state and then buttons
            if (ms == MouseStates.Down)
            {
                // Cycle through all button objects.
                for (int i = 0; i < NUMBER_OF_BUTTONS; i++)
                {
                    // Check if the mouse cursor is inside a button
                    if (buttons[i] != null && buttons[i].Clicked(Mouse.GetState().Position.ToVector2()))
                    {
                        if (jobLogics[i].CheckLevelCost(currencyOnHand))
                        {
                            currencyOnHand = jobLogics[i].AddLevel(currencyOnHand);
                            jobLevels[i]++;
                            if(jobLogics[i].jobLevel == 1)
                            {
                                jobLogics[i].lastTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
                                jobLogics[i].nextTime = (float)gameTime.ElapsedGameTime.TotalSeconds + jobLogics[i].timeForAutoComplete;
                            }
                        }
                    }
                }

                if (manualWorkButton.Clicked(Mouse.GetState().Position.ToVector2()))
                {
                    currencyOnHand += 1;
                }
            }

            // Method for looping JobLogics
            UpdateJobLogics();

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.TransparentBlack);

            // TODO: Add your drawing code here

            spriteBatch.Begin();

            foreach (Button b in buttons)
            {
                if(b != null)
                {
                    b.Draw(spriteBatch);
                }
            }

            foreach(ProgressBar p in progressBars)
            {
                if(p != null)
                {
                    p.Draw(spriteBatch);
                }
            }

            manualWorkButton.Draw(spriteBatch);

            DrawStrings();

            spriteBatch.End();

            base.Draw(gameTime);
        }

        //************** My Methods *******************//

        // Gets the basic button stuff set-up and positioned. It was ugly, so now it is here.
        void InitializeThemButtons()
        {
            // Manuel Work Button
            manualWorkButton = new Button(200, 600, 340, 520);

            // Button Backgrounds
            buttons[0] = new Button(150, 400, 100, 100);
            buttons[1] = new Button(150, 400, 100, 300);
            buttons[2] = new Button(150, 400, 600, 100);
            buttons[3] = new Button(150, 400, 600, 300);

            // Progress bars for buttons
            // X + 135 and Y + 85 For Progress Bars
            progressBars[0] = new ProgressBar(0, 100, new Rectangle(235, 185, 250, 50), null, null);
            progressBars[1] = new ProgressBar(0, 100, new Rectangle(235, 385, 250, 50), null, null);
            progressBars[2] = new ProgressBar(0, 100, new Rectangle(735, 185, 250, 50), null, null);
            progressBars[3] = new ProgressBar(0, 100, new Rectangle(735, 385, 250, 50), null, null);

            // Names for buttons
            buttonNames[0] = "Pass Love Notes";
            buttonNames[1] = "Give Hugs";
            buttonNames[2] = "Hand Holding";
            buttonNames[3] = "Kiss On The Cheek";

            // Levels of Buttons
            jobLevels[0] = 0;
            jobLevels[1] = 0;
            jobLevels[2] = 0;
            jobLevels[3] = 0;

            // JobLogic initializers.
            jobLogics[0] = new JobLogic(10f, 1.02f, 1, 0, 1, 0);
            jobLogics[1] = new JobLogic(100f, 1.04f, 5, 0, 20, 0);
            jobLogics[2] = new JobLogic(1000f, 1.1f, 10, 0, 400, 0);
            jobLogics[3] = new JobLogic(10000f, 1.5f, 20, 0, 1000, 0);
        }

        // Redraws all strings. Called by Draw()
        void DrawStrings()
        {
            // Job title + cost texts
            spriteBatch.DrawString(font, buttonNames[0] + " (" + (int)jobLogics[0].costForLevel + ")",
                buttons[0].bounds.Location.ToVector2() + new Vector2(200, 25), Color.Black);

            spriteBatch.DrawString(font, buttonNames[1] + " (" + (int)jobLogics[1].costForLevel + ")", 
                buttons[1].bounds.Location.ToVector2() + new Vector2(200, 25), Color.Black);

            spriteBatch.DrawString(font, buttonNames[2] + " (" + (int)jobLogics[2].costForLevel + ")", 
                buttons[2].bounds.Location.ToVector2() + new Vector2(200, 25), Color.Black);

            spriteBatch.DrawString(font, buttonNames[3] + " (" + (int)jobLogics[3].costForLevel + ")", 
                buttons[3].bounds.Location.ToVector2() + new Vector2(200, 25), Color.Black);

            // Show how much money we has.
            spriteBatch.DrawString(bigFont, currencyOnHand.ToString(), manualWorkButton.bounds.Location.ToVector2()
                + new Vector2(410, 40), Color.Black);

            // Level + earning texts
            spriteBatch.DrawString(bigFont, jobLevels[0].ToString() + " (" + jobLogics[0].earnings + ")", 
                buttons[0].bounds.Location.ToVector2() + new Vector2(20, 40), Color.Black);

            spriteBatch.DrawString(bigFont, jobLevels[1].ToString() + " (" + jobLogics[1].earnings + ")", 
                buttons[1].bounds.Location.ToVector2() + new Vector2(20, 40), Color.Black);

            spriteBatch.DrawString(bigFont, jobLevels[2].ToString() + " (" + jobLogics[2].earnings + ")", 
                buttons[2].bounds.Location.ToVector2() + new Vector2(20, 40), Color.Black);

            spriteBatch.DrawString(bigFont, jobLevels[3].ToString() + " (" + jobLogics[3].earnings + ")", 
                buttons[3].bounds.Location.ToVector2() + new Vector2(20, 40), Color.Black);

            // Credits Info
            spriteBatch.DrawString(font, "Game made by Benjamin Holton\nCopyright of Psychosis Entertainment and Benjamin Holton",
                new Vector2(10, 10), Color.Red);
        }

        // Loops through JobLogics[]. Called by Update()
        void UpdateJobLogics()
        {
            for(int i = 0; i < NUMBER_OF_BUTTONS; i++)
            {
                // If the job level is greater than 0 and the target time is greater than current time...
                if(jobLogics[i].jobLevel > 0 && jobLogics[i].nextTime >= (float)(gameTime.Elapsed.TotalSeconds))
                {
                    float percentage = (jobLogics[i].nextTime - (float)gameTime.Elapsed.TotalSeconds) / jobLogics[i].timeForAutoComplete;
                    progressBars[i].GivePercentage(1 - percentage);
                }
                // else if the job is greater than 0 and target time has passed...
                else if(jobLogics[i].jobLevel > 0 && jobLogics[i].nextTime <= (float)(gameTime.Elapsed.TotalMilliseconds / 1000))
                {
                    currencyOnHand += jobLogics[i].earnings;
                    jobLogics[i].lastTime = (float)gameTime.Elapsed.TotalSeconds;
                    jobLogics[i].nextTime = (float)gameTime.Elapsed.TotalSeconds + jobLogics[i].timeForAutoComplete;
                    progressBars[i].GivePercentage(0);
                }
                // NOTE: If the job level is 0, it does nothing.
            }
        }
    }
}
