using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace TheBlackRoom.WinForms
{
    /// <summary>
    /// TreeView that uses OS / explorer display (icons, hover, etc)
    /// </summary>
    public class NativeTreeView : TreeView
    {
        protected override void CreateHandle()
        {
            base.CreateHandle();
            NativeMethods.SetWindowTheme(this.Handle, "explorer", null);
        }
    }
}
