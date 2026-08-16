using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

public partial class MessageBoard : Container
{
	public static MessageBoard CURRENT_MESSAGE_BOARD {get; private set;}

	public enum MessageType
	{	
		DEFAULT = 0,
		ONE_PLAYER_DEATH = 1
	};

	[Export(PropertyHint.FilePath)]
	public string[] MessageTypes {get; private set;}

	private List<PackedScene> packedMessageTypes = new List<PackedScene>();


	public override void _Ready()
	{
		//Always the newest message board.
		CURRENT_MESSAGE_BOARD = this;

		foreach(string messageObject in MessageTypes)
		{
			packedMessageTypes.Add(GD.Load<PackedScene>(messageObject));
		}

		GD.Print(packedMessageTypes.Count);
	}

	public static void CreateMessage(MessageType type, params Message.Component[] components)
	{
		Message message = (Message)CURRENT_MESSAGE_BOARD.packedMessageTypes[(int)type].Instantiate();
		message.SetTextComponents(components);

		CURRENT_MESSAGE_BOARD.AddChild(message);
	}
}
