﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using OpenVIII.Encoding.Tags;
using System;
using System.IO;
using System.Linq;

namespace OpenVIII
{
    public static class Module_overture_debug
    {
        #region Fields

        private const float speed = 1f;
        private static bool bFadingIn = true;
        private static bool bNames = true;

        //by default first should fade in, wait, then fire fading out and wait for finish, then loop
        private static bool bWaitingSplash, bFadingOut;

        private static float fSplashWait, Fade;
        private static OverturepublicModule publicModule = OverturepublicModule._4Squaresoft;
        private static double publicTimer;
        private static int splashIndex, splashName = 1, splashLoop = 1;
        private static Splash splashTex = null;

        #endregion Fields

        #region Enums

        private enum OverturepublicModule
        {
            _0InitSound,
            _1WaitBeforeFirst,
            _2PlaySequence,
            _3SequenceFinishedPlayMainMenu,
            _4Squaresoft
        }

        #endregion Enums

        #region Properties

        private static float Fadespd1 => (float)(Memory.gameTime.ElapsedGameTime.TotalMilliseconds / 500f) * speed;
        private static float Fadespd2 => (float)Fadespd5;
        private static float Fadespd3 => (float)(Memory.gameTime.ElapsedGameTime.TotalMilliseconds / 5000.0f) * speed;
        private static float Fadespd4 => (float)(Memory.gameTime.ElapsedGameTime.TotalMilliseconds / 2000.0f) * speed;
        private static double Fadespd5 => (Memory.gameTime.ElapsedGameTime.TotalMilliseconds / 1000.0d) * speed;

        #endregion Properties

        #region Methods

        public static void Draw()
        {
            switch (publicModule)
            {
                case OverturepublicModule._0InitSound:
                case OverturepublicModule._1WaitBeforeFirst:
                    Memory.graphics.GraphicsDevice.Clear(Color.Black);
                    break;

                case OverturepublicModule._2PlaySequence:
                    Memory.graphics.GraphicsDevice.Clear(Color.Black);
                    DrawSplash();
                    break; //actually this is our entry point for draw;
                case OverturepublicModule._3SequenceFinishedPlayMainMenu:
                    DrawLogo(); //after this ends, jump into main menu module
                    break;

                case OverturepublicModule._4Squaresoft:
                    publicModule = OverturepublicModule._0InitSound;
                    Module_movie_test.Index = 103;//103;
                    Module_movie_test.ReturnState = MODULE.OVERTURE_DEBUG;
                    Memory.module = MODULE.MOVIETEST;
                    break;
            }
        }

        public static void ResetModule()
        {
            publicModule = 0;
            publicTimer = 0.0f;
            bFadingIn = true;
            bWaitingSplash = false;
            fSplashWait = 0.0f;
            bFadingOut = false;
            Fade = 0;
            splashIndex = 0;
            splashName = splashLoop = 1;
            Memory.spriteBatch.GraphicsDevice.Clear(Color.Black);
            Memory.module = MODULE.OVERTURE_DEBUG;
            publicModule = OverturepublicModule._4Squaresoft;
            Module_movie_test.ReturnState = MODULE.OVERTURE_DEBUG;
        }

        public static void SplashUpdate(ref int _splashIndex)
        {
            if (splashTex == null)
            {
                ReadSplash();
            }

            if (bFadingIn)
            {
                Fade += Fadespd1;
                if (Fade > 1.0f)
                {
                    Fade = 1.0f;
                    bFadingIn = false;
                    bWaitingSplash = true;
                }
            }
            if (bFadingOut)
            {
                if (splashLoop + 1 >= 0x0F && splashName >= 0x0F)
                {
                    bFadingIn = false;
                    bFadingOut = true;
                    bWaitingSplash = false;
                    publicTimer = 0.0f;
                    Fade = 1.0f;
                    publicModule++;
                    return;
                }
                Fade -= Fadespd1;
                if (Fade < 0.0f)
                {
                    bFadingIn = true;
                    bFadingOut = false;
                    Fade = 0.0f;
                    _splashIndex++;
                    if (bNames)
                    {
                        splashName++;
                    }
                    else
                    {
                        splashLoop++;
                    }

                    if (_splashIndex > 1)
                    {
                        bNames = !bNames;
                        _splashIndex = 0;
                    }

                    ReadSplash();
                }
            }
            if (bWaitingSplash)
            {
                if (bNames)
                {
                    if (fSplashWait > 4.8f)
                    {
                        bWaitingSplash = false;
                        bFadingOut = true;
                        fSplashWait = 0.0f;
                    }
                }
                else
                {
                    if (fSplashWait > 6.5f)
                    {
                        bWaitingSplash = false;
                        bFadingOut = true;
                        fSplashWait = 0.0f;
                    }
                }
                Memory.SuppressDraw = true;
                fSplashWait += Fadespd2;
            }
            //loop 01-14 + name01-14;
        }

        public static void Update()
        {
            if (Input2.DelayedButton(FF8TextTagKey.Confirm) || Input2.DelayedButton(FF8TextTagKey.Cancel) || Input2.DelayedButton(Keys.Space))
            {
                init_debugger_Audio.StopMusic();
                Memory.module = MODULE.MAINMENU_DEBUG;
            }
            switch (publicModule)
            {
                case OverturepublicModule._0InitSound:
                    InitSound();
                    break;

                case OverturepublicModule._1WaitBeforeFirst:
                    Memory.SuppressDraw = true;
                    WaitForFirst();
                    break;

                case OverturepublicModule._2PlaySequence:
                    SplashUpdate(ref splashIndex);
                    break;
            }
        }

