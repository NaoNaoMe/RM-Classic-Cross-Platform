using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;

using Gtk;

namespace rmApplication
{
	[System.ComponentModel.ToolboxItem (true)]
	public partial class SubViewControl : Gtk.Bin
	{
		private ListStore storeMain;
		private System.IO.Ports.SerialPort serialPort;
		private TreeIter userIter;

		private class CheckedRowItem
		{
			public CheckedRowItem (bool Check, string Size)
			{
				this.Check = Check;
				this.Size = Size;
			}

			public bool Check;
			public string Size;
		}

		private List<CheckedRowItem> checkedRowList;

		public class Components
		{
			public enum CommMode
			{
				NotDefine,
				Serial,
				NetWork
			}

			public CommMode CommunicationMode { set; get; }
			public string CommPort { set; get; }
			public int CommBaudRate { set; get; }
			public string NetIP { set; get; }
			public int NetPort { set; get; }
			public string Password { set; get; }

			public bool RsrcActiveFlg { set; get; }
			public bool CommActiveFlg { set; get; }
			public bool LoggingActiveFlg { set; get; }
			public string ValidMapPath { set; get; }
			public DateTime ValidMapLastWrittenDate { set; get; }

			public List<MapFactor> MapList { set; get; }
			public List<ViewSetting> ViewSettingList { set; get; }
			public string SettingName { set; get; }
			public string SettingVer { set; get; }
			public string TargetVer { set; get; }

			public Components()
			{
				CommunicationMode = CommMode.NotDefine;
				CommPort = "";
				CommBaudRate = 9600;
				NetIP = "192.168.0.255";
				NetPort = 49152;
				Password = "0000FFFF";

				RsrcActiveFlg = false;
				CommActiveFlg = false;
				LoggingActiveFlg = false;
				ValidMapPath = "";
				ValidMapLastWrittenDate = DateTime.MinValue;

				MapList = new List<MapFactor>();
				ViewSettingList = new List<ViewSetting>();
				SettingName = "Sample";
				SettingVer = "001";
				TargetVer = "";

			}

			public Components( Components data )
			{
				CommunicationMode = data.CommunicationMode;
				CommPort = data.CommPort;
				CommBaudRate = data.CommBaudRate;
				NetIP = data.NetIP;
				NetPort = data.NetPort;
				Password = data.Password;

				RsrcActiveFlg = data.RsrcActiveFlg;
				CommActiveFlg = data.CommActiveFlg;
				LoggingActiveFlg = data.LoggingActiveFlg;
				ValidMapPath = data.ValidMapPath;
				ValidMapLastWrittenDate = data.ValidMapLastWrittenDate;

				MapList = data.MapList;
				ViewSettingList = data.ViewSettingList;
				SettingName = data.SettingName;
				SettingVer = data.SettingVer;
				TargetVer = data.TargetVer;

			}

		}

		public Components myComponents;
		public CommProtocol myCommProtocol;

		private enum RecordMode
		{
			ClipBoard,
			CSV
		}

		private struct SocketsAsyncParam
		{
			public System.Net.Sockets.TcpClient Client;
			public byte[] ReadBuff;
		}

		private const int DEFAULT_COLUMNS = 16;
		private const int SELECT_NUM = 32;
		private const int MAX_TOTAL_SIZE = 128;

		private const string GROUP_TEMPORARY_TAG = "x:Test";
		private const string TARGET_VER_TAG = "_TgV";
		private const string SETTING_VER_TAG = "_StV";

		private const int COMM_LOG_MAX = 500;
		private const int RCV_LOGDATA_MAX = 10000;

		private const int ERR_NUM_NOTHING = 0x00;
		private const int ERR_NUM_SIZE = 0x01;
		private const int ERR_NUM_ADDRESS = 0x02;
		private const int ERR_NUM_OFFSET = 0x04;

		private DumpWindow DumpFormInstance;

		private SocketsAsyncParam SocketsParam;
		private DateTime LogStartTime;
		private List<List<string>> RcvLogData;
		private int ContinueCnt;
		private int LastSlvCnt;
		private int WarningShowUpCount;

		public string getViewSettingFileName()
		{
			if (string.IsNullOrEmpty(myComponents.TargetVer) == true)
			{
				return myComponents.SettingName + SETTING_VER_TAG + myComponents.SettingVer;

			}
			else
			{
				return myComponents.SettingName + SETTING_VER_TAG + myComponents.SettingVer + TARGET_VER_TAG + myComponents.TargetVer;

			}

		}

		public string getViewName( string fileName )
		{
			string retText = null;

			if (fileName == null)
			{

			}
			else
			{
				int firstCharacter = fileName.IndexOf(SETTING_VER_TAG);
				int secondCharacter = fileName.IndexOf(TARGET_VER_TAG);


				if (firstCharacter > 0)
				{
					string text;
					int tmpIndex;
					int length;

					length = firstCharacter;
					text = fileName.Substring(0, length);
					myComponents.SettingName = text;

					if ((secondCharacter > 0) &&
						(secondCharacter > firstCharacter))
					{
						tmpIndex = firstCharacter + 4;
						length = secondCharacter - tmpIndex;
						text = fileName.Substring(tmpIndex, length);
						myComponents.SettingVer = text;

						tmpIndex = secondCharacter + 4;
						length = fileName.Length - tmpIndex;
						text = fileName.Substring(tmpIndex, length);
						myComponents.TargetVer = text;

						entryTgtVer.Text = myComponents.TargetVer;

					}
					else
					{
						tmpIndex = firstCharacter + 4;
						length = fileName.Length - tmpIndex;
						text = fileName.Substring(tmpIndex, length);
						myComponents.SettingVer = text;

					}

					retText = myComponents.SettingName + "(" + myComponents.SettingVer + ")";

				}

			}

			return retText;

		}


		public void loadViewSettingFile( ViewSetting tmp )
		{
			myComponents.ViewSettingList = new List<ViewSetting>();

			var pageList = new List<string>();

			string groupName = null;

			var tmpVSettingFactor = new ViewSetting();

			foreach (var factor in tmp.DataSetting)
			{
				if (groupName == null)
				{
					groupName = factor.Group;
					tmpVSettingFactor.DataSetting.Add(factor);

					if (groupName == null)
					{
						groupName = SubViewControl.GROUP_TEMPORARY_TAG;
						tmpVSettingFactor.DataSetting[0].Group = groupName;

					}

					pageList.Add(groupName);

				}
				else if ((factor.Group != null) &&
						  (factor.Group != groupName))
				{
					groupName = factor.Group;
					pageList.Add(groupName);

					myComponents.ViewSettingList.Add(tmpVSettingFactor);

					tmpVSettingFactor = new ViewSetting();

					tmpVSettingFactor.DataSetting.Add(factor);

				}
				else
				{
					tmpVSettingFactor.DataSetting.Add(factor);

				}

			}

			if (tmpVSettingFactor != null)
			{
				myComponents.ViewSettingList.Add(tmpVSettingFactor);

			}

			comboboxPage.Changed -= new System.EventHandler(pageValComboBox_SelectedIndexChanged);

			//comboboxPage.Clear();		// I'm not sure what it doesn't need a Clear method.
			ListStore comboModel = new ListStore(typeof(string));
			comboboxPage.Model = comboModel;

			foreach (var factor in pageList)
			{
				comboModel.AppendValues(factor);

			}

			comboboxPage.Changed += new System.EventHandler(pageValComboBox_SelectedIndexChanged);

			if ((myComponents.MapList != null) &&
				(myComponents.MapList.Count > 0))
			{
				PutWarningMessage( "Purge map file info." );

				myComponents.MapList = new List<MapFactor>();
				myComponents.ValidMapPath = null;
				myComponents.ValidMapLastWrittenDate = DateTime.MinValue;

			}

			comboboxPage.Active = 0;

		}


