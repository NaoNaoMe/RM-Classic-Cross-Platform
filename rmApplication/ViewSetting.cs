using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Xml.Serialization;

namespace rmApplication
{
	[Serializable()]
	public class DataSetting
	{
		[XmlIgnore]
		public int RowCount { set; get; }

		[XmlElement("Group")]
		public string Group { set; get; }

		[XmlElement("Check")]
		public bool Check { set; get; }

		[XmlIgnore]
		public int ErrNum { set; get; }

		[XmlElement("Size")]
		public string Size { set; get; }

		[XmlElement("Variable")]
		public string Variable { set; get; }

		[XmlElement("AddrLock")]
		public bool AddrLock { set; get; }

		[XmlElement("Address")]
		public string Address { set; get; }

		[XmlElement("Offset")]
		public string Offset { set; get; }

		[XmlElement("Name")]
		public string Name { set; get; }

		[XmlElement("Type")]
		public string Type { set; get; }

		[XmlIgnore]
		public string ReadText { set; get; }

		[XmlIgnore]
		public string ReadValue { set; get; }

		[XmlIgnore]
		public string WriteText { set; get; }

		[XmlElement("WriteValue")]
		public string WriteValue { set; get; }

		[XmlIgnore]
		public bool Assert { set; get; }

		public DataSetting()
		{
			RowCount	= 0;
			Group		= null;
			Check		= false;
			ErrNum		= 0;
			Size		= "1";
			Variable	= null;
			AddrLock	= false;
			Address		= null;
			Offset		= "0";
			Name		= null;
			Type		= "Hex";
			ReadText	= null;
			ReadValue	= null;
			WriteText	= null;
			WriteValue	= null;
			Assert		= false;
		}

		public DataSetting( DataSetting data )
		{
			RowCount	= data.RowCount;
			Group		= data.Group;
			Check		= data.Check;
			ErrNum		= data.ErrNum;
			Size		= data.Size;
			Variable	= data.Variable;
			AddrLock	= data.AddrLock;
			Address		= data.Address;
			Offset		= data.Offset;
			Name		= data.Name;
			Type		= data.Type;
			ReadText	= data.ReadText;
			ReadValue	= data.ReadValue;
			WriteText	= data.WriteText;
			WriteValue	= data.WriteValue;
			Assert		= false;
		}

	}

	[Serializable()]
	[XmlRoot("ViewSetting")]
	public class ViewSetting
	{
		[XmlElement("DataSetting")]
		public List<DataSetting> DataSetting { get; set; }

		public ViewSetting()
		{
			DataSetting = new List<DataSetting>();
		}

		public ViewSetting( ViewSetting data)
		{
			DataSetting = new List<DataSetting>(data.DataSetting);

		}

	}

}
