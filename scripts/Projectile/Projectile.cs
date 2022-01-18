using Godot;

public class Projectile : Node2D
{

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
		SetState(new ProjectileState_Move(this));
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

	public void SetState(ProjectileState state)
	{
		if (m_state != null)
		{
			m_state.OnExit();
		}
		m_state = state;
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
		SetState(new ProjectileState_Destroy(this));
	}

	#endregion // Public methods

}

