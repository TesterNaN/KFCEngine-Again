using System;
using System.Xml;
using KidFansChannel.DataCore;

namespace KidFansChannel.GalCore.Commands
{
	public class Movie : ScriptCommand
	{
		private string _movieStr;

		private int _movieId;

		public string MovieStr
		{
			get
			{
				return _movieStr;
			}
			set
			{
				_movieStr = value;
			}
		}

		public int MovieId
		{
			get
			{
				return _movieId;
			}
			set
			{
				_movieId = value;
			}
		}

		public Movie()
		{
			Single = true;
			Lasting = true;
			Command = "movie";
			DataName = "movie";
		}

		public override void ConvertXml(XmlNode node, NodeInfo info)
		{
			base.ConvertXml(node, info);
			_movieStr = ScriptHelper.GetArgumentString(node, info, "file");
			LoadDataInfo();
		}

		protected void LoadDataInfo()
		{
			XmlDocument xmlDocument = new XmlDocument();
			_movieId = -1;
			if (!ScriptCommand.Data.LoadXmlDocument(DataName, _movieStr + ".xml", xmlDocument))
			{
				return;
			}
			XmlNode documentElement = xmlDocument.DocumentElement;
			if (documentElement.Name != "movie")
			{
				throw new ScriptStructureException(documentElement, new NodeInfo(), "Movie's document element should be movie.");
			}
			XmlNode node;
			if ((node = documentElement.SelectSingleNode("movie")) != null)
			{
				_movieId = ScriptHelper.GetArgumentInteger(node, new NodeInfo(), "id");
				if (_movieId < 0)
				{
					throw new ScriptArgumentException(node, new NodeInfo(), "id", ArgumentType.Integer, "Id should not less than 0.");
				}
				ScriptCommand.GameState.UpdateMovie(_movieId);
			}
		}

		public override void InitCommand()
		{
			base.InitCommand();
			KidFansChannel.DataCore.DataCore.DataType dataType = ScriptCommand.Data.CheckData(DataName, _movieStr + ".wmv", KidFansChannel.DataCore.DataCore.DataType.File);
			if (dataType == KidFansChannel.DataCore.DataCore.DataType.File)
			{
				if (ScriptCommand.GalCore.GameVideo != null)
				{
					ScriptCommand.GalCore.GameVideo.Play(ScriptCommand.Data.GetFile());
					ScriptCommand.GalCore.GameVideo.Stopped += GameVideo_Stopped;
				}
				else
				{
					Lasting = false;
					FinishCommand();
				}
			}
			else
			{
				FinishCommand();
			}
		}

		private void GameVideo_Stopped(object sender, EventArgs e)
		{
			Lasting = false;
			FinishCommand();
		}
	}
}
