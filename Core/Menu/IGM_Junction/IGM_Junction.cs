﻿using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace OpenVIII
{
    public partial class IGM_Junction : Menu
    {
        //private Mode _mode;

        #region Enums

        public enum Items : byte
        {
            Junction,
            Off,
            Auto,
            Ability,
            HP,
            Str,
            Vit,
            Mag,
            Spr,
            Spd,
            Luck,
            Hit,
            ST_A,
            ST_D,
            EL_A,
            EL_D,
            ST_A_D,
            EL_A_D,
            Stats,
            ST_A2,
            GF,
            Magic,
            AutoAtk,
            AutoMag,
            AutoDef,
            RemAll,
            RemMag,
            ChooseGFtojunction,
            Chooseslottojunction,
            Choosemagictojunction,
            RemovealljunctionedGFandmagic,
            Removealljunctionedmagic,
            CurrentEXP,
            NextLEVEL,
            _,
            LV,
            ForwardSlash,
            Percent
        }

        public enum Mode
        {
            None,
            TopMenu,
            TopMenu_Junction,
            TopMenu_Off,
            TopMenu_Auto,
            Abilities,
            Abilities_Commands,
            Abilities_Abilities,
            RemMag,
            RemAll,
            TopMenu_GF_Group,
            Mag_Pool_Stat,
            Mag_Pool_EL_A,
            Mag_Pool_EL_D,
            Mag_Pool_ST_A,
            Mag_Pool_ST_D,
            Mag_Stat,
            Mag_EL_A,
            Mag_ST_A,
            Mag_EL_D,
            Mag_ST_D,
            ConfirmChanges
        }

        public enum SectionName : byte
        {
            /// <summary>
            /// Junction OFF Auto Ability
            /// </summary>
            TopMenu,

            /// <summary>
            /// Top Right
            /// </summary>
            Title,

            /// <summary>
            /// Description Help
            /// </summary>
            Help,

            /// <summary>
            /// Character Stats
            /// </summary>
            Mag_Group,

            /// <summary>
            /// 4 Commands you can use in battle
            /// </summary>
            Commands,

            /// <summary>
            /// Portrait Name HP EXP Rank?
            /// </summary>
            CharacterInfo,

            /// <summary>
            /// Top menu where you select junction GF or Magic
            /// </summary>
            TopMenu_Junction,

            /// <summary>
            /// Top Menu where you select unjunction all or magic
            /// </summary>
            TopMenu_Off,

            /// <summary>
            /// Top Menu where you select automaticly sort by ATK DEF or MAG
            /// </summary>
            TopMenu_Auto,

            /// <summary>
            /// Junction commands/abilities
            /// </summary>
            TopMenu_Abilities,

            /// <summary>
            /// Remove all Magic?
            /// </summary>
            RemMag,

            /// <summary>
            /// Remove all Junctions?
            /// </summary>
            RemAll,

            /// <summary>
            /// GF junction screen
            /// </summary>
            TopMenu_GF_Group,

            /// <summary>
            /// Confirm changes screen
            /// </summary>
            ConfirmChanges,
        }

        #endregion Enums

        #region Properties

        public static Dictionary<Items, FF8String> Descriptions { get; private set; }
        public static Dictionary<Items, FF8String> Misc { get; private set; }
        public static Dictionary<Items, FF8String> Titles { get; private set; }

        #endregion Properties

        #region Methods

        public void ChangeHelp(FF8String str) => ((IGMDataItem.HelpBox)Data[SectionName.Help]).Data = str;

        public override bool Inputs()
        {
            if (GetMode().Equals(Mode.None)) SetMode(Mode.TopMenu);
            bool ret = false;
            if (Enabled)
            {
                switch (GetMode())
                {
                    case Mode.TopMenu:
                        ret = ((IGMData_TopMenu)Data[SectionName.TopMenu]).Inputs();
                        break;

                    case Mode.TopMenu_Junction:
                        ret = ((IGMData_TopMenu_Junction)Data[SectionName.TopMenu_Junction]).Inputs();
                        break;

                    case Mode.TopMenu_Off:
                        ret = ((IGMData_TopMenu_Off_Group)Data[SectionName.TopMenu_Off]).Inputs();
                        break;

                    case Mode.TopMenu_Auto:
                        ret = ((IGMData_TopMenu_Auto_Group)Data[SectionName.TopMenu_Auto]).Inputs();
                        break;

                    case Mode.Abilities:
                        ret = ((IGMData_Abilities_Group)Data[SectionName.TopMenu_Abilities]).Inputs();
                        break;

                    case Mode.Abilities_Commands:
                        ret = ((IGMData_Abilities_Group)Data[SectionName.TopMenu_Abilities]).ITEM[2, 0].Inputs();
                        break;

                    case Mode.Abilities_Abilities:
                        ret = ((IGMData_Abilities_Group)Data[SectionName.TopMenu_Abilities]).ITEM[3, 0].Inputs();
                        break;

                    case Mode.RemMag:
                        ret = ((IGMData.Dialog.Confirm)Data[SectionName.RemMag]).Inputs();
                        break;

                    case Mode.RemAll:
                        ret = ((IGMData.Dialog.Confirm)Data[SectionName.RemAll]).Inputs();
                        break;

                    case Mode.ConfirmChanges:
                        ret = ((IGMData.Dialog.Confirm)Data[SectionName.ConfirmChanges]).Inputs();
                        break;

                    case Mode.TopMenu_GF_Group:
                        ret = ((IGMData_GF_Group)Data[SectionName.TopMenu_GF_Group]).ITEM[1, 0].Inputs();
                        break;

                    case Mode.Mag_Pool_Stat:
                    case Mode.Mag_Pool_EL_A:
                    case Mode.Mag_Pool_EL_D:
                    case Mode.Mag_Pool_ST_A:
                    case Mode.Mag_Pool_ST_D:
                    case Mode.Mag_Stat:
                    case Mode.Mag_EL_A:
                    case Mode.Mag_EL_D:
                    case Mode.Mag_ST_A:
                    case Mode.Mag_ST_D:
                        ret = ((IGMData_Mag_Group)Data[SectionName.Mag_Group]).Inputs();
                        break;

                    default:
                        break;
                }
            }
            return ret;
        }


        protected override void Init()
        {
            SetMode((Mode)0);
            Size = new Vector2 { X = 840, Y = 630 };

            Titles = new Dictionary<Items, FF8String> {
                    {Items.Junction, Memory.Strings.Read(Strings.FileID.MNGRP,2,217) },
                    {Items.Off, Memory.Strings.Read(Strings.FileID.MNGRP,2,219) },
                    {Items.Auto, Memory.Strings.Read(Strings.FileID.MNGRP,2,221) },
                    {Items.Ability, Memory.Strings.Read(Strings.FileID.MNGRP,2,223) },
                    {Items.HP, Memory.Strings.Read(Strings.FileID.MNGRP,2,225) },
                    {Items.Str, Memory.Strings.Read(Strings.FileID.MNGRP,2,227) },
                    {Items.Vit, Memory.Strings.Read(Strings.FileID.MNGRP,2,229) },
                    {Items.Mag, Memory.Strings.Read(Strings.FileID.MNGRP,2,231) },
                    {Items.Spr, Memory.Strings.Read(Strings.FileID.MNGRP,2,233) },
                    {Items.Spd, Memory.Strings.Read(Strings.FileID.MNGRP,2,235) },
                    {Items.Luck, Memory.Strings.Read(Strings.FileID.MNGRP,2,237) },
                    {Items.Hit, Memory.Strings.Read(Strings.FileID.MNGRP,2,239) },
                    {Items.ST_A,Memory.Strings.Read(Strings.FileID.MNGRP,2,243)},
                    {Items.ST_D,Memory.Strings.Read(Strings.FileID.MNGRP,2,245)},
                    {Items.EL_A,Memory.Strings.Read(Strings.FileID.MNGRP,2,247)},
                    {Items.EL_D,Memory.Strings.Read(Strings.FileID.MNGRP,2,249)},
                    {Items.ST_A_D,Memory.Strings.Read(Strings.FileID.MNGRP,2,251)},
                    {Items.EL_A_D,Memory.Strings.Read(Strings.FileID.MNGRP,2,253)},
                    {Items.Stats,Memory.Strings.Read(Strings.FileID.MNGRP,2,255)},
                    { Items.ST_A2,Memory.Strings.Read(Strings.FileID.MNGRP, 2, 257)},
                    {Items.GF,Memory.Strings.Read(Strings.FileID.MNGRP,2,262)},
                    { Items.Magic,Memory.Strings.Read(Strings.FileID.MNGRP, 2, 264)},
                    {Items.AutoAtk,Memory.Strings.Read(Strings.FileID.MNGRP,2,269)},
                    {Items.AutoMag,Memory.Strings.Read(Strings.FileID.MNGRP,2,271)},
                    {Items.AutoDef,Memory.Strings.Read(Strings.FileID.MNGRP,2,273)},
                    {Items.RemAll,Memory.Strings.Read(Strings.FileID.MNGRP,2,275)},
                    {Items.RemMag,Memory.Strings.Read(Strings.FileID.MNGRP,2,277)},
                };

            Misc = new Dictionary<Items, FF8String> {
                { Items.CurrentEXP, Memory.Strings.Read(Strings.FileID.MNGRP, 0, 23)  },
                { Items.NextLEVEL, Memory.Strings.Read(Strings.FileID.MNGRP, 0, 24)  },
                { Items._,Memory.Strings.Read(Strings.FileID.MNGRP,2,266)},
                { Items.HP,Memory.Strings.Read(Strings.FileID.MNGRP,0,26)},
                { Items.LV,Memory.Strings.Read(Strings.FileID.MNGRP,0,27)},
                { Items.ForwardSlash,Memory.Strings.Read(Strings.FileID.MNGRP,0,25)},
                { Items.Percent,Memory.Strings.Read(Strings.FileID.MNGRP,0,29)},
                };

            Descriptions = new Dictionary<Items, FF8String> {
                    {Items.Junction, Memory.Strings.Read(Strings.FileID.MNGRP,2,218) }
                };
            Data.Add(SectionName.CharacterInfo, new IGMData_CharacterInfo());
            Data.Add(SectionName.Commands, new IGMData.Commands(new Rectangle(615, 150, 210, 192)));
            Data.Add(SectionName.Help, new IGMDataItem.HelpBox(Descriptions[Items.Junction], pos: new Rectangle(15, 69, 810, 78), title: Icons.ID.HELP));
            Data.Add(SectionName.TopMenu, new IGMData_TopMenu());
            Data.Add(SectionName.Title, 
                new IGMDataItem.Box(Titles[Items.Junction], pos: new Rectangle(615, 0, 225, 66)));
            Data.Add(SectionName.Mag_Group, new IGMData_Mag_Group(
                new IGMData_Mag_Stat_Slots(),
                new IGMData_Mag_PageTitle(),
                new IGMData.Pool.Magic(),
                new IGMData_Mag_EL_A_D_Slots(),
                new IGMData_Mag_EL_A_Values(),
                new IGMData_Mag_EL_D_Values(),
                new IGMData_Mag_ST_A_D_Slots(),
                new IGMData_Mag_ST_A_Values(),
                new IGMData_Mag_ST_D_Values()
                ));
            Data.Add(SectionName.TopMenu_Junction, new IGMData_TopMenu_Junction());
            Data.Add(SectionName.TopMenu_Off, new IGMData_TopMenu_Off_Group(
                    new IGMDataItem.Box(Titles[Items.Off], pos: new Rectangle(0, 12, 169, 54), options: Box_Options.Center | Box_Options.Middle),
                new IGMData_TopMenu_Off()
                ));
            Data.Add(SectionName.TopMenu_Auto, new IGMData_TopMenu_Auto_Group(
                    new IGMDataItem.Box(Titles[Items.Auto], pos: new Rectangle(0, 12, 169, 54), options: Box_Options.Center | Box_Options.Middle),
                new IGMData_TopMenu_Auto()));
            Data.Add(SectionName.TopMenu_Abilities, new IGMData_Abilities_Group(
                new IGMData_Abilities_CommandSlots(),
                new IGMData_Abilities_AbilitySlots(),
                new IGMData_Abilities_CommandPool(),
                new IGMData_Abilities_AbilityPool()
                ));
            FF8String Yes = Memory.Strings.Read(Strings.FileID.MNGRP, 0, 57);
            FF8String No = Memory.Strings.Read(Strings.FileID.MNGRP, 0, 58);
            Data.Add(SectionName.TopMenu_GF_Group, new IGMData_GF_Group(
                new IGMData_GF_Junctioned(),
                new IGMData_GF_Pool(),
                new IGMDataItem.Box(pos: new Rectangle(440, 345, 385, 66))
                ));

            Data.Add(SectionName.RemMag, new IGMData_ConfirmRemMag(data: Memory.Strings.Read(Strings.FileID.MNGRP, 2, 280), title: Icons.ID.NOTICE, opt1: Yes, opt2: No, pos: new Rectangle(180, 174, 477, 216)));
            Data.Add(SectionName.RemAll, new IGMData_ConfirmRemAll(data: Memory.Strings.Read(Strings.FileID.MNGRP, 2, 279), title: Icons.ID.NOTICE, opt1: Yes, opt2: No, pos: new Rectangle(170, 174, 583, 216)));
            Data.Add(SectionName.ConfirmChanges, new IGMData_ConfirmChanges(data: Memory.Strings.Read(Strings.FileID.MNGRP, 0, 73), title: Icons.ID.NOTICE, opt1: Yes, opt2: Memory.Strings.Read(Strings.FileID.MNGRP, 2, 268), pos: new Rectangle(280, 174, 367, 216)));

            base.Init();
        }

        #endregion Methods
    }
}