using System;
using System.Linq;
using Godot;

namespace Raele.InspectorCallout.Attributes;

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public class HideWhenAttribute : Attribute
{
	/// <summary>
	/// The name of a field, property, or method.
	/// </summary>
	public string Condition { get; private set; }
	/// <summary>
	/// The expected value of the condition to hide the property. Defaults to true.
	/// </summary>
	public Variant ExpectedValue { get; private set; } = true;

	public HideWhenAttribute(string condition)
		=> this.Condition = condition;
	public HideWhenAttribute(string condition, bool expectedValue) : this(condition, Variant.From(expectedValue)) {}
	public HideWhenAttribute(string condition, long expectedValue) : this(condition, Variant.From(expectedValue)) {}
	public HideWhenAttribute(string condition, double expectedValue) : this(condition, Variant.From(expectedValue)) {}
	public HideWhenAttribute(string condition, string expectedValue) : this(condition, Variant.From(expectedValue)) {}
	public HideWhenAttribute(string condition, Variant expectedValue)
		: this(condition)
		=> this.ExpectedValue = expectedValue;

	public virtual bool TestShow(GodotObject subject)
	{
		if (subject.GetPropertyList().Any(property => property["name"].AsString() == this.Condition))
			return !subject.Get(this.Condition).Equals(this.ExpectedValue);

		if (subject.HasMethod(this.Condition))
			return !subject.Call(this.Condition).Equals(this.ExpectedValue);

		GD.PushWarning($"{nameof(HideWhenAttribute)} in script \"{subject.GetType().Name}\" could not be evaluated because no property or method named \"{this.Condition}\" was found.");

		return false;
	}
}
