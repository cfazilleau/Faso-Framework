using System;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public class RequirePersistentManagerAttribute : Attribute
{
	#region Variables

	public Type requiredManagerType1 = null;
	public Type requiredManagerType2 = null;
	public Type requiredManagerType3 = null;
	public Type requiredManagerType4 = null;
	public Type requiredManagerType5 = null;

	#endregion

	#region Methods

	public RequirePersistentManagerAttribute(Type type1, Type type2 = null, Type type3 = null, Type type4 = null, Type type5 = null)
	{
		requiredManagerType1 = type1;
		requiredManagerType2 = type2;
		requiredManagerType3 = type3;
		requiredManagerType4 = type4;
		requiredManagerType5 = type5;
	}

	#endregion
}