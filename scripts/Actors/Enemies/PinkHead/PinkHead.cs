using Godot;

public class PinkHead : Node2D
{

	#region Nodes

	private Sprite node_sprite;

	private CPUParticles2D node_explosionParticles;

	private Area2D node_hitbox;
	private CollisionShape2D node_hitbox_collisionShape;

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

	private bool m_wasFinishedEmitting = false;
	private bool m_isFinishedEmitting = false;

	private Vector2 m_originalPosition = Vector2.Zero;
	private float m_timer = 0f;

	#endregion // Fields



	#region Godot methods

	public override void _EnterTree()
	{
		node_sprite = GetNode<Sprite>("Sprite");

		node_explosionParticles = GetNode<CPUParticles2D>("ExplosionParticles");

		node_hitbox = GetNode<Area2D>("Hitbox");
		node_hitbox_collisionShape = node_hitbox.GetNode<CollisionShape2D>("CollisionShape2D");

		node_projectileSpawnPosition = GetNode<Position2D>("ProjectileSpawnPosition");
		node_projectileSpawnTimer = GetNode<Timer>("ProjectileSpawnTimer");

		node_projectiles = GetNode<Node>("Projectiles");
	}

	public override void _Ready()
	{
		node_projectileSpawnTimer.Connect("timeout", this, nameof(SpawnProjectile));

		m_originalPosition = Position;
	}

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
			node_sprite.Visible = false;
			node_hitbox_collisionShape.Disabled = true;
			node_explosionParticles.Restart();
			node_explosionParticles.Emitting = true;
			node_projectileSpawnTimer.Stop();
		}

		if (m_isHit && !m_wasFinishedEmitting)
		{
			if (!node_explosionParticles.Emitting && node_projectiles.GetChildCount() == 0)
			{
				m_isFinishedEmitting = true;
			}
		}

		if (m_isFinishedEmitting && !m_wasFinishedEmitting)
		{
			m_wasFinishedEmitting = true;
			EmitSignal(nameof(OnHit));
			QueueFree();
		}
	}

	#endregion // Godot methods



	#region Private methods

	private void SpawnProjectile()
	{
		Projectile projectile = ProjectilePackedScene.Instance<Projectile>();
		projectile.Rotation = node_projectileSpawnPosition.Rotation;
		node_projectiles.AddChild(projectile);
		projectile.GlobalPosition = node_projectileSpawnPosition.GlobalPosition;
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

