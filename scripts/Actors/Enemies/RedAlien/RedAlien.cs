using Godot;

public class RedAlien : Node2D
{

	#region Nodes

	private AnimatedSprite node_animatedSprite_left;
	private AnimatedSprite node_animatedSprite_right;

	private Area2D node_hitbox;
	private CollisionShape2D node_hitbox_collisionShape;

	#endregion // Nodes



	#region Signals

	[Signal] public delegate void OnHit();

	#endregion // Signals



	#region Properties

	[Export] public PackedScene PackedScene_DieParticles { get; set; }

	[Export] public Vector2 Velocity { get; set; } = Vector2.Zero;

	[Export] public Rect2 Bounds { get; set; } = new Rect2(-16, -8, 272, 392);

	#endregion // Properties



	#region Fields

	private bool m_wasHit = false;
	private bool m_isHit = false;

	private bool m_wasDie = false;
	private bool m_isDie = false;

	private bool m_wasOutOfBounds = false;
	private bool m_isOutOfBounds = false;

	private CPUParticles2D m_dieParticles;

	#endregion // Fields



	#region Godot methods

	public override void _EnterTree()
	{
		node_animatedSprite_left = GetNode<AnimatedSprite>("AnimatedSpriteLeft");
		node_animatedSprite_right = GetNode<AnimatedSprite>("AnimatedSpriteRight");

		node_hitbox = GetNode<Area2D>("Hitbox");
		node_hitbox_collisionShape = node_hitbox.GetNode<CollisionShape2D>("CollisionShape2D");
	}

	public override void _PhysicsProcess(float delta)
	{
		if (!m_isHit && !m_isOutOfBounds)
		{
			Translate(Velocity * delta);

			Rect2 rect = new Rect2(Position.x - 16, Position.y - 8, 32, 16);

			if (!Bounds.Intersects(rect))
			{
				m_isOutOfBounds = true;
				QueueFree();
			}
		}

		if (m_isHit && !m_wasHit)
		{
			m_wasHit = true;
			node_animatedSprite_left.Visible = false;
			node_animatedSprite_right.Visible = false;
			node_hitbox_collisionShape.Disabled = true;
			m_dieParticles = PackedScene_DieParticles.Instance<CPUParticles2D>();
			AddChild(m_dieParticles);
			m_dieParticles.Emitting = true;
		}

		if (m_wasHit && !m_isDie)
		{
			if (!m_dieParticles.Emitting)
			{
				m_isDie = true;
			}
		}

		if (m_isDie && !m_wasDie)
		{
			m_wasDie = true;
			EmitSignal(nameof(OnHit));
			RemoveChild(m_dieParticles);
			m_dieParticles.QueueFree();
			QueueFree();
		}
	}

	#endregion // Godot methods



	#region Private methods

	private void OnAreaEnteredHitbox(Area2D area)
	{
		m_isHit = true;
	}

	private void OnBodyEnteredHitbox(PhysicsBody2D body)
	{
		m_isHit = true;
	}

	#endregion // Private methods

}

