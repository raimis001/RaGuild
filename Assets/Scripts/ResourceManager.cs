using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
	using UnityEditor;
#endif

public class ResourceData {
	public int id;
	public int amount = 0;

	public Sprite Image {
		get { return ResourceManager.GetResourceSprite(id); }
	}

	public ResourceData(int item_id = -1, int amount = 0) {
		this.id = item_id;
		this.amount = amount;
	}
}

public class ResourceList {
	public List<ResourceData> List = new List<ResourceData>();

	public int Count {
		get { return List.Count; }
	}

	public void Add(ResourceData data) {
		List.Add(data);
	}
	public void Add(int item_id, int amount) {
		List.Add(new ResourceData(item_id, amount));
	}
	public void Add() {
		List.Add(new ResourceData());
	}

	public ResourceData Union(ResourceData data) {
		ResourceData res = GetData(data.id);
		if (res == null) {
			List.Add(data);
			return data;
		}

		foreach (ResourceData r in List) {
			if (r.id < 0 || r.amount <= 0) {
				r.id = data.id;
				r.amount = data.amount;
				return r;
			}
		}

		res.amount += data.amount;
		
		return res;
	}
	public ResourceData Union(int item_id, int amount) {


		return Union(new ResourceData(item_id, amount));
	}

	public ResourceData GetData(int item_id) {
		ResourceData result = null;
		foreach (ResourceData data in List) {
			if (data.id == item_id) {
				result = data;
				break;
			}
		}
		return result;
	}

	public int GetAmount(int item_id) {
		ResourceData data = GetData(item_id);
		if (data == null) return 0;

		return data.amount;
	}
}

public class ResourceInvenotory {
	public List<ResourceData> List = new List<ResourceData>();

	public int MaxSlots = 6;
	public int MaxAmounts = 30;

	public ResourceInvenotory(int slots, int amounts) {
		MaxSlots = slots;
		MaxAmounts = amounts;
		for (int i = 0; i < MaxSlots; i++) {
			List.Add(new ResourceData(-1, 0));
		}
	}

	public int Amount(int item_id) {
		int amt = 0;

		foreach (ResourceData data in List) {
			if (data == null || data.id != item_id)  continue;
			amt += data.amount;
		}

		return amt;
	}

	public bool Check(int item_id, int amount) {
		int amt = amount;

		foreach (ResourceData data in List) {
			if (data == null || data.id != item_id) continue;
			
			if (data.amount >= amt) return true;

			amt = amt - data.amount;
		}

		return false;
	}

	public int Remove(int item_id, int amount) {
		int amt = amount;

		foreach (ResourceData data in List) {
			if (data.id != item_id) continue;

			if (data.amount >= amt) {
				data.amount -= amt;
				if (data.amount < 1) data.id = -1;
				return 0;
			}

			amt = amt - data.amount;
			data.amount = 0;
			data.id = -1;
		}		
		return amt;
	}

	public int Add(int item_id, int amount) {

		int amt = amount;
		foreach (ResourceData data in List) {
			if (data.id != item_id) continue;
			if (data.amount >= MaxAmounts) continue;

			data.amount += amt;
			if (data.amount <= MaxAmounts) return 0;

			amt = data.amount - MaxAmounts;
			data.amount = MaxAmounts;

		}

		foreach (ResourceData data in List) {
			if (data.id > -1) continue;

			data.id = item_id;
			data.amount = amt;
			if (data.amount <= MaxAmounts) return 0;

			amt = data.amount - MaxAmounts;
			data.amount = MaxAmounts;
		}

		return amt;
	}

}

public enum ResourceTypes {
	RESOURCE, 
	BUILDING,
	CAR
}

public class ResourceObject  {

	public ResourceTypes Type = ResourceTypes.RESOURCE;
	public int ID;
	public string Name;
	public string Desciption;
	public string SpriteName;
	public int BasePrice;
	public float Time;
	public int Amount;

	public ResourceList Request = new ResourceList();
	public ResourceList Available = new ResourceList();

	public Sprite Image {
		get { return ResourceManager.GetResourceSprite(SpriteName); }
	}

	public ResourceObject(int id) {
		ID = id;
	}

	public void AddRequest(int item_id, int amount) {
		Request.Add(item_id, amount);
	}

}

public class ResourceWorker {
	
