using Godot;

public class GameHUDController : Control
{

	#region Nodes

	private TextureRect node_textureRect_lives_icon;
	private Label node_label_lives_cross;
	private Label node_label_lives_value;
	private Label node_label_score_text;
	private Label node_label_score_equals;
	private Label node_label_score_value;

	private Slider node_bossHealthSlider;

	private AnimationPlayer node_animationPlayer;

	#endregion // Nodes



	#region Properties

	public int Lives
	{
		get => int.Parse(node_label_lives_value.Text);
		set => node_label_lives_value.Text = value.ToString();
	}
	public int Score
	{
		get => int.Parse(node_label_score_value.Text);
		set => node_label_score_value.Text = value.ToString();
	}

	public int BossHealth
	{
		get => (int)node_bossHealthSlider.Value;
		set => node_bossHealthSlider.Value = value;
	}

	#endregion // Properties



	#region Godot methods

	public override void _EnterTree()
	{
		node_textureRect_lives_icon = GetNode<TextureRect>("TextureRect_Lives_Icon");
		node_label_lives_cross = GetNode<Label>("Label_Lives_Cross");
		node_label_lives_value = GetNode<Label>("Label_Lives_Value");
		node_label_score_text = GetNode<Label>("Label_Score_Text");
		node_label_score_equals = GetNode<Label>("Label_Score_Equals");
		node_label_score_value = GetNode<Label>("Label_Score_Value");

		node_bossHealthSlider = GetNode<Slider>("HSlider");

		node_animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
	}

	#endregion // Godot methods



	#region Public methods

	public void ShowBossHealthBar()
	{
		node_bossHealthSlider.Modulate = Colors.White;
	}
	public void HideBossHealthBar()
	{
		node_bossHealthSlider.Modulate = new Color(1f, 1f, 1f, 0f);
	}

	public void RunCredits()
	{
		node_animationPlayer.Play("credits_0001");
	}

	private void OnCreditsFinished()
	{
		GetTree().ChangeScene("res://scenes/TitleScreen.tscn");
	}

	#endregion // Public methods

}

