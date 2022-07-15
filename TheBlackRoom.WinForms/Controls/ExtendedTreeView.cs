/*
 * Copyright (c) 2022 Christopher Hayes
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 */
using System.Windows.Forms;

namespace TheBlackRoom.WinForms.Controls
{
    /// <summary>
    /// Extended TreeView
    ///
    /// TreeView with ability to capture middle mouse button clicks
    /// </summary>
    public class ExtendedTreeView : TreeView
    {
        TreeNode mouseDownNode = null;

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);

            switch (m.Msg)
            {
                case NativeMethods.WM_MBUTTONDOWN:
                    //get down position
                    var ptdown = this.PointToClient(MousePosition);

                    //trigger event
                    this.OnMouseDown(new MouseEventArgs(MouseButtons.Middle, 0, ptdown.X, ptdown.Y, 0));

                    //save clicked on node
                    mouseDownNode = this.GetNodeAt(ptdown);
                    break;

                case NativeMethods.WM_MBUTTONUP:
                    //get up position
                    var ptup = this.PointToClient(MousePosition);

                    //trigger event
                    this.OnMouseUp(new MouseEventArgs(MouseButtons.Middle, 0, ptup.X, ptup.Y, 0));

                    //https://referencesource.microsoft.com/#system.windows.forms/winforms/managed/system/winforms/TreeView.cs

                    //If the hit-tested node here is the same as the node we hit-tested
                    //on mouse down then we will fire our OnNodeMouseClick event.
                    if (mouseDownNode != null)
                    {
                        if (this.GetNodeAt(ptup) == mouseDownNode)
                        {
                            this.OnNodeMouseClick(new TreeNodeMouseClickEventArgs(mouseDownNode,
                                MouseButtons.Middle, 1, ptup.X, ptup.Y));
                        }

                        mouseDownNode = null;
                    }
                    break;

                case NativeMethods.WM_MBUTTONDBLCLK:
                    //get position
                    var ptdbl = this.PointToClient(MousePosition);

                    //get double clicked node
                    var node = this.GetNodeAt(ptdbl);

                    //there is probably some other logic we should use to check the fired events,
                    //but this works from what I can tell.
                    if (node != null)
                    {
                        this.OnNodeMouseDoubleClick(new TreeNodeMouseClickEventArgs(node,
                            MouseButtons.Middle, 1, ptdbl.X, ptdbl.Y));
                    }
                    break;
            }
        }
    }
}
