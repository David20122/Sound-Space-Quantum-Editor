using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media.Imaging;
using System;
using System.Collections.ObjectModel;

namespace New_SSQE
{
    public partial class BookmarksWindow : Window
    {
        internal static BookmarksWindow? Instance;
        private static ObservableCollection<Bookmark> Dataset = new();

        public BookmarksWindow()
        {
            Instance = this;
            Icon = new WindowIcon(new Bitmap("assets/textures/Empty.png"));

            InitializeComponent();
            BookmarkList.Items = Dataset;
        }

        public static void ShowWindow()
        {
            Instance?.Close();

            new BookmarksWindow().Show();
            Instance?.ResetList();
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if (long.TryParse(BookmarkOffsetBox.Text, out var offset) && long.TryParse(BookmarkEndOffsetBox.Text, out var endOffset))
            {
                if (endOffset < offset)
                    endOffset = offset;

                foreach (var item in MainWindow.Instance.Bookmarks)
                    if (item.Ms == offset && item.EndMs == endOffset)
                        return;

                MainWindow.Instance.Bookmarks.Add(new Bookmark(BookmarkTextBox.Text ?? "", offset, endOffset));

                MainWindow.Instance.SortBookmarks();
            }
        }

        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            if (BookmarkList.SelectedItems.Count > 0)
            {
                var bookmark = GetBookmarkFromSelected(0);

                if (long.TryParse(BookmarkOffsetBox.Text, out var offset) && long.TryParse(BookmarkEndOffsetBox.Text, out var endOffset))
                {
                    foreach (var item in MainWindow.Instance.Bookmarks)
                        if (item.Text == BookmarkTextBox.Text && item.Ms == offset && item.EndMs == endOffset)
                            return;

                    bookmark.Text = BookmarkTextBox.Text;
                    bookmark.Ms = offset;
                    bookmark.EndMs = endOffset;

                    MainWindow.Instance.SortBookmarks();
                }
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < BookmarkList.SelectedItems.Count; i++)
            {
                var bookmark = GetBookmarkFromSelected(i);
                MainWindow.Instance.Bookmarks.Remove(bookmark);
            }

            MainWindow.Instance.SortBookmarks();
        }

        private void CurrentPosButton_Click(object sender, RoutedEventArgs e)
        {
            if (BookmarkOffsetBox.Text != BookmarkEndOffsetBox.Text)
                BookmarkOffsetBox.Text = ((int)Settings.settings["currentTime"].Value).ToString();
            BookmarkEndOffsetBox.Text = ((int)Settings.settings["currentTime"].Value).ToString();
        }

        private void BookmarkSelection_Changed(object sender, SelectionChangedEventArgs e)
        {
            if (BookmarkList.SelectedItems.Count > 0)
            {
                var bookmark = GetBookmarkFromSelected(0);

                BookmarkTextBox.Text = bookmark.Text;
                BookmarkOffsetBox.Text = bookmark.Ms.ToString();
                BookmarkEndOffsetBox.Text = bookmark.EndMs.ToString();
            }
        }

        private Bookmark GetBookmarkFromSelected(int index)
        {
            var selected = BookmarkList.SelectedItems;
            var bookmarks = MainWindow.Instance.Bookmarks;

            var bookmark = selected[index] as Bookmark;

            for (int i = 0; i < bookmarks.Count; i++)
                if (bookmarks[i].Text == bookmark.Text && bookmarks[i].Ms == bookmark.Ms && bookmarks[i].EndMs == bookmark.EndMs)
                    return bookmarks[i];

            return bookmarks[0];
        }

        public void ResetList()
        {
            var bookmarks = MainWindow.Instance.Bookmarks;

            Dataset.Clear();
            for (int i = 0; i < bookmarks.Count; i++)
                Dataset.Add(new Bookmark(bookmarks[i].Text, bookmarks[i].Ms, bookmarks[i].EndMs));

            if (Dataset.Count > 0)
            {
                BookmarkTextBox.Text = Dataset[0].Text;
                BookmarkOffsetBox.Text = Dataset[0].Ms.ToString();
                BookmarkEndOffsetBox.Text = Dataset[0].EndMs.ToString();
            }
        }
    }
}
