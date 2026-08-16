using Godot;
using System;
using System.Runtime.InteropServices.Marshalling;

public partial class PlayerOrbitCamera : Camera3D
{
	[Export]
	private Node3D player;

	[Export]
	private Node3D anchor;
	[Export]
	private float zoomSpeed = 0.5f;
	[Export]
	private float rotateSpeed = 1.0f;
	[Export]
	private float lerpSpeed = 3.0f;

	[ExportGroup("Follow Distance")]
	[Export]
	private float maxFollowDistance = 7.0f;
	[Export]
	private float minFollowDistance = 1.0f;


	private Vector2 moveSpeed = new();
	private Vector3 rotation;
	private float distance;

	private const float HALF_PI = Mathf.Pi/2.0f;

	/// <summary>
	/// If true, the mouse is locked and the camera moves with the mouse.
	/// Otherwise, the camera stays in place and allows the mouse to move.
	/// </summary>
	private bool orbitingEnabled = false;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		anchor ??= GetParent<Node3D>();
		distance = maxFollowDistance;
		rotation = anchor.Transform.Basis.GetRotationQuaternion().GetEuler();

		//Lock mouse and control camera.
		EnableOrbiting();
	}


	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{	
		rotation.X += -moveSpeed.Y * (float)delta * rotateSpeed;
		rotation.Y += -moveSpeed.X * (float)delta * rotateSpeed;

		if(rotation.X < -HALF_PI)
		{
			rotation.X = -HALF_PI;
		}else if(rotation.X > HALF_PI)
		{
			rotation.X = HALF_PI;
		}

		moveSpeed = new();
		
		SetIdentity();
		TranslateObjectLocal(new Vector3(0.0f, 0.0f, distance));

		Vector3 anchorPos = anchor.Position;

		anchor.SetIdentity();
		Transform3D tempTransform = anchor.Transform;
		tempTransform.Basis = new Basis(Quaternion.FromEuler(rotation));
		anchor.Transform = tempTransform;

		//Move to the player.
		anchorPos = anchorPos.Lerp(player.Position, lerpSpeed * (float)delta);
		anchor.Position = anchorPos;
	}

    public override void _Input(InputEvent @event)
    {
        base._Input(@event);
		if(@event is InputEventMouseMotion mouseMotionEvent)
		{
			moveSpeed = mouseMotionEvent.Relative;
		}else if(@event is InputEventMouseButton mouseButtonEvent)
		{
			float addDist = 0.0f;
			if(mouseButtonEvent.ButtonIndex == MouseButton.WheelDown)
			{
				addDist += zoomSpeed;
			}else if(mouseButtonEvent.ButtonIndex == MouseButton.WheelUp)
			{
				addDist -= zoomSpeed;
			}

			distance = Mathf.Clamp(distance + addDist, minFollowDistance, maxFollowDistance);
		}
    }

	public void EnableOrbiting()
	{
		if(!orbitingEnabled)
		{
			orbitingEnabled = true;
			Input.MouseMode = Input.MouseModeEnum.Captured;
		}
	}

	public void DisableOrbiting()
	{
		if(orbitingEnabled)
		{
			orbitingEnabled = false;
			Input.MouseMode = Input.MouseModeEnum.Visible;
		}
	}

}
