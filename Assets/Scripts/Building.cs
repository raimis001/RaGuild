using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Building : MonoBehaviour {

	public delegate void ResourceChange();
	public static event ResourceChange OnResourceChange;

	public static int BuildID = 0;

	public DialogClass Dialog;
	public string Caption = "Farm field";

	[HideInInspector]
	public int resource_id;
	[HideInInspector]
	public ResourceObject Resource;

	[HideInInspector]
	public int id;

	[HideInInspector]
	public List<int> AwailableWorks = new List<int>();

	[HideInInspector]
	public WorkersList ActiveWorks = new WorkersList();
	protected int SlotCount = 1;

	[HideInInspector]
	public ResourceInvenotory Inventory;
	protected int MaxInventorySlots = 6;
	protected int MaxInventorySize = 30;


	[HideInInspector]
	public LaborersList Laborers = new LaborersList();
	protected int MaxLaborers = 5;

	[HideInInspector]
	public ResourceList ShopList = new ResourceList();
	protected int ShopCount = 0;

	[HideInInspector]
	public float ShopTime = 1;
	[HideInInspector]
	public int ShopBan = 5;
	float _shopTime = 0;

	public CarClass Car;

	virtual protected void Awake() {
	Debug.Log("Building awake");

		id = BuildID++;

		Init();

		Resource = ResourceManager.GetResoucreObject(resource_id);

		Inventory = new ResourceInvenotory(MaxInventorySlots, MaxInventorySize);

		Transform icon = transform.FindChild("Icon");
		if (icon) {
			icon.gameObject.GetComponent<SpriteRenderer>().sprite = Resource.Image;
		}

		AwailableWorks.Add(1);

		for (int i = 0; i < MaxLaborers; i++) {
			Laborers.Add(i);
		}

		for (int i = 0; i < SlotCount; i++) {
			ActiveWorks.Add(null);
		}

		for (int i = 0; i < ShopCount; i++) {
			ShopList.Add(new ResourceData());
		}

		_shopTime = ShopTime;
	}

	virtual protected void Start() {
	}

	virtual protected void Init() {

	}

	bool CheckResource(ResourceWorker worker) {
		if (worker.Resource.Request.Count < 1) return true;

		bool enough = true;

		foreach (ResourceData data in worker.Resource.Request.List) {
			if (data.id < 0) continue;
			if (!Inventory.Check(data.id, data.amount)) {
				enough = false;
				break;
			}
		}

		if (!enough) return false;

		foreach (ResourceData data in worker.Resource.Request.List) {
			if (data.id < 0) continue;
			Inventory.Remove(data.id, data.amount);
		}

		return true;
	}

	virtual protected void Update() {

		foreach (ResourceWorker worker in ActiveWorks.List) {
			if (worker == null) continue;
			if (worker.Active) continue;
			if (worker.Resource == null) continue;
			if (worker.Laborers.Count == 0) continue;

			if (!CheckResource(worker)) continue;
			
			worker.StartWork(this);

		}

		_shopTime -= Time.deltaTime;
		if (_shopTime <= 0) {
			foreach (ResourceData res in ShopList.List) {
				if (res == null || res.id < 0) continue;

				float amount = Inventory.Amount(res.id);
				if (amount < ShopBan) continue; 

				int amt = (amount > 10) ? (int)(amount * 0.1f) : 1;
				Inventory.Remove(res.id, amt);

				ResourceObject obj = ResourceManager.ResourcesList[res.id];
				ResourceManager.Money += obj.BasePrice * amt;

			}

			_shopTime = ShopTime;
		}

	}

	virtual protected void OnMouseDown() {
		//Dialog.SetBuilding(this.gameObject);
		//Dialog.gameObject.SetActive(true);

		if (Car != null) Car.SetCarPath(transform.position);
	}

	virtual public void EndWorker(ResourceWorker worker) {
		Inventory.Add(worker.Resource.ID, worker.Resource.Amount);

		if (OnResourceChange != null) OnResourceChange();

	}
}
