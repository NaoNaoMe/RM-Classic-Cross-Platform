using System;
using System.Collections;
using System.Collections.Generic;

using Gtk;

namespace rmApplication
{
	public partial class OptionDialog : Gtk.Dialog
	{
		private class BaudRateItem
		{
			public BaudRateItem (string Ailias, int Value)
			{
				this.Ailias = Ailias;
				this.Value = Value;
			}

			public string Ailias;
			public int Value;
		}

		private SubViewControl SubViewCtrl;

		List<BaudRateItem> BaudRateItems;

		public OptionDialog (SubViewControl tmp)
		{
			this.Build ();
			SubViewCtrl = tmp;

			SubViewControl.Components.CommMode commMode = SubViewCtrl.myComponents.CommunicationMode;

			if (commMode == SubViewControl.Components.CommMode.NetWork)
			{
				radiobuttonNetwork.Active = true;
			}
			else
			{
				radiobuttonSerial.Active = true;
			}

			string[] PortList = System.IO.Ports.SerialPort.GetPortNames();

			ListStore comboModelPort = new ListStore(typeof(string));
			comboboxCommPort.Model = comboModelPort;

			bool foundFlg = false;
			int index = 0;

			foreach (var PortName in PortList)
			{
				comboModelPort.AppendValues (PortName);

				if (PortName == SubViewCtrl.myComponents.CommPort){
					comboboxCommPort.Active = index;
					foundFlg = true;

				}

				index++;

			}

			if ((foundFlg == false) && (PortList.Length > 0) ) {
				comboboxCommPort.Active = PortList.Length - 1;
				
			}

			entryLocalIP.Text = SubViewCtrl.myComponents.NetIP;
			entryLocalPort.Text = SubViewCtrl.myComponents.NetPort.ToString();

			BaudRateItems = new List<BaudRateItem> ();
			BaudRateItems.Add (new BaudRateItem("4800bps", 4800));
			BaudRateItems.Add (new BaudRateItem("9600bps", 9600));
			BaudRateItems.Add (new BaudRateItem("19200bps", 19200));
			BaudRateItems.Add (new BaudRateItem("38400bps", 38400));
			BaudRateItems.Add (new BaudRateItem("57600bps", 57600));
			BaudRateItems.Add (new BaudRateItem("115200bps", 115200));

			ListStore comboModel = new ListStore(typeof(string));
			comboboxBaudRate.Model = comboModel;

			index = 0;

			foreach (BaudRateItem item in BaudRateItems) {
				comboModel.AppendValues (item.Ailias);

				if (item.Value == SubViewCtrl.myComponents.CommBaudRate) {
					comboboxBaudRate.Active = index;
				}

				index++;
			}

			entryPassword.Text = SubViewCtrl.myComponents.Password;

			CommProtocol.Components.RmAddr adrWidth = SubViewCtrl.myCommProtocol.myComponents.SelectByte;

			if (adrWidth == CommProtocol.Components.RmAddr.Byte4)
			{
				radiobuttonAddrWd4byte.Active = true;
			}
			else
			{
				radiobuttonAddrWd2byte.Active = true;

			}

			buttonOk.Clicked += new EventHandler (buttonOk_Click);
			buttonCancel.Clicked += new EventHandler (buttonCancel_Click);

		}

		private void buttonOk_Click(object sender, EventArgs args)
		{
			if (radiobuttonSerial.Active)
			{
				SubViewCtrl.myComponents.CommPort = comboboxCommPort.ActiveText;

				SubViewCtrl.myComponents.CommunicationMode = SubViewControl.Components.CommMode.Serial;

			}
			else if (radiobuttonNetwork.Active)
			{
				SubViewCtrl.myComponents.NetIP = entryLocalIP.Text;
				SubViewCtrl.myComponents.NetPort = int.Parse(entryLocalPort.Text);

				SubViewCtrl.myComponents.CommunicationMode = SubViewControl.Components.CommMode.NetWork;

			}

			int index = comboboxBaudRate.Active;
			SubViewCtrl.myComponents.CommBaudRate = BaudRateItems[index].Value;

			SubViewCtrl.myComponents.Password = entryPassword.Text;

			if (radiobuttonAddrWd2byte.Active)
			{
				SubViewCtrl.myCommProtocol.myComponents.SelectByte = CommProtocol.Components.RmAddr.Byte2;

			}
			else
			{
				SubViewCtrl.myCommProtocol.myComponents.SelectByte = CommProtocol.Components.RmAddr.Byte4;

			}
			
		}

		private void buttonCancel_Click(object sender, EventArgs args)
		{

		}

	}
}

