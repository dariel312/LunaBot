using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LunaBot
{
    public enum CharState
    {
        Idle = 0,
        Moving = 2,
        Sitting = 3,
        Casting = 11,
        Dead = 18,
        Waiting = 21
    }
}
