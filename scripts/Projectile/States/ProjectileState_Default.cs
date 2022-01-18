using Godot;

public class ProjectileState_Default : ProjectileState
{

	#region Constructors

	public ProjectileState_Default(Projectile projectile) : base(projectile) { }

	#endregion // Constructors



	#region Public methods

	public override void OnEnter() { }

	public override void OnExit() { }

	public override void OnPhysicsProcess(float delta) { }

	public override void OnProcess(float delta) { }

	public override void OnAreaEntered(Area2D area) { }

	public override void OnBodyEntered(PhysicsBody2D body) { }

	#endregion // Public methods

}

