using System.Collections.Generic;
using Godot;

public class Projectile : Node2D
{

	#region Enums

	public enum EStateName { Null, Init, Moving, Destroying }

	#endregion // Enums



	#region Nodes

	private Sprite node_sprite;

	private Area2D node_area2D;
	private CollisionShape2D node_collisionShape2D;

	private Node node_audioStreamPlayers;
	private AudioStreamPlayer node_audioStreamPlayer_instantiate;
	private AudioStreamPlayer node_audioStreamPlayer_free;

	#endregion // Nodes



	#region Properties

	[Export] public float MoveSpeed { get; set; } = 128f;
	[Export] public int Damage { get; set; } = 1;
	[Export] public float Lifetime { get; set; } = 8f;
	public float LifetimeTimer { get; set; } = 0f;

	#endregion // Properties



	#region Fields

	private Dictionary<EStateName, ProjectileState> m_states = new Dictionary<EStateName, ProjectileState>();

	private ProjectileState m_state;

	#endregion // Fields



	#region Godot methods

	public override void _EnterTree()
	{
		node_sprite = GetNode<Sprite>("Sprite");

		node_area2D = GetNode<Area2D>("Area2D");
		node_collisionShape2D = node_area2D.GetNode<CollisionShape2D>("CollisionShape2D");

		node_audioStreamPlayers = GetNode<Node>("AudioStreamPlayers");
		node_audioStreamPlayer_instantiate = node_audioStreamPlayers.GetNode<AudioStreamPlayer>("AudioStreamPlayer_Instantiate");
		node_audioStreamPlayer_free = node_audioStreamPlayers.GetNode<AudioStreamPlayer>("AudioStreamPlayer_Free");
	}

	public override void _Ready()
	{
		m_states.Add(EStateName.Null, new ProjectileState_Null(this));
		m_states.Add(EStateName.Init, new ProjectileState_Init(this));
		m_states.Add(EStateName.Moving, new ProjectileState_Moving(this));
		m_states.Add(EStateName.Destroying, new ProjectileState_Destroying(this));

		m_state = m_states[EStateName.Null];

		SetState(EStateName.Init);
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

	public void SetState(EStateName stateName)
	{
		m_state.OnExit();
		m_state = m_states[stateName];
		m_state.OnEnter();
	}

	public void PlaySound_Instantiate()
	{
		node_audioStreamPlayer_instantiate.Play();
	}

	public void PlaySound_Free()
	{
		node_audioStreamPlayer_free.Play();
	}

	public bool IsPlaying_Instantiate()
	{
		return node_audioStreamPlayer_instantiate.Playing;
	}

	public bool IsPlaying_Free()
	{
		return node_audioStreamPlayer_free.Playing;
	}

	public void ShowSprite()
	{
		node_sprite.Visible = true;
	}

	public void HideSprite()
	{
		node_sprite.Visible = false;
	}

	public void EnableCollision()
	{
		node_collisionShape2D.Disabled = false;
	}

	public void DisableCollision()
	{
		node_collisionShape2D.Disabled = true;
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
		SetState(EStateName.Destroying);
	}

	#endregion // Public methods

}

