using Godot;
using System;

public class PlayerInformation
{
	public string Name{ get; private set; }
	public Color Color{ get; private set; }
	
	public PlayerInformation(string name, Color color)
	{
		this.Name = name;
		this.Color = color;
	}
}
