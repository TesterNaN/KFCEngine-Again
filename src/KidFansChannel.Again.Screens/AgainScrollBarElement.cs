using System.Drawing;
using KidFansChannel.GalCore;
using KidFansChannel.GalCore.Screens;
using KidFansChannel.Game2DCore;

namespace KidFansChannel.Again.Screens
{
	public class AgainScrollBarElement : ScrollBarElement
	{
		private Picture picScrollBar;

		private Picture[] picScrollBarParts = new Picture[3];

		private bool _dataReady;

		public override bool DataReady
		{
			get
			{
				if (_dataReady)
				{
					return true;
				}
				if (picScrollBar != null && picScrollBar.IsCreated)
				{
					picScrollBarParts[0] = GameManager.Graphics.CreateSubPicture(picScrollBar, new Rectangle(0, 0, 15, 7));
					picScrollBarParts[1] = GameManager.Graphics.CreateSubPicture(picScrollBar, new Rectangle(0, 7, 15, 30));
					picScrollBarParts[2] = GameManager.Graphics.CreateSubPicture(picScrollBar, new Rectangle(0, 37, 15, 8));
					_dataReady = true;
					return true;
				}
				return false;
			}
		}

		public override void InitData()
		{
			picScrollBar = GameManager.Data.LoadPicture("system", "scroll_bar.png");
		}

		public override void FinalData()
		{
			for (int i = 0; i < 3; i++)
			{
				picScrollBarParts[i].Dispose();
			}
			picScrollBar.Dispose();
			_dataReady = false;
		}

		public override void Init()
		{
			ReConstruct();
			for (int i = 0; i < 3; i++)
			{
				Add(new Element(picScrollBarParts[i]), "scrollbar" + i);
			}
		}

		public override void Final()
		{
			base.Final();
			for (int i = 0; i < 3; i++)
			{
				base["scrollbar" + i].Dispose();
			}
			Dispose();
		}

		protected override void UpdateBar()
		{
			int thumbStart = (int)(Start * (float)Size + 0.5f);
			int thumbSize = ((Length != 0f) ? ((int)(Length * (float)Size + 0.5f)) : Size);
			if (thumbSize > 15)
			{
				base["scrollbar0"].Position = new Point(0, thumbStart);
				SetVisible("scrollbar1", true);
				base["scrollbar1"].Position = new Point(0, thumbStart + 7);
				base["scrollbar1"].ElementPicture.ScaleY = (float)(thumbSize - 15) / 30f;
				base["scrollbar2"].Position = new Point(0, thumbStart + thumbSize - 8);
				return;
			}
			int thumbCenter = (int)((Start + Length / 2f) * (float)Size + 0.5f);
			SetVisible("scrollbar1", false);
			if (thumbCenter < 7)
			{
				base["scrollbar0"].Position = new Point(0, 0);
				base["scrollbar2"].Position = new Point(0, 7);
			}
			else if (thumbCenter > Size - 8)
			{
				base["scrollbar0"].Position = new Point(0, Size - 15);
				base["scrollbar2"].Position = new Point(0, Size - 8);
			}
			else
			{
				base["scrollbar0"].Position = new Point(0, thumbCenter - 7);
				base["scrollbar2"].Position = new Point(0, thumbCenter);
			}
		}
	}
}
