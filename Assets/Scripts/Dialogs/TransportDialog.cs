using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class TransportDialog : DialogClass {

	public guiScroll TransportPanel;
	public guiScroll ResourceList;
	public guiScroll BuildingList;

	public guiSlotList AvalableList;

	public GameObject SelectedTransport;

	public guiSlot SourceSlot;
	public guiSlot DestinationSlot;

	public Slider TransportPosition;
	public Transform TransportImage;

	TransportObject CurrentTransport = null;
	int CurrentSlot = -1;

	// Use this for initialization
	void Start () {


	}

	

	// Update is called once per frame
	void Update () {
		if (CurrentTransport == null) return;

		TransportPosition.value = CurrentTransport.Progress;
		TransportImage.localScale = new Vector3(CurrentTransport.reverse ? -1 : 1, 1, 1);

		guiSlot slot = TransportPanel.GetItem<guiSlot>(CurrentSlot);
		slot.Progress.value = CurrentTransport.Progress;

		if (!CurrentTransport.Delivered) return;
		CurrentTransport.Delivered = false;

		for (int i = 0; i < CurrentTransport.Slots.Count; i++) {
			ResourceData data = CurrentTransport.Slots[i];
			slot = AvalableList.GetItem<guiSlot>(i);
			slot.AddonText.text = data.amount > 0 ? data.amount.ToString() : "";
		}
	}
	public override void SetBuilding(GameObject objectBuild) {

		TransportPanel.ClearItems();
		for (int i = 0; i < TransportBuilding.Transports.Count; i++) {

			GameObject obj = TransportPanel.AddItem();
			guiSlot slot = obj.GetComponent<guiSlot>();
				slot.Worker = new ResourceData(2001);
				slot.Type = guiSlot.SlotType.PROGRESS;
				slot.Drag = guiSlot.DragType.NONE;
				slot.Data = i;
				slot.OnSlotClick = OnTransportClick;
		}


		ResourceList.ClearItems();

		foreach (ResourceObject res in ResourceManager.ResourcesList.Values) {
			if (res.Type != ResourceTypes.RESOURCE) continue;

			GameObject obj = ResourceList.AddItem();
				obj.GetComponent<guiSlot>().Worker = new ResourceData(res.ID, 0);

		}

		
		BuildingList.ClearItems();

		 GameObject[] buildings = GameObject.FindGameObjectsWithTag("building");
		 foreach (GameObject obj in buildings) {
				GameObject created = BuildingList.AddItem();
				Building building = obj.GetComponent<Building>();

				created.GetComponent<guiSlot>().Worker = new ResourceData(building.resource_id);
				created.GetComponent<guiSlot>().Data = building.id;

				//guiBuildingSlot slot = created.GetComponent<guiBuildingSlot>();
					//slot.id = building.id;
		 }

		 CurrentTransport = null;
		 SelectedTransport.SetActive(false);

	}


	void OnTransportClick(guiSlot slot) {
		CurrentSlot = (int)slot.Data;

		guiSlot s = TransportPanel.GetItem<guiSlot>(CurrentSlot);
		Debug.Log(s.ToString());

		CurrentTransport = TransportBuilding.Transports[CurrentSlot];


		AvalableList.ClearItems();
		for (int i = 0; i < CurrentTransport.Slots.Count; i++) {
			ResourceData data = CurrentTransport.Slots[i];
			GameObject obj = AvalableList.AddItem();
			guiSlot slt = obj.GetComponent<guiSlot>();
			slt.Data = i;
			slt.Type = guiSlot.SlotType.TEXT;
			slt.Drag = guiSlot.DragType.DROP;
			slt.Worker = data;
			slt.AddonText.text = data.amount > 0 ? data.amount.ToString() : "";
			slt.OnSlotEndDrag = AvailableEndDrag;
		}

		if (CurrentTransport.Source == null) {
			SourceSlot.Worker = new ResourceData();
		} else {
			SourceSlot.Worker = new ResourceData(CurrentTransport.Source.resource_id);
		}

		if (CurrentTransport.Destination == null) {
			DestinationSlot.Worker = new ResourceData();
		} else {
			DestinationSlot.Worker = new ResourceData(CurrentTransport.Destination.resource_id);
		}



		SelectedTransport.SetActive(true);

		//Debug.Log("Select transport id:" + CurrentTransport.ID);
	}
	void AvailableEndDrag(guiSlot slot) {
		if (GUImain.DragObject == null) return;
		ResourceObject res = ResourceManager.GetResoucreObject(GUImain.DragObject.Resource.id);
		if (res == null || res.Type != ResourceTypes.RESOURCE) return;

		int slotID = (int)slot.Data;
		CurrentTransport.Slots[slotID].id = GUImain.DragObject.Resource.id;
		CurrentTransport.Slots[slotID].amount = 0;

		//slot.Icon.sprite = CurrentTransport.Slots[slotID].Image;
		slot.Worker = CurrentTransport.Slots[slotID];
		slot.AddonText.text = "";

	}
	public void EndDrag(int slotID) {
		if (GUImain.DragObject == null) return;
		ResourceObject res = ResourceManager.GetResoucreObject(GUImain.DragObject.Resource.id);
		if (res == null || res.Type != ResourceTypes.RESOURCE) return;

		CurrentTransport.Slots[slotID].id = GUImain.DragObject.Resource.id;
	}

	public void EndDragSource() {
		if (CurrentTransport == null) return;
		if (GUImain.DragObject == null) return;
		ResourceObject res = ResourceManager.GetResoucreObject(GUImain.DragObject.Resource.id);
		if (res == null || res.Type != ResourceTypes.BUILDING) {
			GUImain.DragObject = null;
			return;
		}

		Debug.Log(GUImain.DragObject.Data);

		GameObject[] buildings = GameObject.FindGameObjectsWithTag("building");
		foreach (GameObject obj in buildings) {
			Building building = obj.GetComponent<Building>();
			if (building.id != (int)GUImain.DragObject.Data) continue;

			SourceSlot.Worker = new ResourceData(GUImain.DragObject.Resource.id);
			SourceSlot.Data = building.id;

			CurrentTransport.Source = building;
			break;
		}
		GUImain.DragObject = null;

	}
	public void EndDragDestination() {
		if (CurrentTransport == null) return;
		if (GUImain.DragObject == null) return;
		ResourceObject res = ResourceManager.GetResoucreObject(GUImain.DragObject.Resource.id);
		if (res == null || res.Type != ResourceTypes.BUILDING) {
			GUImain.DragObject = null;
			return;
		}

		//Debug.Log(GUImain.DragObject.Data);

		GameObject[] buildings = GameObject.FindGameObjectsWithTag("building");
		foreach (GameObject obj in buildings) {
			Building building = obj.GetComponent<Building>();
			if (building.id != (int)GUImain.DragObject.Data) continue;

			DestinationSlot.Worker = new ResourceData(GUImain.DragObject.Resource.id);
			DestinationSlot.Data = building.id;

			CurrentTransport.Destination = building;
			break;
		}
		GUImain.DragObject = null;
	}
}
