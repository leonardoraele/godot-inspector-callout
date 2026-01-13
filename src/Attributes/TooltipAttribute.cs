using System;

namespace Raele.InspectorCallout.Attributes;

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public class TooltipAttribute : Attribute
{
	public string Text { get; private set; }

	public TooltipAttribute(string text)
		=> this.Text = text;
}
