using System.Collections.Generic;
using Godot;

public class Alien : Node2D
{

	#region Nodes

	private AnimatedSprite node_animatedSprite_left;
	private AnimatedSprite node_animatedSprite_right;

	private Area2D node_hitbox;
	private CollisionShape2D node_hitbox_collisionShape;

	#endregion // Nodes



	#region Signals

	[Signal] public delegate void OnHit(Alien alien);
	[Signal] public delegate void OnDie(Alien alien);

	#endregion // Signals



	#region Properties

	[Export] public PackedScene PackedScene_DieParticles { get; set; }

	[Export] public List<string> GroupsToIgnore_Area { get; set; }
	[Export] public List<string> GroupsToIgnore_Body { get; set; }

	[Export] public Vector2 Velocity { get; set; } = Vector2.Zero;
	[Export] public float Speed { get; set; } = 32;

	[Export] public Rect2 Bounds { get; set; } = new Rect2(-16, -8, 272, 392);

	[Export] public int ScoreValue { get; set; } = 500;

	#endregion // Properties



	#region Fields

	private bool m_wasHit = false;
	private bool m_isHit = false;

	private bool m_wasDie = false;
	private bool m_isDie = false;

	private bool m_wasOutOfBounds = false;
	private bool m_isOutOfBounds = false;

	private CPUParticles2D m_dieParticles;

	private RectangleShape2D m_rectShape;

	#endregion // Fields



	#region Godot methods

	public override void _EnterTree()
	{
		node_animatedSprite_left = GetNode<AnimatedSprite>("AnimatedSpriteLeft");
		node_animatedSprite_right = GetNode<AnimatedSprite>("AnimatedSpriteRight");

		node_hitbox = GetNode<Area2D>("Hitbox");
		node_hitbox_collisionShape = node_hitbox.GetNode<CollisionShape2D>("CollisionShape2D");
		m_rectShape = node_hitbox_collisionShape.Shape as RectangleShape2D;
	}

	public override void _Ready()
	{
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
		if (!m_isHit && !m_isOutOfBounds)
		{
			Translate(Velocity * delta);

			Rect2 rect = new Rect2(
				Position.x - m_rectShape.Extents.x,
				Position.y - m_rectShape.Extents.y,
				m_rectShape.Extents.x * 2,
				m_rectShape.Extents.y * 2
			);

			if (!Bounds.Intersects(rect))
			{
				m_isOutOfBounds = true;
				QueueFree();
				return;
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
			EmitSignal(nameof(OnHit), this);
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
			EmitSignal(nameof(OnDie), this);
			RemoveChild(m_dieParticles);
			m_dieParticles.QueueFree();
			QueueFree();
		}
	}

	#endregion // Godot methods



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

	#endregion // Private methods

}