	public ResourceObject Resource;
	public LaborersList Laborers = new LaborersList();
	public float CurrentTime = 0;
	public float Progress {
		get {
			if (Resource == null || !Active) return 0.01f;
			float r = 1 - CurrentTime / Resource.Time;
			if (r == 0) r = 0.01f;
			return r;
		}
	}
	public bool Active = false;

	public ResourceWorker(int item_id) {
		Resource = ResourceManager.GetResoucreObject(item_id);
		if (Resource == null) return;
	}

	public void StartWork(Building building) { 
		if (Resource == null) return;

		building.StartCoroutine(UpdateWork(building));
	}

	IEnumerator UpdateWork(Building building) {
		Active = true;
		CurrentTime = Resource.Time;

		float konf = (((1.9f - (float)(Laborers.Count - 1) * 0.1f) / 2f) * (float)Laborers.Count);
		while (CurrentTime > 0) {
			CurrentTime -= Time.smoothDeltaTime * konf;
			yield return null;
		}

		building.EndWorker(this);

		Active = false;
	}
}

public class WorkersList {
	public List<ResourceWorker> List = new List<ResourceWorker>();

	public void Add(ResourceWorker worker) {
		List.Add(worker);
	}

	public void CreateWorker(int item_id) {
		List.Add(new ResourceWorker(item_id));
	}
}

public class LaborerObject {
	public int ID;
	public string Name;

	public LaborerObject(int laborer_id) {
		ID = laborer_id;
		Name = "Dzeks " + ID.ToString();
	}
}

public class LaborersList {
	public List<LaborerObject> List = new List<LaborerObject>();

	public void Add(int laborer) {
		List.Add(new LaborerObject(laborer));
	}
	public void Add(LaborerObject laborer) {
		List.Add(laborer);
	}
	public int Count {
		get { return List.Count; }
	}
	public LaborerObject GetLaborer() {
		if (List.Count == 0) return null;
		LaborerObject laborer = List[0];
		List.RemoveAt(0);
		return laborer;
	}
}

public class TransportObject {
	static int transportID = 0;

	public int ID;

	Vector3 from;
	Vector3 target;
	Vector3 current;

	public bool reverse = false;

	public float Speed = 0.03f;
	public int Amount = 50;

	public Building Source;
	public Building Destination;

	public bool Delivered = false;

	public List<ResourceData> Slots = new List<ResourceData>();

	bool Active = false;

	public TransportObject(int slots, int amountMax) {
		ID = transportID++;

		Amount = amountMax;
		for (int i = 0; i < slots; i++) {
			ResourceData data = new ResourceData();
			Slots.Add(data);
		}

	}

	public float Progress {
		get {
			if (!Active) return 0.01f;

			float all = Vector3.Distance(from, target);
			float dist = Vector3.Distance(current, target);
			//Debug.Log("All:" + all.ToString("0.00") + " dist:" + dist.ToString("0.00"));
			return 1 - dist / all;
		}
	}

	public void CheckStart() {
		Active = false;
		if (Source == null || Destination == null) return;

		bool active = false;
		for (int i = 0; i < Slots.Count; i++) {
			if (Slots[i].id > -1) {
				active = true;
				break;
			}
		}

		if (!active) return;

		from = Source.gameObject.transform.position;
		target = Destination.gameObject.transform.position;
		current = from;

		Active = true;
		LoadCargo();
		Delivered = true;
	}

	public void LoadCargo() {
		for (int i = 0; i < Slots.Count; i++) {
			if (Slots[i].id < 0) continue;

			int amt = Amount - Slots[i].amount;
			Slots[i].amount += amt - Source.Inventory.Remove(Slots[i].id, amt);
		}
	}

	public void UnloadCargo() {
		for (int i = 0; i < Slots.Count; i++) {
			if (Slots[i].id < 0) continue;

			Slots[i].amount = Destination.Inventory.Add(Slots[i].id, Slots[i].amount);

		}

	}

	public void Update() {
		if (!Active) {
			CheckStart();
			return;
		}

		Vector3 a = reverse ? current - target :  target - current;
		float magnitude = a.magnitude;
		current += a / magnitude * Speed;
		if ((!reverse && Progress >= 0.99f) || (reverse && Progress <= 0.01f)) {

			if (!reverse) {
				UnloadCargo();
			} else {
				LoadCargo();
			}

			Delivered = true;
			reverse = !reverse;
		}
	}
}

#if UNITY_EDITOR
	[InitializeOnLoad]
#endif
public class ResourceManager {

