using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.IO;

[XmlRoot("AudioLogsCollection")]
public class DataReaderTextContainer {
	
	[XmlArray("AudioLogs")]
	[XmlArrayItem("LogContent")]
	public List <Log> audioLogs = new List<Log>();
	
	public static DataReaderTextContainer Load(string path)
	{
		TextAsset _xml = Resources.Load <TextAsset> (path);
		
		XmlSerializer serializer = new XmlSerializer (typeof(DataReaderTextContainer));
		
		StringReader reader = new StringReader (_xml.text);
		
		DataReaderTextContainer audioLogs = serializer.Deserialize(reader) as DataReaderTextContainer;
		//DataReaderTextContainer logTexts = serializer.Deserialize(reader) as DataReaderTextContainer;
		
		reader.Close();
		
		return audioLogs;
	}
	
}