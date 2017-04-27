using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;

namespace RemoSon2
{
	public class Sin
	{
		double res_freq = 64;//64.453125;
		const double TAU = 2 * Math.PI;
		int formatChunkSize = 16;
		int headerSize = 8;
		short formatType = 1;
		short tracks = 1;
		int samplesPerSecond = 44100;
		short bitsPerSample = 16;
		short frameSize;
		int bytesPerSecond;
		int waveSize = 4;
		int samples;
		int dataChunkSize;
		int fileSize;
		double amp = 16383 >> 2;

		private MediaElement Player1;
		private MediaElement Player2;

		public Sin(MediaElement p1, MediaElement p2)
		{
			Player1 = p1;
			Player2 = p2;
		}

		public void PlayBeep(UInt16 frequency, int msDuration, MediaElement _PlayTo)
		{
			var mStrm = new MemoryStream();
			BinaryWriter writer = new BinaryWriter(mStrm);

			samples = (int)((decimal)samplesPerSecond * msDuration / 1000);
			frameSize = (short)(tracks * ((bitsPerSample + 7) / 8));
			bytesPerSecond = samplesPerSecond * frameSize;
			dataChunkSize = samples * frameSize;
			fileSize = waveSize + headerSize + formatChunkSize + headerSize + dataChunkSize;

			// var encoding = new System.Text.UTF8Encoding();
			writer.Write(0x46464952); // = encoding.GetBytes("RIFF")
			writer.Write(fileSize);
			writer.Write(0x45564157); // = encoding.GetBytes("WAVE")
			writer.Write(0x20746D66); // = encoding.GetBytes("fmt ")
			writer.Write(formatChunkSize);
			writer.Write(formatType);
			writer.Write(tracks);
			writer.Write(samplesPerSecond);
			writer.Write(bytesPerSecond);
			writer.Write(frameSize);
			writer.Write(bitsPerSample);
			writer.Write(0x61746164); // = encoding.GetBytes("data")
			writer.Write(dataChunkSize);
			{
				double theta = res_freq * frequency * TAU / (double)samplesPerSecond;
				// 'volume' is UInt16 with range 0 thru Uint16.MaxValue ( = 65 535)
				// we need 'amp' to have the range of 0 thru Int16.MaxValue ( = 32 767)
				for (int step = 0; step < samples; step++)
				{
					short s = (short)(amp * Math.Sin(theta * (double)step));
					writer.Write(s);
				}
			}
			mStrm.Seek(0, SeekOrigin.Begin);


			_PlayTo.SetSource(mStrm.AsRandomAccessStream(), "");
			_PlayTo.Stop();
			_PlayTo.Play();
		}

		public async void PlaySound(ushort posx, ushort posy, Data Data)
		{
			var Dispatcher = CoreApplication.MainView.CoreWindow.Dispatcher;
			await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
			{
				if (Math.Abs(Data.old_posx - posx) >= 1)
				{
					Data.old_posx = posx;

					Data.FX = (ushort)(((posx + Data.FHigh)));// - res_freq/4);

					if (posx != 4)
						PlayBeep(Data.FX, 5000, Player1);

					else
					{
						Player1.Stop();
						Data.FX = 0;
					}
				}
				if (Math.Abs(Data.old_posy - posy) >= 1)
				{
					Data.old_posy = posy;

					Data.FY = (ushort)(posy + Data.FLow);// + res_freq / 2);

					if (posy != 4)
						PlayBeep(Data.FY, 5000, Player2);
					else
					{
						Player2.Stop();
						Data.FY = 0;
					}
				}
			});
		}
	}
}
