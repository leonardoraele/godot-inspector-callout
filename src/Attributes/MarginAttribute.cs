using System;
using Godot;

namespace Raele.InspectorCallout.Attributes;

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public class MarginAttribute(int HEIGHT = 16) : Attribute
{
	public virtual void Evaluate(InspectorPlugin plugin)
		=> plugin.AddCustomControl(new Control { CustomMinimumSize = new Vector2(0, HEIGHT) });
}
