using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace RemoSon2
{
	public class Data : INotifyPropertyChanged
	{
		public double old_posx = 4;
		public double old_posy = 4;

		private bool isOn = true;
		public bool IsOn
		{
			get { return isOn; }
			set { isOn = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsOn))); }
		}

		private ushort fX = 0;
		public ushort FX
		{
			get { return fX; }
			set { fX = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(FX))); }
		}

		private ushort fY = 0;
		public ushort FY
		{
			get { return fY; }
			set { fY = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(FY))); }
		}

		private ushort fLow = 110;
		public ushort FLow
		{
			get { return fLow; }
			set { fLow = value;PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(FLow))); }
		}

		private ushort fHigh = 110;
		public ushort FHigh
		{
			get { return fHigh; }
			set { fHigh = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(FHigh))); }
		}

		public Data()
		{

		}
		public event PropertyChangedEventHandler PropertyChanged;
	}

	public class FreqConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, string language)
		{
			ushort? v = value as ushort?;
			if (v == 0)
				return "OFF";
			double res_freq = 64;
			double? final = v * res_freq;
			return (int)final + "Hz";
		}

		public object ConvertBack(object value, Type targetType, object parameter, string language)
		{
			throw new NotImplementedException();
		}
	}
}
