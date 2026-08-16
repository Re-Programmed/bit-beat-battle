using Godot;
using System;
using System.Collections.Generic;

public partial class BeatDisplay : HBoxContainer
{
	[Export]
	private string beatPulseAnimation, eighthPulseAnimation, beatIdleAnimation;

	[Export(PropertyHint.File)]
	private string beatGraphicScene;

	private List<BeatGraphic> beatGraphics = new List<BeatGraphic>();

	public override void _Ready()
	{
		//SetNumberOfBeats(4);
		
	}

	public override void _Process(double delta)
	{
		
	}

	public void _on_beat(int measureBeat, int totalBeats)
	{
		for(int i = 0; i < beatGraphics.Count; i++)
		{
			if(i + 1 == measureBeat)
			{
				((AnimationNodeStateMachinePlayback)beatGraphics[i].Animator.Get("parameters/playback")).Travel(beatPulseAnimation);
			}
			else
			{
				((AnimationNodeStateMachinePlayback)beatGraphics[i].Animator.Get("parameters/playback")).Travel(beatIdleAnimation);
			}
		}
	}

	public void _on_eighth(int measureBeat, int totalBeats)
	{
		for(int i = 0; i < beatGraphics.Count; i++)
		{
			if(i + 1 == measureBeat)
			{
				//beatGraphics[i].Eighth.Texture = beatHighlightTexture;
			}
			else
			{
				//beatGraphics[i].Eighth.Texture = beatTexture;
			}
		}
	}

	public void _on_song_change(Song song)
	{
		this.SetNumberOfBeats(song.Divisor);
	}

	public void SetNumberOfBeats(int beats)
	{
		beatGraphics.Clear();

		for(int i = 0; i < this.GetChildCount(); i++)
		{
			this.RemoveChild(this.GetChild(i));
		}

		PackedScene beatGraphicPackedScene = GD.Load<PackedScene>(beatGraphicScene);

		for(int beatIndex = 0; beatIndex < beats; beatIndex++)
		{
			BeatGraphic currentBeat = beatGraphicPackedScene.Instantiate<BeatGraphic>();
			
            AddChild(currentBeat);
			beatGraphics.Add(currentBeat);
		}
	}
}