        private static void DrawLogo()
        {
            if (splashTex != null)
            {
                if (!bWaitingSplash)
                {
                    Memory.graphics.GraphicsDevice.Clear(Color.White);
                    Memory.SpriteBatchStartAlpha(ss: SamplerState.AnisotropicClamp);
                }
                else
                {
                    Memory.SpriteBatchStartStencil(ss: SamplerState.AnisotropicClamp);
                }
                Memory.spriteBatch.Draw(splashTex,
                    new Rectangle(0, 0, Memory.graphics.GraphicsDevice.Viewport.Width, Memory.graphics.GraphicsDevice.Viewport.Height),
                    new Rectangle(0, 0, splashTex.Width, splashTex.Height)
                    , Color.White * Fade);
                if (bFadingIn)
                {
                    Fade += Fadespd3;
                }

                if (bFadingOut)
                {
                    Fade -= Fadespd4;
                }

                if (Fade < 0.0f)
                {
                    bFadingIn = true;
                    ReadSplash(true);
                    bFadingOut = false;
                }
                if (bFadingIn && Fade > 1.0f && !bWaitingSplash)
                {
                    publicTimer += Fadespd5;
                }

                if (publicTimer > 5.0f)
                {
                    bWaitingSplash = true;
                    bFadingOut = true;
                }
                Memory.SpriteBatchEnd();
                if (bWaitingSplash && Fade < 0.0f)
                {
                    Memory.module = MODULE.MAINMENU_DEBUG;
                }
            }
        }

        private static void DrawSplash()
        {
            if (splashTex == null)
            {
                return;
            }

            Memory.SpriteBatchStartStencil(ss: SamplerState.AnisotropicClamp);
            Memory.spriteBatch.Draw(splashTex,
                new Rectangle(0, 0, Memory.graphics.GraphicsDevice.Viewport.Width, Memory.graphics.GraphicsDevice.Viewport.Height),
                new Rectangle(0, 0, splashTex.Width, splashTex.Height)
                , Color.White * Fade);
            Memory.SpriteBatchEnd();
        }

        private static void InitSound()
        {
            init_debugger_Audio.PlayMusic(79, loop: false);
            Memory.MusicIndex = ushort.MaxValue; // reset pos after playing overture; will loop back to start if push next
            publicModule++;
        }

        private static void ReadSplash(bool bLogo = false) => splashTex = new Splash(bNames ? splashName : splashLoop, bNames, bLogo);

        private static void WaitForFirst()
        {
            if (publicTimer > 6.0f)
            {
                publicModule++;
                Console.WriteLine("MODULE_OVERTURE: DEBUG MODULE 2");
            }
            publicTimer += Fadespd5;
        }

        #endregion Methods
    }

    public class Splash : IDisposable
    {
        #region Fields

        private const string loops = "loop";

        private const string names = "name";

        private ArchiveWorker aw;

        private string filename;

        #endregion Fields

        #region Constructors

        public Splash(int splashNum, bool bNames = true, bool bLogo = false)
        {
            if (splashNum > 0x0f)
            {
                return;
            }
            aw = new ArchiveWorker(Memory.Archives.A_MAIN);
            GetName(splashNum, bNames, bLogo);
            ReadSplash();
        }

        #endregion Constructors

        #region Destructors

        ~Splash()
        {
            Dispose();
        }

        #endregion Destructors

        #region Properties

        public bool IsDisposed { get; private set; } = false;
        public Texture2D tex { get; private set; }
        public int Height => tex?.Height ?? 0;
        public int Width => tex?.Width ?? 0;

        #endregion Properties

        #region Methods

        public static implicit operator Texture2D(Splash s) => s.tex;

        public void Dispose()
        {
            if (!IsDisposed)
            {
                IsDisposed = true;
                tex.Dispose();
            }
        }

        public override string ToString() => filename;

        private void GetName(int splashNum, bool bNames, bool bLogo)
        {
            string[] lof = aw.GetListOfFiles();
            filename = !bLogo
                ? bNames
                    ? lof.First(x => x.ToLower().Contains($"{names}{splashNum.ToString("D2")}"))
                    : lof.First(x => x.ToLower().Contains($"{loops}{splashNum.ToString("D2")}"))
                : lof.First(x => x.ToLower().Contains($"ff8.lzs"));
        }

        //Splash is 640x400 16BPP typical TIM with palette of ggg bbbbb a rrrrr gg
        private void ReadSplash()
        {
            byte[] buffer = ArchiveWorker.GetBinaryFile(Memory.Archives.A_MAIN, filename);
            string fn = Path.GetFileNameWithoutExtension(filename);
            uint uncompSize = BitConverter.ToUInt32(buffer, 0);
            buffer = buffer.Skip(4).ToArray(); //hotfix for new LZSS
            buffer = LZSS.DecompressAllNew(buffer);
            TIM_OVERTURE tim = new TIM_OVERTURE(buffer);
            if ((fn.Equals("ff8", StringComparison.OrdinalIgnoreCase))||(fn.IndexOf("loop", StringComparison.OrdinalIgnoreCase)>=0))
            {
                tim.IgnoreAlpha = true;
            }

            tex = (Texture2D)TextureHandler.Create(fn, tim, 0);//TIM2.Overture(buffer);
            //using (FileStream fs = File.Create(Path.Combine("D:\\main", Path.GetFileNameWithoutExtension(filename) + ".png")))
            //    splashTex.SaveAsPng(fs, splashTex.Width, splashTex.Height);

            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        #endregion Methods
    }
}