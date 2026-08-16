using Godot;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

public partial class BillboardCharacter : CharacterBody3D
{
	/// <summary>
	/// If the character is currently dead.
	/// </summary>
	public bool Dead{ get; private set;}

	public PlayerInformation Information {get; private set;} = new PlayerInformation("New Player", new Color(0, 0, 1));

	/// <summary>
	/// The speed at which this character moves.
	/// </summary>
	[ExportGroup("Movement")]
	[Export]
	private float speed = 15.0f;

	/// <summary>
	/// How much faster to accelerate when sprinting.
	/// </summary>
	[Export]
	private float sprintingSpeedMultiplier = 1.5f; 

	/// <summary>
	/// The amount of force applied against the motion of this character each physics update.
	/// </summary>
	[Export]
	private float dragMultiplier = 0.05f;

	/// <summary>
	/// The maximum speed this character can move at.
	/// </summary>
	[Export]
	private float terminalMovementSpeed = 200.0f;

	/// <summary>
	/// How fast the character can go when sprinting.
	/// </summary>
	[Export]
	private float terminalSprintSpeed = 300.0f;

	/// <summary>
	/// How much velocity is added vertically to this character when jumping.
	/// </summary>
	[ExportSubgroup("Jump")]
	[Export]
	private float jumpHeight = 100.0f;
	/// <summary>
	/// How much additional velocity is given for holding the jump button after initially jumping.
	/// </summary>
	[Export]
	private float jumpAddition = 15.0f;
	/// <summary>
	/// The maximum amount of time you can hold the jump button to gain additional boost.
	/// </summary>
	[Export]
	private double maxJumpHoldLength = 0.5;
	/// <summary>
	/// How long the character has been gaining force upward by jumping.
	/// </summary>
	private double jumpTime = 0.0;

	/// <summary>
	/// True if the character is sprinting.
	/// </summary>
	private bool sprinting = false;

	/// <summary>
	/// Stores the sum of all movement calls to be applied in the next physics tick.
	/// Gets reset to 0 after a physics update.
	/// </summary>
	private Vector3 currentMovementDirection;

	/// <summary>
	/// A list of the reasons why this character might be unable to move.
	/// </summary>
	List<FreezeReason> frozen = new List<FreezeReason>();

	public enum DeathCause
	{
		NONE,	//No message.
		FELL
	}

	public static readonly String[] DEATH_MESSAGES =
	[
		"",
		" fell"
	];

	public override void _Ready()
	{
		
	}

	public override void _Process(double delta)
	{
		
	}

	/// <summary>
	/// Sets if the character should move using sprint speed or normal walking speed.
	/// </summary>
	/// <param name="sprinting">If the character should be set to be sprinting.</param>
	protected void setSprinting(bool sprinting)
	{
		this.sprinting = sprinting;
	}

	/// <summary>
	/// Sets this character to be unable to move for a given reason (this allows characters to be frozen for multiple reasons at once).
	/// </summary>
	/// <param name="reason">Why the character is frozen.</param>
	/// <param name="setFrozen">If the character should recieve this frozen attribute or if it should be removed.</param>
	public void SetFrozen(FreezeReason reason, bool setFrozen)
	{
		if(frozen.Contains(reason))
		{
			if(!setFrozen)
			{
				frozen.Remove(reason);
			}
		}else
		{
			if(setFrozen)
			{
				frozen.Add(reason);
			}
		}
	}

	/// <summary>
	/// Returns true if this character cannot move currently.
	/// </summary>
	/// <returns></returns>
	public bool IsFrozen()
	{
		return frozen.Count > 0;
	}

