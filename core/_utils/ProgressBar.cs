#region Usings
using System;
using System.Diagnostics;
using System.Text;
using System.Threading;
#endregion

namespace Altium.Utils
{
	/// <summary>
	/// An ASCII progress bar
	/// https://gist.github.com/DanielSWolf/0ab6a96899cc5377bf54
	/// </summary>
	internal class ProgressBar : IDisposable, IProgress<double>
	{
		#region Class members
		private const int blockCount = 10;
		private readonly TimeSpan animationInterval = TimeSpan.FromSeconds( 1.0 / 8 );
		private const string animation = @"|/-\";

		private readonly Timer timer;

		private double currentProgress = 0;
		private string currentText = string.Empty;
		private bool disposed = false;
		private int animationIndex = 0;

		private Stopwatch _stopwatch;
		#endregion

		#region Class initialization
		/// <summary>
		/// 
		/// </summary>
		public ProgressBar()
		{
			timer = new Timer( TimerHandler );

			// A progress bar is only for temporary display in a console window.
			// If the console output is redirected to a file, draw nothing.
			// Otherwise, we'll end up with a lot of garbage in the target file.
			if( !Console.IsOutputRedirected )
			{
				ResetTimer();
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="message"></param>
		public ProgressBar( string message ) : this() => Start( message );
		/// <summary>
		/// 
		/// </summary>
		public void Dispose()
		{
			lock( timer )
			{
				disposed = true;
				UpdateText( string.Empty );
			}
		}
		#endregion

		#region Class public methods
		/// <summary>
		/// 
		/// </summary>
		/// <param name="value"></param>
		public void Report( double value )
		{
			// Make sure value is in [0..1] range
			value = Math.Max( 0, Math.Min( 1, value ) );
			Interlocked.Exchange( ref currentProgress, value );
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="text"></param>
		/// <param name="color"></param>
		public void Start( string text, ConsoleColor color = ConsoleColor.Yellow )
		{
			Console.ForegroundColor = color;
			Console.Write( text );
			Console.ResetColor();

			_stopwatch = Stopwatch.StartNew();
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="text"></param>
		/// <param name="color"></param>
		public void Complete( string text, ConsoleColor color = ConsoleColor.Green )
		{
			Report( 1 );
			TimerHandler( null );

			_stopwatch.Stop();
			var time = TimeSpan.FromMilliseconds( _stopwatch.ElapsedMilliseconds );

			var memory = string.Format( "{0} peak working set | {1} private bytes",
							Process.GetCurrentProcess().PeakWorkingSet64.ToSizeSuffix(),
							Process.GetCurrentProcess().PrivateMemorySize64.ToSizeSuffix());

			Console.ForegroundColor = color;
			Console.WriteLine( $" {text}." );
			Console.ResetColor();

			Dispose();
		}
		#endregion

		#region Class 'Timer' methods
		/// <summary>
		/// 
		/// </summary>
		/// <param name="state"></param>
		private void TimerHandler( object state )
		{
			lock( timer )
			{
				if( disposed ) return;

				int progressBlockCount = ( int )( currentProgress * blockCount );
				int percent = ( int )( currentProgress * 100 );
				var time = TimeSpan.FromMilliseconds( _stopwatch.ElapsedMilliseconds );

				string text = string.Format( "[{0}{1}] {2,3}% [{4:0.00}s] {3}",
					new string( '#', progressBlockCount ), new string( '-', blockCount - progressBlockCount ),
					percent,
					animation [ animationIndex++ % animation.Length ], time.TotalSeconds );
				UpdateText( text );

				ResetTimer();
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="text"></param>
		private void UpdateText( string text )
		{
			// Get length of common portion
			int commonPrefixLength = 0;
			int commonLength = Math.Min( currentText.Length, text.Length );
			while( commonPrefixLength < commonLength && text [ commonPrefixLength ] == currentText [ commonPrefixLength ] )
			{
				commonPrefixLength++;
			}

			// Backtrack to the first differing character
			StringBuilder outputBuilder = new StringBuilder();
			outputBuilder.Append( '\b', currentText.Length - commonPrefixLength );

			// Output new suffix
			outputBuilder.Append( text.Substring( commonPrefixLength ) );

			// If the new text is shorter than the old one: delete overlapping characters
			int overlapCount = currentText.Length - text.Length;
			if( overlapCount > 0 )
			{
				outputBuilder.Append( ' ', overlapCount );
				outputBuilder.Append( '\b', overlapCount );
			}

			Console.Write( outputBuilder );
			currentText = text;
		}
		/// <summary>
		/// 
		/// </summary>
		private void ResetTimer()
		{
			timer.Change( animationInterval, TimeSpan.FromMilliseconds( -1 ) );
		}
		#endregion
	}
}
