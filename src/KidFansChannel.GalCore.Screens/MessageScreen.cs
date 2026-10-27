using System.Collections.Generic;
using System.Drawing;
using KidFansChannel.DataCore;
using KidFansChannel.GalCore.Records;

namespace KidFansChannel.GalCore.Screens
{
	public abstract class MessageScreen : GameScreen
	{
		protected class MessageDisplay
		{
			public int Index;

			public int From;

			public int To;

			public int LinePos;

			public int LineNum;

			public string Text;

			public string Speaker;

			public string Voice;

			public Color Color = Color.Empty;

			public ContentAlignment Align = ContentAlignment.TopLeft;

			public static List<Color> SpeakerColor = new List<Color>();

			public static Color SelectTrueColor = Color.Empty;

			public static Color SelectFalseColor = Color.Empty;

			public MessageDisplay(Record record)
			{
				if (record is TextRecord)
				{
					int speakerNameLength = ((TextRecord)record).SpeakerNameLength;
					if (speakerNameLength == 0)
					{
						Speaker = null;
					}
					else
					{
						Speaker = record.Text.Substring(0, speakerNameLength);
					}
					Text = record.Text.Substring(speakerNameLength);
					if (record.HasVoice)
					{
						Voice = ((TextRecord)record).Voice;
					}
					if (((TextRecord)record).Speakers.Count > 0)
					{
						int speakerIndex = ((TextRecord)record).Speakers[0];
						if (SpeakerColor.Count > speakerIndex)
						{
							Color = SpeakerColor[speakerIndex];
						}
					}
				}
				else if (record is SelectRecord)
				{
					if (((SelectRecord)record).Selected)
					{
						Color = SelectTrueColor;
					}
					else
					{
						Color = SelectFalseColor;
					}
					Text = record.Text;
					Align = ContentAlignment.TopCenter;
				}
				else
				{
					Text = record.Text;
				}
				LineNum = record.Line;
				From = 0;
				To = LineNum - 1;
			}
		}

		protected LinkedList<MessageDisplay> Messages = new LinkedList<MessageDisplay>();

		public static int PageLineNum = 10;

		public static int TextWithSpeakerWidth = 600;

		public static int TextWithoutSpeakerWidth = 700;

		protected bool IsVoicing
		{
			get
			{
				return GameManager.Sound.IsPlayingVoice;
			}
		}

		public float ScrollBarStart
		{
			get
			{
				if (Messages.Count == 0)
				{
					return 0f;
				}
				return ((float)GameManager.GameState.CurrentGameState.RecordLines[Messages.First.Value.Index] + (float)Messages.First.Value.From) / (float)GameManager.GameState.CurrentGameState.RecordLines[GameManager.GameState.CurrentGameState.Records.Count];
			}
		}

		public float ScrollBarLength
		{
			get
			{
				if (Messages.Count == 0)
				{
					return 1f;
				}
				int totalLines = GameManager.GameState.CurrentGameState.RecordLines[GameManager.GameState.CurrentGameState.Records.Count];
				if (totalLines > PageLineNum)
				{
					return (float)PageLineNum / (float)totalLines;
				}
				return 1f;
			}
		}

		public override bool InitScreen()
		{
			if (!base.InitScreen())
			{
				return false;
			}
			int remainingLines = PageLineNum;
			int recordIndex = GameManager.GameState.CurrentGameState.Records.Count - 1;
			while (remainingLines > 0 && recordIndex >= 0)
			{
				MessageDisplay messageDisplay = new MessageDisplay(GameManager.GameState.CurrentGameState.Records[recordIndex]);
				messageDisplay.Index = recordIndex;
				messageDisplay.LinePos = remainingLines - messageDisplay.LineNum;
				if (messageDisplay.LinePos < 0)
				{
					messageDisplay.From = -messageDisplay.LinePos;
					messageDisplay.LinePos = 0;
					remainingLines = 0;
					Messages.AddFirst(messageDisplay);
					break;
				}
				remainingLines -= messageDisplay.LineNum;
				Messages.AddFirst(messageDisplay);
				recordIndex--;
			}
			if (recordIndex < 0 && Messages.Count > 0)
			{
				int linePos = Messages.First.Value.LinePos;
				Move(-linePos);
			}
			return true;
		}

