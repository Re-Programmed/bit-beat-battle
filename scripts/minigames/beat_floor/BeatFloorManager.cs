using Godot;
using System;

public partial class BeatFloorManager : MinigameManager
{
    public override MinigameType GetMinigameType()
    {
        return MinigameType.BEAT_FLOOR;
    }

    protected override void startMinigame()
    {
		//Test
        FloorGenerator.GenerateNextFloor();

		MessageBoard.CreateMessage(MessageBoard.MessageType.DEFAULT, new Message.Component[] { new Message.Component("Started BEAT FLOOR", new Color(0.5f, 0.5f, 1.0f)) });
    }

}
