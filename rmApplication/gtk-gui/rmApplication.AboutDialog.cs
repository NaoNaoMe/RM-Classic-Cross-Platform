
// This file has been generated by the GUI designer. Do not modify.
namespace rmApplication
{
	public partial class AboutDialog
	{
		private global::Gtk.Table tableMain;
		
		private global::Gtk.Label labelCompanyName;
		
		private global::Gtk.Label labelCopyright;
		
		private global::Gtk.Label labelProductName;
		
		private global::Gtk.Label labelVersion;
		
		private global::Gtk.ScrolledWindow GtkScrolledWindow;
		
		private global::Gtk.TextView textviewDescription;
		
		private global::Gtk.Button buttonOk;

		protected virtual void Build ()
		{
			global::Stetic.Gui.Initialize (this);
			// Widget rmApplication.AboutDialog
			this.WidthRequest = 300;
			this.HeightRequest = 250;
			this.Name = "rmApplication.AboutDialog";
			this.Title = global::Mono.Unix.Catalog.GetString ("AboutDialog");
			this.WindowPosition = ((global::Gtk.WindowPosition)(4));
			this.Modal = true;
			this.Resizable = false;
			// Internal child rmApplication.AboutDialog.VBox
			global::Gtk.VBox w1 = this.VBox;
			w1.Name = "dialog1_VBox";
			w1.BorderWidth = ((uint)(2));
			// Container child dialog1_VBox.Gtk.Box+BoxChild
			this.tableMain = new global::Gtk.Table (((uint)(4)), ((uint)(2)), false);
			this.tableMain.Name = "tableMain";
			this.tableMain.RowSpacing = ((uint)(6));
			this.tableMain.ColumnSpacing = ((uint)(6));
			// Container child tableMain.Gtk.Table+TableChild
			this.labelCompanyName = new global::Gtk.Label ();
			this.labelCompanyName.Name = "labelCompanyName";
			this.labelCompanyName.Xalign = 0F;
			this.labelCompanyName.LabelProp = global::Mono.Unix.Catalog.GetString ("CompanyName");
			this.tableMain.Add (this.labelCompanyName);
			global::Gtk.Table.TableChild w2 = ((global::Gtk.Table.TableChild)(this.tableMain [this.labelCompanyName]));
			w2.TopAttach = ((uint)(3));
			w2.BottomAttach = ((uint)(4));
			w2.XOptions = ((global::Gtk.AttachOptions)(4));
			w2.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child tableMain.Gtk.Table+TableChild
			this.labelCopyright = new global::Gtk.Label ();
			this.labelCopyright.Name = "labelCopyright";
			this.labelCopyright.Xalign = 0F;
			this.labelCopyright.LabelProp = global::Mono.Unix.Catalog.GetString ("Copyright");
			this.tableMain.Add (this.labelCopyright);
			global::Gtk.Table.TableChild w3 = ((global::Gtk.Table.TableChild)(this.tableMain [this.labelCopyright]));
			w3.TopAttach = ((uint)(2));
			w3.BottomAttach = ((uint)(3));
			w3.XOptions = ((global::Gtk.AttachOptions)(4));
			w3.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child tableMain.Gtk.Table+TableChild
			this.labelProductName = new global::Gtk.Label ();
			this.labelProductName.Name = "labelProductName";
			this.labelProductName.Xalign = 0F;
			this.labelProductName.LabelProp = global::Mono.Unix.Catalog.GetString ("ProductName");
			this.tableMain.Add (this.labelProductName);
			global::Gtk.Table.TableChild w4 = ((global::Gtk.Table.TableChild)(this.tableMain [this.labelProductName]));
			w4.XOptions = ((global::Gtk.AttachOptions)(4));
			w4.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child tableMain.Gtk.Table+TableChild
			this.labelVersion = new global::Gtk.Label ();
			this.labelVersion.Name = "labelVersion";
			this.labelVersion.Xalign = 0F;
			this.labelVersion.LabelProp = global::Mono.Unix.Catalog.GetString ("Version");
			this.tableMain.Add (this.labelVersion);
			global::Gtk.Table.TableChild w5 = ((global::Gtk.Table.TableChild)(this.tableMain [this.labelVersion]));
			w5.TopAttach = ((uint)(1));
			w5.BottomAttach = ((uint)(2));
			w5.XOptions = ((global::Gtk.AttachOptions)(4));
			w5.YOptions = ((global::Gtk.AttachOptions)(4));
			w1.Add (this.tableMain);
			global::Gtk.Box.BoxChild w6 = ((global::Gtk.Box.BoxChild)(w1 [this.tableMain]));
			w6.Position = 0;
			w6.Expand = false;
			w6.Fill = false;
			// Container child dialog1_VBox.Gtk.Box+BoxChild
			this.GtkScrolledWindow = new global::Gtk.ScrolledWindow ();
			this.GtkScrolledWindow.Name = "GtkScrolledWindow";
			this.GtkScrolledWindow.ShadowType = ((global::Gtk.ShadowType)(1));
			// Container child GtkScrolledWindow.Gtk.Container+ContainerChild
			this.textviewDescription = new global::Gtk.TextView ();
			this.textviewDescription.Buffer.Text = "Description";
			this.textviewDescription.CanFocus = true;
			this.textviewDescription.Name = "textviewDescription";
			this.textviewDescription.Editable = false;
			this.GtkScrolledWindow.Add (this.textviewDescription);
			w1.Add (this.GtkScrolledWindow);
			global::Gtk.Box.BoxChild w8 = ((global::Gtk.Box.BoxChild)(w1 [this.GtkScrolledWindow]));
			w8.Position = 1;
			// Internal child rmApplication.AboutDialog.ActionArea
			global::Gtk.HButtonBox w9 = this.ActionArea;
			w9.Name = "dialog1_ActionArea";
			w9.Spacing = 10;
			w9.BorderWidth = ((uint)(5));
			w9.LayoutStyle = ((global::Gtk.ButtonBoxStyle)(4));
			// Container child dialog1_ActionArea.Gtk.ButtonBox+ButtonBoxChild
			this.buttonOk = new global::Gtk.Button ();
			this.buttonOk.CanDefault = true;
			this.buttonOk.CanFocus = true;
			this.buttonOk.Name = "buttonOk";
			this.buttonOk.UseStock = true;
			this.buttonOk.UseUnderline = true;
			this.buttonOk.Label = "gtk-ok";
			this.AddActionWidget (this.buttonOk, -5);
			global::Gtk.ButtonBox.ButtonBoxChild w10 = ((global::Gtk.ButtonBox.ButtonBoxChild)(w9 [this.buttonOk]));
			w10.Expand = false;
			w10.Fill = false;
			if ((this.Child != null)) {
				this.Child.ShowAll ();
			}
			this.DefaultWidth = 400;
			this.DefaultHeight = 300;
			this.Show ();
		}
	}
}
