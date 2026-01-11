#if TOOLS
using Godot;

namespace Raele.InspectorCallout;

[Tool]
public partial class Plugin : EditorPlugin
{
	private InspectorPlugin InspectorPlugin = new();

	public override void _EnterTree()
	{
		this.AddInspectorPlugin(this.InspectorPlugin);
	}

	public override void _ExitTree()
	{
		this.RemoveInspectorPlugin(this.InspectorPlugin);
	}
}
#endif