		public bool loadMapFile(string path)
		{
			myComponents.MapList = new List<MapFactor>();

			bool retFlg = false;
			string[] textArray;

			try
			{
				var sr = new System.IO.StreamReader(path, System.Text.Encoding.GetEncoding("utf-8"));

				string wholeText = sr.ReadToEnd();
				textArray = wholeText.Replace("\r\n", "\n").Split('\n');	// you shold Consider about unix enviroment!!

				sr.Close();

			}
			catch (Exception ex)
			{
				MessageBox.ShowWarning(ex.Message);

				return retFlg;

			}

			var date = System.IO.File.GetLastWriteTime(path);

			if (retFlg == false)
			{
				retFlg = ReadElfMap.Interpret(textArray, myComponents.MapList);

			}

			if (retFlg == false)
			{
				retFlg = RmAddressMap.Interpret(textArray, myComponents.MapList);

			}

			if (retFlg == true)
			{
				myComponents.ValidMapPath = path;
				myComponents.ValidMapLastWrittenDate = date;

				List<string> variableList = new List<string>();

				foreach (var factor in myComponents.MapList)
				{
					variableList.Add(factor.VariableName);

				}

				if ((myComponents.ViewSettingList != null) &&
					(myComponents.ViewSettingList.Count > 0))
				{
					reviseDataFromViewSettingList();

				}

				if (string.IsNullOrEmpty(myComponents.TargetVer) == true)
				{
					myComponents.TargetVer = System.IO.Path.GetFileNameWithoutExtension(path);
					entryTgtVer.Text = myComponents.TargetVer;

				}

			}
			else
			{
				myComponents.ValidMapPath = null;
				myComponents.ValidMapLastWrittenDate = DateTime.MinValue;

			}

			return retFlg;
		}

		private void reviseDataFromViewSettingList()
		{
			foreach (var factor in myComponents.ViewSettingList)
			{
				foreach (var itemDS in factor.DataSetting)
				{
					if ((myComponents.MapList == null) ||
						(myComponents.MapList.Count <= 0))
					{
						//not import MapFile

					}
					else
					{
						if ((itemDS.Variable != null) &&
							(itemDS.AddrLock != true))
						{
							string tmpVariable = itemDS.Variable.ToString();

							MapFactor result = myComponents.MapList.Find(key => key.VariableName == tmpVariable);

							if (result != null)
							{
								if ((result.Size == "1") ||
									(result.Size == "2") ||
									(result.Size == "4"))
								{
									itemDS.Size = result.Size;

								}
								
								itemDS.Address = result.Address;

							}
							else
							{
								itemDS.Address = null;

							}

						}

					}

				}

			}

		}


		private void refreshTreeView( int index )
		{
			if (myComponents.ViewSettingList == null)
			{
				return;
			}

			if (index <= -1)
			{
				index = 0;

			}

			storeMain = new ListStore (typeof (DataSetting));
			foreach (DataSetting item in myComponents.ViewSettingList[index].DataSetting) {
				storeMain.AppendValues (item);
			}

			treeviewMain.Model = storeMain;

			TreeIter iter;
			if (storeMain.GetIterFirst (out iter))
			{
				int i = 1;

				do
				{
					DataSetting factor = (DataSetting) storeMain.GetValue (iter, 0);

					factor.RowCount = i;
					i++;

					if ((string.IsNullOrEmpty(factor.Size) == true) ||
						(string.IsNullOrEmpty(factor.Type) == true) ||
						(string.IsNullOrEmpty(factor.WriteValue) == true))
					{

					}
					else
					{
						int intSize = int.Parse(factor.Size);
						Exception ex;

						string retText = TypeConvert.ToHexChars(factor.Type, intSize, factor.WriteValue, out ex);
						factor.WriteText = retText;

					}


				} while(storeMain.IterNext (ref iter));

			}

		}

		public bool checkTreeViewCells()
		{
			int maxIndex = 0;

			bool errFlg = false;
			int totalSize = 0;

			TreeIter iter;
			if (storeMain.GetIterFirst (out iter))
			{
				do
				{
					DataSetting factor = (DataSetting) storeMain.GetValue (iter, 0);

					if((bool) factor.Check)
					{
						maxIndex++;

						if ( factor.ErrNum != ERR_NUM_NOTHING )
						{
							errFlg = true;

						}
						else
						{
							string sizeText = factor.Size;
							int num = int.Parse(sizeText);
							totalSize += num;

						}

					}

				} while(storeMain.IterNext (ref iter));

			}


			if (maxIndex == 0)
			{
				PutWarningMessage( "No cheked cells." );
			}
			else if (errFlg == true)
			{
				PutWarningMessage( "Invalid data found." );
			}
			else if (totalSize > MAX_TOTAL_SIZE)
			{
				PutWarningMessage( "Total size is invalid. ( Total size <= 128 )" );
			}
			else if (maxIndex > SELECT_NUM)
			{
				PutWarningMessage( "Selected item is invalid. ( Total item <= 32 )" );
			}
			else
			{

			}

			labelArea1.Text = "Checked cell number =" + maxIndex.ToString() + " / " + "Total size =" + totalSize.ToString() + "bytes";

			if ( (totalSize > 0) &&
				(myComponents.CommunicationMode != Components.CommMode.NotDefine))
			{
				// SlipCode(1byte) + (MSCnt(1byte) + payload(?byte) + crc(1byte)) * 2 + SlipCode(1byte)
				double frameSize = 1 + (1 + totalSize + 1) * 2 + 1;
				double abyteTxTime = (1 / (double)myComponents.CommBaudRate) * 10;
				double targetTxTime = frameSize * abyteTxTime * 1000;

				labelArea2.Text = "Target Max Tx Time =" + targetTxTime.ToString("F2") + "ms";

			}
			else
			{
				labelArea2.Text = "-";

			}


			return errFlg;

		}


		private StringBuilder makeLogData(RecordMode mode)
		{
			StringBuilder text = new StringBuilder();

			if (RcvLogData == null)
			{
				return text;
			}

			string note = "Start Logging time: " + LogStartTime.ToString();

			text.AppendLine(note);

			string delimiter;

			if (mode == RecordMode.ClipBoard)
			{
				delimiter = "\t";
			}
			else
			{
				delimiter = ",";
			}

			string header = "Rcv Status" + delimiter + "OS Timer" + delimiter + "Count";

			TreeIter iter;
			if (storeMain.GetIterFirst (out iter)) {
				do {
					DataSetting factor = (DataSetting)storeMain.GetValue (iter, 0);

					bool checkFlg = factor.Check;
					string nameText = factor.Name;

					if (checkFlg == true) {
						header = header + delimiter + nameText;

					}

				} while(storeMain.IterNext (ref iter));

				text.AppendLine(header);

			}

			if (RcvLogData.Count != 0)
			{
				foreach (var list in RcvLogData)
				{
					string line = "";

					foreach (var data in list)
					{
						if (line == "")
						{
							line = data;

						}
						else
						{
							line = line + delimiter + data;

						}

					}

					text.AppendLine(line);

				}

			}

			return text;

		}

		#if false
		private void editingCtrl(bool swFlg)
		{
		}

		private void visibleCtrl(bool swFlg)
		{
		}
		#endif

		private void readDUTVersion()
		{
			if (myComponents.CommActiveFlg == false)
			{
				return;

			}

			myCommProtocol.readVersion(myComponents.Password);

		}


		private void renewLogSetting()
		{
			if (myComponents.CommActiveFlg == false)
			{
				return;

			}

			List<CommProtocol.SetLogParam> listParam = new List<CommProtocol.SetLogParam>();
			checkedRowList = new List<CheckedRowItem> ();

			bool errFlg = false;

			TreeIter iter;
			if (storeMain.GetIterFirst (out iter))
			{
				do
				{
					DataSetting factor = (DataSetting) storeMain.GetValue (iter, 0);

					bool checkFlg = factor.Check;
					string sizeText = factor.Size;

					if( checkFlg == true )
					{
						CommProtocol.SetLogParam tmpParam = new CommProtocol.SetLogParam();

						string addressText = factor.Address;
						string offsetText = factor.Offset;
						string typeText = factor.Type;

						if ((string.IsNullOrEmpty(sizeText) == true) ||
							(string.IsNullOrEmpty(addressText) == true) ||
							(string.IsNullOrEmpty(offsetText) == true) ||
							(string.IsNullOrEmpty(typeText) == true))
						{
							errFlg = true;
							break;

						}
						else
						{
							Int64 intAddress = 0;
							Int64 intOffset = 0;

							try
							{
								intAddress = Convert.ToInt64(addressText, 16);
								intOffset = Convert.ToInt64(offsetText);

							}
							catch (Exception ex)
							{
								PutWarningMessage( ex.Message );

								errFlg = true;
								break;

							}

							Exception ex_text = null;

							string newAddressText = TypeConvert.ToHexChars(numeralSystem.UDEC, 4, ((intAddress + intOffset).ToString()), out ex_text);

							if (ex_text != null)
							{
								return;

							}

							tmpParam.Size = sizeText;
							tmpParam.Address = newAddressText;
							listParam.Add(tmpParam);

						}

					}

					checkedRowList.Add(new CheckedRowItem(checkFlg, sizeText));

				} while(storeMain.IterNext (ref iter));

			}

			if (listParam.Count == 0)
			{

			}
			else if (errFlg == true)
			{

			}
			else
			{
				myCommProtocol.setLogData(listParam);

				myCommProtocol.setLogModeStart();

			}

		}


