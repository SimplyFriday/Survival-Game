using Godot;

public partial class MovingBox : Node3D
{
	[Export]
	public Vector3 Origin { get; set; }

	[Export]
	public Vector3 Destination { get; set; }
	
	[Export]
	public float PauseTime { get; set; }

	[Export]
	public float Speed { get; set; }

	private sbyte _direction = 1;
	private float _progress;
	private double _pauseTimer = 0;

	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _PhysicsProcess(double delta)
	{
		if (_pauseTimer <= 0)
		{
			var change = (float)delta * _direction * Speed;
			_progress = Mathf.Clamp(_progress + change, 0, 1);

			Transform = new Transform3D(Basis, Origin.Lerp(Destination, _progress));

			if (_progress == 0 || _progress == 1)
			{
				_pauseTimer = PauseTime;
				_direction *= -1;
			}
		}
		else
		{
			_pauseTimer -= delta;
		}
	}
}
