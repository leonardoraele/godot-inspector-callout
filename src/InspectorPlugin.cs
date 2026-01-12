#if TOOLS
using Godot;

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

		if ((property["usage"].AsInt64() & (long) PropertyUsageFlags.Editor) == 0)
			return false;

		if (property.ContainsKey("info"))
		{
			string info = property["info"].AsString();
			if (string.IsNullOrWhiteSpace(info))
				return false;
			AddDialogAbove(info, EditorIcons.IconName.NodeInfo);
		}

		if (property.ContainsKey("comment"))
		{
			string comment = property["comment"].AsString();
			if (string.IsNullOrWhiteSpace(comment))
				return false;
			AddLabelBelow(comment, EditorIcons.IconName.VisualShaderNodeComment, Colors.DimGray);
		}

		if ("" is string key && (property.ContainsKey(key = "warn") || property.ContainsKey(key = "warning")))
		{
			string message = property[key].AsString();
			if (string.IsNullOrWhiteSpace(message))
				return false;
			AddLabelBelow(message, EditorIcons.IconName.StatusWarning, Colors.Yellow with { S = .5f });
		}

		if (property.ContainsKey("error"))
		{
			string error = property["error"].AsString();
			if (string.IsNullOrWhiteSpace(error))
				return false;
			AddLabelBelow(error, EditorIcons.IconName.StatusError, Colors.Red with { S = .75f });
		}

		return false;

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
