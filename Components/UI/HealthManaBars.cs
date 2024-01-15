using Godot;
using System;

public partial class HealthManaBars : Control
{
	private Player _player;
	private ProgressBar _hpBar;
	private ProgressBar _manaBar;

	public override void _Ready()
	{
		_player = GetTree().GetFirstNodeInGroup("Player") as Player;
		_manaBar = GetNode<ProgressBar>("VBoxContainer/ManaBar");
		_hpBar = GetNode<ProgressBar>("VBoxContainer/HPBar");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		_manaBar.MaxValue = _player.MaxMana;
		_manaBar.Value = _player.CurrentMana;

		_hpBar.MaxValue = _player.MaxHP;
		_hpBar.Value = _player.CurrentHP;
	}
} 
