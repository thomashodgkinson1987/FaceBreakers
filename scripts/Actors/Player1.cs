using Godot;

public class Player1 : Node2D
{

	#region Nodes

	private Node2D node_view;
	private Node2D node_projectiles;
	private Node node_audioStreamPlayers;

	private AnimatedSprite node_animatedSprite;
	private Area2D node_area2D;
	private CollisionShape2D node_collisionShape2D;
	private Position2D node_position2D_projectile;

	private AudioStreamPlayer node_audioStreamPlayer_shoot;
	private AudioStreamPlayer node_audioStreamPlayer_hit;
	private AudioStreamPlayer node_audioStreamPlayer_die;

	#endregion // Nodes



	#region Signals

	[Signal] public delegate void OnShootJustPressed(Player1 player1, Projectile projectile);

	#endregion // Signals



	#region Properties

	[Export] public PackedScene PackedScene_Projectile { get; set; }

	[Export] public int HitPoints { get; set; } = 3;
	[Export] public int MaxHitPoints { get; set; } = 3;

	[Export] public float MoveSpeed { get; set; } = 96f;

	public Vector2 Input_Direction { get; set; } = Vector2.Zero;

	public bool IsInputJustPressed_Shoot { get; set; } = false;
	public bool IsInputPressed_Shoot { get; set; } = false;
	public bool IsInputJustReleased_Shoot { get; set; } = false;

	#endregion // Properties



	#region Fields

	#endregion // Fields



	#region Godot methods

	public override void _EnterTree()
	{
		node_view = GetNode<Node2D>("View");
		node_projectiles = GetNode<Node2D>("Projectiles");
		node_audioStreamPlayers = GetNode<Node>("AudioStreamPlayers");

		node_animatedSprite = node_view.GetNode<AnimatedSprite>("AnimatedSprite");
		node_area2D = node_view.GetNode<Area2D>("Area2D");
		node_collisionShape2D = node_area2D.GetNode<CollisionShape2D>("CollisionShape2D");
		node_position2D_projectile = node_view.GetNode<Position2D>("Position2D_Projectile");

		node_audioStreamPlayer_shoot = node_audioStreamPlayers.GetNode<AudioStreamPlayer>("AudioStreamPlayer_Shoot");
		node_audioStreamPlayer_hit = node_audioStreamPlayers.GetNode<AudioStreamPlayer>("AudioStreamPlayer_Hit");
		node_audioStreamPlayer_die = node_audioStreamPlayers.GetNode<AudioStreamPlayer>("AudioStreamPlayer_Die");
	}

	public override void _Ready() {  }

	public override void _PhysicsProcess(float delta)
	{
		HandleInput(delta);
	}

	public override void _Process(float delta)
	{
		if (Input.IsActionPressed("rotate-left"))
		{
			node_view.Rotate(4 * delta);
		}
		else if (Input.IsActionPressed("rotate-right"))
		{
			node_view.Rotate(-4 * delta);
		}
		else if (Input.IsActionPressed("reset-rotation"))
		{
			node_view.Rotation = 0;
		}

		UpdateInput();
		UpdateAnimation();
	}

	#endregion // Godot methods



	#region Private methods

	private void HandleInput(float delta)
	{
		HandleInput_Direction(delta);
		HandleInput_Shoot();
	}

	private void HandleInput_Direction(float delta)
	{
		Vector2 position = node_view.Position;

		position += Input_Direction * MoveSpeed * delta;

		node_view.Position = position;
	}

	private void HandleInput_Shoot()
	{
		if (IsInputJustPressed_Shoot)
		{
			Projectile projectile = PackedScene_Projectile.Instance() as Projectile;
			projectile.GlobalRotation = node_view.GlobalRotation;
			projectile.GlobalPosition = node_position2D_projectile.GlobalPosition;
			node_projectiles.AddChild(projectile);
			EmitSignal(nameof(OnShootJustPressed), this, projectile);
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

