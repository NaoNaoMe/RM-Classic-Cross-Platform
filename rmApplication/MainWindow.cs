using System;
//using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
//using System.IO;

using Gtk;

namespace rmApplication
{
	public partial class MainWindow: Gtk.Window
	{
		private const string WINDOW_TITLE = "RM Classic CP";

		SubViewControl subViewControl1;

		public MainWindow () : base (Gtk.WindowType.Toplevel)
		{
			Build ();

			this.Title = WINDOW_TITLE;

			subViewControl1 = new SubViewControl ();

			vboxMain.Add (subViewControl1);

			openViewFileAction.Activated += new EventHandler (openViewFileToolStripMenuItem_Click);
			openMapFileAction.Activated += new EventHandler (openMapFileToolStripMenuItem_Click);
			saveViewFileAction.Activated += new EventHandler (saveViewFileToolStripMenuItem_Click);
			saveMapFileAction.Activated += new EventHandler (saveMapFileToolStripMenuItem_Click);
			SetupAction.Activated += new EventHandler (optionsToolStripMenuItem_Click);
			ExitAction.Activated += new EventHandler (exitToolStripMenuItem_Click);
		}

		protected void OnDeleteEvent (object sender, DeleteEventArgs a)
		{
			subViewControl1.commonClosingRoutine();
			Application.Quit ();
			a.RetVal = true;
		}

		private void openViewFileToolStripMenuItem_Click (object sender, EventArgs args)
		{
			{
				FileChooserDialog ofd = new FileChooserDialog ("Choose the file to open",
					this,
					FileChooserAction.Open,
					"Cancel",ResponseType.Cancel,
					"Open",ResponseType.Accept);

				ofd.Filter = new FileFilter ();
				ofd.Filter.AddPattern ("*.xml");

				if (ofd.Run() == (int)ResponseType.Accept)
				{
					string pathName = ofd.Filename;

					XmlSerializer serializer = new XmlSerializer(typeof(ViewSetting));

					ViewSetting deserializedData = new ViewSetting();

					try
					{
						System.IO.StreamReader reader = new System.IO.StreamReader(pathName);
						deserializedData = (ViewSetting)serializer.Deserialize(reader);
						reader.Close();

					}
					catch (Exception ex)
					{
						deserializedData = null;
						MessageBox.ShowWarning(ex.Message);

					}

					if (deserializedData != null)
					{
						subViewControl1.loadViewSettingFile(deserializedData);

						string fileName = System.IO.Path.GetFileNameWithoutExtension(pathName);
						string viewName = subViewControl1.getViewName(fileName);

						if (viewName != null)
						{
							this.Title = viewName + " - " + WINDOW_TITLE;
						}
					
					}

				}

				ofd.Destroy();

			}

		}

		private void openMapFileToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (subViewControl1.myComponents.CommActiveFlg == true)
			{
				MessageBox.ShowWarning ("Stop communication.");

			}
			else
			{
				FileChooserDialog ofd = new FileChooserDialog ("Choose the file to open",
					this,
					FileChooserAction.Open,
					"Cancel",ResponseType.Cancel,
					"Open",ResponseType.Accept);

				ofd.Filter = new FileFilter ();
				ofd.Filter.AddPattern ("*.map");

				if (ofd.Run () == (int)ResponseType.Accept)
				{
					string pathName = ofd.Filename;

					if (subViewControl1.loadMapFile(pathName) == false)
					{
						MessageBox.ShowWarning ("Can't read map file");

					}

				}
					
				ofd.Destroy();

			}

		}

		private void saveViewFileToolStripMenuItem_Click(object sender, EventArgs e)
		{
			FileChooserDialog ofd = new FileChooserDialog ("Choose the file to save",
				this,
				FileChooserAction.Save,
				"Cancel",ResponseType.Cancel,
				"Save",ResponseType.Accept);
			string text = subViewControl1.getViewSettingFileName ();
			ofd.CurrentName = text + ".xml";
			ofd.Filter = new FileFilter ();
			ofd.Filter.AddPattern ("*.xml");

			if (ofd.Run() == (int)ResponseType.Accept)
			{
				var tmpVSettingFactor = new ViewSetting();

				foreach (var factor in subViewControl1.myComponents.ViewSettingList)
				{
					foreach (var item in factor.DataSetting)
					{
						tmpVSettingFactor.DataSetting.Add(item);

					}

				}

				try
				{
					System.IO.FileStream fs = new System.IO.FileStream(ofd.Filename, System.IO.FileMode.Create);
					XmlSerializer serializer = new XmlSerializer(typeof(ViewSetting));
					serializer.Serialize(fs, tmpVSettingFactor);
					fs.Close();
				}
				catch (Exception ex)
				{
					MessageBox.ShowWarning (ex.Message);

				}

			}

			ofd.Destroy();

		}

		private void saveMapFileToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if ( (subViewControl1.myComponents.MapList == null ) ||
				(subViewControl1.myComponents.MapList.Count == 0 ) )
			{
				return;

			}

			FileChooserDialog ofd = new FileChooserDialog ("Choose the file to save",
				this,
				FileChooserAction.Save,
				"Cancel",ResponseType.Cancel,
				"Save",ResponseType.Accept);
			ofd.CurrentName = "test.map";
			ofd.Filter = new FileFilter ();
			ofd.Filter.AddPattern ("*.map");

			if (ofd.Run() == (int)ResponseType.Accept)
			{
				List<string> textList = new List<string>();

				bool ret = RmAddressMap.Convert(textList, subViewControl1.myComponents.MapList);

				if( ret == true )
				{
					try
					{
						System.IO.StreamWriter sw = new System.IO.StreamWriter( ofd.Filename,
							false,
							System.Text.Encoding.GetEncoding("utf-8"));

						foreach (var item in textList)
						{
							sw.WriteLine(item);

						}

						sw.Close();

					}
					catch (Exception ex)
					{
						MessageBox.ShowWarning (ex.Message);

					}

				}


			}

			ofd.Destroy();


		}

		private void exitToolStripMenuItem_Click(object sender, EventArgs e)
		{
			subViewControl1.commonClosingRoutine();
			Application.Quit ();
		}

		private void optionsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (subViewControl1.myComponents.CommActiveFlg == true)
			{
				MessageBox.ShowWarning ("Stop communication.");

			}
			else
			{
				OptionDialog od = new OptionDialog (subViewControl1);
				od.Run ();
				od.Destroy();

			}

		}

	}

}
