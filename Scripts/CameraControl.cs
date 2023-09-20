using Godot;
public partial class CameraControl : Node3D 
{
	[Export]
	public float Sensitivity {get; set;} = 1.0f;

	[Export]
	public int FovStep {get; set;} = 5;
	
	[Export]
	public string FollowObjectPath {get; set;}

	// Constant to slow rotation down to a manageable speed, such that Sensitivity can be a single digit number
	private float _rotationScaling  = 0.005f;

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
		_collider = GetNode<RayCast3D>("CameraCollider");
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
			//GlobalTransform = new Transform(GlobalTransform.basis, SPATIAL_TO_MOVE_TO.GlobalTransform.origin);
			_camera.GlobalTransform = new Transform3D(	_camera.GlobalTransform.Basis,
														_camera.GlobalTransform.Origin.Lerp(
															_collider.GetCollisionPoint(), 
															0.1f));
		} else 
		{
			_camera.GlobalTransform = new Transform3D(	_camera.GlobalTransform.Basis,
														_desiredPosition.GlobalTransform.Origin);
		}
	}

	public override void _UnhandledInput(InputEvent inputEvent)
	{
		base._UnhandledInput(inputEvent);

		if ( Input.IsActionJustPressed("unlock_camera"))
		{
			Input.MouseMode = Input.MouseModeEnum.Captured;
		} 
		else if (Input.IsActionPressed("unlock_camera"))
		{
			Input.MouseMode = Input.MouseModeEnum.Visible;
		}

		if (inputEvent is InputEventMouseMotion mouseEvent &&
			Input.IsActionPressed("unlock_camera"))
		{
			RotateObjectLocal(	Vector3.Left, 
								mouseEvent.Relative.Y * _rotationScaling * Sensitivity);

			Rotate(	Vector3.Down, 
					mouseEvent.Relative.X * _rotationScaling * Sensitivity);

			// Clamp rotation so that it can't go upside down or under the floor
			Rotation = new Vector3(	Mathf.Clamp(Rotation.X, -1.4f, 0), 
									Rotation.Y, 
									0);
		}

		if (Input.IsActionJustPressed("zoom_in")) 
		{
			var cam = GetNode<Camera3D>("Camera");
			var sanitized = Mathf.Clamp(cam.Fov -= FovStep, 40, 100);
			cam.Fov = sanitized;
		}

		if (Input.IsActionJustPressed("zoom_out"))
		{
			var cam = GetNode<Camera3D>("Camera");
			var sanitized = Mathf.Clamp(cam.Fov += FovStep, 40, 100);
			cam.Fov = sanitized;
		}
	}
}
