using System.Collections.Generic;
using Godot;

public class Projectile : Node2D
{

	#region Enums

	public enum EState { Null, Init, Move, Die, Free }
	public enum ESound { Init, Die }

	#endregion // Enums



	#region Nodes

	protected Node node_sounds;
	protected AudioStreamPlayer node_sounds_init;
	protected AudioStreamPlayer node_sounds_die;

	protected Sprite node_sprite;

	protected Area2D node_hitbox;
	protected CollisionShape2D node_hitbox_collisionShape;

	#endregion // Nodes



	#region Properties

	[Export] public PackedScene PackedScene_DieParticles { get; set; }

	[Export] public List<string> GroupsToIgnore_Area { get; set; }
	[Export] public List<string> GroupsToIgnore_Body { get; set; }

	[Export] public float Speed { get; set; } = 128f;
	public Vector2 Velocity { get; set; } = Vector2.Zero;

	[Export] public float Lifetime { get; set; } = 8f;
	public float LifetimeTimer { get; set; } = 0f;

	#endregion // Properties



	#region Fields

	private Dictionary<EState, ProjectileState> m_states;
	private Dictionary<ESound, AudioStreamPlayer> m_sounds;

	private ProjectileState m_state;

	#endregion // Fields



	#region Godot methods

	public override void _EnterTree()
	{
		node_sprite = GetNode<Sprite>("Sprite");

		node_sounds = GetNode<Node>("Sounds");
		node_sounds_init = node_sounds.GetNode<AudioStreamPlayer>("Init");
		node_sounds_die = node_sounds.GetNode<AudioStreamPlayer>("Die");

		node_hitbox = GetNode<Area2D>("Hitbox");
		node_hitbox_collisionShape = node_hitbox.GetNode<CollisionShape2D>("CollisionShape2D");
	}

	public override void _Ready()
	{
		m_states = new Dictionary<EState, ProjectileState>();
		m_states.Add(EState.Null, new ProjectileState_Null(this));
		m_states.Add(EState.Init, new ProjectileState_Init(this));
		m_states.Add(EState.Move, new ProjectileState_Move(this));
		m_states.Add(EState.Die, new ProjectileState_Die(this));
		m_states.Add(EState.Free, new ProjectileState_Free(this));

		m_sounds = new Dictionary<ESound, AudioStreamPlayer>();
		m_sounds.Add(ESound.Init, node_sounds_init);
		m_sounds.Add(ESound.Die, node_sounds_die);

		m_state = m_states[EState.Null];

		if (GroupsToIgnore_Area == null)
		{
			GroupsToIgnore_Area = new List<string>();
		}
		if (GroupsToIgnore_Body == null)
		{
			GroupsToIgnore_Body = new List<string>();
		}

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

	public void Play_Sound(ESound sound) => m_sounds[sound].Play();
	public void Stop_Sound(ESound sound) => m_sounds[sound].Stop();
	public void Stop_Sound_All()
	{
		foreach (KeyValuePair<ESound, AudioStreamPlayer> kvp in m_sounds)
		{
			kvp.Value.Stop();
		}
	}

	public bool Get_Sound_Playing(ESound sound) => m_sounds[sound].Playing;

	public bool Get_Visible() => node_sprite.Visible;
	public void Set_Visible(bool visible) => node_sprite.Visible = visible;
	public void Toggle_Visible() => node_sprite.Visible = !node_sprite.Visible;

	public void Set_CollisionEnabled(bool enabled)
	{
		node_hitbox_collisionShape.Disabled = !enabled;
	}

	public void Move(float delta)
	{
		Translate(Velocity * delta);
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

		m_state.OnAreaEnteredHitbox(area);
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

		m_state.OnBodyEnteredHitbox(body);
	}

	#endregion // Private methods

}

