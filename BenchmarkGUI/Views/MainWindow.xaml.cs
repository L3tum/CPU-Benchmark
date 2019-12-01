#region using

using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

#endregion

namespace BenchmarkGUI.Views
{
	public class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
#if DEBUG
			this.AttachDevTools();
#endif
		}

		private void InitializeComponent()
		{
			AvaloniaXamlLoader.Load(this);
		}
	}
}