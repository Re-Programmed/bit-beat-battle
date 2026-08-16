using Godot;
using System;
using System.Collections.Generic;

public partial class AnimationRandomizer : AnimationPlayer
{
	[Export]
	private StringName[] potentialAnimations = new StringName[0];

	public override void _Ready()
	{
		int randIndex = Mathf.FloorToInt(GD.Randf() * potentialAnimations.Length);
		this.Play(potentialAnimations[randIndex]);
	}

	public override void _Process(double delta)
	{
	}
}
