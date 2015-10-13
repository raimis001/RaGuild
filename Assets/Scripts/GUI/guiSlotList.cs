using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class guiSlotList : MonoBehaviour {

	public GameObject Prefab;
	public Transform List;

	List<GameObject> _itemsList = new List<GameObject>();

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void ClearItems() {
		_itemsList.Clear();

		while (List.childCount > 0) {
			GameObject.DestroyImmediate(List.GetChild(0).gameObject);
		}


	}
	public GameObject AddItem(GameObject prefab = null) {
		if (prefab == null) prefab = Prefab;

		GameObject result = Instantiate(prefab);
		result.transform.SetParent(List);
		result.transform.localScale = Vector3.one;

		_itemsList.Add(result);

		return result;
	}

	public T GetItem<T>(int itemID) {
		if (_itemsList[itemID] == null) return default(T);
		return (T)_itemsList[itemID].GetComponent<T>();
	}

}
