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
        public bool SupportCitrixClient = false;

        // Tray icon menu
        private ContextMenu tray_menu;
        private ContextMenu picture_menu;

        public KeepAwakeForm()
        {
            
            InitializeComponent();
            tray_menu = new ContextMenu();
            tray_menu.MenuItems.Add(0, new MenuItem("Enable", new System.EventHandler(pictureBox1_Click)));
            tray_menu.MenuItems.Add(1, new MenuItem("Citrix Client", new System.EventHandler(citrixClient_Click)));
            tray_menu.MenuItems.Add(2, new MenuItem("Keep on top", new System.EventHandler(keeponTop_Click)));
            tray_menu.MenuItems[1].Enabled = false;
            tray_menu.MenuItems.Add(3, new MenuItem("Minimize", new System.EventHandler(MinimizeFromTray)));
            tray_menu.MenuItems.Add(4, new MenuItem("Exit", new System.EventHandler(Exit_Click)));
            tray_menu.MenuItems.Add(5, new MenuItem("-"));
            tray_menu.MenuItems.Add(6, new MenuItem("About", new System.EventHandler(AboutBox_Load)));
            notifyIcon1.ContextMenu = tray_menu;

            picture_menu = new ContextMenu();
            picture_menu.MenuItems.Add(0, new MenuItem("Enable", new System.EventHandler(pictureBox1_Click)));
            picture_menu.MenuItems.Add(1, new MenuItem("Citrix Client", new System.EventHandler(citrixClient_Click)));
            picture_menu.MenuItems.Add(2, new MenuItem("Keep on top", new System.EventHandler(keeponTop_Click)));
            picture_menu.MenuItems[1].Enabled = false;
            picture_menu.MenuItems.Add(3, new MenuItem("Minimize", new System.EventHandler(MinimizeFromTray)));
            picture_menu.MenuItems.Add(4, new MenuItem("Exit", new System.EventHandler(Exit_Click)));
            picture_menu.MenuItems.Add(5, new MenuItem("-"));
            picture_menu.MenuItems.Add(6, new MenuItem("About", new System.EventHandler(AboutBox_Load)));
            pictureBox1.ContextMenu = picture_menu;
        }

        private void keeponTop_Click(object sender, EventArgs e)
        {
            if (TopMost == true)
            {
                TopMost = false;
                tray_menu.MenuItems[2].Checked = false;
                picture_menu.MenuItems[2].Checked = false;  
            }
            else
            {
                TopMost = true;
                tray_menu.MenuItems[2].Checked = true;
                picture_menu.MenuItems[2].Checked = true;  

            }
        }

        private void citrixClient_Click(object sender, EventArgs e)
        {
            if (tray_menu.MenuItems[1].Checked == true || picture_menu.MenuItems[1].Checked == true)
            {
                SupportCitrixClient = false;
                tray_menu.MenuItems[1].Checked = false;
                picture_menu.MenuItems[1].Checked = false;                
            }
            else
            {
                SupportCitrixClient = true;
                tray_menu.MenuItems[1].Checked = true;
                picture_menu.MenuItems[1].Checked = true;
            }
            
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
                toolTip1.SetToolTip(this.pictureBox1, Properties.Resources.String1);
                notifyIcon1.BalloonTipTitle = Properties.Resources.String1;
                notifyIcon1.BalloonTipText = Properties.Resources.String1;
                this.notifyIcon1.Text = Properties.Resources.String1; 
                notifyIcon1.Icon = Properties.Resources.alien_sleep_icon_32x32;
                tray_menu.MenuItems[0].Checked = false;
                tray_menu.MenuItems[1].Enabled = false;
                picture_menu.MenuItems[0].Checked = false;
                picture_menu.MenuItems[1].Enabled = false;
              }
            else // Wake up
            {
                pictureBox1.Image = Properties.Resources.alien_awake_icon_96x96;
                pictureBox1.Tag = "Awake";
                notifyIcon1.BalloonTipTitle = Properties.Resources.String4;
                notifyIcon1.BalloonTipText = Properties.Resources.String4; 
                this.notifyIcon1.Text = "Awake";
                tray_menu.MenuItems[0].Checked = true;
                tray_menu.MenuItems[1].Enabled = true;
                picture_menu.MenuItems[0].Checked = true;
                picture_menu.MenuItems[1].Enabled = true;
                notifyIcon1.Icon = Properties.Resources.alien_awake_icon_32x32;
                KeepAwake.Enable();
                timer = new UITimer();
                timer.Tick += new EventHandler(timer_Tick); // Everytime timer ticks, timer_Tick will be called
                timer.Interval = (1000) * (30);             // Timer will tick every 30 seconds
                timer.Enabled = true;                       // Enable the timer
                timer.Start();
                toolTip1.SetToolTip(pictureBox1, Properties.Resources.String4);
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
            if (SupportCitrixClient)
            {
                KeepAwake.sendKeyToCitrix();
            }

            Blink();
        }

       private void Blink()
       {
           // Blink
           notifyIcon1.Icon = global::KeepAwake.Properties.Resources.alien_sleep_icon_32x32;
           pictureBox1.Image = Properties.Resources.Alien_sleep_icon_96x96;
           pictureBox1.Refresh();
           Thread.Sleep(500);
           pictureBox1.Image = Properties.Resources.alien_awake_icon_96x96;
           notifyIcon1.Icon = global::KeepAwake.Properties.Resources.alien_awake_icon_32x32;
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

       private void MinimizeFromTray(object sender, EventArgs e)
       {
           notifyIcon1.Visible = true;
           notifyIcon1.ShowBalloonTip(500);
           this.Hide();
           tray_menu.MenuItems[2].Enabled = false; // Disable minimize tray entry
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

       private void notifyIcon1_Resize(object sender, EventArgs e)
       {
           if (FormWindowState.Minimized == this.WindowState)
           {
               notifyIcon1.Visible = true;
               notifyIcon1.ShowBalloonTip(500);
               this.Hide();
           }
           else if (FormWindowState.Normal == this.WindowState)
           {
               //notifyIcon1.Visible = false;
           }
       }
       private void notifyIcon1_MouseClick(object sender, MouseEventArgs e)
       {
           this.Show();
           this.WindowState = FormWindowState.Normal;
           tray_menu.MenuItems[2].Enabled = true;
       }

       private void AboutBox_Load(object sender, EventArgs e)
       {
           AboutBox about = new AboutBox(this);
           about.Show();
       }

       private void minimizeToolStripMenuItem1_Click(object sender, EventArgs e)
       {
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

       private void pictureBox1_Click(object sender, MouseEventArgs e)
       {

       }

    }
}
