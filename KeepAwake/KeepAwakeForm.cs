using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using UITimer = System.Windows.Forms.Timer;

namespace KeepAwake
{
    public partial class KeepAwakeForm : Form
    {
        KeepAwake KeepAwake = new KeepAwake();
        public UITimer timer;

        // Tray icon menu
        private ContextMenu tray_menu;  

        public KeepAwakeForm()
        {
            InitializeComponent();
            tray_menu = new ContextMenu();
            tray_menu.MenuItems.Add(0, new MenuItem("Enable", new System.EventHandler(pictureBox1_Click)));
            tray_menu.MenuItems.Add(1, new MenuItem("Exit", new System.EventHandler(Exit_Click)));
            tray_menu.MenuItems.Add(2, new MenuItem("-"));
            tray_menu.MenuItems.Add(3, new MenuItem("About", new System.EventHandler(AboutBox_Load)));
            notifyIcon1.ContextMenu = tray_menu;
        }
        public String pictureBoxTag
        {
            get
            {
                return (String)pictureBox1.Tag;
            }
        }

        protected void Exit_Click(Object sender, System.EventArgs e)
        {
            Close();
        }
        protected void Hide_Click(Object sender, System.EventArgs e)
        {
            Hide();
        }
        protected void Show_Click(Object sender, System.EventArgs e)
        {
            Show();
        }
       private void pictureBox1_Click(object sender, EventArgs e)
        {
            if ((String)pictureBox1.Tag == "Awake")
            {
                pictureBox1.Image = Properties.Resources.Alien_sleep_icon_96x96;
                pictureBox1.Tag = "Asleep";
                KeepAwake.Disable();
                timer.Enabled = false;                       // Enable the timer
                timer.Stop();
                toolTip1.SetToolTip(this.pictureBox1, "Zzzzz...Click on me to stay up");
                notifyIcon1.BalloonTipTitle = "Zzzzz...Click on me to stay up";
                notifyIcon1.BalloonTipText = "Zzzzz...Click on me to stay up";
                this.notifyIcon1.Text = "Zzzzz...sleeping";
                notifyIcon1.Icon = Properties.Resources.alien_sleep_icon_32x32;
                tray_menu.MenuItems[0].Checked = false;
              }
            else // Wake up
            {
                pictureBox1.Image = Properties.Resources.alien_awake_icon_96x96;
                pictureBox1.Tag = "Awake";
                notifyIcon1.BalloonTipTitle = "I'm keeping you up";
                notifyIcon1.BalloonTipText = "I'm keeping you up";
                this.notifyIcon1.Text = "Awake";
                tray_menu.MenuItems[0].Checked = true;
                notifyIcon1.Icon = Properties.Resources.alien_awake_icon_32x32;
                KeepAwake.Enable();
                timer = new UITimer();
                timer.Tick += new EventHandler(timer_Tick); // Everytime timer ticks, timer_Tick will be called
                timer.Interval = (1000) * (30);             // Timer will tick every 30 seconds
                timer.Enabled = true;                       // Enable the timer
                timer.Start();   
                toolTip1.SetToolTip(pictureBox1, "I'm keeping you up");                
            }
            
            pictureBox1.Refresh();
        }

       /// <summary>
       /// Sends F15 key
       /// </summary>
       /// <param name="sender"></param>
       /// <param name="e"></param>
       public void timer_Tick(object sender, EventArgs e)
        {
            KeepAwake.PressKey();
            // Blink
            pictureBox1.Image = Properties.Resources.Alien_sleep_icon_96x96;
            pictureBox1.Refresh();
            Thread.Sleep(500);
            pictureBox1.Image = Properties.Resources.alien_awake_icon_96x96;
            pictureBox1.Refresh();
        }

       private void KeepAwakeForm_FormClosed(object sender, EventArgs e)
       {
           KeepAwake KeepAwake = new KeepAwake();
           KeepAwake.Disable();
       }

       private void toolTip1_Popup(object sender, PopupEventArgs e)
       {
       }

       private void TrayMinimizerForm_Resize(object sender, EventArgs e)
       {
           //notifyIcon1.BalloonTipTitle = "Minimize to Tray App";
           //notifyIcon1.BalloonTipText = "You have successfully minimized your form.";

           if (FormWindowState.Minimized == this.WindowState)
           {
               notifyIcon1.Visible = true;
               notifyIcon1.ShowBalloonTip(500);
               this.Hide();
           }
           else if (FormWindowState.Normal == this.WindowState)
           {
               notifyIcon1.Visible = false;
           }
       }
       private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
       {
           this.Show();
           this.WindowState = FormWindowState.Normal;
       }

       private void AboutBox_Load(object sender, EventArgs e)
       {
           AboutBox about = new AboutBox(this);
           about.Show();
       }

    }
}
