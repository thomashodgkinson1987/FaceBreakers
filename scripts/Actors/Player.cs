using System.Collections.Generic;
using Godot;

public class Player : Node2D
{

	#region Enums

	public enum EState { Null, Init, Move, Hit, Destroy }
	public enum ESound { Shoot, Hit, Die }

	#endregion // Enums



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



	#region Properties

	[Export] public PackedScene PackedScene_Projectile { get; set; }

	[Export] public float Speed { get; set; } = 96f;
	[Export] public Vector2 Velocity { get; set; } = Vector2.Zero;

	public PlayerInputController InputController { get; private set; }

	#endregion // Properties



	#region Fields

	private Dictionary<EState, IPlayerState> m_states;
	private IPlayerState m_state;

	private Dictionary<ESound, AudioStreamPlayer> m_sounds;

	#endregion // Fields



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

	public override void _Ready()
	{
		m_states = new Dictionary<EState, IPlayerState>();
		m_states.Add(EState.Null, new PlayerState_Null(this));
		m_states.Add(EState.Init, new PlayerState_Init(this));
		m_states.Add(EState.Move, new PlayerState_Move(this));
		m_states.Add(EState.Hit, new PlayerState_Hit(this));

		m_state = m_states[EState.Null];

		m_sounds = new Dictionary<ESound, AudioStreamPlayer>();
		m_sounds.Add(ESound.Shoot, node_audioStreamPlayer_shoot);
		m_sounds.Add(ESound.Hit, node_audioStreamPlayer_hit);
		m_sounds.Add(ESound.Die, node_audioStreamPlayer_die);

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

	public void Reset() => Set_State(Player.EState.Init);

	public void Set_State(EState state)
	{
		m_state.OnExit();
		m_state = m_states[state];
		m_state.OnEnter();
	}

	public void Set_Animation(string animationName) => node_animatedSprite.Play(animationName);

	public void Play_Sound(ESound sound) => m_sounds[sound].Play();
	public void Stop_Sound(ESound sound) => m_sounds[sound].Stop();

	public bool Get_SoundPlaying(ESound sound) => m_sounds[sound].Playing;

	public bool Get_Visible() => Visible;
	public void Set_Visible(bool visible) => Visible = visible;
	public void Toggle_Visible() => Visible = !Visible;

	public bool Get_CollisionEnabled() => !node_collisionShape2D.Disabled;
	public void Set_CollisionEnabled(bool enabled) => node_collisionShape2D.Disabled = !enabled;
	public void Toggle_CollisionEnabled() => node_collisionShape2D.Disabled = !node_collisionShape2D.Disabled;

	public void OnAreaEntered(Area2D area) => m_state.OnAreaEntered(area);
	public void OnBodyEntered(PhysicsBody2D body) => m_state.OnBodyEntered(body);

	public void Destroy() => Set_State(EState.Destroy);

	public void Move()
	{
		Vector2 velocity = node_kinematicBody2D.MoveAndSlide(Velocity, Vector2.Zero);
		Vector2 position = node_kinematicBody2D.GlobalPosition;

		node_kinematicBody2D.Position = Vector2.Zero;

		Velocity = velocity;
		Position = position;
	}

	public void ShootProjectile()
	{
		//TODO: make a pool for this
		Projectile projectile = PackedScene_Projectile.Instance() as Projectile;
		node_projectiles.AddChild(projectile);
		projectile.GlobalPosition = node_position2D_projectile.GlobalPosition;
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

	public void FreeAllProjectiles()
	{
		foreach(Projectile projectile in node_projectiles.GetChildren())
		{
			projectile.QueueFree();
		}
	}

	public void DestroyAllProjectiles()
	{
		foreach(Projectile projectile in node_projectiles.GetChildren())
		{
			projectile.Destroy();
		}
	}

	#endregion // Public methods

}

