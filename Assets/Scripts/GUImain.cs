using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragClass {
	
	public ResourceData Resource = null;
	public object Data = null;

	public DragClass(ResourceData res, object data = null) {
		Resource = res;
		Data = data;
	}
}

public class GUImain : MonoBehaviour {

	public static DragClass DragObject = null;

	public GameObject BuildingPanel;
	public GameObject TransportPanel;
	public Text MoneyText;

	public Transform Cursor;

	void Enable() {
		ResourceManager.OnMoneyChange += OnMoneyChange;
	}
	void Disable() {
		ResourceManager.OnMoneyChange -= OnMoneyChange;
	}

	// Use this for initialization
	void Start () {
		BuildingPanel.SetActive(false);
		TransportPanel.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () {
		if (DragObject != null) {
			if (!Cursor.gameObject.activeSelf) {
				Cursor.gameObject.GetComponent<Image>().sprite = GUImain.DragObject.Resource.Image;
				Cursor.gameObject.SetActive(true);
			}

			Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			pos.z = -1;
			Cursor.position = Input.mousePosition;

			if (Input.GetMouseButtonUp(0) || !EventSystem.current.IsPointerOverGameObject()) {
				GUImain.DragObject = null;
				if (Cursor.gameObject.activeSelf) {
					Cursor.gameObject.SetActive(false);
				}
			}
		}
		MoneyText.text = ResourceManager.Money.ToString();
	}

	void OnMoneyChange() {
		MoneyText.text = ResourceManager.Money.ToString();
	}
}
