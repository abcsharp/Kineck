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
		private bool bowing;
		private Skeleton[] skeletons;

		public Form1()
		{
			InitializeComponent();
			var connectedSensors=KinectSensor.KinectSensors.Where(s=>s.Status==KinectStatus.Connected);
			if(connectedSensors.Count()==0){
				MessageBox.Show("Kinectが接続されていません。","Kineck",MessageBoxButtons.OK,MessageBoxIcon.Error);
				Application.Exit();
			}
			sensor=connectedSensors.First();
			sensor.SkeletonStream.Enable();
			sensor.SkeletonFrameReady+=SkeletonFrameReady;
			skeletons=new Skeleton[sensor.SkeletonStream.FrameSkeletonArrayLength];
			try{
				sensor.Start();
			}catch(Exception){
				MessageBox.Show("エラーが発生しました。","Kineck",MessageBoxButtons.OK,MessageBoxIcon.Error);
				Application.Exit();
			}
			bowing=false;
			sensor.ElevationAngle=5;
		}

		void SkeletonFrameReady(object sender,SkeletonFrameReadyEventArgs e)
		{
			using(var skeletonFrame=e.OpenSkeletonFrame()){
				if(skeletonFrame!=null){
					skeletonFrame.CopySkeletonDataTo(skeletons);
					if(skeletons[0].TrackingState==SkeletonTrackingState.Tracked&&!bowing&&checkBox1.Checked){
						button1.Enabled=false;
						bowing=true;
						ThreadPool.QueueUserWorkItem(Bow);
					}
				}
			}
		}

		private void Form1_FormClosing(object sender,FormClosingEventArgs e)
		{
			sensor.Stop();
		}

		private void Bow(object arg)
		{
			try{
				sensor.ElevationAngle=sensor.MinElevationAngle;
				Thread.Sleep(250);
				sensor.ElevationAngle=5;
			}catch(InvalidOperationException){
				MessageBox.Show("お辞儀に失敗しました。","Kineck",MessageBoxButtons.OK,MessageBoxIcon.Error);
			}
			Thread.Sleep(250);
			Invoke((Action)(()=>button1.Enabled=true));
			bowing=false;
		}

		private void button1_Click(object sender,EventArgs e)
		{
			button1.Enabled=false;
			bowing=true;
			ThreadPool.QueueUserWorkItem(Bow);
		}
	}
}
