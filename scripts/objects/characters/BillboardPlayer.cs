using Godot;
using System;

public partial class BillboardPlayer : BillboardCharacter
{

    public override void _PhysicsProcess(double delta)
    {
		//Cancel movement if dead.
		if(Dead){ 
			base._PhysicsProcess(delta);
			return; 
		}

		Node3D camera = GetViewport().GetCamera3D().GetParent<Node3D>();
		Vector2 inputDirection = Input.GetVector(InputMapCodes.GetCode(InputMap.PLAYER_MOVE_LEFT), InputMapCodes.GetCode(InputMap.PLAYER_MOVE_RIGHT), InputMapCodes.GetCode(InputMap.PLAYER_MOVE_FORWARD), InputMapCodes.GetCode(InputMap.PLAYER_MOVE_BACK));

		//Orient the input direction to be based on where the camera is looking.
		Vector3 movementDirection = new Vector3(inputDirection.X, 0, inputDirection.Y).Rotated(Vector3.Up, camera.Rotation.Y);

		//Update if the player should be sprinting.
		setSprinting(Input.IsActionPressed(InputMapCodes.GetCode(InputMap.PLAYER_SPRINT)));

		MoveLaterally(movementDirection, IsOnFloor() ? 1.0f : 0.15f);

		if(Input.IsActionPressed(InputMapCodes.GetCode(InputMap.PLAYER_JUMP)))
		{
			Jump(delta, false);
		}else if(Input.IsActionJustReleased(InputMapCodes.GetCode(InputMap.PLAYER_JUMP)))
		{
			Jump(delta, true);
		}

		base._PhysicsProcess(delta);
    }


}
