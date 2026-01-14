using System;

namespace Raele.InspectorCallout.Attributes;

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public class TooltipAttribute : Attribute
{
	public readonly string Text;
	public TooltipAttribute(string text)
		=> this.Text = text;
}
