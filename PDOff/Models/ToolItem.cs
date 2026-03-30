using Avalonia.Media;

namespace PDOff.Models;

public record ToolItem(string Id, string Title, string Description, StreamGeometry Icon, Color AccentColor);