		private void renewTimingSetting(string timingNum)
		{
			if (myComponents.CommActiveFlg == false)
			{
				return;

			}

			Exception ex_text = null;

			string timngVal = TypeConvert.ToHexChars(numeralSystem.UDEC, 2, timingNum, out ex_text);

			myCommProtocol.setTiming(timngVal);

		}


		private void writeData(string size, string address, string offset, string writeVal)
		{
			if (myComponents.CommActiveFlg == false)
			{
				return;

			}

			Int64 intAddress = 0;
			Int64 intOffset = 0;

			try
			{
				intAddress = Convert.ToInt64(address, 16);
				intOffset = Convert.ToInt64(offset);

			}
			catch (Exception ex)
			{
				PutWarningMessage( ex.Message );
				return;

			}

			Exception ex_text = null;

			address = TypeConvert.ToHexChars(numeralSystem.UDEC, 4, ((intAddress + intOffset).ToString()), out ex_text);

			writeVal = TypeConvert.FromHexChars(numeralSystem.HEX, int.Parse(size), writeVal);

			if (ex_text != null)
			{
				return;

			}

			if (string.IsNullOrEmpty (writeVal) == true) {
				return;
			}

			myCommProtocol.wirteData(size, address, writeVal);

		}


		private bool serialPort_DataReceived()
		{
			if (myComponents.CommActiveFlg == false)
			{
				return true;

			}

			if (serialPort.IsOpen == false)
			{
				return true;

			}

			try
			{
				int size = serialPort.BytesToRead;

				if( size > 0 )
				{
					byte[] rcvbuff = new byte[size];

					serialPort.Read(rcvbuff, 0, size);

					myCommProtocol.decode(rcvbuff);

				}

			}
			catch (Exception ex)
			{
				PutWarningMessage( ex.Message );

			}

			return true;
		}


		private bool serialPort_DataSend(List<byte> frame)
		{
			bool retFlg = false;

			if (serialPort.IsOpen == false)
			{

			}
			else
			{
				try
				{
					List<byte> txBuff = new List<byte>(frame);

					byte[] tmp = myCommProtocol.encode(txBuff);

					serialPort.Write(tmp, 0, tmp.Length);

					retFlg = true;

				}
				catch (Exception ex)
				{
					PutWarningMessage( ex.Message );

				}

			}

			return retFlg;
		}


		private bool serialPort_SelectState(bool reqFlg)
		{
			bool retFlg = false;

			if (reqFlg == true)
			{
				if ((serialPort.IsOpen == false) &&
					(myComponents.CommunicationMode == Components.CommMode.Serial) &&
					(string.IsNullOrEmpty (myComponents.CommPort) == false) &&
					(myComponents.CommBaudRate != 0))
				{
					serialPort.PortName = myComponents.CommPort;
					serialPort.BaudRate = myComponents.CommBaudRate;

					serialPort.DataBits = 8;
					serialPort.Parity = System.IO.Ports.Parity.None;
					serialPort.StopBits = System.IO.Ports.StopBits.One;
					serialPort.Handshake = System.IO.Ports.Handshake.None;

					try
					{
						serialPort.Open();

						retFlg = true;

					}
					catch (Exception ex)
					{
						MessageBox.ShowWarning(ex.Message);

					}

				}


			}
			else
			{
				if (serialPort.IsOpen == true)
				{
					serialPort.Close();

					retFlg = true;

				}

			}

			return retFlg;
		}


		private void sockets_DataReceived(IAsyncResult ar)
		{
			if (myComponents.CommActiveFlg == false)
			{
				return;

			}

			SocketsAsyncParam ap = (SocketsAsyncParam)ar.AsyncState;

			if(ap.Client.Connected == false)
			{
				return;
			}

			System.Net.Sockets.NetworkStream stream = ap.Client.GetStream();

			int size = stream.EndRead(ar);

			byte[] rcvbuff = new byte[size];

			for (int i = 0; i < size; i++)
			{
				rcvbuff [i] = ap.ReadBuff [i];

			}

			myCommProtocol.decode(rcvbuff);

			stream.BeginRead(ap.ReadBuff, 0, ap.ReadBuff.Length, new AsyncCallback(sockets_DataReceived), ap);

		}


		private bool sockets_DataSend(List<byte> frame)
		{
			bool retFlg = false;

			System.Net.Sockets.NetworkStream stream = SocketsParam.Client.GetStream();

			List<byte> txBuff = new List<byte>(frame);

			byte[] tmp = myCommProtocol.encode(txBuff);

			stream.Write(tmp, 0, tmp.Length);

			retFlg = true;

			return retFlg;
		}


		private bool sockets_SelectState(bool reqFlg)
		{
			bool retFlg = false;

			if (reqFlg == true)
			{
				if ((myComponents.CommunicationMode == Components.CommMode.NetWork) &&
					(string.IsNullOrEmpty (myComponents.NetIP) == false) &&
					(myComponents.NetPort != 0))
				{
					try
					{
						SocketsParam.Client = new System.Net.Sockets.TcpClient();
						SocketsParam.ReadBuff = new byte[16];
						SocketsParam.Client.Connect(myComponents.NetIP, myComponents.NetPort);

						System.Net.Sockets.NetworkStream stream = SocketsParam.Client.GetStream();

						stream.BeginRead(SocketsParam.ReadBuff, 0, SocketsParam.ReadBuff.Length, new AsyncCallback(sockets_DataReceived), SocketsParam);

						stream = SocketsParam.Client.GetStream();
						retFlg = true;

					}
					catch (Exception ex)
					{
						MessageBox.ShowWarning(ex.Message);

					}

				}
				

			}
			else
			{
				if (SocketsParam.Client != null)
				{
					SocketsParam.Client.Close();
					retFlg = true;

				}

			}

			return retFlg;
		}


		public bool commResource_CheckState()
		{
			bool retFlg = false;

			switch (myComponents.CommunicationMode)
			{
			case Components.CommMode.NotDefine:

				break;

			case Components.CommMode.Serial:
				if (serialPort.IsOpen == true)
				{
					retFlg = true;

				}

				break;

			case Components.CommMode.NetWork:
				if ( SocketsParam.Client.Connected == true)
				{
					retFlg = true;

				}

				break;

			}

			return retFlg;
		}


		public SubViewControl ()
		{
			this.Build ();

			myComponents = new Components();
			myCommProtocol = new CommProtocol();

			commonInitialRoutine();

		}


		public SubViewControl(Components tmp)
		{
			this.Build ();

			myComponents = tmp;
			myCommProtocol = new CommProtocol();

			commonInitialRoutine();

		}

		public void commonInitialRoutine()
		{
			// construct tree view
			ConstructAppearance (treeviewMain);

			// create model
			ViewSetting tmp = new ViewSetting ();
			for (int i = 0; i < DEFAULT_COLUMNS; i++) {
				tmp.DataSetting.Add (new DataSetting());
			}

			loadViewSettingFile (tmp);

			this.ShowAll ();

			serialPort = new System.IO.Ports.SerialPort("MySerialPort", 38400);

			GLib.Timeout.Add(10, new GLib.TimeoutHandler(serialPort_DataReceived));
			GLib.Timeout.Add(100, new GLib.TimeoutHandler(mainTimer_Tick));

			buttonRsrc.Clicked += new EventHandler (opclRsrcButton_Click);
			buttonComm.Clicked += new EventHandler (opclCommButton_Click);
			entryTiming.KeyPressEvent += new KeyPressEventHandler (timingValTextBox_KeyPress);
			buttonDumpData.Clicked += new EventHandler (dumpEntryButton_Click);
			buttonDataLog.Clicked += new EventHandler (boolDataLogButton_Click);
			checkbuttonCustomize.Clicked += new EventHandler (checkbuttonCustomize_Click);

			changeDispTx(false);
			changeDispRx(false);

			ContinueCnt = 1;
			LastSlvCnt = 0;

		}

