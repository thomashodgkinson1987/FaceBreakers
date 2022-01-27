using System.Collections.Generic;
using Godot;

public class Boss : Node2D
{

	#region Nodes

	private AnimatedSprite node_animatedSprite;

	private Area2D node_hitbox;
	private CollisionPolygon2D node_hitbox_collisionPolygon;

	private Position2D node_projectileSpawnPosition;

	private Node node_projectiles;

	private Node node_shields;

	private AnimationPlayer node_animationPlayer1;
	private AnimationPlayer node_animationPlayer2;

	private Node node_sounds;

	private AudioStreamPlayer node_audioStreamPlayer1;
	private AudioStreamPlayer node_audioStreamPlayer2;
	private AudioStreamPlayer node_audioStreamPlayer3;
	private AudioStreamPlayer node_audioStreamPlayer4;
	private AudioStreamPlayer node_audioStreamPlayer5;
	private AudioStreamPlayer node_audioStreamPlayer6;
	private AudioStreamPlayer node_audioStreamPlayer7;
	private AudioStreamPlayer node_audioStreamPlayer8;
	private AudioStreamPlayer node_audioStreamPlayer9;
	private AudioStreamPlayer node_audioStreamPlayer10;
	private AudioStreamPlayer node_audioStreamPlayer11;
	private AudioStreamPlayer node_audioStreamPlayer12;
	private AudioStreamPlayer node_audioStreamPlayer13;
	private AudioStreamPlayer node_audioStreamPlayer14;
	private AudioStreamPlayer node_audioStreamPlayer15;
	private AudioStreamPlayer node_audioStreamPlayer16;

	#endregion // Nodes



	#region Signals

	[Signal] public delegate void OnHit(Boss boss);
	[Signal] public delegate void OnDie(Boss boss);

	#endregion // Signals



	#region Properties

	[Export] private PackedScene PackedScene_Projectile { get; set; }
	[Export] private PackedScene PackedScene_PizzaProjectile { get; set; }
	[Export] public PackedScene PackedScene_DieParticles { get; set; }

	[Export] public List<string> GroupsToIgnore_Area { get; set; }
	[Export] public List<string> GroupsToIgnore_Body { get; set; }

	[Export] public int ScoreValue { get; set; } = 100000;

	[Export] public int Health { get; set; } = 100;
	[Export] public int MaxHealth { get; set; } = 100;

	[Export] public Color MaxHealthColor { get; set; } = Colors.White;
	[Export] public Color NoHealthColor { get; set; } = Colors.Red;

	#endregion // Properties



	#region Fields

	private RandomNumberGenerator m_rng;

	private bool m_wasHit = false;
	private bool m_isHit = false;

	private bool m_wasDie = false;
	private bool m_isDie = false;

	private bool m_isReturn = false;
	private bool m_wasReturn = false;

	private int m_hitCounter = 0;

	private Vector2 m_diePosition = Vector2.Zero;

	private float m_returnTime = 4f;
	private float m_returnTimeTimer = 0f;

	private int m_lastChoice = 0;

	private CPUParticles2D m_dieParticles;

	private List<AudioStreamPlayer> m_allSounds = new List<AudioStreamPlayer>();
	private List<AudioStreamPlayer> m_entranceSounds = new List<AudioStreamPlayer>();
	private List<AudioStreamPlayer> m_notHurtSounds = new List<AudioStreamPlayer>();
	private List<AudioStreamPlayer> m_minorHurtSounds = new List<AudioStreamPlayer>();
	private List<AudioStreamPlayer> m_hurtSounds = new List<AudioStreamPlayer>();
	private List<AudioStreamPlayer> m_majorHurtSounds = new List<AudioStreamPlayer>();
	private List<AudioStreamPlayer> m_dieSounds = new List<AudioStreamPlayer>();
	private List<AudioStreamPlayer> m_miscSounds = new List<AudioStreamPlayer>();

	#endregion // Fields



	#region Godot methods

