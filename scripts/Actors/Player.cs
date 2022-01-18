using Godot;

public class Player : Node2D
{

	#region Nodes

	private AnimatedSprite node_animatedSprite;

	private Area2D node_area2D;
	private CollisionShape2D node_collisionShape2D;

	private Position2D node_position2D_projectile;

	private Node node_audioStreamPlayers;
	private AudioStreamPlayer node_audioStreamPlayer_shoot;
	private AudioStreamPlayer node_audioStreamPlayer_hit;
	private AudioStreamPlayer node_audioStreamPlayer_die;

	private Node node_projectiles;

	#endregion // Nodes



	#region Signals

	[Signal] public delegate void OnJustPressed_Shoot(Player player, Projectile projectile);

	#endregion // Signals



	#region Properties

	[Export] public PackedScene PackedScene_Projectile { get; set; }

	[Export] public float MoveSpeed { get; set; } = 96f;

	public Vector2 Input_Direction { get; set; } = Vector2.Zero;

	public bool IsInputJustPressed_Shoot { get; set; } = false;
	public bool IsInputPressed_Shoot { get; set; } = false;
	public bool IsInputJustReleased_Shoot { get; set; } = false;

	#endregion // Properties



	#region Godot methods

	public override void _EnterTree()
	{
		node_animatedSprite = GetNode<AnimatedSprite>("AnimatedSprite");

		node_area2D = GetNode<Area2D>("Area2D");
		node_collisionShape2D = node_area2D.GetNode<CollisionShape2D>("CollisionShape2D");

		node_position2D_projectile = GetNode<Position2D>("Position2D_Projectile");

		node_audioStreamPlayers = GetNode<Node>("AudioStreamPlayers");
		node_audioStreamPlayer_shoot = node_audioStreamPlayers.GetNode<AudioStreamPlayer>("AudioStreamPlayer_Shoot");
		node_audioStreamPlayer_hit = node_audioStreamPlayers.GetNode<AudioStreamPlayer>("AudioStreamPlayer_Hit");
		node_audioStreamPlayer_die = node_audioStreamPlayers.GetNode<AudioStreamPlayer>("AudioStreamPlayer_Die");

		node_projectiles = GetNode<Node>("Projectiles");
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



	#region Private methods

	private void HandleInput(float delta)
	{
		HandleInput_Direction(delta);
		HandleInput_Shoot();
	}

	private void HandleInput_Direction(float delta)
	{
		Vector2 position = Position;
		position += Input_Direction * MoveSpeed * delta;
		Position = position;
	}

	private void HandleInput_Shoot()
	{
		if (IsInputJustPressed_Shoot)
		{
			Projectile projectile = PackedScene_Projectile.Instance() as Projectile;
			node_projectiles.AddChild(projectile);
			projectile.View.GlobalPosition = node_position2D_projectile.GlobalPosition;
			projectile.View.Rotation = Rotation;
			EmitSignal(nameof(OnJustPressed_Shoot), this, projectile);
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

