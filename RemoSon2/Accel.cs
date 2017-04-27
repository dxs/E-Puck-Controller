using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Sensors;
using Windows.Foundation;

namespace RemoSon2
{
	public class Accel : INotifyPropertyChanged
	{
		private Accelerometer _accelerometer;
		private Data Data;
		private Sin Sin;
		private bool isOn = false;
		public bool IsOn
		{
			get { return isOn; }
			set
			{
				isOn = value;
				if (IsOn)
					_accelerometer.ReadingChanged += new TypedEventHandler<Accelerometer, AccelerometerReadingChangedEventArgs>(ReadingChanged);
				else
					_accelerometer.ReadingChanged -= new TypedEventHandler<Accelerometer, AccelerometerReadingChangedEventArgs>(ReadingChanged);
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsOn)));
			}
		}

		private double StabX = 0;
		private double StabY = 45;

		private double X = 0;
		private double Y = 0;
		private double Z = 0;

		public event PropertyChangedEventHandler PropertyChanged;

		public Accel(Data _data, Sin s)
		{
			Data = _data;
			Sin = s;
			_accelerometer = Accelerometer.GetDefault();
			if (_accelerometer != null)
			{
				// Establish the report interval
				uint minReportInterval = _accelerometer.MinimumReportInterval;
				uint reportInterval = minReportInterval > 16 ? minReportInterval : 16;
				_accelerometer.ReportInterval = 200;

				// Assign an event handler for the reading-changed event
				
			}
		}

		/// <summary>
		/// Lecture pour l'inclunaison vers l'avant en Y (tablette verticale = 1g, horizontale = 0)
		/// Lecture pour la rotation en X (0 au centre, 1 à 90° a droite, -1 a gauche
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void ReadingChanged(Accelerometer sender, AccelerometerReadingChangedEventArgs args)
		{
			ushort posx = 4, posy = 4;
			AccelerometerReading reading = args.Reading;
			X = (reading.AccelerationX);
			Y = (reading.AccelerationY);
			Z = reading.AccelerationZ;
			//Debug.WriteLine($"X : {X}, Y : {Y}, Z : {Z}");

			/*Retrouver l'angle en Y*/
			Y *= 0.8;
			Y += 0.1;
			double MappedAngleY = 180 / Math.PI * Math.Asin(Y);
			double MappedAngleX = 180 / Math.PI * Math.Asin(X);
			if (MappedAngleY > StabY - 2 && MappedAngleY < StabY + 2)
				posy = 4;
			else
			{
				//calcul ecart
				double ecart = StabY - MappedAngleY;
				if (ecart < 0)
				{
					ecart = Math.Abs(ecart);
					if (ecart > 2)//Val exp
						posy = 5;
					if (ecart > 8)//Val exp
						posy = 6;
					if (ecart > 13)//Val exp
						posy = 7;
					if (ecart > 18)//Val exp
						posy = 8;
				}
				else
				{
					ecart = Math.Abs(ecart);
					if (ecart > 2)//Val exp
						posy = 3;
					if (ecart > 4)//Val exp
						posy = 2;
					if (ecart > 6)//Val exp
						posy = 1;
					if (ecart > 10)//Val exp
						posy = 0;
				}
			}
			double upper = 5;
			double downer = -5;
			if (Math.Abs(MappedAngleX) < upper && Math.Abs(MappedAngleX) > downer)
				posx = 4;
			else
			{
				if (MappedAngleX < 90)
					posx = 8;
				if (MappedAngleX < 35)
					posx = 7;
				if (MappedAngleX < 25)
					posx = 6;
				if (MappedAngleX < 15)
					posx = 5;
				if (MappedAngleX < -5)
					posx = 3;
				if (MappedAngleX < -15)
					posx = 2;
				if (MappedAngleX < -25)
					posx = 1;
				if (MappedAngleX < -35)
					posx = 0;
			}

			Sin.PlaySound(posx, posy, Data);
		}

		public void SetCalib()
		{
			StabX = X;
			StabY = 180 / Math.PI * Math.Asin(Y);
		}
	}
}
