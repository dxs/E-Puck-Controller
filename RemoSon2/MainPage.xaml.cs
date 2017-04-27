using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace RemoSon2
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
		Data Data = new Data();
		Accel Accel = null;
		Sin Sin = null;

		public MainPage()
        {
            this.InitializeComponent();
			Sin = new Sin(Player1,Player2);
			Accel = new Accel(Data,Sin);
        }

		private void MyJoystick_OnJoystickMoved(object sender, JoystickUserControl.JoystickEventArgs e)
		{
			if (Accel.IsOn)
				return;

			double x = e.XValue;
			double y = e.YValue;

			ushort posx = 0;
			ushort posy = (ushort)(((y + 1) * 4) + 0.5); //recupere la pos en x avec de increment de 1 de 0 à 8

			if (Data.IsOn)
				posx = (ushort)(((x + 1) * 4) + 0.5);
			else
				posx = (ushort)(9 - (((x + 1) * 4) + 0.5));

			Sin.PlaySound(posx, posy,Data);
		}

		private void MyJoystick_OnJoystickReleased(object sender, JoystickUserControl.JoystickEventArgs e)
		{
			Player1.Stop();
			Player2.Stop();
			Data.FX = 0;
			Data.FY = 0;
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			Accel.SetCalib();
		}
	}
}
