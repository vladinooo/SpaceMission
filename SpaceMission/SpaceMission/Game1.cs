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

namespace SpaceMission
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    /// 
    public enum GameState { Loading, Running, Won, Lost }

    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        SpriteFont statsFont;
        SpriteFont instructionsFont;
        GameState currentGameState = GameState.Loading;

        KeyboardState lastKeyboardState = new KeyboardState();
        KeyboardState currentKeyboardState = new KeyboardState();

        SpaceShip spaceship;
        Ground ground;
        Obstacle star;
        Obstacle obstacle;
        Obstacle astronaut1, astronaut2, astronaut3;

        Random rnd;

        TimeSpan roundTimer, roundTime;
        int astronautsCollected;

        // audio
        AudioEngine audioEngine;
        WaveBank waveBank;
        SoundBank soundBank;
        Cue trackCue;

        // moving obstacles limits
        int forwardCounter1 = 0;
        int backwardCounter1 = 0;
        int forwardCounter2 = 0;
        int backwardCounter2 = 0;
        int forwardCounter3 = 0;
        int backwardCounter3 = 0;
        int forwardCounter4 = 0;
        int backwardCounter4 = 0;
        int spinForwCounter = 0;
        int spinBackwCounter = 0;
        int jumpForwCounter = 0;
        int jumpBackCounter = 0;


        List<Obstacle> listOfObstacles1 = new List<Obstacle>(5);
        List<Obstacle> listOfObstacles2 = new List<Obstacle>(5);
        List<Obstacle> listOfObstacles3 = new List<Obstacle>(5);
        List<Obstacle> listOfObstacles4 = new List<Obstacle>(5);
        List<Obstacle> listOfObstacles5 = new List<Obstacle>(2);
        List<Obstacle> listOfObstacles6 = new List<Obstacle>(2);
        List<Obstacle> listOfAstronauts = new List<Obstacle>(5);
        List<Obstacle> listOfStars = new List<Obstacle>(400);


        // main camera
        Camera camera = new Camera();

        // lifebar
        Texture2D lifebar;
        Vector2 lifebarPosition = new Vector2(0, 150);

        // uhd
        Texture2D uhd;
        Vector2 uhdPosition = new Vector2(0, 0);

        // logo
        Texture2D logo;
        Vector2 logoPosition = new Vector2(20, 150);

        int hit = 0;
        public Boolean groundHit = false;

        // score
        int score = 10000;



        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            rnd = new Random();
            roundTime = GameConstants.RoundTime;

            // call to get screen resolution
            graphics.PreparingDeviceSettings += new EventHandler<PreparingDeviceSettingsEventArgs>(graphics_PreparingDeviceSettings);

            // full screen
            #if !DEBUG
            graphics.IsFullScreen = true;
            #endif
        }


        protected override void Initialize()
        {
            // initialize objects
            spaceship = new SpaceShip();
            ground = new Ground();

            base.Initialize();
        }


        /*******************************************************************************************
        * Main Load
        * *****************************************************************************************/
        protected override void LoadContent()
        {
            // load font
            spriteBatch = new SpriteBatch(GraphicsDevice);
            statsFont = Content.Load<SpriteFont>("Fonts/StatsFont");
            instructionsFont = Content.Load<SpriteFont>("Fonts/InstructionsFont");

            // load sounds and play initial sound
            audioEngine = new AudioEngine("Content/Audio/GameAudio.xgs");
            waveBank = new WaveBank(audioEngine, "Content/Audio/Wave Bank.xwb");
            soundBank = new SoundBank(audioEngine, "Content/Audio/Sound Bank.xsb");
            trackCue = soundBank.GetCue("Tracks");
            trackCue.Play();

            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // load spaceship
            spaceship.LoadContent(Content, "Models/spaceship");


            CreateObstacles1();
            CreateObstacles2();
            CreateObstacles3();
            CreateObstacles4();
            CreateObstacles5();
            CreateObstacles6();
            CreateAstronauts();
            CreateStars();

            // load ground
            ground.LoadContent(Content, "Models/ground");

            // load lifebar
            lifebar = Content.Load<Texture2D>("Textures/lifebar5");

            // load uhd
            uhd = Content.Load<Texture2D>("Textures/uhd");

            // load logo
            logo = Content.Load<Texture2D>("Textures/logo");

        }


        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }


        /*******************************************************************************************
        * Main Update
        * *****************************************************************************************/
        protected override void Update(GameTime gameTime)
        {
            currentKeyboardState = Keyboard.GetState();

            // Loading state
            if (currentGameState == GameState.Loading)
            {
                if ((lastKeyboardState.IsKeyDown(Keys.Enter)) && (currentKeyboardState.IsKeyUp(Keys.Enter)))
                {
                    roundTimer = roundTime;
                    currentGameState = GameState.Running;
                }
                // Allows the game to exit
                if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                    this.Exit();
            }

            // Running state
            if (currentGameState == GameState.Running)
            {
                // collision checks
                CheckForCollision();

                // move obstacles
                moveObstacles1();
                moveObstacles2();
                moveObstacles3();
                moveObstacles4();
                moveObstacles5();
                moveObstacles6();
                moveAstronauts();

                // update spaceship
                spaceship.Update(currentKeyboardState, camera, ground);

                // switch cameras
                if (currentKeyboardState.IsKeyDown(Keys.Space) && lastKeyboardState.IsKeyUp(Keys.Space))
                    camera.SwitchCameraMode();

                // Allows the game to exit
                if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                    this.Exit();

                // check if game is won
                if (astronautsCollected == GameConstants.numOfAstronauts)
                {
                    currentGameState = GameState.Won;
                }

                roundTimer -= gameTime.ElapsedGameTime;

                // check if game is lost
                if ((roundTimer < TimeSpan.Zero) && (astronautsCollected != GameConstants.numOfAstronauts))
                {
                    currentGameState = GameState.Lost;
                }
                if ((hit == 5) || (groundHit))
                {
                    currentGameState = GameState.Lost;
                }
            }

            // Won/Lost state
            if ((currentGameState == GameState.Won) || (currentGameState == GameState.Lost))
            {
                
                // Allows the game to exit
                if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                    this.Exit();
            }

            // for camera switch only
            lastKeyboardState = currentKeyboardState;

            base.Update(gameTime);
        }


        /*******************************************************************************************
        * Collision checks
        * *****************************************************************************************/
        private void CheckForCollision()
        {
            for (int i = 0; i < listOfObstacles1.Count; ++i)
            {
                if (spaceship.CollidesWith(listOfObstacles1[i].model, listOfObstacles1[i].GetWorld()))
                {
                    // collision!
                    //Console.WriteLine("Obstacles1 Collision!!!!");
                    listOfObstacles1.RemoveAt(i);
                    PlayCue("Explosions");
                    ++hit;
                    DecreaseLife();
                    score -= 523;
                }
            }
            for (int i = 0; i < listOfObstacles2.Count; ++i)
            {
                if (spaceship.CollidesWith(listOfObstacles2[i].model, listOfObstacles2[i].GetWorld()))
                {
                    // collision!
                    //Console.WriteLine("Obstacles2 Collision!!!!");
                    listOfObstacles2.RemoveAt(i);
                    PlayCue("Explosions");
                    ++hit;
                    DecreaseLife();
                    score -= 611;
                }
            }
            for (int i = 0; i < listOfObstacles3.Count; ++i)
            {
                if (spaceship.CollidesWith(listOfObstacles3[i].model, listOfObstacles3[i].GetWorld()))
                {
                    // collision!
                    //Console.WriteLine("Obstacles3 Collision!!!!");
                    listOfObstacles3.RemoveAt(i);
                    PlayCue("Explosions");
                    ++hit;
                    DecreaseLife();
                    score -= 413;
                }
            }
            for (int i = 0; i < listOfObstacles4.Count; ++i)
            {
                if (spaceship.CollidesWith(listOfObstacles4[i].model, listOfObstacles4[i].GetWorld()))
                {
                    // collision!
                    //Console.WriteLine("Obstacles4 Collision!!!!");
                    listOfObstacles4.RemoveAt(i);
                    PlayCue("Explosions");
                    ++hit;
                    DecreaseLife();
                    score -= 588;
                }
            }
            
            for (int i = 0; i < listOfObstacles5.Count; ++i)
            {
                if (spaceship.CollidesWith(listOfObstacles5[i].model, listOfObstacles5[i].GetWorld()))
                {
                    // collision!
                    //Console.WriteLine("Obstacles5 Collision!!!!");
                    listOfObstacles5.RemoveAt(i);
                    PlayCue("Explosions");
                    ++hit;
                    DecreaseLife();
                    score -= 721;
                }
            }
            for (int i = 0; i < listOfObstacles6.Count; ++i)
            {
                if (spaceship.CollidesWith(listOfObstacles6[i].model, listOfObstacles6[i].GetWorld()))
                {
                    // collision!
                    //Console.WriteLine("Obstacles6 Collision!!!!");
                    listOfObstacles6.RemoveAt(i);
                    PlayCue("Explosions");
                    ++hit;
                    DecreaseLife();
                    score -= 884;
                }
            }
            for (int i = 0; i < listOfAstronauts.Count; ++i)
            {
                if (spaceship.CollidesWith(listOfAstronauts[i].model, listOfAstronauts[i].GetWorld()))
                {
                    // collision!
                    //Console.WriteLine("Astronaut Collision!!!!");
                    listOfAstronauts.RemoveAt(i);
                    PlayCue("Reward");
                    ++astronautsCollected;
                    score += 333;
                }
            }

        }


        /*******************************************************************************************
        * Main Draw
        * *****************************************************************************************/
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            switch (currentGameState)
            {
                case GameState.Loading:
                    DrawSplashScreen();
                    break;

                case GameState.Running:
                    DrawGameplayScreen();
                    break;

                case GameState.Won:
                    DrawWinOrLossScreen(GameConstants.StrGameWon);
                    break;

                case GameState.Lost:
                    DrawWinOrLossScreen(GameConstants.StrGameLost);
                    break;
            };

            base.Draw(gameTime);

        }


        /*******************************************************************************************
        * Create obstacles 1, 2, 3, 4, 5, 6
        * *****************************************************************************************/
        private void CreateObstacles1()
        {
            for (int i = 0; i <= 5; ++i)
            {
                obstacle = new Obstacle();
                obstacle.SetPosition(-80, i * 30, -80);
                obstacle.LoadContent(Content, "Models/asteroid");
                listOfObstacles1.Add(obstacle);
            }
        }

        private void CreateObstacles2()
        {
            for (int i = 0; i <= 5; ++i)
            {
                obstacle = new Obstacle();
                obstacle.SetPosition(-80, i * 30, 80);
                obstacle.LoadContent(Content, "Models/asteroid");
                listOfObstacles2.Add(obstacle);
            }
        }

        private void CreateObstacles3()
        {
            for (int i = 0; i <= 5; ++i)
            {
                obstacle = new Obstacle();
                obstacle.SetPosition(-80, i * 30, -50);
                obstacle.LoadContent(Content, "Models/asteroid");
                listOfObstacles3.Add(obstacle);
            }
        }

        private void CreateObstacles4()
        {
            for (int i = 0; i <= 5; ++i)
            {
                obstacle = new Obstacle();
                obstacle.SetPosition(80, i * 30, -60);
                obstacle.LoadContent(Content, "Models/asteroid");
                listOfObstacles4.Add(obstacle);
            }
        }

        private void CreateObstacles5()
        {
            for (int i = 0; i <= 1; ++i)
            {
                obstacle = new Obstacle();
                obstacle.SetPosition(0, 150, i * 50);
                obstacle.LoadContent(Content, "Models/asteroid");
                listOfObstacles5.Add(obstacle);
            }
        }

        private void CreateObstacles6()
        {
            for (int i = 0; i <= 1; ++i)
            {
                obstacle = new Obstacle();
                obstacle.SetPosition(0, 0, i * 50);
                obstacle.LoadContent(Content, "Models/asteroid");
                listOfObstacles6.Add(obstacle);
            }
        }


        /*******************************************************************************************
        * Create astronauts
        * *****************************************************************************************/
        private void CreateAstronauts()
        {
            astronaut1 = new Obstacle();
            astronaut1.SetPosition(10, 50, 30);
            astronaut1.LoadContent(Content, "Models/astronaut");
            listOfAstronauts.Add(astronaut1);

            astronaut2 = new Obstacle();
            astronaut2.SetPosition(-60, 150, -40);
            astronaut2.LoadContent(Content, "Models/astronaut");
            listOfAstronauts.Add(astronaut2);

            astronaut3 = new Obstacle();
            astronaut3.SetPosition(20, 100, -50);
            astronaut3.LoadContent(Content, "Models/astronaut");
            listOfAstronauts.Add(astronaut3);
        }


        /*******************************************************************************************
        * Create stars
        * *****************************************************************************************/
        private void CreateStars()
        {
            for (int i = 0; i <= 400; ++i)
            {
                star = new Obstacle();
                star.SetPosition(rnd.Next(-1000, 2000), rnd.Next(-1000, 1000), rnd.Next(-1000, 3000));
                star.LoadContent(Content, "Models/star");
                listOfStars.Add(star);
            }
        }


        /*******************************************************************************************
        * Move obstacles1
        * *****************************************************************************************/
        private void moveObstacles1()
        {
            foreach (Obstacle obstacle in listOfObstacles1)
            {
                if (forwardCounter1 < 900)
                {
                    Matrix movement = obstacle.GetWorld();
                    movement = Matrix.CreateTranslation(1f, 0, 0);
                    obstacle.SetWorld(movement);
                    ++forwardCounter1;
                }

                if (forwardCounter1 == 900)
                {
                    if (backwardCounter1 < 900)
                    {
                        Matrix movement = obstacle.GetWorld();
                        movement = Matrix.CreateTranslation(-1f, 0, 0);
                        obstacle.SetWorld(movement);
                        ++backwardCounter1;
                    }
                    else
                    {
                        forwardCounter1 = 0;
                        backwardCounter1 = 0;
                    }
                }

            }
        }


        /*******************************************************************************************
        * Move obstacles2
        * *****************************************************************************************/
        private void moveObstacles2()
        {
            foreach (Obstacle obstacle in listOfObstacles2)
            {
                if (forwardCounter2 < 1800)
                {
                    Matrix movement = obstacle.GetWorld();
                    movement = Matrix.CreateTranslation(.5f, 0, 0);
                    obstacle.SetWorld(movement);
                    ++forwardCounter2;
                }

                if (forwardCounter2 == 1800)
                {
                    if (backwardCounter2 < 1800)
                    {
                        Matrix movement = obstacle.GetWorld();
                        movement = Matrix.CreateTranslation(-.5f, 0, 0);
                        obstacle.SetWorld(movement);
                        ++backwardCounter2;
                    }
                    else
                    {
                        forwardCounter2 = 0;
                        backwardCounter2 = 0;
                    }
                }

            }
        }


        /*******************************************************************************************
        * Move obstacles3
        * *****************************************************************************************/
        private void moveObstacles3()
        {
            foreach (Obstacle obstacle in listOfObstacles3)
            {
                if (forwardCounter3 < 1700)
                {
                    Matrix movement = obstacle.GetWorld();
                    movement = Matrix.CreateTranslation(0, 0, .4f);
                    obstacle.SetWorld(movement);
                    ++forwardCounter3;
                }

                if (forwardCounter3 == 1700)
                {
                    if (backwardCounter3 < 1700)
                    {
                        Matrix movement = obstacle.GetWorld();
                        movement = Matrix.CreateTranslation(0, 0, -.4f);
                        obstacle.SetWorld(movement);
                        ++backwardCounter3;
                    }
                    else
                    {
                        forwardCounter3 = 0;
                        backwardCounter3 = 0;
                    }
                }

            }
        }


        /*******************************************************************************************
        * Move obstacles4
        * *****************************************************************************************/
        private void moveObstacles4()
        {
            foreach (Obstacle obstacle in listOfObstacles4)
            {
                if (forwardCounter4 < 2200)
                {
                    Matrix movement = obstacle.GetWorld();
                    movement = Matrix.CreateTranslation(0, 0, .3f);
                    obstacle.SetWorld(movement);
                    ++forwardCounter4;
                }

                if (forwardCounter4 == 2200)
                {
                    if (backwardCounter4 < 2200)
                    {
                        Matrix movement = obstacle.GetWorld();
                        movement = Matrix.CreateTranslation(0, 0, -.3f);
                        obstacle.SetWorld(movement);
                        ++backwardCounter4;
                    }
                    else
                    {
                        forwardCounter4 = 0;
                        backwardCounter4 = 0;
                    }
                }

            }
        }


        /*******************************************************************************************
        * Move to middle obstacles
        * *****************************************************************************************/
        private void moveObstacles5()
        {
            foreach (Obstacle obstacle in listOfObstacles5)
            {
                if (spinForwCounter < 2000)
                {
                    Matrix movement = obstacle.GetWorld();
                    movement = Matrix.CreateTranslation(0, -.1f, 0);
                    movement *= Matrix.CreateRotationY(.01f);
                    obstacle.SetWorld(movement);
                    ++spinForwCounter;
                }

                if (spinForwCounter == 2000)
                {
                    if (spinBackwCounter < 2000)
                    {
                        Matrix movement = obstacle.GetWorld();
                        movement = Matrix.CreateTranslation(0, .1f, 0);
                        movement *= Matrix.CreateRotationY(-.01f);
                        obstacle.SetWorld(movement);
                        ++spinBackwCounter;
                    }
                    else
                    {
                        spinForwCounter = 0;
                        spinBackwCounter = 0;
                    }
                }

            }
        }


        /*******************************************************************************************
        * Move bottom midle obstacle
        * *****************************************************************************************/
        private void moveObstacles6()
        {
            foreach (Obstacle obstacle in listOfObstacles6)
            {
                Matrix movement = obstacle.GetWorld();
                movement = Matrix.CreateRotationY(-.01f);
                obstacle.SetWorld(movement);
            }
        }


        /*******************************************************************************************
        * Move astronauts up and down
        * *****************************************************************************************/
        private void moveAstronauts()
        {
            foreach (Obstacle astronaut in listOfAstronauts)
            {
                if (jumpForwCounter < 300)
                {
                    Matrix movement = astronaut.GetWorld();
                    movement = Matrix.CreateTranslation(0, -.08f, 0);
                    astronaut.SetWorld(movement);
                    ++jumpForwCounter;
                }

                if (jumpForwCounter == 300)
                {
                    if (jumpBackCounter < 300)
                    {
                        Matrix movement = astronaut.GetWorld();
                        movement = Matrix.CreateTranslation(0, .08f, 0);
                        astronaut.SetWorld(movement);
                        ++jumpBackCounter;
                    }
                    else
                    {
                        jumpForwCounter = 0;
                        jumpBackCounter = 0;
                    }
                }
            }
        }



        /*******************************************************************************************
        * Play sound cue
        * *****************************************************************************************/
        public void PlayCue(string cue)
        {
            soundBank.PlayCue(cue);
        }


        /*******************************************************************************************
        * Draw splash screen
        * *****************************************************************************************/
        private void DrawSplashScreen()
        {
            float xOffsetText, yOffsetText;
            Vector2 viewportSize = new Vector2(GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);
            Vector2 strCenter;

            graphics.GraphicsDevice.Clear(Color.Black);

            xOffsetText = yOffsetText = 0;
            Vector2 strInstructionsSize = instructionsFont.MeasureString(GameConstants.StrInstructions1);
            Vector2 strPosition;
            strCenter = new Vector2(strInstructionsSize.X / 2, strInstructionsSize.Y / 2);

            yOffsetText = (viewportSize.Y / 2 - strCenter.Y);
            xOffsetText = (viewportSize.X / 2 - strCenter.X);
            strPosition = new Vector2((int)xOffsetText, (int)yOffsetText);

            spriteBatch.Begin();
            spriteBatch.DrawString(instructionsFont, GameConstants.StrInstructions1, strPosition, Color.White);
            spriteBatch.End();

            // Draw logo
            spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);
            spriteBatch.Draw(logo, new Vector2((Window.ClientBounds.Width / 2) - (logo.Width / 2), 100), Color.White);
            spriteBatch.End();

            DepthStencilState dss = new DepthStencilState();
            dss.DepthBufferEnable = true;
            GraphicsDevice.DepthStencilState = dss;

        }


        /*******************************************************************************************
        * Draw game play screen
        * *****************************************************************************************/
        private void DrawGameplayScreen()
        {
            // rendering order fix
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            GraphicsDevice.BlendState = BlendState.Opaque;
            GraphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;

            // draw obstacles
            foreach (Obstacle obstacle in listOfObstacles1)
            {
                obstacle.Draw(camera);
            }
            foreach (Obstacle obstacle in listOfObstacles2)
            {
                obstacle.Draw(camera);
            }
            foreach (Obstacle obstacle in listOfObstacles3)
            {
                obstacle.Draw(camera);
            }
            foreach (Obstacle obstacle in listOfObstacles4)
            {
                obstacle.Draw(camera);
            }
            foreach (Obstacle obstacle in listOfObstacles5)
            {
                obstacle.Draw(camera);
            }
            foreach (Obstacle obstacle in listOfObstacles6)
            {
                obstacle.Draw(camera);
            }

            // draw astronauts
            foreach (Obstacle astronaut in listOfAstronauts)
            {
                astronaut.Draw(camera);
            }

            // draw stars
            foreach (Obstacle star in listOfStars)
            {
                star.Draw(camera);
            }

            // draw spaceship
            spaceship.Draw(camera);

            // draw ground
            ground.Draw(camera);

            // Draw uhd
            spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);
            spriteBatch.Draw(uhd, uhdPosition, Color.White);
            spriteBatch.End();

            // draw stats
            DrawStats();

            // Draw lifebar
            spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);
            spriteBatch.Draw(lifebar, lifebarPosition, Color.White);
            spriteBatch.End();

        }


        /*******************************************************************************************
        * Draw stats
        * *****************************************************************************************/
        private void DrawStats()
        {
            float xOffsetText, yOffsetText;
            string str1 = GameConstants.StrTimeRemaining;
            string str2 = GameConstants.StrAstronautsCollected + astronautsCollected.ToString() + " of " + GameConstants.numOfAstronauts.ToString();
            Rectangle rectSafeArea;

            // will decrease score by 1 on every clock tick
            switch ((int)roundTimer.TotalSeconds)
            { 
                case 115:
                    score -= 1;
                    break;

                case 105:
                    score -= 1;
                    break;

                case 90:
                    score -= 1;
                    break;

                case 80:
                    score -= 1;
                    break;

                case 70:
                    score -= 1;
                    break;

                case 60:
                    score -= 1;
                    break;

                case 50:
                    score -= 1;
                    break;

                case 40:
                    score -= 1;
                    break;

                case 30:
                    score -= 1;
                    break;

                case 20:
                    score -= 1;
                    break;

                case 10:
                    score -= 1;
                    break;
            }

            str1 += (roundTimer.TotalSeconds).ToString();

            

            // Calculate str1 position
            rectSafeArea = GraphicsDevice.Viewport.TitleSafeArea;

            xOffsetText = rectSafeArea.X;
            yOffsetText = rectSafeArea.Y;

            Vector2 strSize = statsFont.MeasureString(str1);
            Vector2 strPosition = new Vector2((int)xOffsetText + 45, (int)yOffsetText + 45);

            spriteBatch.Begin();
            spriteBatch.DrawString(statsFont, str1, strPosition, Color.Black);
            strPosition.Y += strSize.Y;
            spriteBatch.DrawString(statsFont, str2, strPosition, Color.Black);
            spriteBatch.End();

            DepthStencilState dss = new DepthStencilState();
            dss.DepthBufferEnable = true;
            GraphicsDevice.DepthStencilState = dss;


        }


        /*******************************************************************************************
        * Draw Win or Loose screen
        * *****************************************************************************************/
        private void DrawWinOrLossScreen(string gameResult)
        {
            float xOffsetText, yOffsetText;
            Vector2 viewportSize = new Vector2(GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);
            Vector2 strCenter;

            xOffsetText = yOffsetText = 0;
            Vector2 strResult = instructionsFont.MeasureString(gameResult);
            Vector2 strPlayAgainSize = instructionsFont.MeasureString(GameConstants.StrExit);
            Vector2 strPosition;
            strCenter = new Vector2(strResult.X / 2, strResult.Y / 2);

            yOffsetText = (viewportSize.Y / 2 - strCenter.Y);
            xOffsetText = (viewportSize.X / 2 - strCenter.X);
            strPosition = new Vector2((int)xOffsetText, (int)yOffsetText);

            spriteBatch.Begin();
            spriteBatch.DrawString(instructionsFont, gameResult + score.ToString(), strPosition, Color.Red);

            strCenter = new Vector2(strPlayAgainSize.X / 2, strPlayAgainSize.Y / 2);
            yOffsetText = (viewportSize.Y / 2 - strCenter.Y) + (float)instructionsFont.LineSpacing;
            xOffsetText = (viewportSize.X / 2 - strCenter.X);
            strPosition = new Vector2((int)xOffsetText, (int)yOffsetText);
            spriteBatch.DrawString(instructionsFont, GameConstants.StrExit, strPosition, Color.AntiqueWhite);

            spriteBatch.End();

            DepthStencilState dss = new DepthStencilState();
            dss.DepthBufferEnable = true;
            GraphicsDevice.DepthStencilState = dss;

        }


        /*******************************************************************************************
        * Decrease life
        * *****************************************************************************************/
        private void DecreaseLife()
        {
            switch (hit)
            {
                case 1:
                    lifebar = Content.Load<Texture2D>("Textures/lifebar4");
                    break;

                case 2:
                    lifebar = Content.Load<Texture2D>("Textures/lifebar3");
                    break;

                case 3:
                    lifebar = Content.Load<Texture2D>("Textures/lifebar2");
                    break;

                case 4:
                    lifebar = Content.Load<Texture2D>("Textures/lifebar1");
                    break;
            }
        }


        /*******************************************************************************************
        * Get the resolution of screen
        * *****************************************************************************************/
        void graphics_PreparingDeviceSettings(object sender, PreparingDeviceSettingsEventArgs e)
        {
            DisplayMode displayMode = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode;
            e.GraphicsDeviceInformation.PresentationParameters.BackBufferFormat = displayMode.Format;
            e.GraphicsDeviceInformation.PresentationParameters.BackBufferWidth = displayMode.Width;
            e.GraphicsDeviceInformation.PresentationParameters.BackBufferHeight = displayMode.Height;

        }
      
  
    }
}
