using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Sound_Space_Editor.Gui;

namespace Sound_Space_Editor
{
    public partial class BookmarkSetup : Form
    {
        public static BookmarkSetup inst;
        public BookmarkSetup()
        {
            inst = this;
            InitializeComponent();
            ResetList();
        }
        public void ResetList()
        {
            BookmarkList.Rows.Clear();
            foreach (var point in GuiSliderTimeline.Bookmarks)
            {
                BookmarkList.Rows.Add(point.Name, point.MS.ToString(), "X");
            }
        }
        private void UpdateList(object sender, DataGridViewCellEventArgs e)
        {
            GuiSliderTimeline.Bookmarks.Clear();
            for (int i = 0; i < BookmarkList.Rows.Count; i++)
            {
                var bookmark = BookmarkList.Rows[i];
                if (bookmark.Cells[1].Value == null)
                    bookmark.Cells[1].Value = 0;
                if (bookmark.Cells[0].Value != null && int.TryParse((string)bookmark.Cells[1].Value, out var time))
                {
                    var bmrk = new Bookmark((string)bookmark.Cells[0].Value, time);
                    GuiSliderTimeline.Bookmarks.Add(bmrk);
                    bookmark.Cells[2].Value = "X";
                }
            }
        }

        private void CurrentButton_Click(object sender, EventArgs e)
        {
            if (BookmarkList.SelectedCells.Count > 0)
                BookmarkList.SelectedCells[0].OwningRow.Cells[1].Value = EditorWindow.Instance.currentTime.TotalMilliseconds.ToString();
        }

        private void BookmarkList_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.RowIndex <= GuiSliderTimeline.Bookmarks.Count - 1 && ((DataGridView)sender)[e.ColumnIndex, e.RowIndex] is DataGridViewButtonCell)
            {
                BookmarkList.Rows.RemoveAt(e.RowIndex);
                GuiSliderTimeline.Bookmarks.RemoveAt(e.RowIndex);
            }
        }
    }
}
