
// This file has been generated by the GUI designer. Do not modify.
namespace rmApplication
{
	public partial class DumpWindow
	{
		private global::Gtk.VBox vboxMain;
		
		private global::Gtk.Table table1st;
		
		private global::Gtk.Button buttonCopy;
		
		private global::Gtk.Button buttonMake;
		
		private global::Gtk.Button buttonRequest;
		
		private global::Gtk.Entry entryAddress;
		
		private global::Gtk.Entry entrySize;
		
		private global::Gtk.Entry entryVariable;
		
		private global::Gtk.Label labelAddress;
		
		private global::Gtk.Label labelEmpty;
		
		private global::Gtk.Label labelSize;
		
		private global::Gtk.Label labelVariable;
		
		private global::Gtk.Frame frameText;
		
		private global::Gtk.Alignment GtkAlignment;
		
		private global::Gtk.ScrolledWindow GtkScrolledWindow;
		
		private global::Gtk.TextView textviewMain;
		
		private global::Gtk.Label GtkLabelText;
		
		private global::Gtk.Frame frameView;
		
		private global::Gtk.Alignment GtkAlignment1;
		
		private global::Gtk.ScrolledWindow GtkScrolledWindow1;
		
		private global::Gtk.TreeView treeviewMain;
		
		private global::Gtk.Label GtkLabelView;

