using System.Collections.Generic;
using Godot;

public class Shield : Node2D
{

	#region Nodes

	private Sprite node_sprite;
	private Area2D node_hitbox;
	private CollisionPolygon2D node_hitbox_collisionPolygon;
	private StaticBody2D node_body;

	#endregion // Nodes



	#region Properties

	[Export] public List<string> GroupsToIgnore_Area { get; set; }
	[Export] public List<string> GroupsToIgnore_Body { get; set; }

	[Export] public int Health { get; set; } = 8;
	[Export] public int MaxHealth { get; set; } = 8;

	[Export] public Color FullHealthColor { get; set; } = Colors.White;
	[Export] public Color NoHealthColor { get; set; } = Colors.Red;

	#endregion // Properties



	#region Fields

	private bool m_wasHit = false;
	private bool m_isHit = false;

	#endregion // Fields



	#region Godot methods

	public override void _EnterTree()
	{
		node_sprite = GetNode<Sprite>("Sprite");
		node_hitbox = GetNode<Area2D>("Hitbox");
		node_hitbox_collisionPolygon = node_hitbox.GetNode<CollisionPolygon2D>("CollisionPolygon2D");
		node_body = GetNode<StaticBody2D>("Body");
	}

	public override void _PhysicsProcess(float delta)
	{
		if (m_isHit)
		{
			m_isHit = false;

			if (Health > 0)
			{
				Health--;
				node_sprite.Modulate = NoHealthColor.LinearInterpolate(FullHealthColor, (float)Health / (float)MaxHealth);
			}
			else
			{
				QueueFree();
			}
		}
	}

	#endregion // Godot methods



	#region Public methods

	public void EnableCollision()
	{
		node_hitbox_collisionPolygon.Disabled = false;
	}
	public void DisableCollision()
	{
		node_hitbox_collisionPolygon.Disabled = true;
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
	}

	#endregion // Private methods

}

