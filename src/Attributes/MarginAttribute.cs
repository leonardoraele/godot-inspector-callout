using System;
using Godot;

namespace Raele.InspectorCallout.Attributes;

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public class MarginAttribute(int HEIGHT = 8) : Attribute
{
	public void Evaluate(InspectorPlugin plugin)
		=> plugin.AddCustomControl(new Control { CustomMinimumSize = new Godot.Vector2(0, HEIGHT) });
}
