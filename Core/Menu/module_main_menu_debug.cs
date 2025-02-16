﻿using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace OpenVIII
{
    public static partial class Module_main_menu_debug
    {
        #region Fields

        //private static float fade, lastfade;

        private static bool LastActive = false;

        private static MainMenuStates state = 0;
        private static Vector2 vp_per;
        private static Vector2 vp;

        #endregion Fields

        #region Enums

        /// <summary>
        /// What state the menus are in.
        /// </summary>
        public enum MainMenuStates
        {
            //Init,
            MainLobby,

            DebugScreen,
            NewGameChoosed,
            LoadGameChooseSlot,
            SaveGameChooseSlot,
            LoadGameChooseGame,
            SaveGameChooseGame,
            LoadGameCheckingSlot,
            SaveGameCheckingSlot,
            LoadGameLoading,
            SaveGameSaving,
            IGM,
            IGM_Junction,
            IGM_Items,
            //BattleMenu
        }

        #endregion Enums

        #region Properties

        public static float vpSpace { get; private set; }
        private static float Fade
        {
            get => Menu.Fade; set
            {
                if(value == 0)
                    Menu.FadeIn();
            }
        }

        private static Dictionary<Enum, Item> strLoadScreen { get; set; }
        public static MainMenuStates State
        {
            get => state; set
            {
                // Draw will call before the next update(). This prevents that.
                Memory.SuppressDraw = true;
                state = value;
            }
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Trigger required draw function.
        /// </summary>
        public static void Draw()
        {
            Memory.graphics.GraphicsDevice.Clear(Color.Black);
            //lastfade = fade;
            switch (State)
            {
                //case MainMenuStates.Init:
                case MainMenuStates.MainLobby:
                    //DrawMainLobby();
                    Menu.IGM_Lobby.Draw();
                    break;

                case MainMenuStates.DebugScreen:
                    DrawDebugLobby();
                    break;

                //case MainMenuStates.NewGameChoosed:
                //    DrawMainLobby();
                //    break;

                case MainMenuStates.LoadGameLoading:
                case MainMenuStates.LoadGameChooseSlot:
                case MainMenuStates.LoadGameCheckingSlot:
                case MainMenuStates.LoadGameChooseGame:
                case MainMenuStates.SaveGameChooseSlot:
                case MainMenuStates.SaveGameCheckingSlot:
                case MainMenuStates.SaveGameChooseGame:
                case MainMenuStates.SaveGameSaving:
                    DrawLGSG();
                    break;

                case MainMenuStates.IGM:
                    Menu.IGM.Draw();
                    break;

                case MainMenuStates.IGM_Junction:
                    Menu.IGM_Junction.Draw();
                    break;

                case MainMenuStates.IGM_Items:
                    Menu.IGM_Items.Draw();
                    break;
                    //case MainMenuStates.BattleMenu:
                    //    Menu.BattleMenus.Draw();
                    //    break;
            }
        }

        public static Slide<Vector2> OffsetSlide = new Slide<Vector2>(new Vector2(-1000, 0), Vector2.Zero, 1000, Vector2.SmoothStep);
        //public static Slide<float> BlinkSlide = new Slide<float>(1f, 0f, 300d, MathHelper.Lerp);
        //public static Slide<float> FadeSlide = new Slide<float>(0f, 1f, 500d, MathHelper.Lerp);

        /// <summary>
        /// Triggers functions depending on state
        /// </summary>
        public static void Update()
        {
            //if (blinkstate)
            //    blink_Amount += Memory.gameTime.ElapsedGameTime.Milliseconds / 2000.0f * 3;
            //else
            //    blink_Amount -= Memory.gameTime.ElapsedGameTime.Milliseconds / 2000.0f * 3;
            //Blink_Amount = BlinkSlide.Update();
            //if (BlinkSlide.Done)
            //{
            //    BlinkSlide.Reverse();
            //    BlinkSlide.Restart();
            //}

            lastscale = scale;
            scale = Memory.Scale();
#pragma warning disable CS0219 // Variable is assigned but its value is never used
            bool forceupdate = false;
#pragma warning restore CS0219 // Variable is assigned but its value is never used

            if (LastActive != Memory.IsActive)
            {
                forceupdate = true;
                LastActive = Memory.IsActive;
            }
            if (lastscale != scale)
            {
                forceupdate = true;
            }

            //if (Fade == 0) FadeSlide.Restart();
            //if (!FadeSlide.Done && State != MainMenuStates.NewGameChoosed)
            //{
            //    //Fade += (float)(Memory.gameTime.ElapsedGameTime.TotalMilliseconds / 1000.0f);
            //    Fade = FadeSlide.Update();
            //}

            vp_per.X = Memory.PreferredViewportWidth;//Memory.graphics.GraphicsDevice.Viewport.Width;
            vp_per.Y = Memory.PreferredViewportHeight;//Memory.graphics.GraphicsDevice.Viewport.Width;
            vp_per = new Vector2(Memory.PreferredViewportWidth, Memory.PreferredViewportHeight);
            vp = new Vector2(Memory.graphics.GraphicsDevice.Viewport.Width, Memory.graphics.GraphicsDevice.Viewport.Height);

            vpSpace = vp_per.Y * 0.09f;
            DFontPos = new Vector2(vp_per.X * .10f, vp_per.Y * .05f) + Offset;

            IGM_focus = Matrix.CreateTranslation((vp_per.X / -2), (vp_per.Y / -2), 0) *
                Matrix.CreateScale(new Vector3(scale.X, scale.Y, 1)) *
                Matrix.CreateTranslation(vp.X / 2, vp.Y / 2, 0);

            ml = InputMouse.Location.Transform(IGM_focus);

            switch (State)
            {
                //case MainMenuStates.Init:
                //    State++;
                //    Init();
                //    break;

                case MainMenuStates.MainLobby:
                    Memory.IsMouseVisible = true;
                    OffsetSlide.Restart();
                    //if (UpdateMainLobby() || (lastfade != fade))
                    //{
                    //    forceupdate = true;
                    //}

                    Menu.IGM_Lobby.Update();
                    break;

                case MainMenuStates.DebugScreen:
                    //Menu.UpdateFade();
                    Memory.IsMouseVisible = true;
                    if (UpdateDebugLobby() || /*(lastfade != fade) ||*/ Offset != Vector2.Zero)
                    {
                        forceupdate = true;
                    }
                    if (!OffsetSlide.Done)
                    {
                        Offset = OffsetSlide.Update();
                    }

                    break;

                //case MainMenuStates.NewGameChoosed:
                //    Memory.IsMouseVisible = false;
                //    UpdateNewGame();
                //    break;

                case MainMenuStates.LoadGameChooseSlot:
                case MainMenuStates.LoadGameCheckingSlot:
                case MainMenuStates.LoadGameChooseGame:
                case MainMenuStates.LoadGameLoading:
                case MainMenuStates.SaveGameChooseSlot:
                case MainMenuStates.SaveGameCheckingSlot:
                case MainMenuStates.SaveGameChooseGame:
                case MainMenuStates.SaveGameSaving:
                    //Menu.UpdateFade();
                    UpdateLGSG();
                    break;

                case MainMenuStates.IGM:
                    Memory.IsMouseVisible = true;
                    Menu.IGM.Update();
                    break;

                case MainMenuStates.IGM_Junction:
                    Memory.IsMouseVisible = true;
                    Menu.IGM_Junction.Update();
                    break;

                case MainMenuStates.IGM_Items:
                    Memory.IsMouseVisible = true;
                    Menu.IGM_Items.Update();
                    break;

                //case MainMenuStates.BattleMenu:
                //    Menu.BattleMenus.Update();
                //    break;
                default:
                    goto case 0;
            }
            //disabled because if you resize the window the next update call undoes this before drawing happens.
            //need a way to detect if drawing has happened before suppressing draw again.
            //if(forceupdate)
        }

        /// <summary>
        /// Init
        /// </summary>
        public static void Init()
        {
            //InitMain();
            //Menu.IGM_Lobby = new IGM_Lobby();
            InitLoad();
            InitDebug();

            //IGM = new IGM();
            //IGM_Junction = new IGM_Junction();
            //IGM_Items = new IGM_Items();
        }

        #endregion Methods
    }
}