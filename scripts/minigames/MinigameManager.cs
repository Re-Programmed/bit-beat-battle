using Godot;
using System;
using System.Runtime.InteropServices.Marshalling;

public abstract partial class MinigameManager : Node
{
	public enum MinigameType
	{
		BEAT_FLOOR	
	};

	/// <summary>
	/// The minigame manager in use for this level.
	/// </summary>
	public static MinigameManager CurrentManager {get; private set;}

	public abstract MinigameType GetMinigameType();

	protected abstract void startMinigame();

	public override void _Ready()
	{
		CurrentManager = this;

		startMinigame();	
	}

	public override void _Process(double delta)
	{
		
	}
}
