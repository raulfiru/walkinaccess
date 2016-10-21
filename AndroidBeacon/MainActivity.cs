using System;
using System.Linq;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using RadiusNetworks.IBeaconAndroid;

namespace AndroidBeacon
{
	[Activity (Label = "Haufe Hello!", MainLauncher = true)]
	public class MainActivity : Activity, IBeaconConsumer
	{
		const string UUID = "7ED7EA6B-A50E-4E79-83A0-903200A1D018";
		const string BEACON_ID = "iBeacon";
		IBeaconManager beaconMgr;
		MonitorNotifier monitorNotifier;
		RangeNotifier rangeNotifier;
		Region monitoringRegion;
		Region rangingRegion;
		TextView beaconStatusLabel;

		public MainActivity ()
		{
			beaconMgr = IBeaconManager.GetInstanceForApplication (this);

			monitorNotifier = new MonitorNotifier ();
			monitoringRegion = new Region (BEACON_ID, UUID, null, null);

			rangeNotifier = new RangeNotifier ();
			rangingRegion = new Region (BEACON_ID, UUID, null, null);
		}

		public void OnIBeaconServiceConnect ()
		{
			beaconMgr.SetMonitorNotifier(monitorNotifier);
			beaconMgr.SetRangeNotifier(rangeNotifier);

			beaconMgr.StartMonitoringBeaconsInRegion (monitoringRegion);
			beaconMgr.StartRangingBeaconsInRegion (rangingRegion);
		}

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			SetContentView (Resource.Layout.Main);

			beaconStatusLabel = FindViewById<TextView> (Resource.Id.beaconStatusLabel);

			beaconMgr.Bind (this);

			monitorNotifier.EnterRegionComplete += EnteredRegion;
			monitorNotifier.ExitRegionComplete += ExitedRegion;

			rangeNotifier.DidRangeBeaconsInRegionComplete += RangingBeaconsInRegion;
		}

		void EnteredRegion (object sender, MonitorEventArgs e)
		{
			ShowMessage ("Welcome back!");
		}

		void ExitedRegion (object sender, MonitorEventArgs e)
		{
			ShowMessage ("Thanks for shopping here!");
		}

		void RangingBeaconsInRegion (object sender, RangeEventArgs e)
		{
			if (e.Beacons.Count > 0) {
				var beacon = e.Beacons.FirstOrDefault ();

				switch ((ProximityType)beacon.Proximity) {
				case ProximityType.Immediate:
				case ProximityType.Near:
				case ProximityType.Far:
					ShowMessage ("Here's a coupon!", true);
					break;
				case ProximityType.Unknown:
					ShowMessage ("Beacon proximity unknown");
					break;
				}
			}
		}

		void ShowMessage(string message)
		{
			ShowMessage (message, false);
		}

		void ShowMessage (string message, bool showCoupon)
		{
			RunOnUiThread (() => {
				beaconStatusLabel.Text = message;

				var couponView = FindViewById<ImageView> (Resource.Id.imageView);

				if (showCoupon)
					couponView.SetImageResource (Resource.Drawable.qrcode);
				else
					couponView.SetImageResource (0);
			});
		}

		protected override void OnDestroy ()
		{
			base.OnDestroy ();

			monitorNotifier.EnterRegionComplete -= EnteredRegion;
			monitorNotifier.ExitRegionComplete -= ExitedRegion;

			rangeNotifier.DidRangeBeaconsInRegionComplete -= RangingBeaconsInRegion;

			beaconMgr.StopMonitoringBeaconsInRegion (monitoringRegion);
			beaconMgr.StopRangingBeaconsInRegion (rangingRegion);
			beaconMgr.UnBind (this);
		}
	}
}