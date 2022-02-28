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
        BookmarkList.Rows.Add(point.Name, point.MS.ToString());
      }
    }
		private void UpdateList(object sender, DataGridViewCellEventArgs e)
		{
      GuiSliderTimeline.Bookmarks.Clear();
      for (int i = 0; i < BookmarkList.Rows.Count; i++)
			{
        var bookmark = BookmarkList.Rows[i];
        if (bookmark.Cells[0].Value != null && bookmark.Cells[1].Value != null)
        {
          var bmrk = new Bookmark((string)bookmark.Cells[0].Value, int.Parse((string)bookmark.Cells[1].Value));
          GuiSliderTimeline.Bookmarks.Add(bmrk);
        }
			}
		}

		private void CurrentButton_Click(object sender, EventArgs e)
		{
      if (BookmarkList.SelectedCells.Count > 0)
			{
        BookmarkList.SelectedCells[0].OwningRow.Cells[1].Value = EditorWindow.Instance.currentTime.TotalMilliseconds.ToString();
			}
		}
	}
}
