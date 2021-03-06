﻿using System;
using CoreBluetooth;
using CoreLocation;
using CoreFoundation;
using UIKit;
using Foundation;

namespace registationBeacon
{
	public partial class ViewController : UIViewController
	{

		CBPeripheralManager peripheralMgr;
		BTPeripheralDelegate peripheralDelegate;


		protected ViewController(IntPtr handle) : base(handle)
		{
			// Note: this .ctor should not contain any initialization logic.
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			var uuid = new NSUuid("A1F30FF0-0A9F-4DE0-90DA-95F88164942E");
			var beaconId = "iOSBeacon";
			var beaconRegion = new CLBeaconRegion(uuid, beaconId)
			{
				NotifyEntryStateOnDisplay = true,
				NotifyOnEntry = true,
				NotifyOnExit = true
			};

			var peripheralData = beaconRegion.GetPeripheralData(new NSNumber(-59));

			peripheralDelegate = new BTPeripheralDelegate();
			peripheralMgr = new CBPeripheralManager(peripheralDelegate, DispatchQueue.DefaultGlobalQueue);
			peripheralMgr.StartAdvertising(peripheralData);
		}

		class BTPeripheralDelegate : CBPeripheralManagerDelegate
		{
			public override void StateUpdated(CBPeripheralManager peripheral)
			{
				if (peripheral.State == CBPeripheralManagerState.PoweredOn)
				{
					Console.WriteLine("iBeacon powered on");
				}
			}
		}

		//public override void DidReceiveMemoryWarning()
		//{
		//	base.DidReceiveMemoryWarning();
		//	// Release any cached data, images, etc that aren't in use.
		//}
	}
}