		private void Move(int offset)
		{
			foreach (MessageDisplay message in Messages)
			{
				message.LinePos += offset;
			}
		}

		protected void ScrollUp(int line)
		{
			if (Messages.Count != 0)
			{
				int usedLines = Messages.Last.Value.LinePos + Messages.Last.Value.To - Messages.Last.Value.From + 1;
				int addedLines = AddFirst(line);
				Move(addedLines);
				RemoveLast(addedLines - (PageLineNum - usedLines));
			}
		}

		private int AddFirst(int line)
		{
			int usedLines = Messages.First.Value.From;
			int filledLines = 0;
			if (usedLines > 0)
			{
				if (usedLines > line)
				{
					Messages.First.Value.From = usedLines - line;
					Messages.First.Value.LinePos -= line;
					return filledLines = line;
				}
				filledLines += usedLines;
				line -= filledLines;
				Messages.First.Value.From = 0;
				Messages.First.Value.LinePos -= filledLines;
			}
			int recordIndex = Messages.First.Value.Index - 1;
			while (line > 0 && recordIndex >= 0)
			{
				MessageDisplay messageDisplay = new MessageDisplay(GameManager.GameState.CurrentGameState.Records[recordIndex]);
				messageDisplay.Index = recordIndex;
				if (messageDisplay.LineNum <= line)
				{
					messageDisplay.LinePos = -filledLines - messageDisplay.LineNum;
					line -= messageDisplay.LineNum;
					filledLines += messageDisplay.LineNum;
					Messages.AddFirst(messageDisplay);
					recordIndex--;
					continue;
				}
				messageDisplay.From = messageDisplay.LineNum - line;
				messageDisplay.LinePos = -filledLines - line;
				filledLines += line;
				line = 0;
				Messages.AddFirst(messageDisplay);
				break;
			}
			return filledLines;
		}

		private int RemoveLast(int line)
		{
			int totalFilled = 0;
			while (line > 0)
			{
				if (Messages.Last.Value.To >= line)
				{
					Messages.Last.Value.To -= line;
					totalFilled += line;
					line = 0;
					break;
				}
				int lineCount = Messages.Last.Value.To - Messages.Last.Value.From + 1;
				totalFilled += lineCount;
				line -= lineCount;
				Messages.RemoveLast();
			}
			return totalFilled;
		}

		protected void ScrollDown(int line)
		{
			if (Messages.Count != 0)
			{
				int usedLines = Messages.Last.Value.LinePos + Messages.Last.Value.To - Messages.Last.Value.From + 1;
				int addedLines = AddLast(line);
				Move(-addedLines);
				RemoveFirst(addedLines - (PageLineNum - usedLines));
			}
		}

		private int AddLast(int line)
		{
			int usedLines = Messages.Last.Value.LineNum - Messages.Last.Value.To - 1;
			int filledLines = 0;
			if (usedLines > 0)
			{
				if (usedLines > line)
				{
					Messages.Last.Value.To = Messages.Last.Value.LineNum - (usedLines - line) - 1;
					return filledLines = line;
				}
				filledLines += usedLines;
				line -= filledLines;
				Messages.Last.Value.To = Messages.Last.Value.LineNum - 1;
			}
			int recordIndex = Messages.Last.Value.Index + 1;
			while (line > 0 && recordIndex < GameManager.GameState.CurrentGameState.Records.Count)
			{
				MessageDisplay messageDisplay = new MessageDisplay(GameManager.GameState.CurrentGameState.Records[recordIndex]);
				messageDisplay.Index = recordIndex;
				if (messageDisplay.LineNum <= line)
				{
					messageDisplay.LinePos = PageLineNum + filledLines;
					line -= messageDisplay.LineNum;
					filledLines += messageDisplay.LineNum;
					Messages.AddLast(messageDisplay);
					recordIndex++;
					continue;
				}
				messageDisplay.To = line - 1;
				messageDisplay.LinePos = PageLineNum + filledLines;
				filledLines += line;
				line = 0;
				Messages.AddLast(messageDisplay);
				break;
			}
			return filledLines;
		}

