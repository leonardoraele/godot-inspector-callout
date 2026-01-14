#if TOOLS
using System;
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
		MemberInfo? member = @object.GetType().GetField(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy) as MemberInfo
			?? @object.GetType().GetProperty(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);

		if (@object is Resource && member?.GetCustomAttribute<ExportCategoryAttribute>() is ExportCategoryAttribute category)
		{
			PanelContainer panel = new();
			panel.CustomMinimumSize = new Vector2(0, 24);
			panel.SizeFlagsHorizontal = Control.SizeFlags.Fill | Control.SizeFlags.Expand;
			// panel.AddThemeStyleboxOverride("panel", new StyleBoxFlat()
			// {
			// 	BgColor = Colors.Red,
			// });
			this.AddCustomControl(panel);
			{
				RichTextLabel label = new()
				{
					Text = $"[b]{category.Name}[/b]",
					BbcodeEnabled = true,
					FitContent = true,
					AutowrapMode = TextServer.AutowrapMode.WordSmart,
					HorizontalAlignment = HorizontalAlignment.Center,
					VerticalAlignment = VerticalAlignment.Center,
				};
				panel.AddChild(label);
			}
		}

		// if (field?.GetCustomAttribute<HideWhenAttribute>() is HideWhenAttribute hideWhen && !hideWhen.TestShow(@object))
		// 	return true;

		if (member?.GetCustomAttribute<SeparatorAttribute>() is SeparatorAttribute separator) separator.Evaluate(this);
		if (member?.GetCustomAttribute<MarginAttribute>() is MarginAttribute margin) margin.Evaluate(this);

		// string? tooltip = member?.GetCustomAttribute<TooltipAttribute>() is TooltipAttribute tooltipAttribute
		// 	? tooltipAttribute.Text
		// 	: null;

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

		if (property.ContainsKey("info") && property["info"].AsString() is string info && !string.IsNullOrWhiteSpace(info))
			AddInfo(info);

		if (property.ContainsKey("comment") && property["comment"].AsString() is string comment && !string.IsNullOrWhiteSpace(comment))
			AddComment(comment);


		{
			if (
				member?.GetCustomAttribute<RequiredAttribute>() is RequiredAttribute required
				&& required.TestIsError(@object, name, out string? error)
			)
			{
				AddError(error);
				return false;
			}
		}

		if (
			"" is string key
			&& (property.ContainsKey(key = "warn") || property.ContainsKey(key = "warning"))
			&& property[key].AsString() is string message
			&& !string.IsNullOrWhiteSpace(message)
		)
			AddWarning(message);

		{
			if (property.ContainsKey("error") && property["error"].AsString() is string error && !string.IsNullOrWhiteSpace(error))
				AddError(error);
		}

		if (
			member?.GetCustomAttribute<CalloutAttribute>() is CalloutAttribute callout
			&& !string.IsNullOrWhiteSpace(callout.Note)
			&& callout.Test(@object)
		)
			switch (callout.Type)
			{
				case CalloutAttribute.CalloutType.Info:
					AddInfo(callout.Note);
					break;
				case CalloutAttribute.CalloutType.Comment:
					AddComment(callout.Note);
					break;
				case CalloutAttribute.CalloutType.Warning:
					AddWarning(callout.Note);
					break;
				case CalloutAttribute.CalloutType.Error:
					AddError(callout.Note);
					break;
				default:
					GD.PushWarning($"Unknown {nameof(CalloutAttribute.CalloutType)} \"{callout.Type}\" in field \"{member.Name}\" of object \"{@object.GetType().Name}\".");
					break;
			}

		return false;

		void AddInfo(string message) => AddDialogAbove(message, EditorIcons.IconName.NodeInfo);
		void AddComment(string message)
			=> AddLabelBelow(message, EditorIcons.IconName.VisualShaderNodeComment, Colors.DimGray, Colors.White with { A = .25f });
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

		void AddLabelBelow(string message, string iconName, Color color, Color? modulate = null)
		{
			MarginContainer margin = new();
			margin.AddThemeConstantOverride("margin_left", 8);
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
						Modulate = modulate ?? Colors.White,
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
					label.AddThemeColorOverride("default_color", color);
					label.SizeFlagsHorizontal = Control.SizeFlags.Fill | Control.SizeFlags.Expand;
					hbox.AddChild(label);
				}
			}
		}
	}
}
#endif
