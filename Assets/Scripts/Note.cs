using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Note : MonoBehaviour
{
	//TextAreaAttribute(int minLines, int maxLines);
	[TextArea(15,20)]
    public string note;
}
