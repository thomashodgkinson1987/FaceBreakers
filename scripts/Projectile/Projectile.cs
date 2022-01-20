using System.Collections.Generic;
using Godot;

public class Projectile : Node2D
{

	#region Enums

	public enum ESound { Init, Free }

	public enum ESpriteVisibilityState { Visible, Invisible }

	public enum ECollisionState { Enable, Disable }

	public enum EState { Null, Init, Move, Destroy }

	#endregion // Enums



	#region Nodes

	private Sprite node_sprite;

	private Area2D node_area2D;
	private CollisionShape2D node_collisionShape2D;

	private Node node_audioStreamPlayers;
	private AudioStreamPlayer node_audioStreamPlayer_init;
	private AudioStreamPlayer node_audioStreamPlayer_free;

	#endregion // Nodes



	#region Properties

	[Export] public float MoveSpeed { get; set; } = 128f;
	[Export] public float Lifetime { get; set; } = 8f;
	public float LifetimeTimer { get; set; } = 0f;

	#endregion // Properties



	#region Fields

	private Dictionary<EState, IProjectileState> m_states = new Dictionary<EState, IProjectileState>();

	private IProjectileState m_state;

	#endregion // Fields



	#region Godot methods

	public override void _EnterTree()
	{
		node_sprite = GetNode<Sprite>("Sprite");

		node_area2D = GetNode<Area2D>("Area2D");
		node_collisionShape2D = node_area2D.GetNode<CollisionShape2D>("CollisionShape2D");

		node_audioStreamPlayers = GetNode<Node>("AudioStreamPlayers");
		node_audioStreamPlayer_init = node_audioStreamPlayers.GetNode<AudioStreamPlayer>("AudioStreamPlayer_Init");
		node_audioStreamPlayer_free = node_audioStreamPlayers.GetNode<AudioStreamPlayer>("AudioStreamPlayer_Free");
	}

	public override void _Ready()
	{
		m_states.Add(EState.Null, new ProjectileState_Null(this));
		m_states.Add(EState.Init, new ProjectileState_Init(this));
		m_states.Add(EState.Move, new ProjectileState_Move(this));
		m_states.Add(EState.Destroy, new ProjectileState_Destroy(this));

		m_state = m_states[EState.Null];

		SetState(EState.Init);
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

	public void SetState(EState state)
	{
		m_state.OnExit();
		m_state = m_states[state];
		m_state.OnEnter();
	}

	public void PlaySound(ESound sound)
	{
		switch (sound)
		{
			case ESound.Init:
				node_audioStreamPlayer_init.Play();
				break;
			case ESound.Free:
				node_audioStreamPlayer_free.Play();
				break;
			default:
				break;
		}
	}

	public bool IsPlaying(ESound sound)
	{
		switch (sound)
		{
			case ESound.Init:
				return node_audioStreamPlayer_init.Playing;
			case ESound.Free:
				return node_audioStreamPlayer_free.Playing;
			default:
				return false;
		}
	}

	public void SetSpriteVisibilityState(ESpriteVisibilityState visibilityState)
	{
		switch (visibilityState)
		{
			case ESpriteVisibilityState.Visible:
				node_sprite.Visible = true;
				break;
			case ESpriteVisibilityState.Invisible:
				node_sprite.Visible = false;
				break;
			default:
				node_sprite.Visible = true;
				break;
		}
	}

	public void ToggleSpriteVisibilityState()
	{
		node_sprite.Visible = !node_sprite.Visible;
	}

	public void SetCollisionState(ECollisionState collisionState)
	{
		switch (collisionState)
		{
			case ECollisionState.Enable:
				node_collisionShape2D.Disabled = false;
				break;
			case ECollisionState.Disable:
				node_collisionShape2D.Disabled = true;
				break;
			default:
				node_collisionShape2D.Disabled = false;
				break;
		}
	}

	public void ToggleCollisionState()
	{
		node_collisionShape2D.Disabled = !node_collisionShape2D.Disabled;
	}

	public void OnAreaEntered(Area2D area)
	{
		m_state.OnAreaEntered(area);
	}

	public void OnBodyEntered(PhysicsBody2D body)
	{
		m_state.OnBodyEntered(body);
	}

	public void Destroy()
	{
		SetState(EState.Destroy);
	}

	#endregion // Public methods

}

