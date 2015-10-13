using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class guiAwailableSlot : MonoBehaviour {

	public Image Icon;

	[HideInInspector]
	public object Data = null;
	
	ResourceData _worker = null;
	public ResourceData Worker {
		get { return _worker; }
		set {
			_worker = value;
			Icon.sprite = _worker == null ? ResourceManager.ErrorSprite : _worker.Image;
		}
	}


	// Use this for initialization
	void Start () {
		Icon.sprite = _worker == null ? ResourceManager.ErrorSprite : _worker.Image;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void BeginDrag() {
		//Debug.Log(Data);
		GUImain.DragObject = new DragClass(Worker, Data);
	}
}
