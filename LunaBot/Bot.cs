using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using Input = InputManager;

namespace LunaBot
{
    public class Bot
    {
        #region Offsets
        //Current base pointer 1/15/17 0x00191588
        public const int Offset_Name = 0x1C0;
        public const int Offset_Level = 0x514;
        public const int Offset_MP = 0x9B0;
        public const int Offset_MPMax = 0x9B4;
        public const int Offset_HP = 0x4AC;
        public const int Offset_HPMax = 0x4B0;
        public const int Offset_State = 0x1EE;
        public const int Offset_Exp = 0x9B4;
        public const int Offset_ExpMax = 0x9F0;
        public const int Offset_X = 0x1f6;
        public const int Offset_Y = 0x1fE;
        public const int Address_TargetID = 0x00BAC21C;
        #endregion

        [DllImport("User32.dll")]
        private static extern IntPtr GetForegroundWindow();
        [DllImport("User32.dll")]
        private static extern bool SetForegroundWindow(IntPtr Handle);

        //Character
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


        //Bot stuff
        public BotProfile Profile {get; set;}
        private IntPtr gameHWnd;
        private Thread worker;
        private ProcessMemory memory;
        private bool inControl = false;
        private System.Timers.Timer combatTimer;

        //Class interface
        public Bot(IntPtr BasePointer)
        {
            string processName = "Bluelandclient";
            Process[] ps = Process.GetProcessesByName(processName);
            
            if (ps.Length > 0)
            {

                gameHWnd = ps[0].MainWindowHandle;
                memory = new ProcessMemory(ps[0]);

                ps[0].EnableRaisingEvents = true;
                ps[0].Exited += onProcess_Exit;

                this.BasePointer = BasePointer;
                BaseAddress = new IntPtr(memory.ReadInt32(BasePointer));

                combatTimer = new System.Timers.Timer();
                combatTimer.Interval = 10000;
                combatTimer.Elapsed += onTimer_ExitCombat;

                worker = new Thread(work);
                worker.Start();
            }
            else throw new Exception(processName + " process was not found.");
       
        }
        public event EventHandler ProcessExit;
        public void Start()
        {
            inControl = true;
        }
        public void Stop()
        {
            inControl = false;
        }
        public void Close()
        {
            worker.Abort();
        }

        //Helper functions
        private void onProcess_Exit(Object sender, EventArgs e)
        {
            if (ProcessExit != null)
                ProcessExit(this, e);
        }
        private void onTimer_ExitCombat(Object sender, EventArgs e)
        {
            inCombat = false;
        }
        private void startCombat()
        {
            combatTimer.Stop();
            combatTimer.Start();
            inCombat = true;
        }
        private bool getBaseAddress()
        {
           Process[] ps = Process.GetProcessesByName("Bluelandclient");

            if (ps.Length > 0)
            {
                gameHWnd = ps[0].MainWindowHandle;
                memory = new ProcessMemory(ps[0]);

                BaseAddress = new IntPtr(memory.ReadInt32(BasePointer));
                return true;
            }
            else return false;

        }
        private void updateStats()
        {
            this.Name   = memory.ReadString(BaseAddress + Offset_Name, 16);
            this.Level = memory.ReadByte(BaseAddress + Offset_Level);
            this.State  = (CharState)memory.ReadInt32(BaseAddress + Offset_State);
            this.HPMax  = memory.ReadInt32(BaseAddress + Offset_HPMax);
            this.MP     = memory.ReadInt32(BaseAddress + Offset_MP);
            this.MPMax  = memory.ReadInt32(BaseAddress + Offset_MPMax);
            this.ExpMax = memory.ReadInt64(BaseAddress + Offset_ExpMax);
            this.X      = memory.ReadFloat(BaseAddress + Offset_X);
            this.Y      = memory.ReadFloat(BaseAddress + Offset_Y);
            this.targetID = memory.ReadInt32(new IntPtr(Address_TargetID));

            //Deltas
            int lastHP = HP;
            this.HP = memory.ReadInt32(BaseAddress + Offset_HP);
            this.deltaHP = HP - lastHP;

            int lastMP = MP;
            this.MP = memory.ReadInt32(BaseAddress + Offset_MP);
            this.deltaMP = MP - lastMP;

            long lastExp = Exp;
            this.Exp = memory.ReadInt64(BaseAddress + Offset_Exp);
            this.deltaExp = Exp - lastExp;
        }
        private void passiveLogic()
        {
            if(State == CharState.Idle)
            {
                if ((float)HP / (float)HPMax < 1 || (float)MP / (float)MPMax <= 0.5f)//Need to rest?
                    Input.Keyboard.KeyPress(Keys.Z, 50); //Sit
                else  //attack
                {
                    Input.Keyboard.KeyPress(Keys.Tab);     //Find new target
                    nextSkill(); //USE NEXT SKILL SEQUENCE TODO
                }
                
            }
            if(State == CharState.Sitting)
            {
                if ((float)HP / (float)HPMax == 1 && (float)MP / (float)MPMax == 1)//Need to rest?
                    Input.Keyboard.KeyPress(Keys.Z, 50); //Rest key
            }
            if(State == CharState.Dead)
            {

            }

        }
        private void combatLogic()
        {
            if (targetID == 0)
                Input.Keyboard.KeyPress(Keys.Tab, 0);     //Find new target
            
            #region HP\MP Checks
            if (Profile.Heal1.Checked && (float)HP / (float)HPMax < Profile.Heal1.ConditionParam)
                Input.Keyboard.KeyPress(Profile.Heal1.Key);

            if (Profile.Heal2.Checked && (float)HP / (float)HPMax < Profile.Heal2.ConditionParam)
                Input.Keyboard.KeyPress(Profile.Heal2.Key);
        
            if (Profile.Heal3.Checked && (float)HP / (float)HPMax < Profile.Heal3.ConditionParam)
                Input.Keyboard.KeyPress(Profile.Heal3.Key);
            
            if (Profile.Mana1.Checked && (float)MP / (float)MPMax < Profile.Mana1.ConditionParam)
                Input.Keyboard.KeyPress(Profile.Mana1.Key);

            if (Profile.Mana2.Checked && (float)MP / (float)MPMax < Profile.Mana2.ConditionParam)
                Input.Keyboard.KeyPress(Profile.Mana2.Key);

            if (Profile.Mana3.Checked && (float)MP / (float)MPMax < Profile.Mana3.ConditionParam)
                Input.Keyboard.KeyPress(Profile.Mana3.Key);
            #endregion

            if (State == CharState.Idle)
                nextSkill();
            else if (State == CharState.Sitting)
                Input.Keyboard.KeyPress(Keys.Z);
        }
        private void work()
        {
            while(true)
            {
                //Update base adress
                if (BaseAddress.ToInt32() != memory.ReadInt32(BasePointer))
                    getBaseAddress();

                updateStats();
                
                //Make sure Luna is active window
                if (GetForegroundWindow() == gameHWnd)
                {
                    //Check if in combat
                    if (deltaHP < 0 || deltaMP < 0)
                        startCombat();


                    //Is bot started?
                    if (inControl)
                    {
                        if (inCombat)
                            combatLogic();
                        else
                            passiveLogic();

                    }
                }

                Thread.Sleep(200);
            }
        }

        private void nextSkill()     //place holder
        {
            Input.Keyboard.KeyPress(Keys.D3, 900);
            Input.Keyboard.KeyPress(Keys.D2, 300);
            Input.Keyboard.KeyPress(Keys.D1, 600);


        }
    }
}
