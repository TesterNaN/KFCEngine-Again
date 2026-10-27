namespace KidFansChannel.GalCore.Screens
{
	public abstract class ScrollBarElement : Element
	{
		private float _start;

		private float _length;

		private int _size;

		public abstract bool DataReady { get; }

		public float Start
		{
			get
			{
				return _start;
			}
			set
			{
				if (_start != value)
				{
					_start = value;
					UpdateBar();
				}
				else
				{
					_start = value;
				}
			}
		}

		public float Length
		{
			get
			{
				return _length;
			}
			set
			{
				if (_length != value)
				{
					if (value <= 1f)
					{
						_length = value;
					}
					else
					{
						_length = 1f;
					}
					UpdateBar();
				}
				else
				{
					_length = value;
				}
			}
		}

		public int Size
		{
			get
			{
				return _size;
			}
			set
			{
				if (_size != value)
				{
					_size = value;
					UpdateBar();
				}
				else
				{
					_size = value;
				}
			}
		}

		public abstract void InitData();

		public abstract void FinalData();

		public abstract void Init();

		public virtual void Final()
		{
			_start = (_length = (_size = 0));
		}

		protected abstract void UpdateBar();
	}
}
