using System;
using Godot;

namespace Raele.InspectorCallout.Attributes;

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public class SeparatorAttribute(int MARGIN = 8) : Attribute
{
	public void Evaluate(InspectorPlugin plugin)
	{
		MarginContainer margin = new();
		margin.AddThemeConstantOverride("margin_top", MARGIN);
		margin.AddThemeConstantOverride("margin_bottom", MARGIN);
		plugin.AddCustomControl(margin);
		{
			HSeparator separator = new();
			margin.AddChild(separator);
		}
	}
}