		protected virtual void Build ()
		{
			global::Stetic.Gui.Initialize (this);
			// Widget rmApplication.DumpWindow
			this.Name = "rmApplication.DumpWindow";
			this.Title = global::Mono.Unix.Catalog.GetString ("DumpWindow");
			this.WindowPosition = ((global::Gtk.WindowPosition)(4));
			// Container child rmApplication.DumpWindow.Gtk.Container+ContainerChild
			this.vboxMain = new global::Gtk.VBox ();
			this.vboxMain.Name = "vboxMain";
			this.vboxMain.Spacing = 6;
			// Container child vboxMain.Gtk.Box+BoxChild
			this.table1st = new global::Gtk.Table (((uint)(3)), ((uint)(4)), false);
			this.table1st.Name = "table1st";
			this.table1st.RowSpacing = ((uint)(6));
			this.table1st.ColumnSpacing = ((uint)(6));
			// Container child table1st.Gtk.Table+TableChild
			this.buttonCopy = new global::Gtk.Button ();
			this.buttonCopy.WidthRequest = 80;
			this.buttonCopy.CanFocus = true;
			this.buttonCopy.Name = "buttonCopy";
			this.buttonCopy.UseUnderline = true;
			this.buttonCopy.Label = global::Mono.Unix.Catalog.GetString ("Copy");
			this.table1st.Add (this.buttonCopy);
			global::Gtk.Table.TableChild w1 = ((global::Gtk.Table.TableChild)(this.table1st [this.buttonCopy]));
			w1.TopAttach = ((uint)(2));
			w1.BottomAttach = ((uint)(3));
			w1.LeftAttach = ((uint)(3));
			w1.RightAttach = ((uint)(4));
			w1.XOptions = ((global::Gtk.AttachOptions)(4));
			w1.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table1st.Gtk.Table+TableChild
			this.buttonMake = new global::Gtk.Button ();
			this.buttonMake.WidthRequest = 80;
			this.buttonMake.CanFocus = true;
			this.buttonMake.Name = "buttonMake";
			this.buttonMake.UseUnderline = true;
			this.buttonMake.Label = global::Mono.Unix.Catalog.GetString ("Make");
			this.table1st.Add (this.buttonMake);
			global::Gtk.Table.TableChild w2 = ((global::Gtk.Table.TableChild)(this.table1st [this.buttonMake]));
			w2.TopAttach = ((uint)(1));
			w2.BottomAttach = ((uint)(2));
			w2.LeftAttach = ((uint)(3));
			w2.RightAttach = ((uint)(4));
			w2.XOptions = ((global::Gtk.AttachOptions)(4));
			w2.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table1st.Gtk.Table+TableChild
			this.buttonRequest = new global::Gtk.Button ();
			this.buttonRequest.WidthRequest = 80;
			this.buttonRequest.CanFocus = true;
			this.buttonRequest.Name = "buttonRequest";
			this.buttonRequest.UseUnderline = true;
			this.buttonRequest.Label = global::Mono.Unix.Catalog.GetString ("Request");
			this.table1st.Add (this.buttonRequest);
			global::Gtk.Table.TableChild w3 = ((global::Gtk.Table.TableChild)(this.table1st [this.buttonRequest]));
			w3.LeftAttach = ((uint)(3));
			w3.RightAttach = ((uint)(4));
			w3.XOptions = ((global::Gtk.AttachOptions)(4));
			w3.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table1st.Gtk.Table+TableChild
			this.entryAddress = new global::Gtk.Entry ();
			this.entryAddress.WidthRequest = 180;
			this.entryAddress.CanFocus = true;
			this.entryAddress.Name = "entryAddress";
			this.entryAddress.IsEditable = true;
			this.entryAddress.InvisibleChar = '●';
			this.table1st.Add (this.entryAddress);
			global::Gtk.Table.TableChild w4 = ((global::Gtk.Table.TableChild)(this.table1st [this.entryAddress]));
			w4.TopAttach = ((uint)(1));
			w4.BottomAttach = ((uint)(2));
			w4.LeftAttach = ((uint)(1));
			w4.RightAttach = ((uint)(2));
			w4.XOptions = ((global::Gtk.AttachOptions)(4));
			w4.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table1st.Gtk.Table+TableChild
			this.entrySize = new global::Gtk.Entry ();
			this.entrySize.WidthRequest = 180;
			this.entrySize.CanFocus = true;
			this.entrySize.Name = "entrySize";
			this.entrySize.IsEditable = true;
			this.entrySize.InvisibleChar = '●';
			this.table1st.Add (this.entrySize);
			global::Gtk.Table.TableChild w5 = ((global::Gtk.Table.TableChild)(this.table1st [this.entrySize]));
			w5.TopAttach = ((uint)(2));
			w5.BottomAttach = ((uint)(3));
			w5.LeftAttach = ((uint)(1));
			w5.RightAttach = ((uint)(2));
			w5.XOptions = ((global::Gtk.AttachOptions)(4));
			w5.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table1st.Gtk.Table+TableChild
			this.entryVariable = new global::Gtk.Entry ();
			this.entryVariable.WidthRequest = 180;
			this.entryVariable.CanFocus = true;
			this.entryVariable.Name = "entryVariable";
			this.entryVariable.IsEditable = true;
			this.entryVariable.InvisibleChar = '●';
			this.table1st.Add (this.entryVariable);
			global::Gtk.Table.TableChild w6 = ((global::Gtk.Table.TableChild)(this.table1st [this.entryVariable]));
			w6.LeftAttach = ((uint)(1));
			w6.RightAttach = ((uint)(2));
			w6.XOptions = ((global::Gtk.AttachOptions)(4));
			w6.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table1st.Gtk.Table+TableChild
			this.labelAddress = new global::Gtk.Label ();
			this.labelAddress.Name = "labelAddress";
			this.labelAddress.LabelProp = global::Mono.Unix.Catalog.GetString ("Address:");
			this.table1st.Add (this.labelAddress);
			global::Gtk.Table.TableChild w7 = ((global::Gtk.Table.TableChild)(this.table1st [this.labelAddress]));
			w7.TopAttach = ((uint)(1));
			w7.BottomAttach = ((uint)(2));
			w7.XOptions = ((global::Gtk.AttachOptions)(4));
			w7.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table1st.Gtk.Table+TableChild
			this.labelEmpty = new global::Gtk.Label ();
			this.labelEmpty.Name = "labelEmpty";
			this.table1st.Add (this.labelEmpty);
			global::Gtk.Table.TableChild w8 = ((global::Gtk.Table.TableChild)(this.table1st [this.labelEmpty]));
			w8.TopAttach = ((uint)(1));
			w8.BottomAttach = ((uint)(2));
			w8.LeftAttach = ((uint)(2));
			w8.RightAttach = ((uint)(3));
			w8.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table1st.Gtk.Table+TableChild
			this.labelSize = new global::Gtk.Label ();
			this.labelSize.Name = "labelSize";
			this.labelSize.LabelProp = global::Mono.Unix.Catalog.GetString ("Size:");
			this.table1st.Add (this.labelSize);
			global::Gtk.Table.TableChild w9 = ((global::Gtk.Table.TableChild)(this.table1st [this.labelSize]));
			w9.TopAttach = ((uint)(2));
			w9.BottomAttach = ((uint)(3));
			w9.XOptions = ((global::Gtk.AttachOptions)(4));
			w9.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table1st.Gtk.Table+TableChild
			this.labelVariable = new global::Gtk.Label ();
			this.labelVariable.Name = "labelVariable";
			this.labelVariable.LabelProp = global::Mono.Unix.Catalog.GetString ("Variable:");
			this.table1st.Add (this.labelVariable);
			global::Gtk.Table.TableChild w10 = ((global::Gtk.Table.TableChild)(this.table1st [this.labelVariable]));
			w10.XOptions = ((global::Gtk.AttachOptions)(4));
			w10.YOptions = ((global::Gtk.AttachOptions)(4));
			this.vboxMain.Add (this.table1st);
			global::Gtk.Box.BoxChild w11 = ((global::Gtk.Box.BoxChild)(this.vboxMain [this.table1st]));
			w11.Position = 0;
			w11.Expand = false;
			w11.Fill = false;
			// Container child vboxMain.Gtk.Box+BoxChild
			this.frameText = new global::Gtk.Frame ();
			this.frameText.WidthRequest = 120;
			this.frameText.Name = "frameText";
			this.frameText.ShadowType = ((global::Gtk.ShadowType)(0));
			// Container child frameText.Gtk.Container+ContainerChild
			this.GtkAlignment = new global::Gtk.Alignment (0F, 0F, 1F, 1F);
			this.GtkAlignment.Name = "GtkAlignment";
			this.GtkAlignment.LeftPadding = ((uint)(12));
			// Container child GtkAlignment.Gtk.Container+ContainerChild
			this.GtkScrolledWindow = new global::Gtk.ScrolledWindow ();
			this.GtkScrolledWindow.Name = "GtkScrolledWindow";
			this.GtkScrolledWindow.ShadowType = ((global::Gtk.ShadowType)(1));
			// Container child GtkScrolledWindow.Gtk.Container+ContainerChild
			this.textviewMain = new global::Gtk.TextView ();
			this.textviewMain.Buffer.Text = "00-AA-23-DD";
			this.textviewMain.CanFocus = true;
			this.textviewMain.Name = "textviewMain";
			this.GtkScrolledWindow.Add (this.textviewMain);
			this.GtkAlignment.Add (this.GtkScrolledWindow);
			this.frameText.Add (this.GtkAlignment);
			this.GtkLabelText = new global::Gtk.Label ();
			this.GtkLabelText.Name = "GtkLabelText";
			this.GtkLabelText.LabelProp = global::Mono.Unix.Catalog.GetString ("<b>Text</b>");
			this.GtkLabelText.UseMarkup = true;
			this.frameText.LabelWidget = this.GtkLabelText;
			this.vboxMain.Add (this.frameText);
			global::Gtk.Box.BoxChild w15 = ((global::Gtk.Box.BoxChild)(this.vboxMain [this.frameText]));
			w15.Position = 1;
			w15.Expand = false;
			// Container child vboxMain.Gtk.Box+BoxChild
			this.frameView = new global::Gtk.Frame ();
			this.frameView.Name = "frameView";
			this.frameView.ShadowType = ((global::Gtk.ShadowType)(0));
			// Container child frameView.Gtk.Container+ContainerChild
			this.GtkAlignment1 = new global::Gtk.Alignment (0F, 0F, 1F, 1F);
			this.GtkAlignment1.Name = "GtkAlignment1";
			this.GtkAlignment1.LeftPadding = ((uint)(12));
			// Container child GtkAlignment1.Gtk.Container+ContainerChild
			this.GtkScrolledWindow1 = new global::Gtk.ScrolledWindow ();
			this.GtkScrolledWindow1.Name = "GtkScrolledWindow1";
			this.GtkScrolledWindow1.ShadowType = ((global::Gtk.ShadowType)(1));
			// Container child GtkScrolledWindow1.Gtk.Container+ContainerChild
			this.treeviewMain = new global::Gtk.TreeView ();
			this.treeviewMain.CanFocus = true;
			this.treeviewMain.Name = "treeviewMain";
			this.GtkScrolledWindow1.Add (this.treeviewMain);
			this.GtkAlignment1.Add (this.GtkScrolledWindow1);
			this.frameView.Add (this.GtkAlignment1);
			this.GtkLabelView = new global::Gtk.Label ();
			this.GtkLabelView.Name = "GtkLabelView";
			this.GtkLabelView.LabelProp = global::Mono.Unix.Catalog.GetString ("<b>View</b>");
			this.GtkLabelView.UseMarkup = true;
			this.frameView.LabelWidget = this.GtkLabelView;
			this.vboxMain.Add (this.frameView);
			global::Gtk.Box.BoxChild w19 = ((global::Gtk.Box.BoxChild)(this.vboxMain [this.frameView]));
			w19.Position = 2;
			this.Add (this.vboxMain);
			if ((this.Child != null)) {
				this.Child.ShowAll ();
			}
			this.DefaultWidth = 410;
			this.DefaultHeight = 308;
			this.Show ();
		}
	}
}