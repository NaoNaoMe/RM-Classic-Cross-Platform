using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Gtk;
using GLib;

namespace rmApplication
{
	public partial class DumpWindow : Gtk.Window
	{
		private enum FixedColumns : int
		{
			Size = 0,
			Type,
			DataStart
		}

		public const string ROW_TERMINATOR = "-";

		private SubViewControl SubViewCtrl;

		public DumpWindow (SubViewControl tmp) : base (Gtk.WindowType.Toplevel)
		{
			SubViewCtrl = tmp;
			this.Build ();

			int maxIndex = 4;
			Type[] t = new Type[maxIndex];
			for (int i = 0; i < maxIndex; i++) {
				t [i] = typeof(string);
			}

			ListStore tmpListStore = new ListStore (t);

			tmpListStore.AppendValues ("1", "Hex", "1", "2");
			tmpListStore.AppendValues ("1", "Hex", "3", "4");
			tmpListStore.AppendValues (ROW_TERMINATOR, "");

			treeviewMain.Model = tmpListStore;
			ConstructAppearance (treeviewMain);


			buttonRequest.Clicked += new EventHandler(requestButton_Click);
			buttonMake.Clicked += new EventHandler(makeButton_Click);
			buttonCopy.Clicked += new EventHandler(copyToClipBoardButton_Click);

			entryVariable.KeyPressEvent += new KeyPressEventHandler (variableTextBox_KeyDown);

			this.ShowAll ();
		}

		private void ClaerAppearance (TreeView treeView)
		{
			foreach (var item in treeView.Columns) {
				treeView.RemoveColumn (item);
			}
		}

		private void ConstructAppearance (TreeView treeView)
		{
			CellRendererText rendererText;
			CellRendererCombo rendererCombo;
			TreeViewColumn column;

			// column for Size
			rendererText = new CellRendererText ();
			rendererText.Editable = true;
			rendererText.Edited += TextEditedForSize;
			column = new TreeViewColumn ("Size", rendererText, "text", FixedColumns.Size);
			column.Sizing = TreeViewColumnSizing.Fixed;
			column.FixedWidth = 60;
			treeView.AppendColumn (column);

			// column for Type
			// Values to be chosen in the ComboBox
			ListStore comboModel = new ListStore(typeof(string));
			comboModel.AppendValues(numeralSystem.HEX);
			comboModel.AppendValues(numeralSystem.UDEC);
			comboModel.AppendValues(numeralSystem.DEC);
			comboModel.AppendValues(numeralSystem.BIN);
			comboModel.AppendValues(numeralSystem.FLT);
			comboModel.AppendValues(numeralSystem.ASCII);

			rendererCombo = new CellRendererCombo ();
			rendererCombo.Editable = true;
			rendererCombo.Edited += TextEditedForType;
			rendererCombo.TextColumn = 0;		//important!!
			rendererCombo.Model = comboModel;
			column = new TreeViewColumn ("Type", rendererCombo, "text", FixedColumns.Type);
			column.Sizing = TreeViewColumnSizing.Fixed;
			column.FixedWidth = 60;
			column.PackStart (rendererCombo, true);
			treeView.AppendColumn (column);

			// column for Size
			int baseIndex = treeviewMain.Model.NColumns;

			for (int index = 0; index < (baseIndex-(int)FixedColumns.DataStart); index++) {
				rendererText = new CellRendererText ();
				string dataText = "data" + index.ToString(); 
				column = new TreeViewColumn (dataText, rendererText, "text", (FixedColumns.DataStart+index));
				column.Sizing = TreeViewColumnSizing.Autosize;
				//column.FixedWidth = 100;
				treeView.AppendColumn (column);
			}

		}

