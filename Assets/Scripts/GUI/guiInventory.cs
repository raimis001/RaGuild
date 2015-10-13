using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class guiInventory : MonoBehaviour {

	public Transform InventoryPanel;

	ResourceInvenotory _inventory = null;
	public ResourceInvenotory Inventory {
		get { return _inventory; }
		set {
			_inventory = value;
			while (InventoryPanel.childCount > 0) {
				GameObject.DestroyImmediate(InventoryPanel.GetChild(0).gameObject);
			}
			Slots.Clear();

			if (_inventory == null) return;

			for (int i = 0; i < _inventory.List.Count; i++) {
				ResourceData data = _inventory.List[i];

				guiSlot slot = guiSlot.Create(InventoryPanel, data, guiSlot.SlotType.TEXT, guiSlot.DragType.NONE, i);

				slot.Icon.enabled = (data.id > -1 && data.amount > 0);
				slot.AddonText.text = data.amount > 0 ? data.amount.ToString() : "";

				Slots.Add(slot);
			}

		}
	}

	List<guiSlot> Slots = new List<guiSlot>();

	// Use this for initialization
	void Start () {

	}

	
	void OnEnable() {
		Building.OnResourceChange += OnResourceChange;
	}

	void OnDisable() {
		Building.OnResourceChange -= OnResourceChange;
	}

	// Update is called once per frame
	void Update () {
	
	}

	void OnResourceChange() {
		if (Inventory == null) return;
		for (int i = 0; i < _inventory.List.Count; i++) {
			ResourceData data = _inventory.List[i];

			guiSlot slot = Slots[i];

			slot.Icon.enabled = (data.id > -1 && data.amount > 0);
			if (slot.Icon.enabled) slot.Icon.sprite = data.Image;
			slot.AddonText.text = data.amount > 0 ? data.amount.ToString() : "";
		}
	}
}
