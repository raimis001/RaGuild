using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TransportBuilding : Building {

	[HideInInspector]
	public static List<TransportObject> Transports = new List<TransportObject>();

	protected override void Init() {
		SlotCount = 1;

		MaxInventorySlots = 0;
		MaxInventorySize = 0;

		MaxLaborers = 5;

		resource_id = 1001;


		TransportObject t;

		t = new TransportObject(1, 50);
		Transports.Add(t);

		t = new TransportObject(2, 30);
		Transports.Add(t);

	}

	protected override void Update() {
		base.Update();

		for (int i = 0; i < Transports.Count; i++) {
			Transports[i].Update();
		}

	}
}
