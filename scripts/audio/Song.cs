using Godot;
using System;

[GlobalClass]
public partial class Song : Resource
{
	[Export]
	public AudioStream Source {get; set;}

	[Export]
	public int BPM {get; set;}

	[Export]
	public float StartOffset { get; set; }

	[Export]
	public int Divisor {get; set;}

	public Song()
		: this(null, 120, 0.0f, 4)
	{
		
	}

	public Song(AudioStream source = null, int bpm = 120, float startOffset = 0.0f, int divisor = 4)
	{
		Source = source;
		BPM = bpm;
		StartOffset = startOffset;
		Divisor = divisor;
	}
}
