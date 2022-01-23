using System.Collections.Generic;
using Godot;

public class Player : Node2D
{

	#region Enums

	public enum EState { Null, Init, Move, Hit, Die, Free }
	public enum ESound { Shoot, Hit, Die }
	public enum EAnimation { Default, Straight, Left, Right }

	#endregion // Enums



	#region Nodes

	private Node node_sounds;
	private AudioStreamPlayer node_sounds_shoot;
	private AudioStreamPlayer node_sounds_hit;
	private AudioStreamPlayer node_sounds_die;

	private AnimatedSprite node_animatedSprite;

	private Area2D node_hitbox;
	private CollisionShape2D node_hitbox_collisionShape;

	private KinematicBody2D node_body;
	private CollisionShape2D node_body_collisionShape;

	private Position2D node_projectileSpawnPosition;

	private Node node_projectiles;

	#endregion // Nodes



	#region Signals

	[Signal] public delegate void OnHit();

	#endregion // Signals



	#region Properties

	[Export] private PackedScene PackedScene_Projectile { get; set; }
	[Export] public PackedScene PackedScene_DieParticles { get; set; }

	[Export] public float Speed { get; set; } = 96f;
	public Vector2 Velocity { get; set; } = Vector2.Zero;
	public PlayerInputController InputController { get; private set; }

	#endregion // Properties



	#region Fields

	private Dictionary<EState, PlayerState> m_states;
	private Dictionary<ESound, AudioStreamPlayer> m_sounds;
	private Dictionary<EAnimation, string> m_animations;

	private PlayerState m_state;

	#endregion // Fields



	#region Godot methods

	public override void _EnterTree()
	{
		node_sounds = GetNode<Node>("Sounds");
		node_sounds_shoot = node_sounds.GetNode<AudioStreamPlayer>("Shoot");
		node_sounds_hit = node_sounds.GetNode<AudioStreamPlayer>("Hit");
		node_sounds_die = node_sounds.GetNode<AudioStreamPlayer>("Die");

		node_animatedSprite = GetNode<AnimatedSprite>("AnimatedSprite");

		node_body = GetNode<KinematicBody2D>("Body");
		node_body_collisionShape = node_body.GetNode<CollisionShape2D>("CollisionShape2D");

		node_hitbox = GetNode<Area2D>("Hitbox");
		node_hitbox_collisionShape = node_hitbox.GetNode<CollisionShape2D>("CollisionShape2D");

		node_projectileSpawnPosition = GetNode<Position2D>("ProjectileSpawnPosition");

		node_projectiles = GetNode<Node>("Projectiles");
	}

	public override void _Ready()
	{
		m_states = new Dictionary<EState, PlayerState>();
		m_states.Add(EState.Null, new PlayerState_Null(this));
		m_states.Add(EState.Init, new PlayerState_Init(this));
		m_states.Add(EState.Move, new PlayerState_Move(this));
		m_states.Add(EState.Hit, new PlayerState_Hit(this));
		m_states.Add(EState.Die, new PlayerState_Die(this));
		m_states.Add(EState.Free, new PlayerState_Free(this));

		m_sounds = new Dictionary<ESound, AudioStreamPlayer>();
		m_sounds.Add(ESound.Shoot, node_sounds_shoot);
		m_sounds.Add(ESound.Hit, node_sounds_hit);
		m_sounds.Add(ESound.Die, node_sounds_die);

		m_animations = new Dictionary<EAnimation, string>();
		m_animations.Add(EAnimation.Default, "default");
		m_animations.Add(EAnimation.Straight, "straight");
		m_animations.Add(EAnimation.Left, "left");
		m_animations.Add(EAnimation.Right, "right");

		InputController = new PlayerInputController();

		m_state = m_states[EState.Null];

		Set_State(EState.Init);
	}

	public override void _PhysicsProcess(float delta)
	{
		m_state.OnPhysicsProcess(delta);
	}

	public override void _Process(float delta)
	{
		m_state.OnProcess(delta);
	}

	#endregion // Godot methods



	#region Public methods

	public void Set_State(EState state)
	{
		m_state.OnExit();
		m_state = m_states[state];
		m_state.OnEnter();
	}

	public void Set_Animation(EAnimation animation) => node_animatedSprite.Play(m_animations[animation]);

	public void Play_Sound(ESound sound) => m_sounds[sound].Play();
	public void Stop_Sound(ESound sound) => m_sounds[sound].Stop();
	public void Stop_Sound_All()
	{
		foreach(KeyValuePair<ESound, AudioStreamPlayer> kvp in m_sounds)
		{
			kvp.Value.Stop();
		}
	}
	public bool Get_SoundPlaying(ESound sound) => m_sounds[sound].Playing;

	public bool Get_Visible() => node_animatedSprite.Visible;
	public void Set_Visible(bool visible) => node_animatedSprite.Visible = visible;
	public void Toggle_Visible() => node_animatedSprite.Visible = !node_animatedSprite.Visible;

	public bool Get_HitboxEnabled() => !node_hitbox_collisionShape.Disabled;
	public void Set_HitboxEnabled(bool enabled) => node_hitbox_collisionShape.Disabled = !enabled;

	public bool Get_CollisionEnabled() => !node_body_collisionShape.Disabled;
	public void Set_CollisionEnabled(bool enabled) => node_body_collisionShape.Disabled = !enabled;

	public void OnAreaEnteredHitbox(Area2D area) => m_state.OnAreaEnteredHitbox(area);
	public void OnBodyEnteredHitbox(PhysicsBody2D body) => m_state.OnBodyEnteredHitbox(body);

	public void Move(float delta)
	{
		Velocity = node_body.MoveAndSlide(Velocity, Vector2.Zero);
		Position = node_body.GlobalPosition;
		node_body.Position = Vector2.Zero;
	}

	public void ShootProjectile()
	{
		Projectile projectile = PackedScene_Projectile.Instance<Projectile>();
		node_projectiles.AddChild(projectile);
		projectile.Position = node_projectileSpawnPosition.GlobalPosition;
		projectile.Rotation = Rotation;

		Play_Sound(ESound.Shoot);
	}

	public void SetProjectileStates(Projectile.EState state)
	{
		foreach (Projectile projectile in node_projectiles.GetChildren())
		{
			projectile.Set_State(state);
		}
	}

	#endregion // Public methods

}