		public void commonClosingRoutine()
		{
			if (myComponents.CommActiveFlg == true)
			{
				if (myComponents.CommunicationMode == Components.CommMode.Serial)
				{
					serialPort_SelectState(false);

				}
				else if (myComponents.CommunicationMode == Components.CommMode.NetWork)
				{
					sockets_SelectState(false);

				}
			
			}
			
		}

		private void pageValComboBox_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (sender != null)
			{
				ComboBox combo = sender as ComboBox;

				int i = combo.Active;
				if( i >= 0 )
				{
					refreshTreeView( i );

					if (myComponents.CommActiveFlg == true)
					{
						bool errFlg = checkTreeViewCells();

						if ( errFlg == false )
						{
							renewLogSetting();

						}
						else
						{
							myCommProtocol.setLogModeStop();

						}

					}

				}

			}

		}

		[GLib.ConnectBefore]
		private void timingValTextBox_KeyPress(object sender, KeyPressEventArgs args)
		{
			if (args.Event.Key == Gdk.Key.Return) {
				string timingNum = entryTiming.Text;
				renewTimingSetting(timingNum);

			}

		}

		private void boolDataLogButton_Click(object sender, EventArgs args)
		{
			string DATALOG_LOGGING_TEXT = "Logging";
			string DATALOG_START_TEXT = "Start Log";
			
			if (myComponents.LoggingActiveFlg == true)
			{
				myComponents.LoggingActiveFlg = false;
				//buttonDataLog.Image = Properties.Resources.Complete_and_ok_gray;
				buttonDataLog.Label = DATALOG_START_TEXT;

				var text = makeLogData(RecordMode.ClipBoard);

				if (text.Length != 0)
				{
					Clipboard clipboard = Clipboard.Get(Gdk.Atom.Intern("CLIPBOARD", false));

					clipboard.Text = text.ToString();

					PutWarningMessage("Logging data has been copied to clipboard.");

				}

			}
			else
			{
				myComponents.LoggingActiveFlg = true;
				//buttonDataLog.Image = Properties.Resources.Complete_and_ok_green;
				buttonDataLog.Label = DATALOG_LOGGING_TEXT;

				LogStartTime = DateTime.MinValue;

			}

		}

		private void dumpEntryButton_Click(object sender, EventArgs e)
		{
			if (DumpFormInstance != null)
			{
				DumpFormInstance.Destroy();
			}

			DumpFormInstance = new DumpWindow(this);
			DumpFormInstance.Show();

		}

		private void opclRsrcButton_Click(object sender, EventArgs args)
		{
			string RSRC_OPEN = "Rsrc Open";
			string RSRC_CLOSE = "Rsrc Close";

			if (myComponents.RsrcActiveFlg == false)
			{
				bool retFlg = false;

				if (myComponents.CommunicationMode == Components.CommMode.Serial)
				{
					retFlg = serialPort_SelectState(true);

				}
				else if (myComponents.CommunicationMode == Components.CommMode.NetWork)
				{
					retFlg = sockets_SelectState(true);

				}

				if ( retFlg == true ) {
					myComponents.RsrcActiveFlg = true;
					buttonRsrc.Label = RSRC_CLOSE;

				}

			}
			else
			{
				if (myComponents.CommActiveFlg == true) {
					buttonComm.Click ();

				}

				bool retFlg = false;

				if (myComponents.CommunicationMode == Components.CommMode.Serial)
				{
					retFlg = serialPort_SelectState(false);

				}
				else if (myComponents.CommunicationMode == Components.CommMode.NetWork)
				{
					retFlg = sockets_SelectState(false);

				}

				if (retFlg == true) {
					myComponents.RsrcActiveFlg = false;
					buttonRsrc.Label = RSRC_OPEN;

				}

			}

		}
			
		private void opclCommButton_Click(object sender, EventArgs args)
		{
			string COMM_OPEN = "Comm Open";
			string COMM_CLOSE = "Comm Close";
			
			if( buttonComm.Label == COMM_OPEN )
			{
				if (checkbuttonCustomize.Active == true) {
					PutWarningMessage("Under Customize mode.");
					return;
				}

				if (System.IO.File.Exists(myComponents.ValidMapPath) == true)
				{
					DateTime now = System.IO.File.GetLastWriteTime(myComponents.ValidMapPath);

					if (now > myComponents.ValidMapLastWrittenDate)
					{
						bool result = MessageBox.ShowQuestion ("Map file was updated.\nDo you want to reload Address in Data Grid View?");

						if (result == true)
						{
							if (loadMapFile(myComponents.ValidMapPath) == false)
							{
								MessageBox.ShowWarning ("Can't read map file");

							}

						}

					}

				}

				if (myComponents.RsrcActiveFlg == false) {
					buttonRsrc.Click ();

				}

				bool errFlg = checkTreeViewCells();

				if ( (myComponents.RsrcActiveFlg == true) &&
					(errFlg == false) )
				{
					myComponents.CommActiveFlg = true;
					
					//opclCommButton.Image = Properties.Resources.FlagThread_red;
					buttonComm.Label = COMM_CLOSE;

					//Revise Timing
					entryTiming.Text = "500";

					textviewCommLog.Buffer.Text = "";

					myCommProtocol.startStopWatch();

					myCommProtocol.initial();

					readDUTVersion();

					renewLogSetting();

				}

			}
			else
			{
				myComponents.CommActiveFlg = false;
				
				buttonComm.Label = COMM_OPEN;

				myCommProtocol.setLogModeStop ();

				myCommProtocol.stopStopWatch();

				if (myComponents.LoggingActiveFlg == true)
				{
					buttonDataLog.Click ();

				}

			}

		}

