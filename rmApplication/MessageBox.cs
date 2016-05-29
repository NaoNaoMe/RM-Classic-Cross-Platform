using System;
using Gtk;

namespace rmApplication
{
	class MessageBox
	{
		public static void ShowWarning(string Msg)
		{
			MessageDialog md = new MessageDialog (null, DialogFlags.Modal, MessageType.Warning, ButtonsType.Ok, Msg);
			md.Run ();
			md.Destroy();

		}

		public static bool ShowQuestion(string Msg)
		{
			MessageDialog md = new MessageDialog (null, DialogFlags.Modal, MessageType.Question, ButtonsType.OkCancel, Msg);
			Gtk.ResponseType result = (ResponseType)md.Run ();
			md.Destroy();

			if (result == Gtk.ResponseType.Ok)
			{
				return true;
			}
			else
			{
				return false;
			}
			
		}
		
	}
}
