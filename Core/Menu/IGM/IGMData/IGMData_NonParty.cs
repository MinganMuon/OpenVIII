﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace OpenVIII
{
    public partial class IGM
    {
        #region Classes

        private class IGMData_NonParty : IGMData.Base
        {
            #region Fields

            private Texture2D _red_pixel;

            #endregion Fields

            #region Constructors

            public IGMData_NonParty() : base(6, 9, new IGMDataItem.Box(pos: new Rectangle { Width = 580, Height = 231, X = 20, Y = 318 }), 2, 3)
            {
            }

            #endregion Constructors

            #region Properties

            public Damageable[] Contents { get; private set; }

            #endregion Properties

            #region Methods

            public override void Draw()
            {
                if (!Memory.State.TeamLaguna && !Memory.State.SmallTeam)
                    base.Draw();
            }

            public override void Refresh()
            {
                if (Memory.State.Characters != null)
                {
                    sbyte pos = 0;
                    bool ret = base.Update();
                    if (!Memory.State.TeamLaguna && !Memory.State.SmallTeam)
                    {
                        for (byte i = 0; Memory.State.Party != null && i < Memory.State.Characters.Count && SIZE != null && pos < SIZE.Length; i++)
                        {
                            if (!Memory.State.Party.Contains((Characters)i) && Memory.State.Characters[(Characters)i].Available)
                            {
                                BLANKS[pos] = false;
                                Refresh(pos++, Memory.State[(Characters)i]);
                            }
                        }
                    }
                    for (; pos < Count; pos++)
                    {
                        for (int i = 0; i < Depth; i++)
                        {
                            BLANKS[pos] = true;
                            ITEM[pos, i] = null;
                        }
                    }
                }
            }

            protected override void Init()
            {
                Table_Options |= Table_Options.FillRows;
                _red_pixel = new Texture2D(Memory.graphics.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
                Color[] color = new Color[] { new Color(74.5f / 100, 12.5f / 100, 11.8f / 100, 100) };
                _red_pixel.SetData(color, 0, _red_pixel.Width * _red_pixel.Height);
                Contents = new Damageable[Count];
                base.Init();
            }

            protected override void InitShift(int i, int col, int row)
            {
                base.InitShift(i, col, row);
                SIZE[i].Inflate(-26, -8);
                if (row >= 1) SIZE[i].Y -= 4;
                if (row >= 2) SIZE[i].Y -= 4;
            }

            private void Refresh(sbyte pos, Damageable damageable)
            {
                Contents[pos] = damageable;
                if (damageable != null)
                {             
                    float yoff = 39;
                    Rectangle rbak = SIZE[pos];
                    Rectangle r = rbak;
                    Color color = new Color(74.5f / 100, 12.5f / 100, 11.8f / 100, .9f);
                    ITEM[pos, 0] = new IGMDataItem.Text(damageable.Name, rbak);
                    CURSOR[pos] = new Point(rbak.X, (int)(rbak.Y + (6 * TextScale.Y)));

                    r.Offset(7, yoff);
                    ITEM[pos, 1] = new IGMDataItem.Icon(Icons.ID.Lv, r, 13);

                    r = rbak;
                    r.Offset((49), yoff);
                    ITEM[pos, 2] = new IGMDataItem.Integer(damageable.Level, r, 2, 0, 1, 3);

                    r = rbak;
                    r.Offset(126, yoff);
                    ITEM[pos, 3] = new IGMDataItem.Icon(Icons.ID.HP2, r, 13);

                    r.Offset(0, 28);
                    r.Width = 118;
                    r.Height = 1;
                    ITEM[pos, 4] = new IGMDataItem.Texture(_red_pixel, r) { Color = Color.Black };
                    r.Width = (int)(r.Width * damageable.PercentFullHP());
                    ITEM[pos, 5] = new IGMDataItem.Texture(_red_pixel, r) { Color = color };

                    r.Width = 118;
                    r.Offset(0, 2);
                    ITEM[pos, 6] = new IGMDataItem.Texture(_red_pixel, r) { Color = Color.Black };
                    r.Width = (int)(r.Width * damageable.PercentFullHP());
                    ITEM[pos, 7] = new IGMDataItem.Texture(_red_pixel, r) { Color = color };
                    //TODO red bar resizes based on current/max hp

                    r = rbak;
                    r.Offset((166), yoff);
                    ITEM[pos, 8] = new IGMDataItem.Integer(damageable.CurrentHP(), r, 2, 0, 1, 4);

                }
            }

            #endregion Methods
        }

        #endregion Classes
    }
}