using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace LunaBot
{
    public class BotProfile
    {
        public Skill Heal1 { get; set; }
        public Skill Heal2 { get; set; }
        public Skill Heal3 { get; set; }

        public Skill Mana1 { get; set; }
        public Skill Mana2 { get; set; }
        public Skill Mana3 { get; set; }

        public BotProfile()
        {
            Heal1 = new Skill();
            Heal2 = new Skill();
            Heal3 = new Skill();

            Mana1 = new Skill();
            Mana2 = new Skill();
            Mana3 = new Skill();
        }
    }                               

    public class Skill
    {
        public bool Checked { get; set; }
        public Keys Key { get; set; }
        public float ConditionParam { get; set; }
        public float Duration { get; set; }

        public Skill()
        {
            Checked = false;
            Key = 0;
            ConditionParam = 0;
            Duration = 0;
        }
    }
}
