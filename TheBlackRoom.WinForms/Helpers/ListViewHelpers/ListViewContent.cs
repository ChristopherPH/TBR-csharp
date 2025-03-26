/*
 * Copyright (c) 2025 Christopher Hayes
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
using System;
using System.Linq;
using System.Windows.Forms;

namespace TheBlackRoom.WinForms.Helpers.ListViewHelpers
{
    /* Note: The main benefit of this class is that it allows you to easily
     *       display and update a listview item without having to manually
     *       deal with mapping object properties to listview subitems.
     *       Also, since this class uses a key to identify matching content
     *       objects this class can be recreated for each object update and
     *       does not need to have its reference maintained.
     *
     * Example Usage:
     *        private class MyObjectListViewContent : ListViewContent<MyObject>
     *        {
     *            public MyObjectListViewContent(MyObject content) : base(content) { }
     *
     *            protected override string[] GetSubItems(MyObject content) =>
     *                new string[] { content.UserName, content.Details, content.TimeStamp };
     *
     *            protected override object GetKey(MyObject content) => content.UserName;
     *        }
     *
     *        void OnObjectEvent(MyObject obj)
     *        {
     *            var content = new MyObjectListViewContent(obj);
     *            var lvi = content.AddOrUpdateListViewItem(lvContent);
     *            if (lvi != null)
     *                lvContent.Sort();
     *        }
     */
    /// <summary>
    /// Wrapper class that helps display an object as a ListViewItem,
    /// with additional methods to add, update, or remove the generated ListViewItem
    /// from a ListView.
    /// </summary>
    /// <typeparam name="T">Type of content object to display as a ListViewItem</typeparam>
    public abstract class ListViewContent<T> where T : class
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="content">Content object to display as a ListViewItem,
        /// or to update or remove an existing ListViewItem</param>
        public ListViewContent(T content)
        {
            this.Content = content;
        }

        /// <summary>
        /// Content object used to display a ListViewItem, or used to update or
        /// remove an existing ListViewItem
        /// </summary>
        public T Content { get; }

        /// <summary>
        /// Returns a unique key for the content object, which is used to lookup
        /// a matching ListViewItem in the ListView
        /// </summary>
        /// <param name="content">Content object to get key from</param>
        /// <returns>Unique key from content object</returns>
        protected abstract object GetKey(T content);

        /// <summary>
        /// Returns a list of strings to display in the ListViewItem
        /// as subitems
        /// </summary>
        /// <param name="content">Content object</param>
        /// <returns>SubItem strings</returns>
        protected abstract string[] GetSubItems(T content);

        /// <summary>
        /// Returns the tooltip for the ListViewItem
        /// </summary>
        /// <param name="content">Content object</param>
        /// <returns>Tooltip string</returns>
        protected virtual string GetToolTip(T content) => string.Empty;

        /// <summary>
        /// Styles the ListViewItem based on the content
        /// </summary>
        /// <param name="content">Content object</param>
        /// <param name="lvi">ListViewItem to style</param>
        protected virtual void StyleListViewItem(T content, ListViewItem lvi) { }

        /// <summary>
        /// Save the content object as the Tag of the ListViewItem
        /// </summary>
        protected bool SaveContentAsTag => true;

        /// <summary>
        /// Find a ListViewItem in the ListView that matches the content key
        /// </summary>
        /// <param name="lv">ListView holding content objects</param>
        /// <returns>ListViewItem that matches the content key</returns>
        public ListViewItem FindListViewItem(ListView lv)
        {
            if (lv == null)
                return null;

            var key = GetKey(Content);
            if (key == null)
                return null;

            return lv.Items.OfType<ListViewItem>()
                .FirstOrDefault(x => x.Tag is T tagContent && GetKey(tagContent).Equals(key));
        }

        /// <summary>
        /// Create a new ListViewItem for the content
        /// </summary>
        /// <returns>Newly created ListViewItem</returns>
        public ListViewItem CreateListViewItem()
        {
            //Create a new ListViewItem with the content subitems
            //and set ListViewItem properties
            var lvi = new ListViewItem(GetSubItems(Content))
            {
                ToolTipText = GetToolTip(Content),
            };

            //Save the content object as the Tag of the ListViewItem
            if (SaveContentAsTag)
                lvi.Tag = Content;

            //Style the ListViewItem
            StyleListViewItem(Content, lvi);

            return lvi;
        }

        /// <summary>
        /// Updates a ListViewItem with the content
        /// </summary>
        /// <param name="lvi">ListViewItem to update</param>
        /// <returns>True if the ListViewItem was updated</returns>
        public bool UpdateListViewItem(ListViewItem lvi)
        {
            if (lvi == null)
                return false;

            if (SaveContentAsTag)
                lvi.Tag = Content;

            bool changed = false;

            var display = GetSubItems(Content);

            for (int ix = 0; ix < Math.Min(lvi.SubItems.Count, display.Length); ix++)
            {
                if (lvi.SubItems[ix].Text != display[ix])
                {
                    lvi.SubItems[ix].Text = display[ix];
                    changed = true;
                }
            }

            if (changed)
                StyleListViewItem(Content, lvi);

            return changed;
        }

        /// <summary>
        /// Remove the ListViewItem from the listview
        /// </summary>
        /// <param name="lv">ListView to remove content object from</param>
        public void RemoveListViewItem(ListView lv)
        {
            if (lv == null)
                return;

            var lvi = FindListViewItem(lv);

            if (lvi != null)
                lv.Items.Remove(lvi);
        }

        /// <summary>
        /// Add or update the ListViewItem in the listview
        /// </summary>
        /// <param name="lv">ListView add or update content object</param>
        /// <returns>The ListViewItem was created or updated, or null</returns>
        public ListViewItem AddOrUpdateListViewItem(ListView lv)
        {
            if (lv == null)
                return null;

            var lvi = FindListViewItem(lv);

            if (lvi == null)
            {
                lvi = CreateListViewItem();

                lv.Items.Add(lvi);

                return lvi;
            }
            else if (UpdateListViewItem(lvi))
            {
                return lvi;
            }

            return null;
        }

        /// <summary>
        /// Get the content object from a ListViewItem
        /// </summary>
        /// <param name="lvi">ListViewItem holding a content object</param>
        /// <returns>Content stored inside ListViewItem</returns>
        public static T GetContent(ListViewItem lvi)
        {
            return lvi?.Tag as T;
        }
    }
}
