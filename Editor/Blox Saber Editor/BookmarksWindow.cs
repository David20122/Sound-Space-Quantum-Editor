using System;
using System.Windows.Forms;
using OpenTK;
using Sound_Space_Editor.Misc;

namespace Sound_Space_Editor
{
    public partial class BookmarksWindow : Form
    {
        public static BookmarksWindow Instance;

        public BookmarksWindow()
        {
            Instance = this;

            InitializeComponent();
            ResetList();
        }

        public void ResetList(int index = 0)
        {
            var editor = MainWindow.Instance;

            BookmarkList.Rows.Clear();

            foreach (var bookmark in editor.Bookmarks)
                BookmarkList.Rows.Add(bookmark.Text, bookmark.Ms);

            if (editor.Bookmarks.Count > 0)
            {
                index = MathHelper.Clamp(index, 0, editor.Bookmarks.Count - 1);

                BookmarkList.CurrentCell = BookmarkList[0, index];

                var bookmark = editor.Bookmarks[index];
                TextBox.Text = bookmark.Text;
                OffsetBox.Value = bookmark.Ms;
            }
            else
            {
                TextBox.Text = "";
                OffsetBox.Value = 0;
            }
        }

        private void CurrentButton_Click(object sender, EventArgs e)
        {
            OffsetBox.Value = (decimal)Settings.settings["currentTime"].Value;
        }

        private void DeleteButton_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in BookmarkList.SelectedRows)
                MainWindow.Instance.Bookmarks.RemoveAt(row.Index);

            ResetList();
        }

        private void OnClosing(object sender, EventArgs e)
        {
            Instance = null;
        }

        private void AddButton_Click(object sender, EventArgs e)
        {
            var editor = MainWindow.Instance;

            var exists = false;

            foreach (var bookmark in editor.Bookmarks)
                exists = exists || bookmark.Ms == OffsetBox.Value;

            if (!exists)
            {
                var bookmark = new Bookmark(TextBox.Text, (long)OffsetBox.Value);

                editor.Bookmarks.Add(bookmark);
                editor.SortBookmarks(false);

                ResetList(editor.Bookmarks.IndexOf(bookmark));
            }
        }

        private void BookmarkList_SelectionChanged(object sender, EventArgs e)
        {
            if (BookmarkList.SelectedRows.Count > 0)
            {
                var row = BookmarkList.SelectedRows[0];
                var bookmark = MainWindow.Instance.Bookmarks[row.Index];

                TextBox.Text = bookmark.Text;
                OffsetBox.Value = bookmark.Ms;
            }
        }

        private void UpdateButton_Click(object sender, EventArgs e)
        {
            var editor = MainWindow.Instance;

            var exists = false;

            var selected = editor.Bookmarks[BookmarkList.CurrentRow.Index];

            foreach (var bookmark in editor.Bookmarks)
                exists = exists || (bookmark.Ms == OffsetBox.Value && bookmark != selected);

            if (!exists)
            {
                selected.Text = TextBox.Text;
                selected.Ms = (long)OffsetBox.Value;

                editor.SortBookmarks(false);

                ResetList(editor.Bookmarks.IndexOf(selected));
            }
        }

        private void OffsetBox_ValueChanged(object sender, EventArgs e)
        {
            OffsetBox.Value = (long)Math.Min(OffsetBox.Value, (decimal)Settings.settings["currentTime"].Max);
        }
    }
}
