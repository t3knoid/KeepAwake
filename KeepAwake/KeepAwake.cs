using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Forms;
using System.Diagnostics;

namespace KeepAwake
{
    public partial class KeepAwake
    {
        EXECUTION_STATE fPreviousExecutionState;
        /// <summary>
        /// Enable's keep awake
        /// </summary>
        public void Enable()
        {
            // Set new state to prevent system sleep
            fPreviousExecutionState = NativeMethods.SetThreadExecutionState(
                EXECUTION_STATE.ES_CONTINUOUS | EXECUTION_STATE.ES_SYSTEM_REQUIRED);
            if (fPreviousExecutionState == 0)
            {
                Console.WriteLine("SetThreadExecutionState failed. Do something here...");
            }
        }

        /// <summary>
        /// Disable keep awake
        /// </summary>
        public void Disable()
        {
            // Restore previous state
            if (NativeMethods.SetThreadExecutionState(fPreviousExecutionState) == 0)
            {
                // No way to recover; already exiting
            }
        }

        public void PressKey() 
        {

            const int VK_F15 = 0x7E; // F15 key
            const uint KEYEVENTF_EXTENDEDKEY = 0x0001;
            
            //Press the key          
            NativeMethods.keybd_event((byte)VK_F15, 0, KEYEVENTF_EXTENDEDKEY | 0, 0);
        }

        public void sendKeyToCitrix()
        {
            IntPtr currentProcess = NativeMethods.GetForegroundWindow(); // Save current window
            Process p = Process.GetProcessesByName("wfica32").FirstOrDefault();
            if (p != null)
            {
                IntPtr h = p.MainWindowHandle;
                NativeMethods.SetForegroundWindow(h);
                PressKey();
                NativeMethods.SetForegroundWindow(currentProcess); // Restore window
            }
            p = Process.GetProcessesByName("CDViewer").FirstOrDefault();
            if (p != null)
            {
                IntPtr h = p.MainWindowHandle;
                NativeMethods.SetForegroundWindow(h);
                PressKey();
                NativeMethods.SetForegroundWindow(currentProcess); // Restore window
            }

        }
    }

    internal static class NativeMethods
    {
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern EXECUTION_STATE SetThreadExecutionState(EXECUTION_STATE esFlags);

        [DllImport("user32.dll")]
        public static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, uint dwExtraInfo);
        
        [DllImport("user32.dll")]
        public static extern int SetForegroundWindow(IntPtr point);

        [DllImport("user32.dll")]
        public static extern IntPtr GetForegroundWindow();
    }
    
    [FlagsAttribute]
    public enum EXECUTION_STATE : uint
    {
        ES_AWAYMODE_REQUIRED = 0x00000040,
        ES_CONTINUOUS = 0x80000000,
        ES_DISPLAY_REQUIRED = 0x00000002,
        ES_SYSTEM_REQUIRED = 0x00000001
        // Legacy flag, should not be used.
        // ES_USER_PRESENT = 0x00000004
    }

}
