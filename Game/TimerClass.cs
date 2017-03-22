using System;
using System.Runtime.InteropServices;

namespace Game
{
	internal class TimerClass
	{
		private long StartTick = 0L;

		[DllImport("kernel32.dll")]
		private static extern long GetTickCount();

		public void Reset()
		{
			this.StartTick = TimerClass.GetTickCount();
		}

		public long GetTicks()
		{
			long tickCount = TimerClass.GetTickCount();
			return tickCount - this.StartTick;
		}
	}
}