		private bool mainTimer_Tick()
		{
			// Warning information
			if( entryTmpMessage.Text != "" )
			{
				WarningShowUpCount++;
				if( WarningShowUpCount > 20 )
				{
					entryTmpMessage.Text = "";
				}

			}
			else
			{
				WarningShowUpCount = 0;

			}

			while (myCommProtocol.myComponents.CommLog.Count != 0)
			{
				string data = myCommProtocol.myComponents.CommLog.Dequeue();

				if (data != null)
				{
					if (textviewCommLog.Buffer.LineCount > COMM_LOG_MAX)
					{
						string text = textviewCommLog.Buffer.Text;
						int offset = text.IndexOf(Environment.NewLine) + Environment.NewLine.Length;
						TextIter siter = textviewCommLog.Buffer.StartIter;
						TextIter niter = textviewCommLog.Buffer.GetIterAtOffset(offset);
						textviewCommLog.Buffer.Delete (ref siter, ref niter);

					}

					TextIter eiter = textviewCommLog.Buffer.EndIter;
					data = data + Environment.NewLine;
					textviewCommLog.Buffer.Insert(ref eiter, data);
					textviewCommLog.ScrollToIter(textviewCommLog.Buffer.EndIter,0,false,0,0);

				}

			}

			if ((commResource_CheckState() == false) ||
				(myComponents.CommActiveFlg == false) )
			{
				changeDispTx(false);
				changeDispRx(false);

				return true;

			}

			bool rcvFlg = myCommProtocol.mainControl();

			if (rcvFlg == true)
			{
				changeDispRx(true);

			}
			else
			{
				changeDispRx(false);

			}

			var isLogMode = myCommProtocol.isLogMode();

			if (isLogMode == false)
			{
				ContinueCnt = 1;
				LastSlvCnt = 0;
				RcvLogData = new List<List<string>>();

			}
			else
			{
				if (ContinueCnt > 10)
				{
					ContinueCnt = 1;

					List<byte> peekTxBuff = myCommProtocol.getTxData();

					if (peekTxBuff == null)
					{
						myCommProtocol.setLogModeStart();

					}

				}
				else
				{
					ContinueCnt++;

				}

				while (myCommProtocol.myComponents.ReceiveStream.Count != 0)
				{
					CommProtocol.RxDataParam rxStream = myCommProtocol.myComponents.ReceiveStream.Dequeue();

					List<string> lostLogBuff = new List<string>();

					int slvCnt = (int)(rxStream.Data[0] & 0x0F);

					if (LastSlvCnt == 0)
					{
						LastSlvCnt = slvCnt + 1;

					}
					else if (slvCnt == LastSlvCnt)
					{
						LastSlvCnt = slvCnt + 1;

						if (LastSlvCnt >= 16)
						{
							LastSlvCnt = 1;
						}
					}
					else
					{
						int tmp = slvCnt - LastSlvCnt;

						if (tmp < 0)
						{
							tmp = 15 + tmp;

						}

						LastSlvCnt = slvCnt + 1;

						lostLogBuff.Add(tmp.ToString() + " messages might be lost");
						lostLogBuff.Add("-");       //for OS Timer
						lostLogBuff.Add("-");       //for Count

					}

					rxStream.Data.RemoveAt(0);

					List<string> listSize = new List<string> ();
					foreach (var item in checkedRowList) {
						if (item.Check) {
							listSize.Add (item.Size);
						}
					}

					bool validflg;
					List<string> rxData = myCommProtocol.interpretRxFrameToHexChars(rxStream.Data, listSize, out validflg);
					List<string> logBuff = new List<string>();

					if (validflg == true)
					{
						logBuff.Add("OK");
						logBuff.Add(rxStream.Time);
						logBuff.Add(slvCnt.ToString());

						TreeIter iter;
						if (storeMain.GetIterFirst (out iter))
						{
							int index = 0;
							int rxIndex = 0;

							do
							{
								DataSetting factor = (DataSetting) storeMain.GetValue (iter, 0);

								if(checkedRowList[index].Check)
								{
									string retText = rxData[rxIndex];
									rxIndex++;

									string typeText = factor.Type;
									string retValue = TypeConvert.FromHexChars(typeText, int.Parse(checkedRowList[index].Size), retText);

									if( iter.Equals(userIter) == false )
									{
										factor.ReadText = retText;
										factor.ReadValue = retValue;
										storeMain.SetValue (iter, 0, factor);		// check!! if you want to show up values quickly.
									}


									logBuff.Add(retValue);

								}

								index++;

							} while(storeMain.IterNext (ref iter));

						}

					}
					else
					{
						logBuff.Add("Invalid DataLength");
						logBuff.Add(rxStream.Time);
						logBuff.Add(slvCnt.ToString());

					}

					if (myComponents.LoggingActiveFlg == true)
					{
						if( LogStartTime == DateTime.MinValue )
						{
							LogStartTime = DateTime.Now;

						}

						if (lostLogBuff.Count != 0)
						{
							RcvLogData.Add(lostLogBuff);

						}

						RcvLogData.Add(logBuff);

						if (RcvLogData.Count > RCV_LOGDATA_MAX)
						{
							RcvLogData.RemoveAt(0);

						}

					}
					else
					{
						RcvLogData = new List<List<string>>();

					}

				}

			}

			List<byte> txBuff = myCommProtocol.getTxData();

			if (txBuff != null)
			{
				changeDispTx(true);

				bool retFlg = false;

				if (myComponents.CommunicationMode == Components.CommMode.Serial)
				{
					retFlg = serialPort_DataSend(txBuff);

				}
				else if (myComponents.CommunicationMode == Components.CommMode.NetWork)
				{
					retFlg = sockets_DataSend(txBuff);

				}

				myCommProtocol.setTxCondtion(retFlg);

			}
			else
			{
				changeDispTx(false);

			}

			entryDUTVer.Text = myCommProtocol.myComponents.DutVersion;

			if ( (DumpFormInstance != null) &&
				(string.IsNullOrEmpty (myCommProtocol.myComponents.DumpData) != true) )
			{
				DumpFormInstance.PutDumpData(myCommProtocol.myComponents.DumpData);
				myCommProtocol.myComponents.DumpData = "";
			}

			return true;

		}


		private void changeDispTx(bool state)
		{
			if( state == true )
			{
				eventboxTx.ModifyBg(Gtk.StateType.Normal,new Gdk.Color(255,165,0));

			}
			else
			{
				eventboxTx.ModifyBg(Gtk.StateType.Normal,new Gdk.Color(255,255,255));

			}

		}

		private void changeDispRx(bool state)
		{
			if( state == true )
			{
				eventboxRx.ModifyBg(Gtk.StateType.Normal,new Gdk.Color(255,0,0));

			}
			else
			{
				eventboxRx.ModifyBg(Gtk.StateType.Normal,new Gdk.Color(255,255,255));

			}

		}

		private void FixedToggledForCheck (object sender, ToggledArgs args)
		{
			TreeIter iter;
			if (storeMain.GetIterFromString (out iter, args.Path)) {
				DataSetting factor = (DataSetting) storeMain.GetValue (iter, 0);

				if( factor.Check == false )
				{
					if ( factor.ErrNum == ERR_NUM_NOTHING )
					{
						factor.Check = true;

						if (myComponents.CommActiveFlg == true)
						{
							bool errFlg = checkTreeViewCells();

							if (errFlg == false) {
								renewLogSetting ();

							} else {
								myCommProtocol.setLogModeStop();

							}

						}

					}
					else
					{
						PutWarningMessage( "Size, Address or Offset might be invalid." );
						
					}

				}
				else
				{
					factor.Check = false;

					if (myComponents.CommActiveFlg == true)
					{
						bool errFlg = checkTreeViewCells();

						if (errFlg == false) {
							renewLogSetting ();

						} else {
							myCommProtocol.setLogModeStop();

						}

					}

				}

				checkTreeViewCells ();

			}

		}

		private void FixedToggledForAddrLock (object sender, ToggledArgs args)
		{
			TreeIter iter;
			if (storeMain.GetIterFromString (out iter, args.Path)) {
				DataSetting factor = (DataSetting) storeMain.GetValue (iter, 0);
				if ((factor.Check == false) ||
					(checkbuttonCustomize.Active == true)) {
					factor.AddrLock = !factor.AddrLock;
				}

			}

		}

		private void TextEditingStarted(object sender, EditingStartedArgs args)
		{
			storeMain.GetIterFromString (out userIter, args.Path);
		}

		private void TextEditingCanceled(object sender, EventArgs args)
		{
			// Press esc key when focus on a cell. This event will occur.
			userIter = new TreeIter ();
		}


		private void TextEditedForGroup(object sender, EditedArgs args)
		{
			if (storeMain.GetIterFromString (out userIter, args.Path)) {
				DataSetting factor = (DataSetting) storeMain.GetValue (userIter, 0);

				if ( (string.IsNullOrEmpty (args.NewText) != true) &&
					(string.IsNullOrEmpty (factor.Group) != true) ) {
					string oldName = factor.Group;

					var index = comboboxPage.Active;
					factor.Group = args.NewText;
					comboboxPage.RemoveText(index);
					comboboxPage.InsertText(index, args.NewText);
					comboboxPage.Active = index;

				}

			}

			userIter = new TreeIter ();

		}

		private void TextEditedForSize(object sender, EditedArgs args)
		{
			if (storeMain.GetIterFromString (out userIter, args.Path)) {
				DataSetting factor = (DataSetting) storeMain.GetValue (userIter, 0);
				if ((factor.Check == false) ||
				    (checkbuttonCustomize.Active == true)) {
					factor.Size = args.NewText;
				}

			}

			userIter = new TreeIter ();

		}

		private void TextEditedForVariable(object sender, EditedArgs args)
		{
			if (storeMain.GetIterFromString (out userIter, args.Path)) {
				DataSetting factor = (DataSetting) storeMain.GetValue (userIter, 0);
				if ((factor.Check == false) ||
					(checkbuttonCustomize.Active == true)) {
					string variableText = args.NewText;

					factor.Variable = variableText;

					if ((myComponents.MapList != null) &&
						(myComponents.MapList.Count > 0) &&
						(string.IsNullOrEmpty (variableText) != true) ) {
						MapFactor result = myComponents.MapList.Find (key => key.VariableName == variableText);

						if (result != null) {
							if ((int.Parse (result.Size) >= 1) &&
								(int.Parse (result.Size) <= 4)) {
								factor.Size = result.Size;

							}

							factor.Address = result.Address;

						} else {
							factor.Check = false;
							factor.Address = null;

						}

					}

				}

			}

			userIter = new TreeIter ();

		}

		private void TextEditedForAddress(object sender, EditedArgs args)
		{
			if (storeMain.GetIterFromString (out userIter, args.Path)) {
				DataSetting factor = (DataSetting) storeMain.GetValue (userIter, 0);
				if ((factor.Check == false) ||
					(checkbuttonCustomize.Active == true)) {
					factor.Address = args.NewText;
				}

			}

			userIter = new TreeIter ();

		}

		private void TextEditedForOffset(object sender, EditedArgs args)
		{
			if (storeMain.GetIterFromString (out userIter, args.Path)) {
				DataSetting factor = (DataSetting) storeMain.GetValue (userIter, 0);
				if ((factor.Check == false) ||
					(checkbuttonCustomize.Active == true)) {
					factor.Offset = args.NewText;
				}

			}

			userIter = new TreeIter ();

		}

