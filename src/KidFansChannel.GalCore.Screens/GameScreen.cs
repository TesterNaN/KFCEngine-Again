using System.Drawing;
using System.Windows.Forms;

namespace KidFansChannel.GalCore.Screens
{
	public abstract class GameScreen : Element
	{
		protected Element FocusedElement;

		protected Element RootElement;

		public bool Finished;

		private bool _dataReady;

		private bool _initialized;

		public virtual bool Hide
		{
			get
			{
				return false;
			}
		}

		public virtual bool DataReady
		{
			get
			{
				return _dataReady;
			}
			protected set
			{
				_dataReady = value;
			}
		}

		public bool Initialized
		{
			get
			{
				return _initialized;
			}
		}

		public virtual void InitData()
		{
			_dataReady = false;
		}

		public virtual void FinalData()
		{
			_dataReady = false;
		}

		public virtual bool InitScreen()
		{
			if (_initialized)
			{
				return false;
			}
			_initialized = true;
			return true;
		}

		public virtual bool FinalScreen()
		{
			if (!_initialized)
			{
				return false;
			}
			_initialized = false;
			return true;
		}

		public virtual void OnMouseClick(Point location)
		{
			if (FocusedElement != null && (FocusedElement.Parent == null || FocusedElement.Parent.GetVisible(FocusedElement.Parent.IndexOf(FocusedElement))))
			{
				FocusedElement.OnMouseMove(location);
				if (FocusedElement.SelectedChild != null)
				{
					FocusedElement.SelectedChild.OnMouseClick();
				}
			}
		}

		public new virtual void OnMouseMove(Point location)
		{
			if (FocusedElement != null && (FocusedElement.Parent == null || FocusedElement.Parent.GetVisible(FocusedElement.Parent.IndexOf(FocusedElement))))
			{
				FocusedElement.OnMouseMove(location);
			}
		}

		public virtual void OnMouseWheel(int delta)
		{
		}

		public new virtual void OnKeyPress(Keys keyData)
		{
			if (keyData == Keys.Space)
			{
				FinalScreen();
			}
			else if (FocusedElement != null && (FocusedElement.Parent == null || FocusedElement.Parent.GetVisible(FocusedElement.Parent.IndexOf(FocusedElement))))
			{
				if (FocusedElement.SelectedChild == null)
				{
					FocusedElement.OnKeyPress(keyData);
				}
				else
				{
					FocusedElement.SelectedChild.OnKeyPress(keyData);
				}
			}
		}

		public new virtual void Draw()
		{
			if (RootElement != null)
			{
				RootElement.Draw();
			}
		}

		public new virtual void Update(float elaspedTime)
		{
			if (RootElement != null)
			{
				RootElement.Update(elaspedTime);
			}
		}
	}
}
