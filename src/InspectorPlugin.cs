#if TOOLS
using System.Reflection;
using Godot;
using Raele.InspectorCallout.Attributes;

namespace Raele.InspectorCallout;

[Tool]
public partial class InspectorPlugin : EditorInspectorPlugin
{
	public override bool _CanHandle(GodotObject @object) => @object != null;

	public override bool _ParseProperty(
		GodotObject @object,
		Variant.Type type,
		string name,
		PropertyHint hintType,
		string hintString,
		PropertyUsageFlags usageFlags,
		bool wide
	)
	{
		Godot.Collections.Dictionary property = new()
		{
			{ "name", name },
			{ "type", (long) type },
			{ "hint", (long) hintType },
			{ "hint_string", hintString },
			{ "usage", (long) usageFlags },
		};

		@object._ValidateProperty(property);

		{
			if (
				@object.GetType().GetField(name)?.GetCustomAttribute<HideWhenAttribute>()
					is HideWhenAttribute attribute
				&& !attribute.TestShow(@object)
			)
				return true;
		}

		if ((property["usage"].AsInt64() & (long) PropertyUsageFlags.Editor) == 0)
			return false;

		if (property.ContainsKey("info") && property["info"].AsString() is string info && !string.IsNullOrWhiteSpace(info))
			AddInfo(info);

		if (property.ContainsKey("comment") && property["comment"].AsString() is string comment && !string.IsNullOrWhiteSpace(comment))
			AddComment(comment);

		if (
			"" is string key
			&& (property.ContainsKey(key = "warn") || property.ContainsKey(key = "warning"))
			&& property[key].AsString() is string message
			&& !string.IsNullOrWhiteSpace(message)
		)
			AddWarning(message);

		if (property.ContainsKey("error") && property["error"].AsString() is string error && !string.IsNullOrWhiteSpace(error))
			AddError(error);

		{
			if (
				@object.GetType().GetField(name)?.GetCustomAttribute<CalloutAttribute>()
					is CalloutAttribute attribute
				&& !string.IsNullOrWhiteSpace(attribute.Note)
				&& attribute.Test(@object)
			)
				switch (attribute.Type)
				{
					case CalloutAttribute.CalloutType.Info:
						AddInfo(attribute.Note);
						break;
					case CalloutAttribute.CalloutType.Comment:
						AddComment(attribute.Note);
						break;
					case CalloutAttribute.CalloutType.Warning:
						AddWarning(attribute.Note);
						break;
					case CalloutAttribute.CalloutType.Error:
						AddError(attribute.Note);
						break;
				}
		}

		return false;

		void AddInfo(string message) => AddDialogAbove(message, EditorIcons.IconName.NodeInfo);
		void AddComment(string message)
			=> AddLabelBelow(message, EditorIcons.IconName.VisualShaderNodeComment, Colors.DimGray);
		void AddWarning(string message)
			=> AddLabelBelow(message, EditorIcons.IconName.StatusWarning, Colors.Yellow with { S = .5f });
		void AddError(string message)
			=> AddLabelBelow(message, EditorIcons.IconName.StatusError, Colors.Red with { S = .75f });

		void AddDialogAbove(string message, string iconName)
		{
			MarginContainer container = new();
			container.AddThemeConstantOverride("margin_left", 4);
			this.AddCustomControl(container);
			{
				PanelContainer panel = new();
				container.AddChild(panel);
				{
					MarginContainer m_container = new();
					m_container.AddThemeConstantOverride("margin_top", 4);
					m_container.AddThemeConstantOverride("margin_bottom", 4);
					m_container.AddThemeConstantOverride("margin_left", 4);
					m_container.AddThemeConstantOverride("margin_right", 4);
					panel.AddChild(m_container);
					{
						HBoxContainer hbox = new();
						hbox.AddThemeConstantOverride("separation", 8);
						m_container.AddChild(hbox);
						{
							TextureRect icon = new()
							{
								Texture = EditorIcons.GetIcon(iconName),
								StretchMode = TextureRect.StretchModeEnum.KeepAspectCentered,
								CustomMinimumSize = new Vector2(16, 16),
								Size = new Vector2(16, 16),
							};
							hbox.AddChild(icon);
						}
						{
							RichTextLabel label = new()
							{
								Text = message,
								CustomMinimumSize = new Vector2(0, 24),
								AutowrapMode = TextServer.AutowrapMode.WordSmart,
								BbcodeEnabled = true,
								FitContent = true,
							};
							label.SizeFlagsHorizontal = Control.SizeFlags.Fill | Control.SizeFlags.Expand;
							hbox.AddChild(label);
						}
					}
				}
			}
		}

		void AddLabelBelow(string message, string iconName, Color color)
		{
			MarginContainer margin = new();
			margin.AddThemeConstantOverride("margin_left", 4);
			this.AddPropertyEditor(name, margin, addToEnd: true);
			{
				HBoxContainer hbox = new();
				margin.AddChild(hbox);
				{
					TextureRect icon = new()
					{
						Texture = EditorIcons.GetIcon(iconName),
						StretchMode = TextureRect.StretchModeEnum.KeepAspectCentered,
						CustomMinimumSize = new Vector2(16, 16),
						Size = new Vector2(16, 16),
					};
					hbox.AddChild(icon);
				}
				{
					RichTextLabel label = new()
					{
						Text = message,
						CustomMinimumSize = new Vector2(0, 24),
						AutowrapMode = TextServer.AutowrapMode.WordSmart,
						BbcodeEnabled = true,
						FitContent = true,
					};
					label.AddThemeColorOverride("font_color", color);
					label.SizeFlagsHorizontal = Control.SizeFlags.Fill | Control.SizeFlags.Expand;
					hbox.AddChild(label);
				}
			}
		}
	}
}
#endif
