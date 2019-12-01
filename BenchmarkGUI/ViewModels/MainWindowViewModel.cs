using ReactiveUI;

namespace BenchmarkGUI.ViewModels
{
	public class MainWindowViewModel : ViewModelBase
	{
		public MainWindowViewModel()
		{
			
		}

		public string Greeting => "Welcome to Avalonia!";
		public uint Threads { get; set; } = 1u;
		public uint Runs { get; set; } = 3u;

		private bool _multithreaded = false;

		public bool Multithreaded
		{
			get => _multithreaded;
			set
			{
				this.RaiseAndSetIfChanged(ref _multithreaded, value);
				NotMultithreaded = !value;
			}
		}

		public bool NotMultithreaded
		{
			get => !_multithreaded;
			set
			{
				this.RaisePropertyChanged(nameof(NotMultithreaded));
			}
		}

		public string Benchmark { get; set; }

		public bool ListBenchmarks { get; set; }

		public bool ListResults { get; set; }

		public bool MemoryEfficient { get; set; } = false;

		public bool QuickRun { get; set; } = false;
	}
}