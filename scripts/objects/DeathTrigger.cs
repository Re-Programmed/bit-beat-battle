using Godot;
using System;

public partial class DeathTrigger : Area3D
{
	[Export]
	private BillboardCharacter.DeathCause deathReason;

    public override void _Ready()
    {
		//Subscribe to the signal.
		this.BodyEntered += _on_body_entered;
    }


	public void _on_body_entered(Node3D body)
	{
		if(body is BillboardCharacter character)
		{
			character.Kill(deathReason);
		} 
	}
}