		private void TextEditedForSize(object sender, EditedArgs args)
		{
			TreeIter iter;
			if (treeviewMain.Model.GetIterFromString (out iter, args.Path)) {
				string tmpSize = (string)treeviewMain.Model.GetValue (iter, (int)FixedColumns.Size);

				if (tmpSize == ROW_TERMINATOR) {
					if ( (args.NewText != ROW_TERMINATOR) &&
						(args.NewText != "0")) {
						treeviewMain.Model.SetValue (iter, (int)FixedColumns.Size, args.NewText);
						treeviewMain.Model.SetValue (iter, (int)FixedColumns.Type, "Hex");
						ListStore tmp = (ListStore)treeviewMain.Model;
						tmp.AppendValues (ROW_TERMINATOR, "");
						treeviewMain.Model = tmp;
					}

				} else if (args.NewText == "0") {
					ListStore tmp = (ListStore)treeviewMain.Model;
					tmp.Remove (ref iter);
					treeviewMain.Model = tmp;

				} else {
					treeviewMain.Model.SetValue (iter, (int)FixedColumns.Size, args.NewText);
				}
			}

		}

		private void TextEditedForType(object sender, EditedArgs args)
		{
			TreeIter iter;
			if (treeviewMain.Model.GetIterFromString (out iter, args.Path)) {
				treeviewMain.Model.SetValue (iter, (int)FixedColumns.Type, args.NewText);

			}

		}


		[ConnectBefore]
		private void variableTextBox_KeyDown(object sender, KeyPressEventArgs args)
		{
			// http://stackoverflow.com/questions/1698437/why-does-pressing-the-return-key-not-trigger-a-keypressevent-in-my-gtk-entry

			if (args.Event.Key == Gdk.Key.Return)
			{
				if (SubViewCtrl.myComponents.MapList != null)
				{
					string tmpVariable = entryVariable.Text;

					MapFactor result = SubViewCtrl.myComponents.MapList.Find(item => item.VariableName == tmpVariable);

					if (result != null)
					{
						entryAddress.Text = result.Address;
						entrySize.Text = result.Size;

					}

				}

			}
		}


		private void requestButton_Click(object sender, EventArgs args)
		{
			string address = entryAddress.Text;
			string size = entrySize.Text;

			bool flg = SubViewCtrl.commResource_CheckState();

			if (flg == false)
			{
				return;

			}

			textviewMain.Buffer.Text = "";

			SubViewCtrl.myCommProtocol.readDumpData(address, size);

			SubViewCtrl.myCommProtocol.setLogModeStart();
		}


		private void makeButton_Click(object sender, EventArgs args)
		{
			string originalText = textviewMain.Buffer.Text;
			var dividedText = originalText.Split('-').ToList();

			int factorSize = 0;

			List<int> listNumSize = new List<int> ();
			List<string> listTextType = new List<string> ();

			TreeIter iter;
			if (treeviewMain.Model.GetIterFirst (out iter)) {
				do
				{
					string tmpSizeText = (string) treeviewMain.Model.GetValue (iter, (int)FixedColumns.Size);
					if( tmpSizeText == ROW_TERMINATOR ) {
						break;
					} else {
						int tmpIntSize = int.Parse(tmpSizeText);
						factorSize += tmpIntSize;
						listNumSize.Add(tmpIntSize);
					}

					string tmpTypeText = (string) treeviewMain.Model.GetValue (iter, (int)FixedColumns.Type);
					listTextType.Add(tmpTypeText);

				} while(treeviewMain.Model.IterNext (ref iter));

			}

			var textSize = dividedText.Count;
			var quotient = textSize / factorSize;
			var remainder = textSize - (factorSize * quotient);

			if ((quotient != 0) &&
				(remainder == 0))
			{
				// http://www.martyndavis.com/?p=357

				string[][] arrayTextBuffer = new string[listNumSize.Count][];

				for (int i = 0; i < listNumSize.Count; i++) {
					arrayTextBuffer [i] = new string[quotient+(int)FixedColumns.DataStart];
					arrayTextBuffer [i] [(int)FixedColumns.Size] = listNumSize [i].ToString();
					arrayTextBuffer [i] [(int)FixedColumns.Type] = listTextType [i];
					
				}

				{
					int j = (int)FixedColumns.DataStart;
					while (dividedText.Count != 0) {
						int i = 0;
						foreach (var item in listNumSize) {
							//int sizeData = int.Parse(arrayTextBuffer [i] [(int)FixedColumns.Size]);
							string typeData = arrayTextBuffer [i] [(int)FixedColumns.Type];

							List<string> listTmp = dividedText.GetRange (0, item);

							string textTmp = "";
							if (typeData != numeralSystem.ASCII) {
								foreach (var tmp in listTmp.Reverse<string>()) {
									textTmp += tmp;
								}
							} else {
								foreach (var tmp in listTmp) {
									textTmp += tmp;
								}
							}

							textTmp = TypeConvert.FromHexChars(typeData, item, textTmp);
							arrayTextBuffer [i] [j] = textTmp;
							dividedText.RemoveRange(0, item);
							i++;
						}
						j++;
					}

				}

				Type[] arrayType = new Type[quotient+(int)FixedColumns.DataStart];
				for (int i = 0; i < (quotient+(int)FixedColumns.DataStart); i++) {
					arrayType [i] = typeof(string);
				}

				ListStore tmpListStore = new ListStore (arrayType);

				foreach (var item in arrayTextBuffer) {
					tmpListStore.AppendValues (item.ToArray ());
				}
				tmpListStore.AppendValues (ROW_TERMINATOR, "");

				treeviewMain.Model = tmpListStore;
				ClaerAppearance (treeviewMain);
				ConstructAppearance (treeviewMain);

			}

		}

