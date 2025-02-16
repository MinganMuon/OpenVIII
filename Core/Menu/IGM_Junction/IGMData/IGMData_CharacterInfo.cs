﻿using Microsoft.Xna.Framework;

namespace OpenVIII
{
    public partial class IGM_Junction
    {
        #region Classes

        private class IGMData_CharacterInfo : IGMData.Base
        {
            #region Constructors

            public IGMData_CharacterInfo() : base(1, 15, new IGMDataItem.Empty(new Rectangle(20, 153, 395, 255)))
            {
            }

            #endregion Constructors

            #region Methods

            /// <summary>
            /// Things that may of changed before screen loads or junction is changed.
            /// </summary>
            public override void Refresh()
            {
                if (Memory.State.Characters != null && Damageable.GetCharacterData(out Saves.CharacterData c))
                {
                    
                base.Refresh();
                ITEM[0, 0] = new IGMDataItem.Face(c.ID.ToFacesID(), new Rectangle(X + 12, Y, 96, 144));
                ITEM[0, 2] = new IGMDataItem.Text(Damageable.Name, new Rectangle(X + 117, Y + 0, 0, 0));

                
                    ITEM[0, 4] = new IGMDataItem.Integer(Damageable.Level, new Rectangle(X + 117 + 35, Y + 54, 0, 0), 13, numtype: Icons.NumType.sysFntBig, padding: 1, spaces: 6);
                    ITEM[0, 5] = Memory.State.Party != null && Memory.State.Party.Contains(c.ID)
                        ? new IGMDataItem.Icon(Icons.ID.InParty, new Rectangle(X + 278, Y + 48, 0, 0), 6)
                        : null;
                    ITEM[0, 7] = new IGMDataItem.Integer(Damageable.CurrentHP(), new Rectangle(X + 152, Y + 108, 0, 0), 13, numtype: Icons.NumType.sysFntBig, padding: 1, spaces: 6);
                    ITEM[0, 9] = new IGMDataItem.Integer(Damageable.MaxHP(), new Rectangle(X + 292, Y + 108, 0, 0), 13, numtype: Icons.NumType.sysFntBig, padding: 1, spaces: 5);
                    ITEM[0, 11] = new IGMDataItem.Integer((int)c.Experience, new Rectangle(X + 192, Y + 198, 0, 0), 13, numtype: Icons.NumType.Num_8x8_2, padding: 1, spaces: 9);
                    ITEM[0, 13] = new IGMDataItem.Integer(c.ExperienceToNextLevel, new Rectangle(X + 192, Y + 231, 0, 0), 13, numtype: Icons.NumType.Num_8x8_2, padding: 1, spaces: 9);
                }
            }

            /// <summary>
            /// Things fixed at startup.
            /// </summary>
            protected override void Init()
            {
                ITEM[0, 1] = new IGMDataItem.Icon(Icons.ID.MenuBorder, new Rectangle(X + 10, Y - 2, 100, 148), scale: new Vector2(1f));
                ITEM[0, 3] = new IGMDataItem.Text(Misc[Items.LV], new Rectangle(X + 117, Y + 54, 0, 0));
                ITEM[0, 6] = new IGMDataItem.Text(Misc[Items.HP], new Rectangle(X + 117, Y + 108, 0, 0));
                ITEM[0, 8] = new IGMDataItem.Text(Misc[Items.ForwardSlash], new Rectangle(X + 272, Y + 108, 0, 0));
                FF8String s = Misc[Items.CurrentEXP] + "\n" + Misc[Items.NextLEVEL];
                ITEM[0, 10] = new IGMDataItem.Text(s, new Rectangle(X, Y + 192, 0, 0));
                ITEM[0, 12] = new IGMDataItem.Icon(Icons.ID.P, new Rectangle(X + 372, Y + 198, 0, 0), 2);
                ITEM[0, 14] = new IGMDataItem.Icon(Icons.ID.P, new Rectangle(X + 372, Y + 231, 0, 0), 2);
                base.Init();
            }

            #endregion Methods
        }

        #endregion Classes
    }
}