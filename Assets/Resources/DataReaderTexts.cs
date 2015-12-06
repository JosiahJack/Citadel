using UnityEngine;
using System.Collections;
using System.Xml;
using System.Xml.Serialization;

public class Log {
	
	[XmlAttribute("name")]
	public string name;
	
	[XmlElement("LogContent")]
	public string LogContent;
	
}