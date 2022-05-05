using System;
using System.Windows.Forms;

namespace InformationTree.Render.WinForms.Extensions
{
    public static class ControlExtensions
    {
        /// <summary>
        /// Invoke an action on the control's thread
        /// </summary>
        public static void InvokeWrapper<T>(this T control, Action<T> action)
            where T : Control
        {
            if (control == null || action == null)
                return;
            if (control.InvokeRequired)
            {
                control.BeginInvoke(() => action(control));
            }
            else
                action(control);
        }
    }
}