		private void TextEditedForName(object sender, EditedArgs args)
		{
			if (storeMain.GetIterFromString (out userIter, args.Path)) {
				DataSetting factor = (DataSetting) storeMain.GetValue (userIter, 0);
				if ((factor.Check == false) ||
					(checkbuttonCustomize.Active == true)) {
					factor.Name = args.NewText;
				}

			}

			userIter = new TreeIter ();

		}

		private void TextEditedForType(object sender, EditedArgs args)
		{
			if (storeMain.GetIterFromString (out userIter, args.Path)) {
				DataSetting factor = (DataSetting) storeMain.GetValue (userIter, 0);

				string typeText = args.NewText;
				string sizeText = factor.Size;

				if (string.IsNullOrEmpty (sizeText) == true) {
					PutWarningMessage( "Size is not defined." );

				} else if ( (typeText == numeralSystem.FLT) &&
					(sizeText != "4") ){
					PutWarningMessage( "Invalid Float conversion!" );

				} else {
					factor.Type = args.NewText;

					string readText = factor.ReadText;

					if (string.IsNullOrEmpty (readText) != true) {
						string rdValueText = TypeConvert.FromHexChars(typeText, int.Parse(sizeText), readText);
						factor.ReadValue = rdValueText;

					}

					string writeText = factor.WriteText;

					if (string.IsNullOrEmpty (writeText) != true) {
						string wrValueText = TypeConvert.FromHexChars(typeText, int.Parse(sizeText), writeText);
						factor.WriteValue = wrValueText;

					}

				}

			}

			userIter = new TreeIter ();

		}

		private void TextEditedForWriteValue(object sender, EditedArgs args)
		{
			if (storeMain.GetIterFromString (out userIter, args.Path)) {
				DataSetting factor = (DataSetting) storeMain.GetValue (userIter, 0);

				if ( factor.ErrNum != ERR_NUM_NOTHING ) {
					PutWarningMessage( "Size, Address or Offset might be invalid." );
					return;

				}

				string sizeText = factor.Size;
				string typeText = factor.Type;
				string wrValueText = args.NewText;

				Exception ex_text = null;

				string writeText = TypeConvert.ToHexChars (typeText, int.Parse (sizeText), wrValueText, out ex_text);

				if (ex_text != null) {
					PutWarningMessage( ex_text.Message );

				} else if (writeText == null) {
					PutWarningMessage( "Write data is invalid." );

				} else {
					factor.WriteValue = wrValueText;
					factor.WriteText = writeText;

				}

			}

			userIter = new TreeIter ();

		}

		private void FixedToggledForWriteTrg (object sender, ToggledArgs args)
		{
			TreeIter iter;
			if (storeMain.GetIterFromString (out iter, args.Path)) {
				DataSetting factor = (DataSetting) storeMain.GetValue (iter, 0);

				if( ( factor.ErrNum == ERR_NUM_NOTHING ) &&
					(string.IsNullOrEmpty (factor.WriteText) != true) ) {
					writeData (factor.Size, factor.Address, factor.Offset, factor.WriteText);

				}

			}

		}

		private void ConstructAppearance (TreeView treeView)
		{
			CellRendererText rendererText;
			CellRendererToggle rendererToggle;
			CellRendererCombo rendererCombo;
			TreeViewColumn column;

			// column for RowCount
			rendererText = new CellRendererText ();
			column = new TreeViewColumn ("", rendererText);
			column.Sizing = TreeViewColumnSizing.Fixed;
			column.FixedWidth = 25;
			column.SetCellDataFunc (rendererText, new Gtk.TreeCellDataFunc (RenderRowCount));
			treeView.AppendColumn (column);

			// column for Group
			rendererText = new CellRendererText ();
			rendererText.Editable = true;
			rendererText.EditingStarted += TextEditingStarted;
			rendererText.Edited += TextEditedForGroup;
			rendererText.EditingCanceled += TextEditingCanceled;
			column = new TreeViewColumn ("Group", rendererText);
			column.Sizing = TreeViewColumnSizing.Fixed;
			column.FixedWidth = 100;
			column.SetCellDataFunc (rendererText, new Gtk.TreeCellDataFunc (RenderGroup));
			column.Visible = false;
			treeView.AppendColumn (column);

			// column for Check(fixed toggles)
			rendererToggle = new CellRendererToggle ();
			rendererToggle.Toggled += new ToggledHandler (FixedToggledForCheck);
			column =  new TreeViewColumn ("Check", rendererToggle);
			column.Sizing = TreeViewColumnSizing.Fixed;
			column.FixedWidth = 60;
			column.SetCellDataFunc (rendererToggle, new Gtk.TreeCellDataFunc (RenderCheck));
			treeView.AppendColumn (column);

			// column for ErrNum
			rendererText = new CellRendererText ();
			column = new TreeViewColumn ("ErrNum", rendererText);
			column.Sizing = TreeViewColumnSizing.Fixed;
			column.FixedWidth = 100;
			column.SetCellDataFunc (rendererText, new Gtk.TreeCellDataFunc (RenderErrNum));
			column.Visible = false;
			treeView.AppendColumn (column);

			// column for Size
			rendererText = new CellRendererText ();
			rendererText.Editable = true;
			rendererText.EditingStarted += TextEditingStarted;
			rendererText.Edited += TextEditedForSize;
			rendererText.EditingCanceled += TextEditingCanceled;
			column = new TreeViewColumn ("Size", rendererText);
			column.Sizing = TreeViewColumnSizing.Fixed;
			column.FixedWidth = 60;
			column.SetCellDataFunc (rendererText, new Gtk.TreeCellDataFunc (RenderSize));
			treeView.AppendColumn (column);

			// column for Variable
			rendererText = new CellRendererText ();
			rendererText.Editable = true;
			rendererText.EditingStarted += TextEditingStarted;
			rendererText.Edited += TextEditedForVariable;
			rendererText.EditingCanceled += TextEditingCanceled;
			column = new TreeViewColumn ("Variable", rendererText);
			column.MinWidth = 100;
			column.Resizable = true;
			column.SetCellDataFunc (rendererText, new Gtk.TreeCellDataFunc (RenderVariable));
			treeView.AppendColumn (column);

			// column for Addrlock(fixed toggles)
			rendererToggle = new CellRendererToggle ();
			rendererToggle.Toggled += new ToggledHandler (FixedToggledForAddrLock);
			column =  new TreeViewColumn ("Addrlock", rendererToggle);
			column.Sizing = TreeViewColumnSizing.Fixed;
			column.FixedWidth = 80;
			column.SetCellDataFunc (rendererToggle, new Gtk.TreeCellDataFunc (RenderAddrLock));
			treeView.AppendColumn (column);

			// column for Address
			rendererText = new CellRendererText ();
			rendererText.Editable = true;
			rendererText.EditingStarted += TextEditingStarted;
			rendererText.Edited += TextEditedForAddress;
			rendererText.EditingCanceled += TextEditingCanceled;
			column = new TreeViewColumn ("Address", rendererText);
			column.Sizing = TreeViewColumnSizing.Fixed;
			column.FixedWidth = 100;
			column.SetCellDataFunc (rendererText, new Gtk.TreeCellDataFunc (RenderAddress));
			treeView.AppendColumn (column);

			// column for Offset
			rendererText = new CellRendererText ();
			rendererText.Editable = true;
			rendererText.EditingStarted += TextEditingStarted;
			rendererText.Edited += TextEditedForOffset;
			rendererText.EditingCanceled += TextEditingCanceled;
			column = new TreeViewColumn ("Offset", rendererText);
			column.Sizing = TreeViewColumnSizing.Fixed;
			column.FixedWidth = 60;
			column.SetCellDataFunc (rendererText, new Gtk.TreeCellDataFunc (RenderOffset));
			treeView.AppendColumn (column);

			// column for Name
			rendererText = new CellRendererText ();
			rendererText.Editable = true;
			rendererText.EditingStarted += TextEditingStarted;
			rendererText.Edited += TextEditedForName;
			rendererText.EditingCanceled += TextEditingCanceled;
			column = new TreeViewColumn ("Name", rendererText);
			column.Sizing = TreeViewColumnSizing.Autosize;
			column.MinWidth = 100;
			column.Resizable = true;
			column.SetCellDataFunc (rendererText, new Gtk.TreeCellDataFunc (RenderName));
			treeView.AppendColumn (column);

			// column for Type
			// Values to be chosen in the ComboBox
			ListStore comboModel = new ListStore(typeof(string));
			comboModel.AppendValues(numeralSystem.HEX);
			comboModel.AppendValues(numeralSystem.UDEC);
			comboModel.AppendValues(numeralSystem.DEC);
			comboModel.AppendValues(numeralSystem.BIN);
			comboModel.AppendValues(numeralSystem.FLT);

			rendererCombo = new CellRendererCombo ();
			rendererCombo.Editable = true;
			rendererCombo.EditingStarted += TextEditingStarted;
			rendererCombo.Edited += TextEditedForType;
			rendererCombo.EditingCanceled += TextEditingCanceled;
			rendererCombo.TextColumn = 0;		//important!!
			rendererCombo.Model = comboModel;
			column = new TreeViewColumn ("Type", rendererCombo);
			column.Sizing = TreeViewColumnSizing.Fixed;
			column.FixedWidth = 60;
			column.PackStart (rendererCombo, true);
			column.SetCellDataFunc (rendererCombo, new Gtk.TreeCellDataFunc (RenderType));
			treeView.AppendColumn (column);

			// column for ReadText
			rendererText = new CellRendererText ();
			column = new TreeViewColumn ("ReadText", rendererText);
			column.Sizing = TreeViewColumnSizing.Fixed;
			column.FixedWidth = 100;
			column.SetCellDataFunc (rendererText, new Gtk.TreeCellDataFunc (RenderReadText));
			column.Visible = false;
			treeView.AppendColumn (column);

			// column for ReadValue
			rendererText = new CellRendererText ();
			column = new TreeViewColumn ("ReadValue", rendererText);
			column.Sizing = TreeViewColumnSizing.Fixed;
			column.FixedWidth = 100;
			column.SetCellDataFunc (rendererText, new Gtk.TreeCellDataFunc (RenderReadValue));
			treeView.AppendColumn (column);

			// column for WriteText
			rendererText = new CellRendererText ();
			column = new TreeViewColumn ("WriteText", rendererText);
			column.Sizing = TreeViewColumnSizing.Fixed;
			column.FixedWidth = 100;
			column.SetCellDataFunc (rendererText, new Gtk.TreeCellDataFunc (RenderWriteText));
			column.Visible = false;
			treeView.AppendColumn (column);

			// column for WriteValue
			rendererText = new CellRendererText ();
			rendererText.Editable = true;
			rendererText.EditingStarted += TextEditingStarted;
			rendererText.Edited += TextEditedForWriteValue;
			rendererText.EditingCanceled += TextEditingCanceled;
			column = new TreeViewColumn ("WriteValue", rendererText);
			column.Sizing = TreeViewColumnSizing.Fixed;
			column.FixedWidth = 100;
			column.SetCellDataFunc (rendererText, new Gtk.TreeCellDataFunc (RenderWriteValue));
			treeView.AppendColumn (column);

			// column for Write Trg(fixed toggles)
			rendererToggle = new CellRendererToggle ();
			rendererToggle.Radio = true;
			rendererToggle.Xalign = 0.0f;
			rendererToggle.Toggled += new ToggledHandler (FixedToggledForWriteTrg);
			column =  new TreeViewColumn ("Write Trg", rendererToggle);
			column.Sizing = TreeViewColumnSizing.Fixed;
			column.FixedWidth = 60;
			column.SetCellDataFunc (rendererToggle, new Gtk.TreeCellDataFunc (RenderWriteTrg));
			treeView.AppendColumn (column);

		}