		private int RemoveFirst(int line)
		{
			int totalFilled = 0;
			while (line > 0)
			{
				if (Messages.First.Value.LineNum - Messages.First.Value.From - 1 >= line)
				{
					Messages.First.Value.From += line;
					Messages.First.Value.LinePos += line;
					totalFilled += line;
					line = 0;
					break;
				}
				int lineCount = Messages.First.Value.To - Messages.First.Value.From + 1;
				totalFilled += lineCount;
				line -= lineCount;
				Messages.RemoveFirst();
			}
			return totalFilled;
		}

		protected void PageUp()
		{
			ScrollUp(PageLineNum);
		}

		protected void PageDown()
		{
			ScrollDown(PageLineNum);
		}

		protected bool FindVoiceUp()
		{
			int startIndex = -1;
			for (LinkedListNode<MessageDisplay> linkedListNode = Messages.First; linkedListNode != null; linkedListNode = linkedListNode.Next)
			{
				if (linkedListNode.Value.Voice != null)
				{
					startIndex = linkedListNode.Value.Index;
					break;
				}
			}
			if (startIndex == -1)
			{
				return false;
			}
			int targetIndex = startIndex;
			for (int i = startIndex - 1; i >= 0; i--)
			{
				if (GameManager.GameState.CurrentGameState.Records[i].HasVoice)
				{
					targetIndex = i;
					break;
				}
			}
			if (startIndex == targetIndex)
			{
				return false;
			}
			int lineOffset = GameManager.GameState.CurrentGameState.RecordLines[targetIndex] - (GameManager.GameState.CurrentGameState.RecordLines[Messages.First.Value.Index] + Messages.First.Value.From);
			ScrollUp(-lineOffset);
			return true;
		}

		protected bool FindVoiceDown()
		{
			int startIndex = -1;
			for (LinkedListNode<MessageDisplay> linkedListNode = Messages.Last; linkedListNode != null; linkedListNode = linkedListNode.Previous)
			{
				if (linkedListNode.Value.Voice != null)
				{
					startIndex = linkedListNode.Value.Index;
					break;
				}
			}
			if (startIndex == -1)
			{
				return false;
			}
			int targetIndex = startIndex;
			for (int i = startIndex + 1; i < GameManager.GameState.CurrentGameState.Records.Count; i++)
			{
				if (GameManager.GameState.CurrentGameState.Records[i].HasVoice)
				{
					targetIndex = i;
					break;
				}
			}
			if (startIndex == targetIndex)
			{
				return false;
			}
			int line = GameManager.GameState.CurrentGameState.RecordLines[targetIndex] + GameManager.GameState.CurrentGameState.Records[targetIndex].Line - (GameManager.GameState.CurrentGameState.RecordLines[Messages.Last.Value.Index] + Messages.Last.Value.To - Messages.Last.Value.From + 1);
			ScrollDown(line);
			return true;
		}

		protected void PlayVoice(string voice)
		{
			switch (GameManager.Data.CheckData("voice", voice + ".ogg", KidFansChannel.DataCore.DataCore.DataType.Package))
			{
			default:
				return;
			case KidFansChannel.DataCore.DataCore.DataType.File:
				GameManager.Sound.OpenVoice(GameManager.Data.GetFile(), true);
				break;
			case KidFansChannel.DataCore.DataCore.DataType.Package:
				GameManager.Sound.OpenVoice(GameManager.Data.GetStream(), true);
				break;
			}
			GameManager.Sound.PlayVoice();
		}

		protected void StopVoice()
		{
			GameManager.Sound.StopVoice();
		}
	}
}
