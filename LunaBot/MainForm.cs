using System;
using System.IO;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace LunaBot
{
    public partial class MainForm : Form
    {
        private Timer refresh;
        private Bot myBot;
        private bool started;
        private bool fileOpened = false;
        private BotProfile profile;
        
        //Helper functions
        private Keys intToKey(int value)
        {
            switch(value)
            {
                case 0: return Keys.D0;
                case 1: return Keys.D1;
                case 2: return Keys.D2;
                case 3: return Keys.D3;
                case 4: return Keys.D4;
                case 5: return Keys.D5;
                case 6: return Keys.D6;
                case 7: return Keys.D7;
                case 8: return Keys.D8;
                case 9: return Keys.D9;
                default: return Keys.Back;
            }
        }
        private int keyToInt(Keys value)
        {
            switch (value)
            {
                case Keys.D0: return 0;
                case Keys.D1: return 1;
                case Keys.D2: return 2;
                case Keys.D3: return 3;
                case Keys.D4: return 4;
                case Keys.D5: return 5;
                case Keys.D6: return 6;
                case Keys.D7: return 7;
                case Keys.D8: return 8;
                case Keys.D9: return 9;
                default: return 0;
            }
        }
        private void refreshData(Object Sender, EventArgs e)
        {
            label_Name.Text = myBot.Name;
            label_Level.Text = myBot.Level.ToString();
            label_HP.Text = Math.Round((myBot.HP/(float)myBot.HPMax*100), 0) + "%";
            label_MP.Text = Math.Round((myBot.MP / (float)myBot.MPMax * 100), 0) + "%";
            label_Exp.Text = Math.Round((myBot.Exp / (float)myBot.ExpMax * 100), 0) + "%";
            label_State.Text = myBot.State.ToString();

        }
        private void saveProfile()
        {

            using (FileStream saveFile = File.Create(saveFileDialog.FileName))
            {
                XmlSerializer xSer = new XmlSerializer(typeof(BotProfile));
                
                //Set bot profile before saved otherwise changed my not occur
                setBotProfile(); 

                xSer.Serialize(saveFile, profile);
                fileOpened = true;
                MessageBox.Show("Profile saved!");
            }

        }
        private void openProfile()
        {
            using (FileStream openFile = File.OpenRead(openFileDialog.FileName))
            {
                XmlSerializer xSer = new XmlSerializer(typeof(BotProfile));
                profile = (BotProfile)xSer.Deserialize(openFile);
            }
        }
        private void setBotProfile()
        {   //HP
            profile.Heal1.Checked = HP1_Checkbox.Checked;
            profile.Heal1.Key = intToKey((int)HP1_Key.Value);
            profile.Heal1.ConditionParam = (float)HP1_Condition.Value;

            profile.Heal2.Checked = HP2_Checkbox.Checked;
            profile.Heal2.Key = intToKey((int)HP2_Key.Value);
            profile.Heal2.ConditionParam = (float)HP2_Condition.Value;

            profile.Heal3.Checked = HP3_Checkbox.Checked;
            profile.Heal3.Key = intToKey((int)HP3_Key.Value);
            profile.Heal3.ConditionParam = (float)HP3_Condition.Value;

            //MP
            profile.Mana1.Checked = MP1_Checkbox.Checked;
            profile.Mana1.Key = intToKey((int)MP1_Key.Value);
            profile.Mana1.ConditionParam = (float)MP1_Condition.Value;

            profile.Mana2.Checked = MP2_Checkbox.Checked;
            profile.Mana2.Key = intToKey((int)MP2_Key.Value);
            profile.Mana2.ConditionParam = (float)MP2_Condition.Value;

            profile.Mana3.Checked = MP3_Checkbox.Checked;
            profile.Mana3.Key = intToKey((int)MP3_Key.Value);
            profile.Mana3.ConditionParam = (float)MP3_Condition.Value;

            myBot.Profile = profile;
        }
        private void displayLoadedProfile()
        {   //HP
            HP1_Checkbox.Checked = profile.Heal1.Checked;
            HP1_Key.Value = keyToInt(profile.Heal1.Key);
            HP1_Condition.Value = (decimal)profile.Heal1.ConditionParam;

            HP2_Checkbox.Checked = profile.Heal2.Checked;
            HP2_Key.Value = keyToInt(profile.Heal2.Key);
            HP2_Condition.Value = (decimal)profile.Heal2.ConditionParam;

            HP3_Checkbox.Checked = profile.Heal3.Checked;
            HP3_Key.Value = keyToInt(profile.Heal3.Key);
            HP3_Condition.Value = (decimal)profile.Heal3.ConditionParam;

            //MP
            MP1_Checkbox.Checked = profile.Mana1.Checked;
            MP1_Key.Value = keyToInt(profile.Mana1.Key);
            MP1_Condition.Value = (decimal)profile.Mana1.ConditionParam;

            MP2_Checkbox.Checked = profile.Mana2.Checked;
            MP2_Key.Value = keyToInt(profile.Mana2.Key);
            MP2_Condition.Value = (decimal)profile.Mana2.ConditionParam;

            MP3_Checkbox.Checked = profile.Mana3.Checked;
            MP3_Key.Value = keyToInt(profile.Mana3.Key);
            MP3_Condition.Value = (decimal)profile.Mana3.ConditionParam;
        }                                           
        
        //Bot Events
        private void Bot_Exit(object sender, EventArgs e)
        {
            this.Close();
        }

        //Window Events
        public MainForm()
        {
            InitializeComponent();
        }
        private void MainForm_Load(object sender, EventArgs e)
        {
            try
            {
                myBot = new Bot(new IntPtr(0x00191588));
                profile = new BotProfile();
                setBotProfile();
                myBot.ProcessExit += Bot_Exit;

                refresh = new Timer();
                refresh.Interval = 100;
                refresh.Tick += refreshData;
                refresh.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Environment.Exit(0);
            }
        }
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            myBot.Close();
        }
        private void Button_Start_Click(object sender, EventArgs e)
        {
            if (started == false)
            {
                setBotProfile();

                //Disable input panels
                groupBox_Lifeguard.Enabled = false;

                //Start bot
                myBot.Start();
                started = true;

                //Change Form
                button_Start.Text = "Stop Botting";
                progressBar.Style = ProgressBarStyle.Marquee;
                progressBar.MarqueeAnimationSpeed = 1;
            }
            else
            {
                //Enable input panels
                groupBox_Lifeguard.Enabled = true;

                //Stop bot
                myBot.Stop();
                started = false;

                //Change form
                button_Start.Text = "Start Botting";
                progressBar.MarqueeAnimationSpeed = 0;
                progressBar.Style = ProgressBarStyle.Continuous;
            }
        }
        private void MenuStrip_Save_Click(object sender, EventArgs e)
        {
            if (!fileOpened && saveFileDialog.ShowDialog() != DialogResult.OK)
                return;
            else saveProfile();
        }
        private void MenuStrip_SaveAs_Click(object sender, EventArgs e)
        {
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
                saveProfile();
        }
        private void MenuStrip_Open_Click(object sender, EventArgs e)
        {
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                openProfile();
                displayLoadedProfile();
            }
        }

        //These events enable and disable their respective panels and set the profile
        private void HP1_Checkbox_CheckedChanged(object sender, EventArgs e)
        {
            HP1_Panel.Enabled = HP1_Checkbox.Checked;
            profile.Heal1.Checked = HP1_Checkbox.Checked;

        }
        private void HP2_Checkbox_CheckedChanged(object sender, EventArgs e)
        {
            HP2_Panel.Enabled = HP2_Checkbox.Checked;
            profile.Heal2.Checked = HP2_Checkbox.Checked;
        }
        private void HP3_Checkbox_CheckedChanged(object sender, EventArgs e)
        {
            HP3_Panel.Enabled = HP3_Checkbox.Checked;
            profile.Heal3.Checked = HP3_Checkbox.Checked;
        }
        private void MP1_CheckBox_CheckedChanged(object sender, EventArgs e)
        {
            MP1_Panel.Enabled = MP1_Checkbox.Checked;
            profile.Mana1.Checked = MP1_Checkbox.Checked;
        }
        private void MP2_Checkbox_CheckedChanged(object sender, EventArgs e)
        {
            MP2_Panel.Enabled = MP2_Checkbox.Checked;
            profile.Mana2.Checked = MP2_Checkbox.Checked;
        }
        private void MP3_Checkbox_CheckedChanged(object sender, EventArgs e)
        {
            MP3_Panel.Enabled = MP3_Checkbox.Checked;
            profile.Mana3.Checked = MP3_Checkbox.Checked;
        }
    }
}