		private void RenderRowCount (Gtk.TreeViewColumn column, Gtk.CellRenderer cell, Gtk.TreeModel model, Gtk.TreeIter iter)
		{
			DataSetting item = (DataSetting) storeMain.GetValue (iter, 0);
			(cell as Gtk.CellRendererText).Text = item.RowCount.ToString();
		}

		private void RenderGroup (Gtk.TreeViewColumn column, Gtk.CellRenderer cell, Gtk.TreeModel model, Gtk.TreeIter iter)
		{
			DataSetting item = (DataSetting) storeMain.GetValue (iter, 0);
			(cell as Gtk.CellRendererText).Text = item.Group;
		}

		private void RenderCheck (Gtk.TreeViewColumn column, Gtk.CellRenderer cell, Gtk.TreeModel model, Gtk.TreeIter iter)
		{
			DataSetting item = (DataSetting) storeMain.GetValue (iter, 0);
			(cell as Gtk.CellRendererToggle).Active = item.Check;
		}

		private void RenderErrNum (Gtk.TreeViewColumn column, Gtk.CellRenderer cell, Gtk.TreeModel model, Gtk.TreeIter iter)
		{
			DataSetting item = (DataSetting) storeMain.GetValue (iter, 0);
			(cell as Gtk.CellRendererText).Text = item.ErrNum.ToString();
		}

		private void RenderSize (Gtk.TreeViewColumn column, Gtk.CellRenderer cell, Gtk.TreeModel model, Gtk.TreeIter iter)
		{
			DataSetting item = (DataSetting) storeMain.GetValue (iter, 0);
			(cell as Gtk.CellRendererText).Text = item.Size;

			bool validFlg = false;

			if (string.IsNullOrEmpty(item.Size) == false)
			{
				if (TypeConvert.IsNumeric(item.Size) == true)
				{
					int num = int.Parse(item.Size);

					if ((num == 1) || (num == 2) || (num == 4))
					{
						validFlg = true;

					}

				}

			}

			int errNum = item.ErrNum;

			if (validFlg == true) {
				(cell as Gtk.CellRendererText).Foreground = "black";
				errNum &= ~(ERR_NUM_SIZE);
			} else {
				(cell as Gtk.CellRendererText).Foreground = "red";
				errNum |= ERR_NUM_SIZE;
			}

			item.ErrNum = errNum;

		}

		private void RenderVariable (Gtk.TreeViewColumn column, Gtk.CellRenderer cell, Gtk.TreeModel model, Gtk.TreeIter iter)
		{
			DataSetting item = (DataSetting) storeMain.GetValue (iter, 0);
			(cell as Gtk.CellRendererText).Text = item.Variable;
		}

		private void RenderAddrLock (Gtk.TreeViewColumn column, Gtk.CellRenderer cell, Gtk.TreeModel model, Gtk.TreeIter iter)
		{
			DataSetting item = (DataSetting) storeMain.GetValue (iter, 0);
			(cell as Gtk.CellRendererToggle).Active = item.AddrLock;
		}

		private void RenderAddress (Gtk.TreeViewColumn column, Gtk.CellRenderer cell, Gtk.TreeModel model, Gtk.TreeIter iter)
		{
			DataSetting item = (DataSetting) storeMain.GetValue (iter, 0);
			(cell as Gtk.CellRendererText).Text = item.Address;

			bool validFlg = false;

			if (string.IsNullOrEmpty(item.Address) == false)
			{
				if (TypeConvert.IsHexString(item.Address) == true)
				{
					validFlg = true;

				}

			}

			int errNum = item.ErrNum;

			if (validFlg == true) {
				(cell as Gtk.CellRendererText).Foreground = "black";
				errNum &= ~(ERR_NUM_ADDRESS);
			} else {
				(cell as Gtk.CellRendererText).Foreground = "red";
				errNum |= ERR_NUM_ADDRESS;
			}

			item.ErrNum = errNum;

		}

		private void RenderOffset (Gtk.TreeViewColumn column, Gtk.CellRenderer cell, Gtk.TreeModel model, Gtk.TreeIter iter)
		{
			DataSetting item = (DataSetting) storeMain.GetValue (iter, 0);
			(cell as Gtk.CellRendererText).Text = item.Offset;

			bool validFlg = false;

			if (string.IsNullOrEmpty(item.Offset) == false)
			{
				if (TypeConvert.IsNumeric(item.Offset) == true)
				{
					validFlg = true;

				}

			}

			int errNum = item.ErrNum;

			if (validFlg == true) {
				(cell as Gtk.CellRendererText).Foreground = "black";
				errNum &= ~(ERR_NUM_OFFSET);
			} else {
				(cell as Gtk.CellRendererText).Foreground = "red";
				errNum |= ERR_NUM_OFFSET;
			}

			item.ErrNum = errNum;

		}

		private void RenderName (Gtk.TreeViewColumn column, Gtk.CellRenderer cell, Gtk.TreeModel model, Gtk.TreeIter iter)
		{
			DataSetting item = (DataSetting) storeMain.GetValue (iter, 0);
			(cell as Gtk.CellRendererText).Text = item.Name;
		}

