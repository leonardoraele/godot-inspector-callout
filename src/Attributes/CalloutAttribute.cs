using System;
using System.Linq;
using Godot;

namespace Raele.InspectorCallout.Attributes;

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public class CalloutAttribute : Attribute
{
	public string Note { get; private set; }
	public CalloutType Type { get; private set; } = CalloutType.Info;
	public StringName? Condition { get; private set; } = null;

	public CalloutAttribute(string note)
		=> this.Note = note;

	public CalloutAttribute(CalloutType type, string note)
		: this(note)
		=> this.Type = type;

	public CalloutAttribute(StringName condition, string note)
		: this(note)
		=> this.Condition = condition;

	public CalloutAttribute(StringName condition, CalloutType type, string note)
	{
		this.Condition = condition;
		this.Type = type;
		this.Note = note;
	}

	public bool Test(GodotObject subject)
	{
		if (string.IsNullOrWhiteSpace(this.Condition?.ToString()))
			return true;

		if (subject.GetPropertyList().Any(property => property["name"].AsStringName() == this.Condition))
			return subject.Get(this.Condition).AsBool();

		if (subject.HasMethod(this.Condition))
			return subject.Call(this.Condition).AsBool();

		GD.PushWarning($"{nameof(CalloutAttribute)} in script \"{subject.GetType().Name}\" could not be evaluated because no property or method named \"{this.Condition}\" was found.");

		return false;
	}

	public enum CalloutType
	{
		Info,
		Comment,
		Warning,
		Error
	}
}

public static class Callout
{
	public class Info : CalloutAttribute
	{
		public Info(string note) : base(CalloutType.Info, note) { }
		public Info(StringName condition, string note) : base(condition, CalloutType.Info, note) { }
	}
	public class Comment : CalloutAttribute
	{
		public Comment(string note) : base(CalloutType.Comment, note) { }
		public Comment(StringName condition, string note) : base(condition, CalloutType.Comment, note) { }
	}
	public class Warning : CalloutAttribute
	{
		public Warning(string note) : base(CalloutType.Warning, note) { }
		public Warning(StringName condition, string note) : base(condition, CalloutType.Warning, note) { }
	}
	public class Error : CalloutAttribute
	{
		public Error(string note) : base(CalloutType.Error, note) { }
		public Error(StringName condition, string note) : base(condition, CalloutType.Error, note) { }
	}
}
