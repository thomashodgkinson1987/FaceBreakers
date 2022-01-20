using System.Collections.Generic;
using Godot;

public class Player : Node2D
{

	#region Enums

	public enum ESound { Shoot, Hit, Die }

	#endregion // Enums



	#region Nodes

	private Node node_audioStreamPlayers;
	private AudioStreamPlayer node_audioStreamPlayer_shoot;
	private AudioStreamPlayer node_audioStreamPlayer_hit;
	private AudioStreamPlayer node_audioStreamPlayer_die;

	private AnimatedSprite node_animatedSprite;

	private KinematicBody2D node_kinematicBody2D;
	private CollisionShape2D node_collisionShape2D;

	private Position2D node_position2D_projectile;

	private Node node_projectiles;

	#endregion // Nodes



	#region Properties

	[Export] public PackedScene PackedScene_Projectile { get; set; }

	[Export] public float Speed { get; set; } = 96f;

	[Export] public Vector2 Velocity { get; set; } = Vector2.Zero;

	public Vector2 Input_Direction { get; set; } = Vector2.Zero;

	public bool IsInputJustPressed_Shoot { get; set; } = false;
	public bool IsInputPressed_Shoot { get; set; } = false;
	public bool IsInputJustReleased_Shoot { get; set; } = false;

	#endregion // Properties



	#region Fields

	private Dictionary<ESound, AudioStreamPlayer> m_sounds;

	#endregion // Fields



	#region Godot methods

	public override void _EnterTree()
	{
		node_audioStreamPlayers = GetNode<Node>("AudioStreamPlayers");
		node_audioStreamPlayer_shoot = node_audioStreamPlayers.GetNode<AudioStreamPlayer>("AudioStreamPlayer_Shoot");
		node_audioStreamPlayer_hit = node_audioStreamPlayers.GetNode<AudioStreamPlayer>("AudioStreamPlayer_Hit");
		node_audioStreamPlayer_die = node_audioStreamPlayers.GetNode<AudioStreamPlayer>("AudioStreamPlayer_Die");

		node_animatedSprite = GetNode<AnimatedSprite>("AnimatedSprite");

		node_kinematicBody2D = GetNode<KinematicBody2D>("KinematicBody2D");
		node_collisionShape2D = node_kinematicBody2D.GetNode<CollisionShape2D>("CollisionShape2D");

		node_position2D_projectile = GetNode<Position2D>("Position2D_Projectile");

		node_projectiles = GetNode<Node>("Projectiles");
	}

	public override void _Ready()
	{
		m_sounds = new Dictionary<ESound, AudioStreamPlayer>();
		m_sounds.Add(ESound.Shoot, node_audioStreamPlayer_shoot);
		m_sounds.Add(ESound.Hit, node_audioStreamPlayer_hit);
		m_sounds.Add(ESound.Die, node_audioStreamPlayer_die);
	}

	public override void _PhysicsProcess(float delta)
	{
		HandleInput(delta);
	}

	public override void _Process(float delta)
	{
		UpdateInput();
		UpdateAnimation();
	}

	#endregion // Godot methods



	#region Public methods

	public void Play_Sound(ESound sound) => m_sounds[sound].Play();
	public bool Get_SoundPlaying(ESound sound) => m_sounds[sound].Playing;

	#endregion // Public methods



	#region Private methods

	private void HandleInput(float delta)
	{
		HandleInput_Direction(delta);
		HandleInput_Shoot();
	}

	private void HandleInput_Direction(float delta)
	{
		Vector2 position = Position;
		Vector2 velocity = Velocity;

		velocity = Input_Direction * Speed;
		velocity = node_kinematicBody2D.MoveAndSlide(velocity, Vector2.Zero);
		position = node_kinematicBody2D.GlobalPosition;

		node_kinematicBody2D.Position = Vector2.Zero;

		Velocity = velocity;
		Position = position;
	}

	private void HandleInput_Shoot()
	{
		if (IsInputJustPressed_Shoot)
		{
			Projectile projectile = PackedScene_Projectile.Instance() as Projectile;
			node_projectiles.AddChild(projectile);
			projectile.GlobalPosition = node_position2D_projectile.GlobalPosition;
			projectile.Rotation = Rotation;
		}
	}

	private void UpdateInput()
	{
		UpdateInput_Direction();
		UpdateInput_Shoot();
	}

	private void UpdateInput_Direction()
	{
		Vector2 input_direction = Vector2.Zero;

		if (Input.IsActionPressed("player_move_left")) input_direction.x -= 1;
		if (Input.IsActionPressed("player_move_right")) input_direction.x += 1;
		if (Input.IsActionPressed("player_move_up")) input_direction.y -= 1;
		if (Input.IsActionPressed("player_move_down")) input_direction.y += 1;

		Input_Direction = input_direction;
	}

	private void UpdateInput_Shoot()
	{
		IsInputJustPressed_Shoot = Input.IsActionJustPressed("player_shoot");
		IsInputPressed_Shoot = Input.IsActionPressed("player_shoot");
		IsInputJustReleased_Shoot = Input.IsActionJustReleased("player_shoot");
	}

	private void UpdateAnimation()
	{
		if (Input_Direction.x == 0)
		{
			node_animatedSprite.Play("straight");
		}
		else if (Input_Direction.x == -1)
		{
			node_animatedSprite.Play("left");
		}
		else if (Input_Direction.x == 1)
		{
			node_animatedSprite.Play("right");
		}
		else
		{
			node_animatedSprite.Play("default");
		}
	}

	#endregion // Private methods

}

