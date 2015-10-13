using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class BuildDialog : DialogClass {

	public guiLaborers ActiveLaborers;
	public RectTransform ActiveSlots;
	public guiScroll AwailableWorks;
	public guiInventory InventoryPanel;

	public guiSlotList ActiveWorks;

	public guiWorkerInfo SelectedWorker;

	public GameObject AwailablePrefab;

	public guiShop ShopPanel;

	[HideInInspector]
	public Building CurrentBuilding;

	//List<guiSlot> ActiveWorks = new List<guiSlot>();

	public Text CaptionText;
	public string Caption {
		set {
			if (CaptionText) CaptionText.text = value;
		}
	}

	// Use this for initialization
	void Start () {
		SelectedWorker.gameObject.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () {
		for (int i = 0; i < CurrentBuilding.ActiveWorks.List.Count; i++) {
			if (CurrentBuilding.ActiveWorks.List[i] == null) continue;
			ActiveWorks.GetItem<guiSlot>(i).Progress.value = CurrentBuilding.ActiveWorks.List[i].Progress;
		}
	}

	void OnEnable() {
		guiWorkerInfo.OnEndDrag += OnEndDrag;
	}

	void OnDisable() {
		guiWorkerInfo.OnEndDrag -= OnEndDrag;
	}

	override public void SetBuilding(GameObject objectBuild) {

		CurrentBuilding = objectBuild.GetComponent<Building>();
		Caption = CurrentBuilding.Caption;

		ActiveLaborers.AddLaborers(CurrentBuilding.Laborers);

		//Pievieno pieejamos darbus
		AwailableWorks.ClearItems();

		foreach (ResourceData data in CurrentBuilding.Resource.Available.List) {
			if (data.id < 0) continue;
			GameObject obj = AwailableWorks.AddItem(AwailablePrefab);
				guiSlot slot = obj.GetComponent<guiSlot>();
				slot.Type = guiSlot.SlotType.NORMAL;
				slot.Drag = guiSlot.DragType.DRAG;
				slot.Worker = new ResourceData(data.id, 0);
		}


		//Pievienojam darbu slotus
		while (ActiveSlots.childCount > 0) {
			GameObject.DestroyImmediate(ActiveSlots.GetChild(0).gameObject);
		}

		ActiveWorks.ClearItems();
		for (int i = 0; i < CurrentBuilding.ActiveWorks.List.Count; i++) {
			ResourceData worker;
			if (CurrentBuilding.ActiveWorks.List[i] != null && CurrentBuilding.ActiveWorks.List[i].Resource != null) {
				worker = new ResourceData(CurrentBuilding.ActiveWorks.List[i].Resource.ID, 0);
			} else {
				worker = new ResourceData();
			}

			GameObject obj = ActiveWorks.AddItem();

			guiSlot slot = obj.GetComponent<guiSlot>();
			slot.Type = guiSlot.SlotType.PROGRESS;
			slot.Drag = guiSlot.DragType.NONE;
			slot.Data = i;
			slot.Worker = worker;
			slot.OnSlotClick = OnWorkerClick;

		}

		//Vekala panelis
		ShopPanel.ShopData = CurrentBuilding.ShopList;


		//Pievienojam noliktavas paneļus
		InventoryPanel.Inventory = CurrentBuilding.Inventory;

		//Iezīmētais darbs
		SelectedWorker.Worker = null;
		SelectedWorker.ListID = -1;
		SelectedWorker.gameObject.SetActive(false);

	}

	public void AddLaborer() {
		if (!SelectedWorker.gameObject.activeSelf) return;
		if (SelectedWorker.Worker == null) return;
		if (CurrentBuilding == null) return;
		if (CurrentBuilding.Laborers.Count == 0) return;
		
		LaborerObject laborer = CurrentBuilding.Laborers.GetLaborer();
		if (laborer == null) return;

		SelectedWorker.Worker.Laborers.Add(laborer);
		SelectedWorker.Redraw();

		ActiveLaborers.RemoveLabor();
	}

	void OnWorkerClick(guiSlot slot) {
		SelectedWorker.Worker = CurrentBuilding.ActiveWorks.List[(int)slot.Data];
		SelectedWorker.ListID = (int)slot.Data;
		SelectedWorker.Redraw();
		SelectedWorker.gameObject.SetActive(true);
	}


	public void OnEndDrag() {
		if (GUImain.DragObject == null) return;
		if (!SelectedWorker.gameObject.activeSelf) return;

		if (SelectedWorker.Worker != null) {
			if (SelectedWorker.Worker.Resource.ID == GUImain.DragObject.Resource.id) return;

			//foreach (int i in SelectedWorker.Worker.AssignLaborers) {

			//}

		} else {
			//Debug.Log("Drag");
			ResourceWorker worker = new ResourceWorker(GUImain.DragObject.Resource.id);
			CurrentBuilding.ActiveWorks.List[SelectedWorker.ListID] = worker;
			SelectedWorker.Worker = worker;
			SelectedWorker.Redraw();

			guiSlot slot = ActiveWorks.GetItem<guiSlot>(SelectedWorker.ListID);
			slot.Icon.sprite = worker.Resource.Image;

		}

	}

}
