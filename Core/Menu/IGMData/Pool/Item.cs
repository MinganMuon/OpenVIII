﻿using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace OpenVIII.IGMData.Pool
{
    public class Item : IGMData.Pool.Base<Saves.Data, Item_In_Menu>
    {
        #region Fields

        private FF8String[] _helpStr;

        private bool Battle = false;
        private bool eventSet = false;

        #endregion Fields

        #region Properties

        private int Targets_Window => Count-3;

        #endregion Properties

        #region Methods

        private void ReInitCompletedEvent(object sender, EventArgs e) => Menu.IGM_Items.ItemChangeHandler?.Invoke(this, new KeyValuePair<Item_In_Menu, FF8String>(Contents[CURSOR_SELECT], HelpStr[CURSOR_SELECT]));

        protected override void DrawITEM(int i, int d)
        {
            if (Targets_Window >= i || !(Target_Group != null && Target_Group.Enabled))
                base.DrawITEM(i, d);
        }

        protected override void Init()
        {
            base.Init();
            _helpStr = new FF8String[Count];
            for (byte pos = 0; pos < Rows; pos++)
            {
                ITEM[pos, 0] = new IGMDataItem.Text(null, SIZE[pos]);
                ITEM[pos, 1] = new IGMDataItem.Integer(0, new Rectangle(SIZE[pos].X + SIZE[pos].Width - 60, SIZE[pos].Y, 0, 0), numtype: Icons.NumType.sysFntBig, spaces: 3);
            }
            ITEM[Count - 1, 2] = new IGMDataItem.Icon(Icons.ID.NUM_, new Rectangle(SIZE[Rows - 1].X + SIZE[Rows - 1].Width - 60, Y, 0, 0), scale: new Vector2(2.5f));
            PointerZIndex = Rows - 1;
        }

        protected override void InitShift(int i, int col, int row)
        {
            base.InitShift(i, col, row);
            //SIZE[i].Inflate(-18, -20);
            //SIZE[i].Y -= 5 * row;
            SIZE[i].Inflate(-22, -8);
            //SIZE[i].Offset(0, 12 + (-8 * row));
            int v = (int)(12 * TextScale.Y);
            SIZE[i].Height = v;
            SIZE[i].Y = Y + 18 + row*((Height-16)/Rows);

        }

        protected override void ModeChangeEvent(object sender, Enum e)
        {
            if (e.Equals(IGM_Items.Mode.SelectItem) || Battle)
            {
                Cursor_Status |= Cursor_Status.Enabled;
            }
            else if (e.Equals(IGM_Items.Mode.UseItemOnTarget))
            {
                Cursor_Status |= Cursor_Status.Blinking;
            }
            else
            {
                Cursor_Status &= ~Cursor_Status.Enabled;
            }
        }

        protected override void PAGE_NEXT()
        {
            int cnt = Pages;
            do
            {
                base.PAGE_NEXT();
                Refresh();
                skipsnd = true;
            }
            while (cnt-- > 0 && !((IGMDataItem.Integer)(ITEM[0, 1])).Enabled);
            Menu.IGM_Items.ItemChangeHandler?.Invoke(this, new KeyValuePair<Item_In_Menu, FF8String>(Contents[CURSOR_SELECT], HelpStr[CURSOR_SELECT]));
        }

        protected override void PAGE_PREV()
        {
            int cnt = Pages;
            do
            {
                base.PAGE_PREV();
                Refresh();

                skipsnd = true;
            }
            while (cnt-- > 0 && !((IGMDataItem.Integer)(ITEM[0, 1])).Enabled);
            Menu.IGM_Items.ItemChangeHandler?.Invoke(this, new KeyValuePair<Item_In_Menu, FF8String>(Contents[CURSOR_SELECT], HelpStr[CURSOR_SELECT]));
        }

        protected override void SetCursor_select(int value)
        {
            if (value != GetCursor_select())
            {
                base.SetCursor_select(value);
                Menu.IGM_Items.ItemChangeHandler?.Invoke(this, new KeyValuePair<Item_In_Menu, FF8String>(Contents[value], HelpStr[value]));
            }
        }

        #endregion Methods

        #region Constructors

        public Item(Rectangle pos, bool battle, int count = 4) : base(count + 1, 3, new IGMDataItem.Box(pos: pos, title: Icons.ID.ITEM), count, 198 / count + 1)
        {
            Battle = battle;
            if(battle)
                ITEM[Targets_Window, 0] = new BattleMenus.IGMData_TargetGroup(Damageable);
        }

        public Item() : this(new Rectangle(5, 150, 415, 480), false, 13)
        {
        }

        #endregion Constructors

        public IReadOnlyList<FF8String> HelpStr => _helpStr;
        public BattleMenus.IGMData_TargetGroup Target_Group => (BattleMenus.IGMData_TargetGroup)(((IGMData.Base)ITEM[Targets_Window, 0]));

        public override void Draw() => base.Draw();

        public override bool Inputs()
        {
            
            bool ret = false;
            if (InputITEM(Targets_Window, 0, ref ret))
            {
            }
            else
            {
                Cursor_Status &= ~Cursor_Status.Blinking;
                Cursor_Status |= Cursor_Status.Enabled;
                return base.Inputs();
            }
            return ret;
        }

        public override bool Inputs_CANCEL()
        {
            if (Battle)
            {
                Hide();
            }
            else
            {
                Menu.IGM_Items.SetMode(IGM_Items.Mode.TopMenu);
                base.Inputs_CANCEL();
            }
            return true;
        }

        public override bool Inputs_OKAY()
        {
            Item_In_Menu item = Contents[CURSOR_SELECT];
            if (Battle)
            {
                Target_Group?.SelectTargetWindows(item);
                Target_Group?.ShowTargetWindows();
            }
            if (item.Target == Item_In_Menu._Target.None)
                return false;
            base.Inputs_OKAY();
            Menu.IGM_Items.SetMode(IGM_Items.Mode.UseItemOnTarget);
            return true;
        }
        public override void HideChildren()
        {
            if (Enabled)
            {
                if (!skipdata)
                {
                    Target_Group.HideChildren();
                    Target_Group.Hide();
                }
            }
        }

        public override void Refresh()
        {
            if (!Battle && !eventSet && Menu.IGM_Items != null)
            {
                Menu.IGM_Items.ModeChangeHandler += ModeChangeEvent;
                Menu.IGM_Items.ReInitCompletedHandler += ReInitCompletedEvent;
                eventSet = true;
            }
            base.Refresh();
            Source = Memory.State;
            if (Source != null && Source.Items != null)
            {
                ((IGMDataItem.Box)CONTAINER).Title = Pages <= 1 ? (Icons.ID?)Icons.ID.ITEM : (Icons.ID?)(Icons.ID.ITEM_PG1 + (byte)Page);
                byte pos = 0;
                int skip = Page * Rows;
                for (byte i = 0; pos < Rows && i < Source.Items.Count; i++)
                {
                    Saves.Item item = Source.Items[i];
                    if (item.ID == 0 || item.QTY == 0) continue; // skip empty values.
                    if (skip-- > 0) continue; //skip items that are on prev pages.
                    Item_In_Menu itemdata = item.DATA ?? new Item_In_Menu();
                    if (Battle && itemdata.Battle == null) continue;
                    if (itemdata.ID == 0) continue; // skip empty values.
                    Font.ColorID color = Font.ColorID.White;
                    byte palette = itemdata.Palette;
                    if (!itemdata.ValidTarget(Battle))
                    {
                        color = Font.ColorID.Grey;
                        BLANKS[pos] = true;
                        palette = itemdata.Faded_Palette;
                    }
                    else
                        BLANKS[pos] = false;
                    ((IGMDataItem.Text)(ITEM[pos, 0])).Data = itemdata.Name;
                    ((IGMDataItem.Text)(ITEM[pos, 0])).Icon = itemdata.Icon;
                    ((IGMDataItem.Text)(ITEM[pos, 0])).Palette = palette;
                    ((IGMDataItem.Text)(ITEM[pos, 0])).FontColor = color;
                    ((IGMDataItem.Integer)(ITEM[pos, 1])).Data = item.QTY;
                    ((IGMDataItem.Integer)(ITEM[pos, 1])).Show();
                    ((IGMDataItem.Integer)(ITEM[pos, 1])).FontColor = color;
                    _helpStr[pos] = itemdata.Description;
                    Contents[pos] = itemdata;
                    pos++;
                }
                for (; pos < Rows; pos++)
                {
                    ((IGMDataItem.Integer)(ITEM[pos, 1])).Hide();
                    if (pos == 0) return; // if page turning. this till be enough to trigger a try next page.
                    ((IGMDataItem.Text)(ITEM[pos, 0])).Data = null;
                    ((IGMDataItem.Integer)(ITEM[pos, 1])).Data = 0;
                    ((IGMDataItem.Text)(ITEM[pos, 0])).Icon = Icons.ID.None;
                    BLANKS[pos] = true;
                }
            }
        }

        public override void Reset()
        {
            HideChildren();
            Hide();
            base.Reset();
        }
    }
}