namespace Benchmarking
{
	public class Options
	{
		public int Threads = 1;
#if DEBUG
		public int Repeated = 1;
#else
		public int Repeated = 3;
#endif
	}
}