//***********************************************
//  Script: Game1 (Love Empire)                 *
//  Created By: Benjamin Holton                 *
//  Created On: 05FEB2018                       *
//  Copyright: Psychosis Entertainment (2018)   *
//***********************************************

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
        Button manualWorkButton;
        Button exitButton;
        Button saveButton;
        Button resetButton;

        // Using a button object for the cursor, never called
        Button mouseCursorButton;

        Texture2D mouseCursor;
        Texture2D buttonBGTex;
        Texture2D progBarBGTex;
        Texture2D progBarColorsTex;
        Texture2D workButtonTex;
        Texture2D exitBtnTex;
        Texture2D saveBtnTex;
        Texture2D resetBtnTex;

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
            this.IsMouseVisible = false;

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

            mouseCursor = Content.Load<Texture2D>(@"images/cursor");
            mouseCursorButton.AssignTexture(mouseCursor);
            
            buttonBGTex = Content.Load<Texture2D>(@"images/UpdatedButtonBG");
            buttons[0].AssignTexture(buttonBGTex);
            buttons[1].AssignTexture(buttonBGTex);
            buttons[2].AssignTexture(buttonBGTex);
            buttons[3].AssignTexture(buttonBGTex);

            progBarBGTex = Content.Load<Texture2D>(@"images/UpdatedProgressBarBG");
            progBarColorsTex = Content.Load<Texture2D>(@"images/UpdatedProgressBarFG");
            progressBars[0].AssignTexture(progBarBGTex, progBarColorsTex);
            progressBars[1].AssignTexture(progBarBGTex, progBarColorsTex);
            progressBars[2].AssignTexture(progBarBGTex, progBarColorsTex);
            progressBars[3].AssignTexture(progBarBGTex, progBarColorsTex);

            exitBtnTex = Content.Load<Texture2D>(@"images/shadedDark35");
            exitButton.AssignTexture(exitBtnTex);
            saveBtnTex = Content.Load<Texture2D>(@"images/shadedDark34");
            saveButton.AssignTexture(saveBtnTex);
            resetBtnTex = Content.Load<Texture2D>(@"images/shadedDark21");
            resetButton.AssignTexture(resetBtnTex);

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
            {
                SaveGame();
                Exit();
            }

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
                // Cycle through job buttons.
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

                // Manual Work Button
                if (manualWorkButton.Clicked(Mouse.GetState().Position.ToVector2()))
                {
                    currencyOnHand += 1;
                }

                // Exit Button
                if (exitButton.Clicked(Mouse.GetState().Position.ToVector2()))
                {
                    SaveGame();
                    Exit();
                }

                // Save Game Button
                if (saveButton.Clicked(Mouse.GetState().Position.ToVector2()))
                {
                    SaveGame();
                }

                // Reset Button
                if (resetButton.Clicked(Mouse.GetState().Position.ToVector2()))
                {
                    ResetGame();
                }
            }

            // Keep mouse cursor at mouse location
            mouseCursorButton.bounds.Location = Mouse.GetState().Position;

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
            exitButton.Draw(spriteBatch);
            saveButton.Draw(spriteBatch);
            resetButton.Draw(spriteBatch);

            DrawStrings();

            mouseCursorButton.Draw(spriteBatch);

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
            progressBars[0] = new ProgressBar(0, 100, new Rectangle(250, 175, 250, 75), null, null);
            progressBars[1] = new ProgressBar(0, 100, new Rectangle(250, 375, 250, 75), null, null);
            progressBars[2] = new ProgressBar(0, 100, new Rectangle(750, 175, 250, 75), null, null);
            progressBars[3] = new ProgressBar(0, 100, new Rectangle(750, 375, 250, 75), null, null);

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
            jobLogics[0] = new JobLogic(10f, .02f, 1, 0, 1, 0);
            jobLogics[1] = new JobLogic(100f, .04f, 5, 0, 20, 0);
            jobLogics[2] = new JobLogic(1000f, .1f, 10, 0, 400, 0);
            jobLogics[3] = new JobLogic(10000f, .5f, 20, 0, 1000, 0);

            // other Buttons
            exitButton = new Button(48, 48, 1232, 0);
            saveButton = new Button(48, 48, 1184, 0);
            resetButton = new Button(48, 48, 1232, 672);

            // Mouse Cursor (not used as a button)
            mouseCursorButton = new Button(31, 23, 0, 0);
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
            spriteBatch.DrawString(font, "Game made by Benjamin Holton\nVisual Assets" +
                " Courtesy of Kenny.nl\nCopyright 2018 of Psychosis Entertainment and Benjamin Holton",
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

        // TODO: Create Save Game Logic
        void SaveGame()
        {

        }

        // TODO: Create Reset Logic
        void ResetGame()
        {

        }
    }
}
