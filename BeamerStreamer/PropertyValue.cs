using System;
using System.Reflection;

namespace BeamerStreamer
{
	[Serializable]
	public class PropertyValue
	{
		public string Name { get; set; }
		public string Value { get; set; }

		public PropertyValue()
		{}

		public PropertyValue(PropertyInfo propertyInfo, object propertyValue)
		{
			Name = propertyInfo.Name;
			Value = propertyValue.ToString();
		}
	}
}