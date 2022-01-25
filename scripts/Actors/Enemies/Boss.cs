using System.Collections.Generic;
using Godot;

public class Boss : Node2D
{

	#region Nodes

	private AnimatedSprite node_animatedSprite;

	private Area2D node_hitbox;
	private CollisionPolygon2D node_hitbox_collisionPolygon;

	private Position2D node_projectileSpawnPosition;
	private Timer node_projectileSpawnTimer;

	private Node node_projectiles;

	private Node node_shields;

	private AnimationPlayer node_animationPlayer1;
	private AnimationPlayer node_animationPlayer2;

	#endregion // Nodes



	#region Signals

	[Signal] public delegate void OnHit(Boss boss);
	[Signal] public delegate void OnDie(Boss boss);

	#endregion // Signals



	#region Properties

	[Export] private PackedScene PackedScene_Projectile { get; set; }
	[Export] public PackedScene PackedScene_DieParticles { get; set; }

	[Export] public List<string> GroupsToIgnore_Area { get; set; }
	[Export] public List<string> GroupsToIgnore_Body { get; set; }

	[Export] public float MinFireWaitTime { get; set; } = 1f;
	[Export] public float MaxFireWaitTime { get; set; } = 3f;

	[Export] public int ScoreValue { get; set; } = 100;

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
		node_hitbox_collisionPolygon = node_hitbox.GetNode<CollisionPolygon2D>("CollisionPolygon2D");

		node_projectileSpawnPosition = GetNode<Position2D>("ProjectileSpawnPosition");
		node_projectileSpawnTimer = GetNode<Timer>("ProjectileSpawnTimer");

		node_projectiles = GetNode<Node>("Projectiles");

		node_shields = GetNode<Node>("Shields");

		node_animationPlayer1 = GetNode<AnimationPlayer>("AnimationPlayer");
		node_animationPlayer2 = node_shields.GetNode<AnimationPlayer>("AnimationPlayer");
	}

	public override void _Ready()
	{
		m_rng = new RandomNumberGenerator();
		m_rng.Randomize();

		float waitTime = m_rng.RandfRange(MinFireWaitTime, MaxFireWaitTime);
		node_projectileSpawnTimer.Start(waitTime);

		node_animatedSprite.Play("idle");

		if (GroupsToIgnore_Area == null)
		{
			GroupsToIgnore_Area = new List<string>();
		}
		if (GroupsToIgnore_Body == null)
		{
			GroupsToIgnore_Body = new List<string>();
		}
	}

	public override void _PhysicsProcess(float delta)
	{
		if (m_isHit && !m_wasHit)
		{
			m_wasHit = true;
			node_animatedSprite.Visible = false;
			node_hitbox_collisionPolygon.Disabled = true;
			node_projectileSpawnTimer.Stop();
			m_dieParticles = PackedScene_DieParticles.Instance<CPUParticles2D>();
			AddChild(m_dieParticles);
			m_dieParticles.Emitting = true;
			EmitSignal(nameof(OnHit), this);
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
			EmitSignal(nameof(OnDie), this);
			RemoveChild(m_dieParticles);
			m_dieParticles.QueueFree();
		}
	}

	#endregion // Godot methods



	#region Public methods

	public void Play_Intro1()
	{
		node_animationPlayer1.Play("boss_intro_0001");
	}
	public void Play_Intro2()
	{
		node_animationPlayer1.Play("boss_intro_0002");
	}
	public void Play_ShowShields()
	{
		node_animationPlayer2.Play("show_shields");
	}

	public void EnableCollision()
	{
		node_hitbox_collisionPolygon.Disabled = false;

		for(int i = 0; i < node_shields.GetChildCount(); i++)
		{
			Shield shield = node_shields.GetChildOrNull<Shield>(i);
			if (shield == null) continue;

			shield.EnableCollision();
		}
	}
	public void DisableCollision()
	{
		node_hitbox_collisionPolygon.Disabled = true;

		for(int i = 0; i < node_shields.GetChildCount(); i++)
		{
			Shield shield = node_shields.GetChildOrNull<Shield>(i);
			if (shield == null) continue;

			shield.DisableCollision();
		}
	}

	#endregion // Public methods



	#region Private methods

	private void OnProjectileSpawnTimerTimeout()
	{
		Projectile projectile = PackedScene_Projectile.Instance<Projectile>();
		projectile.Rotation = node_projectileSpawnPosition.Rotation;
		node_projectiles.AddChild(projectile);
		projectile.GlobalPosition = node_projectileSpawnPosition.GlobalPosition;

		m_rng.Randomize();
		float waitTime = m_rng.RandfRange(MinFireWaitTime, MaxFireWaitTime);
		node_projectileSpawnTimer.Start(waitTime);

		node_animatedSprite.Play("fire");
	}

	private void OnResetAnimationTimerTimeout()
	{
		node_animatedSprite.Play("idle");
	}

	private void OnAreaEnteredHitbox(Area2D area)
	{
		for(int i = 0; i < GroupsToIgnore_Area.Count; i++)
		{
			if (area.Owner.IsInGroup(GroupsToIgnore_Area[i]))
			{
				return;
			}
		}

		m_isHit = true;
	}

	private void OnBodyEnteredHitbox(PhysicsBody2D body)
	{
		for(int i = 0; i < GroupsToIgnore_Body.Count; i++)
		{
			if (body.Owner.IsInGroup(GroupsToIgnore_Body[i]))
			{
				return;
			}
		}

		m_isHit = true;
	}

	#endregion // Private methods

}

