using Godot;
using System;

public partial class Message : RichTextLabel
{
	[Export]
	private MessageBoard.MessageType type;
	
	private double fadeTime = 1.1;

	public struct Component
	{
		public string Text;
		public Color Color; 

		public Component(string text, Color color)
		{
			this.Text = text;
			this.Color = color;
		}
	}

	public void SetTextComponents(Component[] components)
	{
		switch(type)
		{
			case MessageBoard.MessageType.DEFAULT:
			case MessageBoard.MessageType.ONE_PLAYER_DEATH:
			{
				string text = "";
				foreach(Component component in components)
				{
					text += $"[color=#{component.Color.ToHtml(false)}]{component.Text}[/color]";
				}
				this.Text = text;
				break;
			}
		}
	}

    public override void _Process(double delta)
    {
        if(fadeTime <= 1.0)
		{
			if(fadeTime < 0.0)
			{
				this.QueueFree();
				return;
			}

			fadeTime -= delta;

			//Fade out.
			this.Modulate = new Color(this.Modulate.R, this.Modulate.G, this.Modulate.B, (float)fadeTime);
		}
    }


	public void _on_kill_timer_end()
	{
		fadeTime -= 0.1;
	}
}