	public override void _EnterTree()
	{
		node_animatedSprite = GetNode<AnimatedSprite>("AnimatedSprite");

		node_hitbox = GetNode<Area2D>("Hitbox");
		node_hitbox_collisionPolygon = node_hitbox.GetNode<CollisionPolygon2D>("CollisionPolygon2D");

		node_projectileSpawnPosition = GetNode<Position2D>("ProjectileSpawnPosition");

		node_projectiles = GetNode<Node>("Projectiles");

		node_shields = GetNode<Node>("Shields");

		node_animationPlayer1 = GetNode<AnimationPlayer>("AnimationPlayer");
		node_animationPlayer2 = node_shields.GetNode<AnimationPlayer>("AnimationPlayer");

		node_sounds = GetNode<Node>("Sounds");

		node_audioStreamPlayer1 = node_sounds.GetNode<AudioStreamPlayer>("AudioStreamPlayer1");
		node_audioStreamPlayer2 = node_sounds.GetNode<AudioStreamPlayer>("AudioStreamPlayer2");
		node_audioStreamPlayer3 = node_sounds.GetNode<AudioStreamPlayer>("AudioStreamPlayer3");
		node_audioStreamPlayer4 = node_sounds.GetNode<AudioStreamPlayer>("AudioStreamPlayer4");
		node_audioStreamPlayer5 = node_sounds.GetNode<AudioStreamPlayer>("AudioStreamPlayer5");
		node_audioStreamPlayer6 = node_sounds.GetNode<AudioStreamPlayer>("AudioStreamPlayer6");
		node_audioStreamPlayer7 = node_sounds.GetNode<AudioStreamPlayer>("AudioStreamPlayer7");
		node_audioStreamPlayer8 = node_sounds.GetNode<AudioStreamPlayer>("AudioStreamPlayer8");
		node_audioStreamPlayer9 = node_sounds.GetNode<AudioStreamPlayer>("AudioStreamPlayer9");
		node_audioStreamPlayer10 = node_sounds.GetNode<AudioStreamPlayer>("AudioStreamPlayer10");
		node_audioStreamPlayer11 = node_sounds.GetNode<AudioStreamPlayer>("AudioStreamPlayer11");
		node_audioStreamPlayer12 = node_sounds.GetNode<AudioStreamPlayer>("AudioStreamPlayer12");
		node_audioStreamPlayer13 = node_sounds.GetNode<AudioStreamPlayer>("AudioStreamPlayer13");
		node_audioStreamPlayer14 = node_sounds.GetNode<AudioStreamPlayer>("AudioStreamPlayer14");
		node_audioStreamPlayer15 = node_sounds.GetNode<AudioStreamPlayer>("AudioStreamPlayer15");
		node_audioStreamPlayer16 = node_sounds.GetNode<AudioStreamPlayer>("AudioStreamPlayer16");

		m_allSounds.Add(node_audioStreamPlayer1);
		m_allSounds.Add(node_audioStreamPlayer2);
		m_allSounds.Add(node_audioStreamPlayer3);
		m_allSounds.Add(node_audioStreamPlayer4);
		m_allSounds.Add(node_audioStreamPlayer5);
		m_allSounds.Add(node_audioStreamPlayer6);
		m_allSounds.Add(node_audioStreamPlayer7);
		m_allSounds.Add(node_audioStreamPlayer8);
		m_allSounds.Add(node_audioStreamPlayer9);
		m_allSounds.Add(node_audioStreamPlayer10);
		m_allSounds.Add(node_audioStreamPlayer11);
		m_allSounds.Add(node_audioStreamPlayer12);
		m_allSounds.Add(node_audioStreamPlayer13);
		m_allSounds.Add(node_audioStreamPlayer14);
		m_allSounds.Add(node_audioStreamPlayer15);
		m_allSounds.Add(node_audioStreamPlayer16);

		m_entranceSounds.Add(node_audioStreamPlayer4);
		m_entranceSounds.Add(node_audioStreamPlayer5);

		m_notHurtSounds.Add(node_audioStreamPlayer13);
		
		m_minorHurtSounds.Add(node_audioStreamPlayer12);
		m_minorHurtSounds.Add(node_audioStreamPlayer15);
		m_minorHurtSounds.Add(node_audioStreamPlayer16);

		m_hurtSounds.Add(node_audioStreamPlayer6);
		m_hurtSounds.Add(node_audioStreamPlayer7);
		m_hurtSounds.Add(node_audioStreamPlayer8);
		
		m_majorHurtSounds.Add(node_audioStreamPlayer10);
		
		m_dieSounds.Add(node_audioStreamPlayer9);
		m_dieSounds.Add(node_audioStreamPlayer11);
		
		m_miscSounds.Add(node_audioStreamPlayer14);
	}

