using Godot;
using System;

public partial class Player : CharacterBody3D
{
	[Export]
	public float Speed { get; set; } = 1.4f;

	[Export]
	public float FallAcceleration { get; set; } = 7.5f;

	[Export]
	public float JumpSpeed { get; set; } = 7.5f;

	private Vector3 _targetVelocity = Vector3.Zero;

	public override void _PhysicsProcess(double delta)
	{
		var direction = Vector3.Zero;

		if (Input.IsActionPressed("move_right"))
		{
			direction.X += 1.0f;
		}

		if (Input.IsActionPressed("move_left"))
		{
			direction.X -= 1.0f;
		}

		if (Input.IsActionPressed("move_forward"))
		{
			direction.Z -= 1.0f;
		}

		if (Input.IsActionPressed("move_back"))
		{
			direction.Z += 1.0f;
		}

		if (direction != Vector3.Zero)
		{
			direction = direction.Normalized();
			GetNode<Node3D>("Pivot").LookAt(Position - direction, Vector3.Up);
		}

		if (Input.IsActionJustPressed("jump") && IsOnFloor())
		{
			direction.Y -= 1;
		}

		// Ground velocity
		_targetVelocity.X = direction.X * Speed;
		_targetVelocity.Z = direction.Z * Speed;
		_targetVelocity.Y = direction.Y * JumpSpeed;

		// Vertical velocity
		if (!IsOnFloor()) // If in the air, fall towards the floor. Literally gravity
		{
			_targetVelocity.Y -= FallAcceleration * (float)delta;
		}

		// Moving the character
		Velocity = _targetVelocity;
		MoveAndSlide();
	}
}
