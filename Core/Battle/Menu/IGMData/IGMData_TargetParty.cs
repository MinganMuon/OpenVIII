﻿using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;

namespace OpenVIII
{
    public partial class BattleMenus
    {
        #region Classes

        public class IGMData_TargetParty : IGMData.Base
        {
            #region Methods

            protected override void InitShift(int i, int col, int row)
            {
                base.InitShift(i, col, row);
                SIZE[i].Inflate(-18, -20);
                SIZE[i].Y -= 7 * row + 2;
                //SIZE[i].Inflate(-22, -8);
                //SIZE[i].Offset(0, 12 + (-8 * row));
                SIZE[i].Height = (int)(12 * TextScale.Y);
            }

            #endregion Methods

            #region Constructors

            public IGMData_TargetParty(Rectangle pos) : base(3, 1, new IGMDataItem.Box(pos: pos, title: Icons.ID.NAME), 1, 3)
            {
            }

            #endregion Constructors

            #region Properties

            public IGMData_TargetEnemies Target_Enemies { get; set; }

            #endregion Properties

            public override void Inputs_Left()
            {
                Cursor_Status &= ~Cursor_Status.Enabled;
                Target_Enemies.Cursor_Status |= Cursor_Status.Enabled;
                Target_Enemies.CURSOR_SELECT = (CURSOR_SELECT + Target_Enemies.Rows) %
                     (Target_Enemies.Rows * Target_Enemies.Cols);
                while (Target_Enemies.BLANKS[Target_Enemies.CURSOR_SELECT] && Target_Enemies.CURSOR_SELECT > 0)
                    Target_Enemies.CURSOR_SELECT--;
                base.Inputs_Left();
            }

            public override bool Inputs_OKAY() => false;

            public override void Inputs_Right()
            {
                Cursor_Status &= ~Cursor_Status.Enabled;
                Target_Enemies.Cursor_Status |= Cursor_Status.Enabled;
                Target_Enemies.CURSOR_SELECT = CURSOR_SELECT % Target_Enemies.Rows;
                while (Target_Enemies.BLANKS[Target_Enemies.CURSOR_SELECT] && Target_Enemies.CURSOR_SELECT > 0)
                    Target_Enemies.CURSOR_SELECT--;
                base.Inputs_Right();
            }

            public override void Refresh()
            {
                if (Memory.State?.Characters != null)
                {
                    List<KeyValuePair<int, Characters>> party = Memory.State.Party.Select((element, index) => new { element, index }).ToDictionary(m => m.index, m => m.element).Where(m => !m.Value.Equals(Characters.Blank)).ToList();
                    byte pos = 0;
                    foreach (KeyValuePair<int, Characters> pm in party)
                    {
                        Saves.CharacterData data = Memory.State[Memory.State.PartyData[pm.Key]];
                        bool ded = data.IsDead;
                        ITEM[pos, 0] = new IGMDataItem.Text(Memory.Strings.GetName(pm.Value), SIZE[pos], ded ? Font.ColorID.Dark_Gray : Font.ColorID.White);

                        pos++;
                    }
                }
            }
        }

        #endregion Classes
    }
}