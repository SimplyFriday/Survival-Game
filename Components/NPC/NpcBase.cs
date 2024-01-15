using Godot;
using System;

public partial class NpcBase : DynamicModelEntity
{
	private float _gravity = (float)ProjectSettings.GetSetting("physics/3d/default_gravity");

	public override void _Ready()
	{
		base._Ready();
	}


	public override void _PhysicsProcess(double delta)
	{
		var targetVelocity = new Vector3(Velocity.X, 0, Velocity.Z);
		if (!IsOnFloor())
		{
			targetVelocity.Y = Velocity.Y - _gravity * (float)delta;
		}

		Velocity = targetVelocity;
		MoveAndSlide();
	}
}