		private void copyToClipBoardButton_Click(object sender, EventArgs e)
		{
			string delimiter = "\t";
			string seriesName = "Series";
			string header = "";
			int maxNumRow = treeviewMain.Model.NColumns - (int)FixedColumns.DataStart;
			string[][] arrayTextBuffer = new string[maxNumRow][];

			int maxNumColumn = 0;
			TreeIter iter;
			if (treeviewMain.Model.GetIterFirst (out iter)) {
				do
				{
					string tmpSizeText = (string) treeviewMain.Model.GetValue (iter, (int)FixedColumns.Size);
					if( tmpSizeText == ROW_TERMINATOR ) {
						break;
					}

					if( header == "" )
					{
						header = seriesName + maxNumColumn.ToString();
					}
					else
					{
						header += delimiter + seriesName + maxNumColumn.ToString();
					}

					maxNumColumn++;

				} while(treeviewMain.Model.IterNext (ref iter));

			}

			for (int i = 0; i < maxNumRow; i++) {
				arrayTextBuffer[i] = new string[maxNumColumn];
			}

			if (treeviewMain.Model.GetIterFirst (out iter)) {
				int j = 0;
				do
				{
					string tmpSizeText = (string) treeviewMain.Model.GetValue (iter, (int)FixedColumns.Size);
					if( tmpSizeText == ROW_TERMINATOR ) {
						break;
					}

					for(int i = 0; i < maxNumRow; i++){
						arrayTextBuffer[i][j] = (string) treeviewMain.Model.GetValue (iter, ((int)FixedColumns.DataStart+i));
					}
					j++;

				} while(treeviewMain.Model.IterNext (ref iter));

			}

			System.Text.StringBuilder sb = new System.Text.StringBuilder();

			sb.AppendLine(header);

			foreach (var item in arrayTextBuffer) {
				string textTmp = "";
				foreach (var factor in item) {
					if( textTmp == "" )
					{
						textTmp = factor;
					}
					else
					{
						textTmp += delimiter + factor;
					}

				}
				sb.AppendLine(textTmp);

			}

			Clipboard clipboard = Clipboard.Get(Gdk.Atom.Intern("CLIPBOARD", false));

			clipboard.Text = sb.ToString();

		}

		public void PutDumpData(string text)
		{
			string tmp = textviewMain.Buffer.Text;

			if (tmp != "")
			{
				tmp += "-";

			}

			tmp += text;
			textviewMain.Buffer.Text = tmp;

		}

	}
		
}

