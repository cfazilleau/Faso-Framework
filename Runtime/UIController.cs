using UnityEngine;

public abstract class UIController : Controller
{
	#region Variables

	[SerializeField]
	private UIPanel[] panels = new UIPanel[0];

	#endregion

	#region Properties

	#endregion

	#region Unity Methods

	protected override void Awake()
	{
		base.Awake();
	}

	#endregion

	#region Custom Methods

	public T GetPanel<T>() where T : UIPanel
	{
		foreach (UIPanel panel in panels)
		{
			if (panel.GetType() == typeof(T))
				return panel as T;
		}

		return null;
	}

	#endregion
}
