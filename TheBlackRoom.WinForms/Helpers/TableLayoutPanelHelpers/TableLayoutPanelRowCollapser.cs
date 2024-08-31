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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace TheBlackRoom.WinForms.Helpers.TableLayoutPanelHelpers
{
    /// <summary>
    /// Helper class for TableLayoutPanels, that allows rows to be collapsed/expanded
    ///
    /// Note: This should be instantiated once the TableLayoutPanel has been fully set up
    /// Note: There should be one empty row at the end of the TableLayoutPanel that can
    ///       expand to fill any empty space
    /// </summary>
    public class TableLayoutPanelRowCollapser
    {
        TableLayoutPanel _tableLayoutPanel = null;
        private Dictionary<int, CollapsibleRow> _collapsibleRows = new Dictionary<int, CollapsibleRow>();

        /// <summary>
        /// Creates a new instance of the class
        /// </summary>
        /// <param name="addEmptyRow">If true, adds an empty row at the end of the TableLayoutPanel</param>
        public TableLayoutPanelRowCollapser(TableLayoutPanel tableLayoutPanel)
        {
            this._tableLayoutPanel = tableLayoutPanel;
        }

        public event EventHandler<EventArgs> RowsChanged;

        protected void OnRowChanged()
        {
            RowsChanged?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Adds a row to be part of the expand/collapse system
        /// </summary>
        public int AddCollapsibleRow(int rowIndex)
        {
            if ((rowIndex < 0) || (rowIndex >= _tableLayoutPanel.RowCount) ||
                (rowIndex >= _tableLayoutPanel.RowStyles.Count))
                return -1;

            if (_collapsibleRows.ContainsKey(rowIndex))
                return -1;

            var rowStyle = _tableLayoutPanel.RowStyles[rowIndex];

            _collapsibleRows[rowIndex] = new CollapsibleRow(rowStyle);

            return rowIndex;
        }

        /// <summary>
        /// Adds a row that holds the given control to be part of the expand/collapse system
        /// </summary>
        public int AddCollapsibleRow(Control control)
        {
            if (control == null)
                return -1;

            if (!_tableLayoutPanel.Controls.Contains(control))
                return -1;

            var rowIndex = _tableLayoutPanel.GetRow(control);
            if (rowIndex == -1)
                return -1;

            return AddCollapsibleRow(rowIndex);
        }

        public int[] AddCollapsibleRows(params int[] rowIndicies)
        {
            var rc = new List<int>();

            if ((rowIndicies != null) && (rowIndicies.Length > 0))
            {
                foreach (var rowIndex in rowIndicies)
                    if (AddCollapsibleRow(rowIndex) != -1)
                        rc.Add(rowIndex);
            }

            return rc.ToArray();
        }

        public int[] AddCollapsibleRows(params Control[] controls)
        {
            var rc = new List<int>();

            if ((controls != null) && (controls.Length >= 0))
            {
                foreach (var ctrl in controls)
                {
                    var ix = AddCollapsibleRow(ctrl);
                    if (ix != -1)
                        rc.Add(ix);
                }

            }

            return rc.ToArray();
        }

        /// <summary>
        /// Resets all rows to their original values
        /// </summary>
        public void ShowAllRows()
        {
            _tableLayoutPanel.SuspendLayout();

            //show all hidden rows, reset scale factor on percent rows
            foreach (var cr in _collapsibleRows.Values)
                cr.ShowRow();

            _tableLayoutPanel.ResumeLayout();

            OnRowChanged();
        }

        /// <summary>
        /// Hides all rows
        /// </summary>
        public void HideAllRows()
        {
            _tableLayoutPanel.SuspendLayout();

            //hide all visible rows
            foreach (var cr in _collapsibleRows.Values.Where(x => !x.IsHidden))
                cr.HideRow();

            _tableLayoutPanel.ResumeLayout();

            OnRowChanged();
        }

        /// <summary>
        /// Hides all rows but the given row
        /// </summary>
        public void ShowOnlyRow(int rowIndex)
        {
            if (!_collapsibleRows.TryGetValue(rowIndex, out var cr))
                return;

            _tableLayoutPanel.SuspendLayout();

            //show the given row
            cr.ShowRow();

            //hide all rows that are not hidden except the givel row
            foreach (var crr in _collapsibleRows.Values.Where(x => x != cr && !x.IsHidden))
                crr.HideRow();

            _tableLayoutPanel.ResumeLayout();

            OnRowChanged();
        }


        /// <summary>
        /// Show a single row, and show at a given scale if the row SizeType==Percent
        /// </summary>
        public void ShowRow(int rowIndex)
        {
            if (!_collapsibleRows.TryGetValue(rowIndex, out var cr))
                return;

            //no point to show rows already shown
            if (!cr.IsHidden)
                return;

            _tableLayoutPanel.SuspendLayout();

            //Show the row, scaled if a percent row
            cr.ShowRow();

            _tableLayoutPanel.ResumeLayout();

            OnRowChanged();
        }

        /// <summary>
        /// Hide a single row if visible
        /// </summary>
        public void HideRow(int rowIndex)
        {
            if (!_collapsibleRows.TryGetValue(rowIndex, out var cr))
                return;

            //no point to hide rows already hidden
            if (cr.IsHidden)
                return;

            _tableLayoutPanel.SuspendLayout();

            cr.HideRow();

            _tableLayoutPanel.ResumeLayout();

            OnRowChanged();
        }

        /// <summary>
        /// Toggles visibilty of a single row
        /// </summary>
        public void ToggleRow(int rowIndex)
        {
            if (!_collapsibleRows.TryGetValue(rowIndex, out var cr))
                return;

            _tableLayoutPanel.SuspendLayout();

            //toggle the row
            if (cr.IsHidden)
            {
                cr.ShowRow();
            }
            else
            {
                cr.HideRow();
            }

            _tableLayoutPanel.ResumeLayout();

            OnRowChanged();
        }

        public void SetHeight(int rowIndex, float height)
        {
            if (!_collapsibleRows.TryGetValue(rowIndex, out var cr))
                return;

            if (cr.Height != height)
            {
                cr.Height = height;
                OnRowChanged();
            }
        }

        public float GetHeight(int rowIndex)
        {
            if (!_collapsibleRows.TryGetValue(rowIndex, out var cr))
                return 0f;

            return cr.Height;
        }

        public bool IsHidden(int rowIndex)
        {
            if (!_collapsibleRows.TryGetValue(rowIndex, out var cr))
                return false;

            return cr.IsHidden;
        }

        public void ResetHeight(int rowIndex)
        {
            if (!_collapsibleRows.TryGetValue(rowIndex, out var cr))
                return;

            cr.ResetHeight();
            OnRowChanged();
        }

        public void ResetHeights()
        {
            foreach (var cr in _collapsibleRows.Values)
                cr.ResetHeight();

            OnRowChanged();
        }

        /// <summary>
        /// Helper class to store original row styles
        /// </summary>
        private class CollapsibleRow
        {
            public CollapsibleRow(RowStyle rowStyle)
            {
                RowSyleReference = rowStyle;
                OriginalSizeType = rowStyle.SizeType;
                OriginalHeight = rowStyle.Height;

                //Set current height directly
                _Height = rowStyle.Height;
            }

            private RowStyle RowSyleReference = null;
            private SizeType OriginalSizeType;
            private float OriginalHeight;

            public float Height
            {
                get => _Height;
                set
                {
                    _Height = Math.Max(1, value);

                    if (!IsHidden)
                        RowSyleReference.Height = _Height;
                }
            }
            private float _Height;

            public bool IsHidden { get; private set; } = false;


            /// <summary>
            /// Hides a row
            /// </summary>
            public void HideRow()
            {
                RowSyleReference.SizeType = SizeType.Absolute;
                RowSyleReference.Height = 0;
                IsHidden = true;
            }

            /// <summary>
            /// Shows a row
            /// </summary>
            public void ShowRow()
            {
                RowSyleReference.SizeType = OriginalSizeType;
                RowSyleReference.Height = Height;
                IsHidden = false;
            }

            public void ResetHeight()
            {
                Height = OriginalHeight;
            }

            public bool IsPercent => OriginalSizeType == SizeType.Percent;
        }
    }
}
