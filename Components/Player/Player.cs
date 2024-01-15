using Godot;
using System.Linq;
using System.Collections.Generic;

public partial class Player : DynamicModelEntity
{
	[Export]
	public float Speed { get; set; } = 8f;

	[Export]
	public float FallAcceleration { get; set; } = (float)ProjectSettings.GetSetting("physics/3d/default_gravity");

	[Export]
	public float JumpForce { get; set; } = 16f;

	[Export]
	public float CurrentHP { get; set; } = 100;

	[Export]
	public float CurrentMana { get; set; } = 100;

	[Export]
	public float MaxHP { get; set; } = 100;

	[Export]
	public float MaxMana { get; set; } = 100;

	private AnimationPlayer _animationPlayer;
	private Node3D _cameraPivot;
	private float _jumpMomentum = 0;

	public override void _Ready()
	{
		GetNode<Node3D>("visual_placeholder").Free();

		base._Ready();

		MotionMode = MotionModeEnum.Grounded;

		_animationPlayer = 	GetChildren()
								.First(c => IsInstanceValid(c))
								.GetNode<AnimationPlayer>("AnimationPlayer");

		_cameraPivot = GetParent().GetNode<Node3D>("CameraPivot");

		_animationPlayer.GetAnimation("Idle").LoopMode = Animation.LoopModeEnum.Linear;
		_animationPlayer.GetAnimation("Run").LoopMode = Animation.LoopModeEnum.Linear;

		_animationPlayer.Play("Idle");
	}
	public override void _PhysicsProcess(double delta)
	{
		var input2d = Input.GetVector("move_right","move_left","move_back","move_forward") * Speed;

		Rotation = new Vector3 (0, _cameraPivot.Rotation.Y, 0);
		
		var targetVelocity = Transform.Basis * new Vector3(input2d.X, 0, input2d.Y);
		// basis seems to normalize the new vector automatically. It's required
		// for rotation to work, but fucks up gravity
		targetVelocity.Y = Velocity.Y;

		if (IsOnFloor()) 
		{
			if (Input.IsActionJustPressed("jump"))
			{
				_jumpMomentum = JumpForce;				
			}
		} 
		
		// Character animation
		if (input2d != Vector2.Zero)
		{
			_animationPlayer?.Play("Run");
		} else 
		{
			_animationPlayer?.Play("Idle");
		}

		if (_jumpMomentum > 0)
		{
			targetVelocity.Y += _jumpMomentum * (float)delta;
			_jumpMomentum -= FallAcceleration * FallAcceleration * (float)delta;
		} else if (!IsOnFloor())
		{
			targetVelocity.Y -= FallAcceleration * (float)delta;
		}

		Velocity = targetVelocity;

		MoveAndSlide();
	}

	private IEnumerable<float> GetJumpLerp(double delta)
	{
		var lerpAmt = 0.0f;

		while(lerpAmt < 1)
		{
			lerpAmt += (float)delta;
			var ret = Mathf.CubicInterpolate(	0, 
												JumpForce,
												0,
												0,
												lerpAmt);
				
				//Lerp(0, JumpForce, lerpAmt);
			yield return ret;

			if (IsOnFloor())
			{
				break;
			}
		}
	}
}
