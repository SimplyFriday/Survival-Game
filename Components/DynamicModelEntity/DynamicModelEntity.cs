using Godot;
using System;

public partial class DynamicModelEntity : CharacterBody3D
{

	[Export]
	public string ModelPath { get; set; }

	public override void _Ready()
	{
		var model = GD.Load<PackedScene>(ModelPath).Instantiate();
		
		var collider = new CollisionShape3D();
		var shape = new CapsuleShape3D();
		var mesh = FindMesh(model);
		var aabb = mesh.GetAabb();

		collider.Shape = shape;
		shape.Radius = Mathf.Min(aabb.Size.X, aabb.Size.Z);
		shape.Height = aabb.Size.Y;
		collider.Transform = new Transform3D(collider.Basis, new Vector3(0,aabb.Size.Y / 2,0));

		collider.Show();

		AddChild(model);
		AddChild(collider);
	}

	// TODO make this more robust so that it can handle more complex models
	private MeshInstance3D FindMesh(Node model)
	{
		foreach (var child in model.GetChildren())
		{
			if (child is MeshInstance3D mesh)
			{
				return mesh;
			}
			else
			{
				return FindMesh(child);
			}
		}

		throw new InvalidOperationException("No MeshInstance3D found in any children");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
