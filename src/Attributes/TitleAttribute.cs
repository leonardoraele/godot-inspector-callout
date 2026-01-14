using System;
using Godot;

namespace Raele.InspectorCallout.Attributes;

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public class TitleAttribute(string TITLE_TEXT) : MarginAttribute(8)
{
	public override void Evaluate(InspectorPlugin plugin)
	{
		base.Evaluate(plugin);
		RichTextLabel label = new()
		{
			BbcodeEnabled = true,
			Text = $"[b]{TITLE_TEXT}[/b]",
			FitContent = true,
			AutowrapMode = TextServer.AutowrapMode.WordSmart,
		};
		plugin.AddCustomControl(label);
	}
}