	public static Dictionary<int, ResourceObject> ResourcesList = new Dictionary<int, ResourceObject>();
	public static Dictionary<string, Sprite> ResourceSprites = new Dictionary<string,Sprite>();

	public static Sprite ErrorSprite;
	public static Sprite BlankSprite;


	public delegate void ResourceChange();
	public static event ResourceChange OnMoneyChange;

	static int _money = 500;
	public static int Money {
		get { return _money; }
		set {
			_money = value;
			if (OnMoneyChange != null) OnMoneyChange();
		}
	}

	public static void Create() {

		Sprite[] sprites = Resources.LoadAll<Sprite>("Icons");

		foreach (Sprite sp in sprites) {
			ResourceSprites.Add(sp.name, sp);
			//Debug.Log(sp.name);
		}

		ErrorSprite = GetResourceSprite("signs_stop");
		BlankSprite = GetResourceSprite("misc folder2");

		ResourceObject obj;

#region ITEMS
		obj = new ResourceObject(1);
			obj.Name = "Wheat";
			obj.Desciption = "Wheat";
			obj.BasePrice = 1;
			obj.Time = 5;
			obj.Amount = 3;
			obj.SpriteName = "food_corn";
		ResourcesList.Add(obj.ID, obj);

		obj = new ResourceObject(2);
			obj.Name = "Wheat flour";
			obj.Desciption = "Wheat flour";
			obj.BasePrice = 1;
			obj.Time = 5;
			obj.Amount = 3;
			obj.SpriteName = "flour_wheat";
			obj.Request.Add(1,5);
		ResourcesList.Add(obj.ID, obj);

		obj = new ResourceObject(3);
			obj.Name = "Wheat bread";
			obj.Desciption = "Wheat bread";
			obj.BasePrice = 1;
			obj.Time = 5;
			obj.Amount = 1;
			obj.SpriteName = "bread_5";
			obj.Request.Add(2, 5);
		ResourcesList.Add(obj.ID, obj);
#endregion

#region BUILDINGS
		obj = new ResourceObject(1000);
			obj.Type = ResourceTypes.BUILDING;
			obj.Name = "Farm";
			obj.Desciption = "Farm";
			obj.BasePrice = 1;
			obj.SpriteName = "BuildingIcons_74";
			obj.Available.Add(1, 0);
		ResourcesList.Add(obj.ID, obj);

		obj = new ResourceObject(1001);
			obj.Type = ResourceTypes.BUILDING;
			obj.Name = "Garage";
			obj.Desciption = "Garage";
			obj.BasePrice = 1;
			obj.SpriteName = "BuildingIcons_76";
		ResourcesList.Add(obj.ID, obj);

		obj = new ResourceObject(1002);
			obj.Type = ResourceTypes.BUILDING;
			obj.Name = "Mill";
			obj.Desciption = "Mill";
			obj.BasePrice = 1;
			obj.SpriteName = "mill";
			obj.Available.Add(2,0);
		ResourcesList.Add(obj.ID, obj);

		obj = new ResourceObject(1003);
			obj.Type = ResourceTypes.BUILDING;
			obj.Name = "Bakery";
			obj.Desciption = "Bakery";
			obj.BasePrice = 1;
			obj.SpriteName = "bakery";
			obj.Available.Add(3, 0);
		ResourcesList.Add(obj.ID, obj);
#endregion

#region CARS
		obj = new ResourceObject(2001);
			obj.Type = ResourceTypes.CAR;
			obj.Name = "Car";
			obj.Desciption = "Car";
			obj.BasePrice = 1;
			obj.SpriteName = "TruckOris_0";
		ResourcesList.Add(obj.ID, obj);
#endregion

		Debug.Log("Resources created!");
	}

	public static Sprite GetResourceSprite(int item_id) {
		if (item_id < 0) return BlankSprite;

		ResourceObject res = GetResoucreObject(item_id);
		return res != null ? res.Image : ErrorSprite;
	}

	public static Sprite GetResourceSprite(string spriteName) {
		//Debug.Log(spriteName);
		Sprite sprite = null;
		ResourceSprites.TryGetValue(spriteName, out sprite);
		if (sprite == null) sprite = ErrorSprite;
		return sprite;
	}

	public static ResourceObject GetResoucreObject(int id) {
		ResourceObject res = null;
		ResourcesList.TryGetValue(id, out res);
		return res;
	}

	static ResourceManager() {
		Create();
	}

}