using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Microsoft.Kinect;

namespace Kineck
{
	public partial class Form1:Form
	{
		private KinectSensor sensor;

		public Form1()
		{
			InitializeComponent();
			var connectedSensors=KinectSensor.KinectSensors.Where(s=>s.Status==KinectStatus.Connected);
			if(connectedSensors.Count()==0){
				MessageBox.Show("Kinectが接続されていません。","Kineck",MessageBoxButtons.OK,MessageBoxIcon.Error);
				Application.Exit();
			}
			sensor=connectedSensors.First();
			try{
				sensor.Start();
			}catch(Exception){
				MessageBox.Show("エラーが発生しました。","Kineck",MessageBoxButtons.OK,MessageBoxIcon.Error);
				Application.Exit();
			}
			sensor.ElevationAngle=5;
		}

		private void Form1_FormClosing(object sender,FormClosingEventArgs e)
		{
			sensor.Stop();
		}

		private void Bow(object arg)
		{
			sensor.ElevationAngle=sensor.MinElevationAngle;
			Thread.Sleep(250);
			try{
				sensor.ElevationAngle=5;
			}catch(InvalidOperationException){
				MessageBox.Show("お辞儀に失敗しました。","Kineck",MessageBoxButtons.OK,MessageBoxIcon.Error);
			}
			Thread.Sleep(250);
			Invoke((Action)(()=>button1.Enabled=true));
		}

		private void button1_Click(object sender,EventArgs e)
		{
			button1.Enabled=false;
			ThreadPool.QueueUserWorkItem(Bow);
		}
	}
}
