using Godot;

public class Projectile : Node2D
{

	#region Nodes

	private Sprite node_sprite;
	private Area2D node_area2D;
	private CollisionShape2D node_collisionShape2D;

	private AudioStreamPlayer node_audioStreamPlayer_instantiate;
	private AudioStreamPlayer node_audioStreamPlayer_free;

	#endregion // Nodes



	#region Properties

	[Export] public float Speed { get; set; } = 128f;
	[Export] public int Damage { get; set; } = 1;
	[Export] public float Lifetime { get; set; } = 8f;

	#endregion // Properties



	#region Fields

	private ProjectileState m_state = new ProjectileState_Default();

	private float m_lifetimeTimer = 0f;

	#endregion // Fields



	#region Godot methods

	public override void _EnterTree()
	{
		node_sprite = GetNode<Sprite>("Sprite");
		node_area2D = GetNode<Area2D>("Area2D");
		node_collisionShape2D = node_area2D.GetNode<CollisionShape2D>("CollisionShape2D");

		node_audioStreamPlayer_instantiate = GetNode<AudioStreamPlayer>("AudioStreamPlayer_Instantiate");
		node_audioStreamPlayer_free = GetNode<AudioStreamPlayer>("AudioStreamPlayer_Free");
	}

	public override void _Ready()
	{
		SetState(new ProjectileState_Move());
	}

	public override void _PhysicsProcess(float delta)
	{
		if (m_lifetimeTimer < Lifetime)
		{
			m_lifetimeTimer += delta;

			if (m_lifetimeTimer >= Lifetime)
			{
				SetState(new ProjectileState_Destroy());
			}
		}

		m_state.OnPhysicsProcess(this, delta);
	}

	public override void _Process(float delta)
	{
		m_state.OnProcess(this, delta);
	}

	#endregion // Godot methods



	#region Public methods

	public void SetState(ProjectileState state)
	{
		m_state.OnExit(this);
		m_state = state;
		m_state.OnEnter(this);
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
		m_state.OnAreaEntered(this, area);
	}
	public void OnBodyEntered(PhysicsBody2D body)
	{
		m_state.OnBodyEntered(this, body);
	}

	#endregion // Public methods

}

public abstract class ProjectileState
{
	public ProjectileState() {  }

	public abstract void OnEnter(Projectile projectile);
	public abstract void OnExit(Projectile projectile);

	public abstract void OnPhysicsProcess(Projectile projectile, float delta);
	public abstract void OnProcess(Projectile projectile, float delta);

	public abstract void OnAreaEntered(Projectile projectile, Area2D area);
	public abstract void OnBodyEntered(Projectile projectile, PhysicsBody2D body);
}

public class ProjectileState_Default : ProjectileState
{
	public ProjectileState_Default() {  }

	public override void OnEnter(Projectile projectile) {  }
	public override void OnExit(Projectile projectile) {  }

	public override void OnPhysicsProcess(Projectile projectile, float delta) {  }
	public override void OnProcess(Projectile projectile, float delta) {  }

	public override void OnAreaEntered(Projectile projectile, Area2D area) {  }
	public override void OnBodyEntered(Projectile projectile, PhysicsBody2D body) {  }
}

public class ProjectileState_Move : ProjectileState
{
	public ProjectileState_Move() {  }
	
	public override void OnEnter(Projectile projectile)
	{
		projectile.PlaySound_Instantiate();
	}
	public override void OnExit(Projectile projectile) {  }

	public override void OnPhysicsProcess(Projectile projectile, float delta)
	{
		projectile.Translate(Vector2.Up * projectile.Speed * delta);
	}
	public override void OnProcess(Projectile projectile, float delta) {  }

	public override void OnAreaEntered(Projectile projectile, Area2D area)
	{
		//TODO Find better way of managing states
		projectile.SetState(new ProjectileState_Destroy());
	}
	public override void OnBodyEntered(Projectile projectile, PhysicsBody2D body)
	{
		//TODO Find better way of managing states
		projectile.SetState(new ProjectileState_Destroy());
	}
}

public class ProjectileState_Destroy : ProjectileState
{
	public ProjectileState_Destroy() {  }
	
	public override void OnEnter(Projectile projectile)
	{
		projectile.PlaySound_Free();
		projectile.HideSprite();
		projectile.DisableCollision();
	}
	public override void OnExit(Projectile projectile) {  }

	public override void OnPhysicsProcess(Projectile projectile, float delta) {  }
	public override void OnProcess(Projectile projectile, float delta)
	{
		if (!projectile.IsPlaying_Free())
		{
			projectile.QueueFree();
		}
	}

	public override void OnAreaEntered(Projectile projectile, Area2D area) {  }
	public override void OnBodyEntered(Projectile projectile, PhysicsBody2D body) {  }
}