		private void RenderType (Gtk.TreeViewColumn column, Gtk.CellRenderer cell, Gtk.TreeModel model, Gtk.TreeIter iter)
		{
			DataSetting item = (DataSetting) storeMain.GetValue (iter, 0);
			(cell as Gtk.CellRendererCombo).Text = item.Type;
		}

		private void RenderReadText (Gtk.TreeViewColumn column, Gtk.CellRenderer cell, Gtk.TreeModel model, Gtk.TreeIter iter)
		{
			DataSetting item = (DataSetting) storeMain.GetValue (iter, 0);
			(cell as Gtk.CellRendererText).Text = item.ReadText;
		}

		private void RenderReadValue (Gtk.TreeViewColumn column, Gtk.CellRenderer cell, Gtk.TreeModel model, Gtk.TreeIter iter)
		{
			DataSetting item = (DataSetting) storeMain.GetValue (iter, 0);
			(cell as Gtk.CellRendererText).Text = item.ReadValue;
		}

		private void RenderWriteText (Gtk.TreeViewColumn column, Gtk.CellRenderer cell, Gtk.TreeModel model, Gtk.TreeIter iter)
		{
			DataSetting item = (DataSetting) storeMain.GetValue (iter, 0);
			(cell as Gtk.CellRendererText).Text = item.WriteText;
		}

		private void RenderWriteValue (Gtk.TreeViewColumn column, Gtk.CellRenderer cell, Gtk.TreeModel model, Gtk.TreeIter iter)
		{
			DataSetting item = (DataSetting) storeMain.GetValue (iter, 0);
			(cell as Gtk.CellRendererText).Text = item.WriteValue;
		}

		private void RenderWriteTrg (Gtk.TreeViewColumn column, Gtk.CellRenderer cell, Gtk.TreeModel model, Gtk.TreeIter iter)
		{
			DataSetting item = (DataSetting) storeMain.GetValue (iter, 0);
			(cell as Gtk.CellRendererToggle).Active = item.Assert;
		}

		private void PutWarningMessage( string text )
		{
			entryTmpMessage.Text = text;
			WarningShowUpCount = 0;
			
		}

		private void checkbuttonCustomize_Click(object sender, EventArgs args)
		{
			if (checkbuttonCustomize.Active == true) {
				if (myComponents.CommActiveFlg == true) {
					PutWarningMessage("Under Communication.");
					checkbuttonCustomize.Active = false;

				} else {
					treeviewMain.ButtonPressEvent += new ButtonPressEventHandler(OnItemButtonPressed);
					var columns = treeviewMain.Columns;
					columns [1].Visible = true;		// Column: Group
				}

			} else {
				treeviewMain.ButtonPressEvent -= new ButtonPressEventHandler(OnItemButtonPressed);
				var columns = treeviewMain.Columns;
				columns [1].Visible = false;		// Column: Group

				TreeIter iter;
				if (storeMain.GetIterFirst (out iter))
				{
					int i = 1;

					do
					{
						DataSetting factor = (DataSetting) storeMain.GetValue (iter, 0);

						factor.RowCount = i;
						i++;

					} while(storeMain.IterNext (ref iter));

				}

			}

		}

		[GLib.ConnectBeforeAttribute]
		protected void OnItemButtonPressed (object sender, ButtonPressEventArgs e)
		{
			if (e.Event.Button == 3) /* right click */
			{
				Menu contextMenu = new Menu();

				MenuItem deleteItem = new MenuItem("Delete this item");
				deleteItem.ButtonPressEvent += new ButtonPressEventHandler(OnDeleteItemButtonPressed);
				MenuItem insertItem = new MenuItem("Insert an item to next row");
				insertItem.ButtonPressEvent += new ButtonPressEventHandler(OnInsertItemButtonPressed);
				MenuItem copyItem = new MenuItem("Copy this item to next row");
				copyItem.ButtonPressEvent += new ButtonPressEventHandler(OnCopyItemButtonPressed);

				MenuItem deletePage = new MenuItem("Delete this page");
				deletePage.ButtonPressEvent += new ButtonPressEventHandler(OnDeletePageButtonPressed);
				MenuItem insertPage = new MenuItem("Insert a page to next");
				insertPage.ButtonPressEvent += new ButtonPressEventHandler(OnInsertPageButtonPressed);
				MenuItem copyPage = new MenuItem("Copy this page to next");
				copyPage.ButtonPressEvent += new ButtonPressEventHandler(OnCopyPageButtonPressed);

				contextMenu.Add(deleteItem);
				contextMenu.Add(insertItem);
				contextMenu.Add(copyItem);

				contextMenu.Add(deletePage);
				contextMenu.Add(insertPage);
				contextMenu.Add(copyPage);

				contextMenu.ShowAll();
				contextMenu.Popup();
			}
		}

		protected void OnDeleteItemButtonPressed (object sender, ButtonPressEventArgs e)
		{
			TreeIter iter;
			if (treeviewMain.Selection.GetSelected (out iter)) {
				var path = storeMain.GetPath (iter);
				var indices = path.Indices;

				if (indices.Length == 1) {
					if (indices [0] == 0) {
						PutWarningMessage( "Can't delete first row." );

					} else {
						storeMain.Remove (ref iter);

					}

				}
			}

		}

		protected void OnInsertItemButtonPressed (object sender, ButtonPressEventArgs e)
		{
			TreeIter iter;
			if (treeviewMain.Selection.GetSelected (out iter)) {
				var path = storeMain.GetPath (iter);
				var indices = path.Indices;

				if (indices.Length == 1) {
					DataSetting tmp = new DataSetting ();
					storeMain.InsertWithValues (indices[0]+1, tmp);

				}

			}

		}

		protected void OnCopyItemButtonPressed (object sender, ButtonPressEventArgs e)
		{
			TreeIter iter;
			if (treeviewMain.Selection.GetSelected (out iter)) {
				var path = storeMain.GetPath (iter);
				var indices = path.Indices;

				if (indices.Length == 1) {
					DataSetting tmp = new DataSetting ((DataSetting) storeMain.GetValue (iter, 0));
					tmp.Group = "";
					storeMain.InsertWithValues (indices[0]+1, tmp);

				}

			}

		}

		protected void OnDeletePageButtonPressed (object sender, ButtonPressEventArgs e)
		{
			var index = comboboxPage.Active;

			if (index != 0)
			{
				bool result = MessageBox.ShowQuestion ("Do you want to delete this page?");
				
				if (result == true)
				{
					myComponents.ViewSettingList.RemoveAt(index);
					comboboxPage.RemoveText (index);
					comboboxPage.Active = index - 1;

				}

			}
			else
			{
				PutWarningMessage( "Can't delete first page." );

			}

		}

		protected void OnInsertPageButtonPressed (object sender, ButtonPressEventArgs e)
		{
			bool result = MessageBox.ShowQuestion ("Do you want to insert a page next to this page?");
			
			if (result == true)
			{
				var index = comboboxPage.Active;
				string groupName = GROUP_TEMPORARY_TAG;

				var tmpVSettingFactor = new ViewSetting();

				for (int i = 0; i < DEFAULT_COLUMNS; i++)
				{
					tmpVSettingFactor.DataSetting.Add(new DataSetting());
				}

				tmpVSettingFactor.DataSetting[0].Group = groupName;

				myComponents.ViewSettingList.Insert((index + 1), tmpVSettingFactor);

				comboboxPage.InsertText((index + 1), groupName);

				comboboxPage.Active = index + 1;

			}

		}

		protected void OnCopyPageButtonPressed (object sender, ButtonPressEventArgs e)
		{
			bool result = MessageBox.ShowQuestion ("Do you want to copy this page to next page?");
			
			if (result == true)
			{
				var index = comboboxPage.Active;
				string groupName = GROUP_TEMPORARY_TAG;

				var tmpVSettingFactor = new ViewSetting();

				foreach (var row in myComponents.ViewSettingList[index].DataSetting)
				{
					DataSetting factor = new DataSetting(row);
					tmpVSettingFactor.DataSetting.Add(factor);

				}

				tmpVSettingFactor.DataSetting[0].Group = groupName;

				myComponents.ViewSettingList.Insert((index + 1), tmpVSettingFactor);

				comboboxPage.InsertText((index + 1), groupName);

				comboboxPage.Active = index + 1;

			}


		}

	}
}
