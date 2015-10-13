using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragClass {
	
	public ResourceData Resource = null;
	public object Data = null;
	public float Scale = 1;

	public DragClass(ResourceData res, object data = null) {
		Resource = res;
		Data = data;
	}
	public DragClass(int item_id, object data = null, float scale = 1) {
		Resource = new ResourceData(item_id,1);
		Data = data;
		Scale = scale;
	}

}

public class GUImain : MonoBehaviour {

	public static DragClass DragObject = null;
	public delegate void CursorClick();
	public static event CursorClick OnCursorClick;


	public GameObject BuildingPanel;
	public GameObject TransportPanel;
	public Text MoneyText;

	public Transform Cursor;

	void OnEnable() {
		ResourceManager.OnMoneyChange += OnMoneyChange;
	}
	void OnDisable() {
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
				Cursor.gameObject.GetComponent<Image>().sprite = DragObject.Resource.Image;
				Cursor.localScale = new Vector3(DragObject.Scale,DragObject.Scale,1);
				Cursor.gameObject.SetActive(true);
			}

			Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			pos.z = -1;
			Cursor.position = Input.mousePosition;

			if (Input.GetMouseButtonUp(0) && !EventSystem.current.IsPointerOverGameObject()) {
				if (OnCursorClick != null) OnCursorClick();
			}

			if (Input.GetMouseButtonUp(1)) {
				CancelSelect();
			}

		} else if (Cursor.gameObject.activeSelf) {
			Cursor.gameObject.SetActive(false);
		}

		MoneyText.text = ResourceManager.Money.ToString();
	}

	void OnMoneyChange() {
		MoneyText.text = ResourceManager.Money.ToString();
	}

	public void CancelSelect() {
		DragObject = null;
	}

	public void StartRoad() {
		DragObject = new DragClass(5001, null, 0.5f);
	}

}
