﻿using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;

namespace OpenVIII
{
    public partial class IGM_Junction
    {
        #region Classes

        private class IGMData_GF_Pool : IGMData.Pool.Base<Saves.Data, GFs>
        {
            #region Constructors

            public IGMData_GF_Pool() : base(5, 3, new IGMDataItem.Box(pos: new Rectangle(440, 149, 385, 193), title: Icons.ID.GF), 4, 4)
            {
            }

            #endregion Constructors

            #region Properties

            public Dictionary<GFs, Characters> JunctionedGFs { get; private set; }

            public List<GFs> UnlockedGFs { get; private set; }

            #endregion Properties

            #region Methods

            public override bool Inputs_CANCEL()
            {
                base.Inputs_CANCEL();
                IGM_Junction.Data[SectionName.TopMenu_GF_Group].Hide();
                IGM_Junction.SetMode(Mode.TopMenu_Junction);
                return true;
            }

            public override bool Inputs_OKAY()
            {
                skipsnd = true;
                init_debugger_Audio.PlaySound(31);
                base.Inputs_OKAY();
                GFs select = Contents[CURSOR_SELECT];
                Characters c = Damageable.GetCharacterData(out Saves.CharacterData c1) && JunctionedGFs.ContainsKey(select) ? JunctionedGFs[select] : c1?.ID ?? Characters.Blank;
                if (c != Characters.Blank)
                {
                    if (c != c1.ID)
                    {
                        //show error msg
                    }
                    else
                    {
                        //Purge everything that you can't have anymore. Because the GF provided for you.
                        List<Kernel_bin.Abilities> a = (Source.Characters[c]).UnlockedGFAbilities;
                        Source.Characters[c].JunctionnedGFs ^= Saves.ConvertGFEnum.FirstOrDefault(x => x.Value == select).Key;
                        List<Kernel_bin.Abilities> b = (Source.Characters[c]).UnlockedGFAbilities;
                        foreach (Kernel_bin.Abilities r in a.Except(b).Where(v => !Kernel_bin.Junctionabilities.ContainsKey(v)))
                        {
                            if (Kernel_bin.Commandabilities.ContainsKey(r))
                            {
                                Source.Characters[c].Commands.Remove(r);
                                Source.Characters[c].Commands.Add(Kernel_bin.Abilities.None);
                            }
                            else if (Kernel_bin.EquipableAbilities.ContainsKey(r))
                            {
                                Source.Characters[c].Abilities.Remove(r);
                                Source.Characters[c].Abilities.Add(Kernel_bin.Abilities.None);
                            }
                        }
                        foreach (Kernel_bin.Abilities r in a.Except(b).Where(v => Kernel_bin.Junctionabilities.ContainsKey(v)))
                        {
                            if (Kernel_bin.Stat2Ability.Any(item => item.Value == r))
                                switch (r)
                                {
                                    case Kernel_bin.Abilities.ST_Atk_J:
                                        Source.Characters[c].Stat_J[Kernel_bin.Stat.ST_Atk] = 0;
                                        break;

                                    case Kernel_bin.Abilities.EL_Atk_J:
                                        Source.Characters[c].Stat_J[Kernel_bin.Stat.EL_Atk] = 0;
                                        break;

                                    case Kernel_bin.Abilities.EL_Def_Jx1:
                                    case Kernel_bin.Abilities.EL_Def_Jx2:
                                    case Kernel_bin.Abilities.EL_Def_Jx4:
                                        byte count = 0;
                                        if (b.Contains(Kernel_bin.Abilities.EL_Def_Jx4))
                                            count = 4;
                                        else if (b.Contains(Kernel_bin.Abilities.EL_Def_Jx2))
                                            count = 2;
                                        else if (b.Contains(Kernel_bin.Abilities.EL_Def_Jx1))
                                            count = 1;
                                        for (; count < 4; count++)
                                            Source.Characters[c].Stat_J[Kernel_bin.Stat.EL_Def_1 + count] = 0;
                                        break;

                                    case Kernel_bin.Abilities.ST_Def_Jx1:
                                    case Kernel_bin.Abilities.ST_Def_Jx2:
                                    case Kernel_bin.Abilities.ST_Def_Jx4:
                                        count = 0;
                                        if (b.Contains(Kernel_bin.Abilities.ST_Def_Jx4))
                                            count = 4;
                                        else if (b.Contains(Kernel_bin.Abilities.ST_Def_Jx2))
                                            count = 2;
                                        else if (b.Contains(Kernel_bin.Abilities.ST_Def_Jx1))
                                            count = 1;
                                        for (; count < 4; count++)
                                            Source.Characters[c].Stat_J[Kernel_bin.Stat.ST_Def_1 + count] = 0;
                                        break;

                                    case Kernel_bin.Abilities.Abilityx3:
                                    case Kernel_bin.Abilities.Abilityx4:
                                        count = 2;
                                        if (b.Contains(Kernel_bin.Abilities.Abilityx4))
                                            count = 4;
                                        else if (b.Contains(Kernel_bin.Abilities.Abilityx3))
                                            count = 3;
                                        for (; count < Source.Characters[c].Abilities.Count; count++)
                                            Source.Characters[c].Abilities[count] = Kernel_bin.Abilities.None;
                                        break;

                                    default:
                                        Source.Characters[c].Stat_J[Kernel_bin.Stat2Ability.FirstOrDefault(x => x.Value == r).Key] = 0;
                                        break;
                                }
                        }
                        IGM_Junction.Refresh();
                        return true;
                    }
                }
                return false;
            }

