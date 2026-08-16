using Godot;
using System;
using System.Diagnostics;

//No support for tempo changes yet... maybe add.

public partial class MusicManager : Node
{
	[Export]
	private Song initalAudio;

	/// <summary>
	/// The player that will play the music.
	/// </summary>
	[Export]
	private AudioStreamPlayer bgMusicPlayer;

	/// <summary>
	/// Emitted every beat of the music.
	/// </summary>
	/// <param name="measureBeat">The current beat within the measure.</param>
	/// <param name="totalBeat">The current beat of the whole song.</param>
	[Signal]
	public delegate void OnBeatEventHandler(int measureBeat, int totalBeat);

	/// <summary>
	/// Emitted every "and" after the current measure beat.
	/// </summary>
	/// <param name="measureBeat">The current beat within the measure.</param>
	/// <param name="totalBeat">The current beat of the whole song.</param>
	[Signal]
	public delegate void OnEighthEventHandler(int measureBeat, int totalBeat);

	/// <summary>
	/// Emitted when the current played song is updated.
	/// </summary>
	/// <param name="song"></param>
	[Signal]
	public delegate void OnSongChangeEventHandler(Song song);

	/// <summary>
	/// Used to track what beat just played.
	/// </summary>
	private int currentBeat = 1, currentMeasureBeat = 1;

	/// <summary>
	/// Used to track how much time is left in the song and until the next beat occurs.
	/// </summary>
	private double songElapsed = 0.0;

	/// <summary>
	/// How long one beat in the current song takes to pass.
	/// </summary>
	private double currentSongBeatLength = 0.0;

	/// <summary>
	/// The current song playing (or null).
	/// </summary>
	private Song currentSong = null;

	/// <summary>
	/// Used to ensure each eighth note is only pulsed one time.
	/// </summary>
	private bool hasPulsedEighth = false;

	/// <summary>
	/// Ensure there is only 1 manager.
	/// </summary>
	public static MusicManager MANAGER {get; private set;}

	public override void _Ready()
	{
		MANAGER = this;

		PlaySong(initalAudio);
	}

	/// <summary>
	/// Stops anything currently playing and plays the specified song.
	/// </summary>
	/// <param name="song">The song to play.</param>
	public static void PlaySong(Song song)
	{
		StopSong();

		MANAGER.EmitSignal(SignalName.OnSongChange, song);

		MANAGER.currentSong = song;
		MANAGER.currentSongBeatLength = CalculateBeatLength(song);

		MANAGER.bgMusicPlayer.Stream = song.Source;
		MANAGER.bgMusicPlayer.Play();
	}

	

	/// <summary>
	/// Returns how long a beat takes to pass in the given song (in seconds). Gives the SPB (seconds per beat).
	/// (1 / BPM) * 60
	/// </summary>
	/// <param name="song">What song to get the SPB for.</param>
	/// <returns></returns>
	public static double CalculateBeatLength(Song song)
	{
		return 60.0 / (double)song.BPM;
	}

	/// <summary>
	/// Stops any currently playing song and resets all necessary variables.
	/// </summary>
	public static void StopSong()
	{
		if(MANAGER.bgMusicPlayer.Playing)
		{
			MANAGER.bgMusicPlayer.Stop();
		}

		MANAGER.currentSong = null;
		MANAGER.currentBeat = 1;
		MANAGER.currentMeasureBeat = 1;
		MANAGER.songElapsed = 0.0;
	}

	/// <summary>
	/// If a song is playing.
	/// </summary>
	/// <returns></returns>
	public static bool IsPlaying()
	{
		return MANAGER.currentSong != null && MANAGER.bgMusicPlayer.Playing;
	}

	public override void _Process(double delta)
	{
		if(IsPlaying())
		{
			songElapsed += delta;

			//The time at which the next beat will occur.
			double nextBeatPosition = MANAGER.currentSong.StartOffset + currentSongBeatLength * (currentBeat - 1);
			double nextEighthPosition = nextBeatPosition + currentSongBeatLength/2.0;

			if(songElapsed >= nextEighthPosition)
			{
				eighth();
			}

			//One beat has passed.
			if(songElapsed >= nextBeatPosition)
			{
				beat();
			}
		}
	}
	
	/// <summary>
	/// Emits a beat signal and increments the current beat.
	/// </summary>
	private void beat()
	{
		EmitSignal(SignalName.OnBeat, currentMeasureBeat++, currentBeat++);
		
		//Go back to 1 at end of measure.
		if(currentMeasureBeat > currentSong.Divisor)
		{
			currentMeasureBeat = 1;
		}

		//After a beat, the next eighth should also pulse.
		hasPulsedEighth = false;
	}

	private void eighth()
	{
		if(hasPulsedEighth){return;}
		EmitSignal(SignalName.OnEighth, currentMeasureBeat, currentBeat);
		hasPulsedEighth = true;
	}
}
