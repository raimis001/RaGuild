using UnityEngine;
using System.Collections;

public class guiShop : MonoBehaviour {

	public guiSlotList SlotPanel;

	ResourceList _shopData;
	public ResourceList ShopData {
		set {
			_shopData = value;

			if (_shopData == null || _shopData.Count < 1) {
				gameObject.SetActive(false);
				return;
			}

			SlotPanel.ClearItems();

			for (int i = 0; i < _shopData.Count; i++) {
				
				ResourceData data = _shopData.List[i];
				GameObject obj = SlotPanel.AddItem();
				guiSlot slot = obj.GetComponent<guiSlot>();
				
					slot.Type = guiSlot.SlotType.NORMAL;
					slot.Drag = guiSlot.DragType.DROP;
					slot.Data = i;
					slot.Worker = data;

					slot.OnSlotEndDrag = OnSlotDrop;
			}

			gameObject.SetActive(true);
		}
	}

	void OnSlotDrop(guiSlot slot) {
		if (GUImain.DragObject == null) return;

		//ResourceData data = _shopData.List[(int)slot.Data];
			slot.Worker.id = GUImain.DragObject.Resource.id;
			slot.Refresh();
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
