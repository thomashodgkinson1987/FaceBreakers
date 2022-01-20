using Godot;

public class ProjectileState_Init : ProjectileState
{

	#region Constructors

	public ProjectileState_Init(Projectile projectile) : base(projectile) { }

	#endregion // Constructors



	#region Public methods

	public override void OnEnter()
	{
		m_projectile.Play_Sound(Projectile.ESound.Init);
		m_projectile.Set_State(Projectile.EState.Move);
	}

	public override void OnExit() { }

	public override void OnPhysicsProcess(float delta) { }

	public override void OnProcess(float delta) { }

	public override void OnAreaEntered(Area2D area) { }

	public override void OnBodyEntered(PhysicsBody2D body) { }

	#endregion // Public methods

}

