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
			if (string.IsNullOrWhiteSpace(property["info"].AsString()))
				return false;
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
						hbox.AddThemeConstantOverride("separation", 4);
						m_container.AddChild(hbox);
						{
							TextureRect icon = new()
							{
								Texture = EditorIcons.GetIcon(EditorIcons.IconName.NodeInfo),
								StretchMode = TextureRect.StretchModeEnum.KeepAspectCentered,
								CustomMinimumSize = new Vector2(16, 16),
								Size = new Vector2(16, 16),
							};
							hbox.AddChild(icon);
						}
						{
							Label label = new()
							{
								Text = property["info"].AsString(),
								CustomMinimumSize = new Vector2(0, 24),
								AutowrapMode = TextServer.AutowrapMode.WordSmart,
							};
							label.SizeFlagsHorizontal = Control.SizeFlags.Fill | Control.SizeFlags.Expand;
							hbox.AddChild(label);
						}
					}
				}
			}
		}

		if (property.ContainsKey("comment"))
		{
			string comment = "💬 " + property["comment"].AsString();
			if (!string.IsNullOrWhiteSpace(comment))
			{
				Label label = new()
				{
					Text = comment,
					CustomMinimumSize = new Vector2(0, 24),
					AutowrapMode = TextServer.AutowrapMode.WordSmart,
				};
				label.AddThemeColorOverride("font_color", Colors.DimGray);
				MarginContainer container = new();
				container.AddThemeConstantOverride("margin_left", 4);
				container.AddChild(label);
				this.AddPropertyEditor(name, container, addToEnd: true);
			}
		}

		if (property.ContainsKey("warn"))
		{
			string warn = "⚠ " + property["warn"].AsString();
			if (!string.IsNullOrWhiteSpace(warn))
			{
				Label label = new()
				{
					Text = warn,
					CustomMinimumSize = new Vector2(0, 24),
					AutowrapMode = TextServer.AutowrapMode.WordSmart,
				};
				label.AddThemeColorOverride("font_color", Colors.Yellow with { S = .5f });
				MarginContainer container = new();
				container.AddThemeConstantOverride("margin_left", 4);
				container.AddChild(label);
				this.AddPropertyEditor(name, container, addToEnd: true);
			}
		}

		if (property.ContainsKey("error"))
		{
			string error = "❌ " + property["error"].AsString();
			if (!string.IsNullOrWhiteSpace(error))
			{
				Label label = new()
				{
					Text = error,
					CustomMinimumSize = new Vector2(0, 24),
					AutowrapMode = TextServer.AutowrapMode.WordSmart,
				};
				label.AddThemeColorOverride("font_color", Colors.Red with { S = .75f });
				MarginContainer container = new();
				container.AddThemeConstantOverride("margin_left", 4);
				container.AddChild(label);
				this.AddPropertyEditor(name, container, addToEnd: true);
			}
		}

		return false;
	}
}
#endif
