using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LunaBot
{
    class Character
    {
        public IntPtr BaseAddress { get; set; }
        public IntPtr BasePointer { get; set; }
        public string Name { get; set; }
        public byte Level { get; set; }
        public CharState State { get; set; }
        public int HP { get; set; }
        public int HPMax { get; set; }
        public int MP { get; set; }
        public int MPMax { get; set; }
        public long Exp { get; set; }
        public long ExpMax { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
        private int targetID;
        private int deltaHP;
        private int deltaMP;
        private long deltaExp;
        private bool inCombat;
    }
}
