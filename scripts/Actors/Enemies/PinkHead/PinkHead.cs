using Godot;

public class PinkHead : Node2D
{

	#region Nodes

	private Sprite node_sprite;

	private KinematicBody2D node_body;
	private CollisionShape2D node_bodyShape;

	private Area2D node_hitbox;
	private CollisionShape2D node_hitboxShape;

	private Position2D node_projectileSpawnPosition;
	private Timer node_projectileSpawnTimer;

	private Node node_projectiles;

	#endregion // Nodes



	#region Signals

	[Signal] public delegate void OnHit();

	#endregion // Signals



	#region Properties

	[Export] private PackedScene ProjectilePackedScene { get; set; }

	#endregion // Properties



	#region Fields

	private bool m_wasHit = false;
	private bool m_isHit = false;

	#endregion // Fields



	#region Godot methods

	public override void _EnterTree()
	{
		node_sprite = GetNode<Sprite>("Sprite");

		node_body = GetNode<KinematicBody2D>("Body");
		node_bodyShape = node_body.GetNode<CollisionShape2D>("CollisionShape2D");

		node_hitbox = GetNode<Area2D>("Hitbox");
		node_hitboxShape = node_hitbox.GetNode<CollisionShape2D>("CollisionShape2D");

		node_projectileSpawnPosition = GetNode<Position2D>("ProjectileSpawnPosition");
		node_projectileSpawnTimer = GetNode<Timer>("ProjectileSpawnTimer");

		node_projectiles = GetNode<Node>("Projectiles");
	}

	public override void _Ready()
	{
		node_projectileSpawnTimer.Connect("timeout", this, nameof(SpawnProjectile));

		m_originalPosition = Position;
	}

	private float m_timer = 0f;
	private Vector2 m_originalPosition;

	public override void _PhysicsProcess(float delta)
	{
		if (!m_isHit)
		{
			m_timer += delta;

			Vector2 position = Position;

			position.x = m_originalPosition.x + (Mathf.Sin(m_timer * 2) * 10f);

			Position = position;
		}

		if (m_isHit && !m_wasHit)
		{
			m_wasHit = true;

			EmitSignal(nameof(OnHit));
			QueueFree();
		}
	}

	#endregion // Godot methods



	#region Private methods

	private void SpawnProjectile()
	{
		Projectile projectile = ProjectilePackedScene.Instance<Projectile>();
		node_projectiles.AddChild(projectile);
		projectile.GlobalPosition = node_projectileSpawnPosition.GlobalPosition;
		projectile.Rotation = node_projectileSpawnPosition.Rotation;
	}

	private void OnAreaEnteredHitbox(Area2D area)
	{
		m_isHit = true;
	}

	private void OnBodyEnteredHitbox(PhysicsBody2D body)
	{
		m_isHit = true;
	}

	#endregion // Private methods

}

