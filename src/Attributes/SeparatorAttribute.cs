using System;
using Godot;

namespace Raele.InspectorCallout.Attributes;

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public class SeparatorAttribute : Attribute
{
	public void Evaluate(InspectorPlugin plugin)
		=> plugin.AddCustomControl(new HSeparator());
}
