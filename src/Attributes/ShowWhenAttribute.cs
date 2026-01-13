using Godot;

namespace Raele.InspectorCallout.Attributes;

 /// <summary>
 /// Attribute to show a property in the inspector only when a certain condition is met.
 /// </summary>
public class ShowWhen : HideWhenAttribute
{
	public ShowWhen(string condition) : base(condition) {}
	public ShowWhen(string condition, bool expectedValue) : this(condition, Variant.From(expectedValue)) {}
	public ShowWhen(string condition, long expectedValue) : this(condition, Variant.From(expectedValue)) {}
	public ShowWhen(string condition, double expectedValue) : this(condition, Variant.From(expectedValue)) {}
	public ShowWhen(string condition, string expectedValue) : this(condition, Variant.From(expectedValue)) {}
	public ShowWhen(string condition, Variant expectedValue) : base(condition, expectedValue) {}

	public override bool TestShow(GodotObject subject)
		=> !base.TestShow(subject);
}
