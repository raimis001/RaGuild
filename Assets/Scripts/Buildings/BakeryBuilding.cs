using UnityEngine;
using System.Collections;

public class BakeryBuilding : Building {

	protected override void Init() {
		SlotCount = 1;

		MaxInventorySlots = 6;
		MaxInventorySize = 30;

		MaxLaborers = 5;

		ShopCount = 6;

		resource_id = 1003;

	}
}
