using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class guiLaborers : MonoBehaviour {

	public RectTransform LaborersList;
	public GameObject LaborerPrefab;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void AddLaborers(LaborersList laborers) {
		Clear();
		foreach (LaborerObject laborer in laborers.List) {
			GameObject obj = Instantiate(LaborerPrefab);
				obj.transform.SetParent(LaborersList);
				obj.transform.localScale = Vector3.one;
				obj.name = laborer.Name;
		}

	}

	public void RemoveLabor() {
		if (LaborersList.childCount > 0) { 
			GameObject.DestroyImmediate(LaborersList.GetChild(0).gameObject);
		}

	}
	public void Clear() {
		while (LaborersList.childCount > 0) {
			GameObject.DestroyImmediate(LaborersList.GetChild(0).gameObject);
		}
	}
	public void Add(LaborerObject laborer) {
		GameObject obj = Instantiate(LaborerPrefab);
			obj.transform.SetParent(LaborersList);
			obj.transform.localScale = Vector3.one;
			obj.name = laborer.Name;
	}
}
