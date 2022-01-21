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

	private Node node_audioStreamPlayers;
	private AudioStreamPlayer node_audioStreamPlayer_shoot;
	private AudioStreamPlayer node_audioStreamPlayer_hit;
	private AudioStreamPlayer node_audioStreamPlayer_die;

	private AnimatedSprite node_animatedSprite;

	private KinematicBody2D node_kinematicBody2D_body;
	private CollisionShape2D node_collisionShape2D_body;

	private Area2D node_area2D_hitbox;
	private CollisionShape2D node_collisionShape2D_hitbox;

	private Position2D node_position2D_projectile;

	private Node node_projectiles;

	#endregion // Nodes



	#region Signals

	[Signal] public delegate void OnHit();

	#endregion // Signals



	#region Properties

	[Export] public PackedScene PackedScene_Projectile { get; set; }

	[Export] public float Speed { get; set; } = 96f;
	
	public Vector2 Velocity { get; set; } = Vector2.Zero;

	public PlayerInputController InputController { get; private set; }

	#endregion // Properties



	#region Fields

	private Dictionary<EState, PlayerState> m_states;
	private PlayerState m_state;

	private Dictionary<ESound, AudioStreamPlayer> m_sounds;

	private Dictionary<EAnimation, string> m_animations;

	#endregion // Fields



	#region Godot methods

	public override void _EnterTree()
	{
		node_audioStreamPlayers = GetNode<Node>("AudioStreamPlayers");
		node_audioStreamPlayer_shoot = node_audioStreamPlayers.GetNode<AudioStreamPlayer>("AudioStreamPlayer_Shoot");
		node_audioStreamPlayer_hit = node_audioStreamPlayers.GetNode<AudioStreamPlayer>("AudioStreamPlayer_Hit");
		node_audioStreamPlayer_die = node_audioStreamPlayers.GetNode<AudioStreamPlayer>("AudioStreamPlayer_Die");

		node_animatedSprite = GetNode<AnimatedSprite>("AnimatedSprite");

		node_kinematicBody2D_body = GetNode<KinematicBody2D>("KinematicBody2D_Body");
		node_collisionShape2D_body = node_kinematicBody2D_body.GetNode<CollisionShape2D>("CollisionShape2D");

		node_area2D_hitbox = GetNode<Area2D>("Area2D_Hitbox");
		node_collisionShape2D_hitbox = node_area2D_hitbox.GetNode<CollisionShape2D>("CollisionShape2D");

		node_position2D_projectile = GetNode<Position2D>("Position2D_Projectile");

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

		m_state = m_states[EState.Null];

		m_sounds = new Dictionary<ESound, AudioStreamPlayer>();
		m_sounds.Add(ESound.Shoot, node_audioStreamPlayer_shoot);
		m_sounds.Add(ESound.Hit, node_audioStreamPlayer_hit);
		m_sounds.Add(ESound.Die, node_audioStreamPlayer_die);

		m_animations = new Dictionary<EAnimation, string>();
		m_animations.Add(EAnimation.Default, "default");
		m_animations.Add(EAnimation.Straight, "straight");
		m_animations.Add(EAnimation.Left, "left");
		m_animations.Add(EAnimation.Right, "right");

		InputController = new PlayerInputController();

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

	public bool Get_SoundPlaying(ESound sound) => m_sounds[sound].Playing;

	public bool Get_Visible() => Visible;
	public void Set_Visible(bool visible) => Visible = visible;
	public void Toggle_Visible() => Visible = !Visible;

	public void Set_CollisionEnabled(bool enabled)
	{
		node_collisionShape2D_body.Disabled = !enabled;
		node_collisionShape2D_hitbox.Disabled = !enabled;
	}

	public bool Get_CollisionEnabled_Body() => !node_collisionShape2D_body.Disabled;
	public void Set_CollisionEnabled_Body(bool enabled) => node_collisionShape2D_body.Disabled = !enabled;
	public void Toggle_CollisionEnabled_Body() => node_collisionShape2D_body.Disabled = !node_collisionShape2D_body.Disabled;

	public bool Get_CollisionEnabled_Hitbox() => !node_collisionShape2D_hitbox.Disabled;
	public void Set_CollisionEnabled_Hitbox(bool enabled) => node_collisionShape2D_hitbox.Disabled = !enabled;
	public void Toggle_CollisionEnabled_Hitbox() => node_collisionShape2D_hitbox.Disabled = !node_collisionShape2D_hitbox.Disabled;
	
	public void OnAreaEnteredHitbox(Area2D area) => m_state.OnAreaEnteredHitbox(area);
	public void OnBodyEnteredHitbox(PhysicsBody2D body) => m_state.OnBodyEnteredHitbox(body);

	public void Move()
	{
		Velocity = node_kinematicBody2D_body.MoveAndSlide(Velocity, Vector2.Zero);
		Position = node_kinematicBody2D_body.GlobalPosition;
		node_kinematicBody2D_body.Position = Vector2.Zero;
	}

	public void ShootProjectile()
	{
		//TODO: make a pool for this
		Projectile projectile = PackedScene_Projectile.Instance() as Projectile;
		node_projectiles.AddChild(projectile);
		projectile.Position = node_position2D_projectile.GlobalPosition;
		projectile.Rotation = Rotation;

		Play_Sound(ESound.Shoot);
	}

	public void StopAllSounds()
	{
		foreach(KeyValuePair<ESound, AudioStreamPlayer> kvp in m_sounds)
		{
			Stop_Sound(kvp.Key);
		}
	}

	public void KillAllProjectiles()
	{
		foreach(Projectile projectile in node_projectiles.GetChildren())
		{
			projectile.Set_State(Projectile.EState.Die);
		}
	}

	public void FreeAllProjectiles()
	{
		foreach(Projectile projectile in node_projectiles.GetChildren())
		{
			projectile.Set_State(Projectile.EState.Free);
		}
	}

	#endregion // Public methods

}

