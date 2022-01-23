using Godot;

public class GreyHead : Node2D
{

	#region Nodes

	private AnimatedSprite node_animatedSprite;

	private Area2D node_hitbox;
	private CollisionShape2D node_hitbox_collisionShape;

	private Position2D node_projectileSpawnPosition;
	private Timer node_projectileSpawnTimer;

	private Timer node_resetAnimationTimer;

	private Node node_projectiles;

	#endregion // Nodes



	#region Signals

	[Signal] public delegate void OnHit();

	#endregion // Signals



	#region Properties

	[Export] private PackedScene ProjectilePackedScene { get; set; }
	[Export] public PackedScene PackedScene_DieParticles { get; set; }

	[Export] public float MinFireWaitTime { get; set; } = 2f;
	[Export] public float MaxFireWaitTime { get; set; } = 5f;

	#endregion // Properties



	#region Fields

	private bool m_wasHit = false;
	private bool m_isHit = false;

	private bool m_wasDie = false;
	private bool m_isDie = false;

	private CPUParticles2D m_dieParticles;

	private RandomNumberGenerator m_rng;

	#endregion // Fields



	#region Godot methods

	public override void _EnterTree()
	{
		node_animatedSprite = GetNode<AnimatedSprite>("AnimatedSprite");

		node_hitbox = GetNode<Area2D>("Hitbox");
		node_hitbox_collisionShape = node_hitbox.GetNode<CollisionShape2D>("CollisionShape2D");

		node_projectileSpawnPosition = GetNode<Position2D>("ProjectileSpawnPosition");
		node_projectileSpawnTimer = GetNode<Timer>("ProjectileSpawnTimer");

		node_resetAnimationTimer = GetNode<Timer>("ResetAnimationTimer");

		node_projectiles = GetNode<Node>("Projectiles");
	}

	public override void _Ready()
	{
		m_rng = new RandomNumberGenerator();
		m_rng.Randomize();

		float waitTime = m_rng.RandfRange(MinFireWaitTime, MaxFireWaitTime);
		node_projectileSpawnTimer.Start(waitTime);

		node_animatedSprite.Play("idle");
	}

	public override void _PhysicsProcess(float delta)
	{
		if (m_isHit && !m_wasHit)
		{
			m_wasHit = true;
			node_animatedSprite.Visible = false;
			node_hitbox_collisionShape.Disabled = true;
			node_projectileSpawnTimer.Stop();
			m_dieParticles = PackedScene_DieParticles.Instance<CPUParticles2D>();
			AddChild(m_dieParticles);
			m_dieParticles.Emitting = true;
		}

		if (m_wasHit && !m_isDie)
		{
			if (!m_dieParticles.Emitting && node_projectiles.GetChildCount() == 0)
			{
				m_isDie = true;
			}
		}

		if (m_isDie && !m_wasDie)
		{
			m_wasDie = true;
			EmitSignal(nameof(OnHit));
			RemoveChild(m_dieParticles);
			m_dieParticles.QueueFree();
			QueueFree();
		}
	}

	#endregion // Godot methods



	#region Private methods

	private void OnProjectileSpawnTimerTimeout()
	{
		Projectile projectile = ProjectilePackedScene.Instance<Projectile>();
		projectile.Rotation = node_projectileSpawnPosition.Rotation;
		node_projectiles.AddChild(projectile);
		projectile.GlobalPosition = node_projectileSpawnPosition.GlobalPosition;

		m_rng.Randomize();
		float waitTime = m_rng.RandfRange(MinFireWaitTime, MaxFireWaitTime);
		node_projectileSpawnTimer.Start(waitTime);

		node_animatedSprite.Play("fire");
		node_resetAnimationTimer.Start();
	}

	private void OnResetAnimationTimerTimeout()
	{
		node_animatedSprite.Play("idle");
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