	public override void _Ready()
	{
		m_rng = new RandomNumberGenerator();
		m_rng.Randomize();

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
		if (m_isHit)
		{
			m_isHit = false;

			Health--;

			if (Health <= 0)
			{
				Health = 0;
				node_animatedSprite.SelfModulate = NoHealthColor;
			}
			else
			{
				node_animatedSprite.SelfModulate = NoHealthColor.LinearInterpolate(MaxHealthColor, (float)Health / (float)MaxHealth);
			}

			m_hitCounter++;

			if (m_hitCounter == 10)
			{
				m_notHurtSounds[0].Play();
			}
			else if (m_hitCounter == 20)
			{
				m_minorHurtSounds[0].Play();
			}
			else if (m_hitCounter == 30)
			{
				m_minorHurtSounds[1].Play();
			}
			else if (m_hitCounter == 40)
			{
				m_minorHurtSounds[2].Play();
			}
			else if (m_hitCounter == 50)
			{
				m_hurtSounds[0].Play();
			}
			else if (m_hitCounter == 60)
			{
				m_hurtSounds[1].Play();
			}
			else if (m_hitCounter == 70)
			{
				m_hurtSounds[2].Play();
			}
			else if (m_hitCounter == 80)
			{
				m_majorHurtSounds[0].Play();
			}
			else if (m_hitCounter == 90)
			{
				m_dieSounds[1].Play();
			}
			else if (m_hitCounter == 100)
			{
				m_wasHit = true;
				m_isDie = true;
				node_animationPlayer1.Stop();
				GetNode<CPUParticles2D>("AnimatedSprite/RayParticlesHolder/CPUParticles2D").Emitting = false;
				GetNode<CollisionShape2D>("AnimatedSprite/RayParticlesHolder/Area2D/CollisionShape2D").Disabled = true;
				DisableBossCollision();
				DisableShieldCollision();
				m_diePosition = Position;
			}

			EmitSignal(nameof(OnHit), this);
		}

		if (m_isDie && !m_wasDie)
		{
			m_wasDie = true;
		}

		if (m_isDie && m_wasDie && !m_isReturn)
		{
			if (m_returnTimeTimer < m_returnTime)
			{
				m_returnTimeTimer += delta;

				Vector2 centre = new Vector2(128, 128);

				if (m_returnTimeTimer < m_returnTime)
				{
					Position = m_diePosition.LinearInterpolate(centre, m_returnTimeTimer / m_returnTime);
				}
				else
				{
					Position = centre;
					m_isReturn = true;
				}
			}
		}

		if (m_isReturn && !m_wasReturn)
		{
			m_isReturn = false;
			m_wasReturn = true;

			m_dieSounds[0].Play();
			node_animationPlayer1.Play("die_0001");
		}

		if (!m_isReturn && m_wasReturn)
		{
			if (!node_animationPlayer1.IsPlaying())
			{
				m_isReturn = true;
				EmitSignal(nameof(OnDie), this);
			}
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

	public void PlayRandomMove()
	{
		int choice = m_rng.RandiRange(1, 3);

		while (choice == m_lastChoice)
		{
			choice = m_rng.RandiRange(1, 3);
		}

		m_lastChoice = choice;

		node_animationPlayer1.Play($"move_000{choice}");
	}

	public void EnableBossCollision()
	{
		node_hitbox_collisionPolygon.Disabled = false;
	}

	public void DisableBossCollision()
	{
		node_hitbox_collisionPolygon.Disabled = true;
	}

	public void EnableShieldCollision()
	{
		for(int i = 0; i < node_shields.GetChildCount(); i++)
		{
			Shield shield = node_shields.GetChildOrNull<Shield>(i);
			if (shield == null) continue;

			shield.EnableCollision();
		}
	}

	public void DisableShieldCollision()
	{
		for(int i = 0; i < node_shields.GetChildCount(); i++)
		{
			Shield shield = node_shields.GetChildOrNull<Shield>(i);
			if (shield == null) continue;

			shield.DisableCollision();
		}
	}

	#endregion // Public methods



	#region Private methods

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

	private void FireProjectile()
	{
		if (m_rng.RandiRange(0, 3) > 0)
		{
			FireNormalProjectile();
		}
		else
		{
			FirePizzaProjectile();
		}
	}

	private void FireNormalProjectile()
	{
		Projectile projectile = PackedScene_Projectile.Instance<Projectile>();
		projectile.Rotation = node_projectileSpawnPosition.Rotation;
		node_projectiles.AddChild(projectile);
		projectile.GlobalPosition = node_projectileSpawnPosition.GlobalPosition;
	}

	private void FirePizzaProjectile()
	{
		PizzaProjectile projectile = PackedScene_PizzaProjectile.Instance<PizzaProjectile>();
		projectile.Rotation = node_projectileSpawnPosition.Rotation;
		node_projectiles.AddChild(projectile);
		projectile.GlobalPosition = node_projectileSpawnPosition.GlobalPosition;
	}

	#endregion // Private methods

}

