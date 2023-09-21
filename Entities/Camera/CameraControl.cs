using System.Linq;
using Godot;
public partial class CameraControl : Node3D
{
	[Export]
	public float Sensitivity { get; set; } = 1.0f;

	[Export]
	public float DistanceStep { get; set; } = 0.5f;

	[Export]
	public string FollowObjectPath { get; set; }

	// Constant to slow rotation down to a manageable speed, such that Sensitivity can be a single digit number
	private float _rotationScaling = 0.0005f;

	private Camera3D _camera;
	private Node3D _desiredPosition;
	private RayCast3D _collider;
	private Node3D _followedNode;

	public override void _Ready()
	{
		base._Ready();

		_followedNode = GetParent().GetNode<Node3D>(FollowObjectPath);
		_camera = GetNode<Camera3D>("Camera");
		
		_desiredPosition = GetNode<Node3D>("DesiredPosition");	
		_desiredPosition.LookAt(Transform.Origin);

		_collider = GetNode<RayCast3D>("CameraCollider");
		
		// Prevent the object being followed from causing collisions
		if (_followedNode is CollisionObject3D colObj)
		{
			_collider.AddException(colObj);
		}
	}

	public override void _Process(double delta)
	{
		base._Process(delta);

		if (_followedNode != null)
		{
			Position = _followedNode.Position;
		}

		if (_collider.IsColliding())
		{
			_camera.GlobalTransform = new Transform3D(_camera.GlobalTransform.Basis,
														_camera.GlobalTransform.Origin.Lerp(
															_collider.GetCollisionPoint(),
															0.1f));
		}
		else
		{
			_camera.GlobalTransform = new Transform3D(_camera.GlobalTransform.Basis,
														_desiredPosition.GlobalTransform.Origin);
		}
	}

	public override void _UnhandledInput(InputEvent inputEvent)
	{
		base._UnhandledInput(inputEvent);

		if (Input.IsActionJustPressed("unlock_camera"))
		{
			Input.MouseMode = Input.MouseModeEnum.Captured;
		}
		else if (!Input.IsActionPressed("unlock_camera"))
		{
			Input.MouseMode = Input.MouseModeEnum.Visible;
		}

		if (inputEvent is InputEventMouseMotion mouseEvent &&
			Input.MouseMode == Input.MouseModeEnum.Captured)
		{
			RotateObjectLocal(Vector3.Left,
								mouseEvent.Relative.Y * _rotationScaling * Sensitivity);

			Rotate(Vector3.Down,
					mouseEvent.Relative.X * _rotationScaling * Sensitivity);

			// Clamp rotation so that it can't go upside down or under the floor
			Rotation = new Vector3(Mathf.Clamp(Rotation.X, -1.4f, 0),
									Rotation.Y,
									0);
		}

		if (Input.IsActionJustPressed("zoom_in"))
		{
			if (_desiredPosition.Transform.Origin.Y > 2)
			{
				_desiredPosition.Transform = ShiftCamera(-DistanceStep);
			}
		}

		if (Input.IsActionJustPressed("zoom_out"))
		{
			if (_desiredPosition.Transform.Origin.Y < 12)
			{
				_desiredPosition.Transform = ShiftCamera(DistanceStep);
			}
		}
	}

	private Transform3D ShiftCamera(float moveDistance)
	{
		var t = new Transform3D(	_desiredPosition.Basis, 
									new Vector3 (
										_desiredPosition.Transform.Origin.X, 
										_desiredPosition.Transform.Origin.Y + moveDistance,
										_desiredPosition.Transform.Origin.Z));

		_collider.Scale = new Vector3(1,t.Origin.Y,1);

		return t;
	}
}
