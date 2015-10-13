using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class guiWorkerInfo : MonoBehaviour {

	public Slider Progress;
	public Image Icon;
	public guiLaborers Laborers;

	public Sprite StartWorkingIcon;

	[HideInInspector]public ResourceWorker Worker;
	[HideInInspector]public int ListID;

	public delegate void EndDragging();
	public static event EndDragging OnEndDrag;

	// Use this for initialization
	void Start () {
		Debug.Log("Start info");
		Redraw();
	}

	public void Redraw() {
		Progress.value = 0.01f;
		Laborers.Clear();

		if (Worker == null) {
			Icon.sprite = StartWorkingIcon;
			return;
		}
		 
		Icon.sprite = Worker.Resource.Image;
		Laborers.AddLaborers(Worker.Laborers);
	}

	// Update is called once per frame
	void Update () {
		if (Worker == null) return;
		
		Progress.value = Worker.Progress;	
	}

	public void EndDrag() {
		if (OnEndDrag != null) OnEndDrag();
	}
}