	public override void _PhysicsProcess(double delta)
	{
		//Add gravity.
		if (!IsOnFloor())
		{
			this.Velocity += GetGravity() * (float)delta;
		}

		Vector3 movementAcceleration = new Vector3(0, 0, 0);

		//Update motion.
		movementAcceleration += currentMovementDirection * (float)delta;
		
		//Only increase lateral velocity if not exceeding the terminal movement speed.
		Vector3 newVelocity = this.Velocity + movementAcceleration;
		applyDrag(ref newVelocity);

		float terminalSpeed = sprinting ? terminalSprintSpeed : terminalMovementSpeed;

		//Lock each axis to the terminal movement speed.
		if(newVelocity.X > terminalSpeed)
		{
			newVelocity.X = terminalSpeed;
		}else if(newVelocity.X < -terminalSpeed)
		{
			newVelocity.X = -terminalSpeed;
		}

		if(newVelocity.Z > terminalSpeed)
		{
			newVelocity.Z = terminalSpeed;
		}else if(newVelocity.Z < -terminalSpeed)
		{
			newVelocity.Z = -terminalSpeed;
		}

		//Update the movement.
		this.Velocity = newVelocity;

		updateMovementDisplay();
		MoveAndSlide();

		currentMovementDirection = Vector3.Zero;
	}

	/// <summary>
	/// Decreases the input velocity by a multiple of the drag multiplier that increases if the velocity is higher.
	/// </summary>
	/// <param name="velocity">The velocity to apply drag to.</param>
	private void applyDrag(ref Vector3 velocity)
	{
		float drag = dragMultiplier;

		//Less drag in the air.
		if(!IsOnFloor())
		{
			drag /= 10.0f;
		}

		if(Mathf.Abs(velocity.X) > 0.0f)
		{
			velocity.X -= drag * velocity.X;
		}

		if(Mathf.Abs(velocity.Z) > 0.0f)
		{
			velocity.Z -= drag * velocity.Z;
		}
	}

	/// <summary>
	/// Apply a normalized force in the given direction with this character's speed.
	/// </summary>
	/// <param name="direction">The direction to move in.</param>
	public void MoveLaterally(Vector3 direction, float speedMult = 1.0f)
	{
		if(IsFrozen()){ return; }
		Vector3 addDir = direction.Normalized() * speedMult * speed * (sprinting ? sprintingSpeedMultiplier : 1.0f);

		if((this.Velocity.X > 0.0f && direction.X < 0.0f) || (this.Velocity.X < 0.0f && direction.X > 0.0f))
		{
			addDir.X *= 1.75f;
		}

		if((this.Velocity.Z > 0.0f && direction.Z < 0.0f) || (this.Velocity.Z < 0.0f && direction.Z > 0.0f))
		{
			addDir.Z *= 1.75f;
		}

		currentMovementDirection += addDir;
	}

	/// <summary>
	/// Apply an upward force based on jumpHeight.
	/// </summary>
	/// <param name="delta">Current frame delta.</param>
	/// <param name="releasingJump">If this is the last frame of jumping.</param>
	public void Jump(double delta, bool releasingJump = true)
	{
		if(IsOnFloor())
		{
			jumpTime = 0.0f;
		}

		if(jumpTime >= maxJumpHoldLength)
		{
			return;
		}

		if(jumpTime > 0.0)
		{
			currentMovementDirection.Y += jumpAddition;
		}
		else
		{
			currentMovementDirection.Y += jumpHeight;
		}

		//If this is the last frame of jumping, then no future frames should be allowed to jump more even if the max time is reached.
		if(releasingJump)
		{
			jumpTime = maxJumpHoldLength;
		}

		jumpTime += delta;
	}

	/// <summary>
	/// Updates how the character should look based on how they are moving. Based on velocity, rotation, etc.
	/// </summary>
	protected void updateMovementDisplay()
	{
		float targetAngle = Mathf.Atan2(Velocity.X, Velocity.Z);
		RotateY(targetAngle - this.Rotation.Y);
	}

	public void Kill(DeathCause cause = DeathCause.NONE)
	{
		if(Dead){return;}

		string message = DEATH_MESSAGES[(int)cause];
		if(message != "")
		{
			MessageBoard.CreateMessage(MessageBoard.MessageType.ONE_PLAYER_DEATH,
				new Message.Component(Information.Name, Information.Color),
				new Message.Component(message, new Color(1, 1, 1))
			);
		}

		Dead = true;
	}
}
