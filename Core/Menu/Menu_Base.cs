﻿using Microsoft.Xna.Framework;
using System;
using System.Linq;

namespace OpenVIII
{
    /// <summary>
    /// Root class all menu objects can grow from.
    /// </summary>
    public abstract class Menu_Base
    {
        #region Fields

        private int _cursor_select;
        protected Damageable _damageable;

        protected Rectangle _pos;

        #endregion Fields

        #region Methods

        protected int GetCursor_select() => _cursor_select;

        protected abstract void Init();

        protected virtual void ModeChangeEvent(object sender, Enum e)
        {
        }

        /// <summary>
        /// For child items.
        /// </summary>
        protected virtual void RefreshChild()
        {
        }

        protected virtual void SetCursor_select(int value)
        {
            if ((Cursor_Status & Cursor_Status.Enabled) != 0 && value >= 0 && CURSOR != null && value < CURSOR.Length && CURSOR[value] != Point.Zero)
                _cursor_select = value;
        }

        #endregion Methods

        /// <summary>
        /// location of where pointer finger will point.
        /// </summary>
        public Point[] CURSOR;

        public Menu_Base CONTAINER { get; set; }

        public int CURSOR_SELECT
        {
            get => GetCursor_select(); set => SetCursor_select(value);
        }

        public Cursor_Status Cursor_Status { get; set; } = Cursor_Status.Disabled;

        /// <summary>
        /// Characters/Enemies/GF
        /// </summary>
        public Damageable Damageable => _damageable;

        /// <summary>
        /// If enabled the menu is Visible and all functionality works. Else everything is hidden and
        /// nothing functions.
        /// </summary>
        public bool Enabled { get; private set; } = true;

        public virtual int Height { get => _pos.Height; set => _pos.Height = value; }

        /// <summary>
        /// Position of party member 0,1,2. If -1 at the time of setting the character wasn't in the party.
        /// </summary>
        public sbyte PartyPos { get; protected set; }

        /// <summary>
        /// Where to draw this item.
        /// </summary>
        public virtual Rectangle Pos { get => _pos; set => _pos = value; }

        public virtual int Width { get => _pos.Width; set => _pos.Width = value; }

        public virtual int X { get => _pos.X; set => _pos.X = value; }

        public virtual int Y { get => _pos.Y; set => _pos.Y = value; }

        public static implicit operator Rectangle(Menu_Base v) => v.Pos;

        public virtual void AddModeChangeEvent(ref EventHandler<Enum> eventHandler) => eventHandler += ModeChangeEvent;

        public abstract void Draw();

        /// <summary>
        /// Hide object prevents drawing, update, inputs.
        /// </summary>
        public virtual void Hide() => Enabled = false;

        public virtual void HideChildren()
        {
        }

        public abstract bool Inputs();

        /// <summary>
        /// Things that change rarely. Like a party member changes or Laguna dream happens.
        /// </summary>
        public virtual void Refresh() => RefreshChild();

        /// <summary>
        /// Update set characters and then refresh.
        /// </summary>
        /// <param name="character"></param>
        /// <param name="Visiblecharacter"></param>
        public virtual void Refresh(Damageable damageable)
        {
            if (damageable != null)
            {
                _damageable = damageable;

                if (Damageable.GetCharacterData(out Saves.CharacterData c))
                {
                    PartyPos = (sbyte)(Memory.State?.PartyData?.Where(x => !x.Equals(Characters.Blank)).ToList().FindIndex(x => x.Equals(c.ID)) ?? -1);
                }
            }
            Refresh();
        }

        public virtual void RemoveModeChangeEvent(ref EventHandler<Enum> eventHandler) => eventHandler -= ModeChangeEvent;

        /// <summary>
        /// Plan is to use this to reset values to a default state if done.
        /// </summary>
        public virtual void Reset() { }

        /// <summary>
        /// Show object enables drawing, update, inputs.
        /// </summary>
        public virtual void Show() => Enabled = true;

        public abstract bool Update();
    }
}