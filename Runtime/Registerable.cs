using UnityEngine;

namespace FasoFramework
{
	public class Registerable<ElementType, ManagerType> : MonoBehaviour where ManagerType : AutoRegisterManager<ElementType> where ElementType : MonoBehaviour
	{
		#region Variables

		#endregion

		#region Properties

		#endregion

		#region Unity Methods

		protected virtual void Awake()
		{
			GameInstance.GetManager<ManagerType>().RegisterElement(this as ElementType);
		}

		protected virtual void OnDestroy()
		{
			GameInstance.GetManager<ManagerType>()?.UnregisterElement(this as ElementType);
		}

		#endregion

		#region Custom Methods

		#endregion
	}
}