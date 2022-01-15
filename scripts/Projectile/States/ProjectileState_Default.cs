using Godot;

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