            public override void Refresh()
            {
                Source = Memory.State;
                JunctionedGFs = Source.JunctionedGFs();
                UnlockedGFs = Source.UnlockedGFs();
                if (Damageable != null && Damageable.GetCharacterData(out Saves.CharacterData c))
                {
                    int pos = 0;
                    int skip = Page * Rows;
                    foreach (GFs g in UnlockedGFs.Where(g => !JunctionedGFs.ContainsKey(g)))
                    {
                        if (pos >= Rows) break;
                        if (skip-- <= 0)
                        {
                            addGF(ref pos, g);
                        }
                    }
                    foreach (GFs g in UnlockedGFs.Where(g => JunctionedGFs.ContainsKey(g) && JunctionedGFs[g] == c.ID))
                    {
                        if (pos >= Rows) break;
                        if (skip-- <= 0)
                        {
                            addGF(ref pos, g, Font.ColorID.Grey);
                        }
                    }
                    foreach (GFs g in UnlockedGFs.Where(g => JunctionedGFs.ContainsKey(g) && JunctionedGFs[g] != c.ID))
                    {
                        if (pos >= Rows) break;
                        if (skip-- <= 0)
                        {
                            addGF(ref pos, g, Font.ColorID.Dark_Gray);
                        }
                    }
                    for (; pos < Rows; pos++)
                    {
                        ITEM[pos, 0] = null;
                        ITEM[pos, 1] = null;
                        ITEM[pos, 2] = null;
                        BLANKS[pos] = true;
                    }
                    base.Refresh();
                    UpdateTitle();
                    UpdateCharacter();
                }
            }

            public override void UpdateTitle()
            {
                base.UpdateTitle();
                if (Pages == 1)
                {
                    ((IGMDataItem.Box)CONTAINER).Title = Icons.ID.GF;
                    ITEM[Count - 1, 0] = ITEM[Count - 2, 0] = null;
                }
                else
                    switch (Page)
                    {
                        case 0:
                            ((IGMDataItem.Box)CONTAINER).Title = Icons.ID.GF_PG1;
                            break;

                        case 1:
                            ((IGMDataItem.Box)CONTAINER).Title = Icons.ID.GF_PG2;
                            break;

                        case 2:
                            ((IGMDataItem.Box)CONTAINER).Title = Icons.ID.GF_PG3;
                            break;

                        case 3:
                            ((IGMDataItem.Box)CONTAINER).Title = Icons.ID.GF_PG4;
                            break;
                    }
            }

            protected override void Init()
            {
                base.Init();
                SIZE[Rows] = SIZE[0];
                SIZE[Rows].Y = Y;
                ITEM[Rows, 2] = new IGMDataItem.Icon(Icons.ID.Size_16x08_Lv_, new Rectangle(SIZE[Rows].X + SIZE[Rows].Width - 30, SIZE[Rows].Y, 0, 0), scale: new Vector2(2.5f));
            }

            protected override void InitShift(int i, int col, int row)
            {
                base.InitShift(i, col, row);
                SIZE[i].Inflate(-22, -8);
                SIZE[i].Offset(0, 12 + (-8 * row));
            }

            protected override void PAGE_NEXT()
            {
                base.PAGE_NEXT();
                UpdateCharacter();
                Refresh();
            }

            protected override void PAGE_PREV()
            {
                base.PAGE_PREV();
                UpdateCharacter();
                Refresh();
            }

            protected override void SetCursor_select(int value)
            {
                if (value != GetCursor_select())
                {
                    base.SetCursor_select(value);
                    UpdateCharacter();
                }
            }

            private void addGF(ref int pos, GFs g, Font.ColorID color = Font.ColorID.White)
            {
                ITEM[pos, 0] = new IGMDataItem.Text(Memory.Strings.GetName(g), SIZE[pos], color);
                ITEM[pos, 1] = JunctionedGFs.ContainsKey(g) ? new IGMDataItem.Icon(Icons.ID.JunctionSYM, new Rectangle(SIZE[pos].X + SIZE[pos].Width - 100, SIZE[pos].Y, 0, 0)) : null;
                ITEM[pos, 2] = new IGMDataItem.Integer(Source.GFs[g].Level, new Rectangle(SIZE[pos].X + SIZE[pos].Width - 50, SIZE[pos].Y, 0, 0), spaces: 3);
                BLANKS[pos] = false;
                Contents[pos] = g;
                pos++;
            }

            private void UpdateCharacter()
            {
                if (IGM_Junction != null)
                {
                    GFs g = Contents[CURSOR_SELECT];
                    var i = (IGMDataItem.Box)((IGMData_GF_Group)IGM_Junction.Data[SectionName.TopMenu_GF_Group]).ITEM[2, 0];
                    i.Data = JunctionedGFs.Count > 0 && JunctionedGFs.ContainsKey(g) ? Memory.Strings.GetName(JunctionedGFs[g]) : null;
                }
            }

            #endregion Methods
        }

        #endregion Classes
    }
}