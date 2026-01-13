using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Godot;
using Raele.GodotUtils.Extensions;

namespace Raele.InspectorCallout.Attributes;

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public class RequiredAttribute : Attribute
{
	public virtual bool TestIsError(GodotObject @object, string propertyName, [NotNullWhen(true)] out string? message)
	{
		Variant value = @object.Get(propertyName);
		if (value.IsEmpty())
		{
			message = $"Property {propertyName} cannot be {value}.";
			return true;
		}
		message = null;
		return false;
	}
}

public static class Required
{
	public class NotNull : RequiredAttribute
	{
		public override bool TestIsError(GodotObject @object, string propertyName, [NotNullWhen(true)] out string? message)
		{
			Godot.Collections.Dictionary? property = @object.GetPropertyList()
				.FirstOrDefault(property => property["name"].AsString() == propertyName);
			if (property == null)
			{
				message = null;
				return false;
			}
			Variant value = @object.Get(propertyName);
			if (value.Equals(Variant.NULL) && property["type"].AsVariantType().IsNullableType())
			{
				message = $"Property \"{propertyName}\" is required.";
				return true;
			}
			message = null;
			return false;
		}
	}
}
