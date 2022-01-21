using Godot;

public abstract class ProjectileState
{

	#region Fields

	protected readonly Projectile m_projectile;

	#endregion // Fields



	#region Constructors

	public ProjectileState(Projectile projectile) => m_projectile = projectile;

	#endregion // Constructors



	#region Public methods

	public abstract void OnEnter();

	public abstract void OnExit();

	public abstract void OnPhysicsProcess(float delta);

	public abstract void OnProcess(float delta);

	public abstract void OnAreaEnteredHitbox(Area2D area);

	public abstract void OnBodyEnteredHitbox(PhysicsBody2D body);

	#endregion // Public methods

}

