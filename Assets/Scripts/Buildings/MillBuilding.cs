using UnityEngine;
using System.Collections;

public class MillBuilding : Building {

	protected override void Init() {
		SlotCount = 1;

		MaxInventorySlots = 12;
		MaxInventorySize = 50;

		MaxLaborers = 5;

		ShopCount = 0;

		resource_id = 1002;

	}



}
