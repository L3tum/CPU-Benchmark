#region using

using Avalonia;
using Avalonia.Markup.Xaml;

#endregion

namespace BenchmarkGUI
{
	public class App : Application
	{
		public override void Initialize()
		{
			AvaloniaXamlLoader.Load(this);
		}
	}
}