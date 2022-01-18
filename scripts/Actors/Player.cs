using Godot;

public class Player : Node2D
{

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



	#region Signals

	[Signal] public delegate void OnJustPressed_Shoot(Player player, Projectile projectile);

	[Signal] public delegate void OnTookDamage(Player player);

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

	public void TakeDamage()
	{
		EmitSignal(nameof(OnTookDamage), this);
	}

	public void AddProjectile(Projectile projectile)
	{
		node_projectiles.AddChild(projectile);
	}

	public void RemoveProjectile(Projectile projectile)
	{
		node_projectiles.RemoveChild(projectile);
	}

	public void FreeAllProjectiles()
	{
		//TODO: find out how to cast a godot array to a type
		//Godot.Collections.Array<Projectile> projectiles = node_projectiles.GetChildren();

		int count = node_projectiles.GetChildCount();

		for (int i = 0; i < count; i++)
		{
			Projectile projectile = node_projectiles.GetChild<Projectile>(i);
			projectile.Destroy();
		}

		//foreach (Node projectile in node_projectiles.GetChildren())
		//{
		//	projectile.Destory();
		//}
	}

	#endregion // Public methods



	#region Private methods

	private void HandleInput(float delta)
	{
		HandleInput_Direction(delta);
		HandleInput_Shoot();
	}

	private void HandleInput_Direction(float delta)
	{
		Translate(Input_Direction * MoveSpeed * delta);
	}

	private void HandleInput_Shoot()
	{
		if (IsInputJustPressed_Shoot)
		{
			Projectile projectile = PackedScene_Projectile.Instance() as Projectile;
			node_projectiles.AddChild(projectile);
			projectile.GlobalPosition = node_position2D_projectile.GlobalPosition;
			projectile.Rotation = Rotation;
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

