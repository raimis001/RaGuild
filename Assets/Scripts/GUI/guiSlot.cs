using UnityEngine;
using System.Collections;
using UnityEngine.UI;


public class guiSlot : MonoBehaviour {

	public enum SlotType {
		NORMAL,
		PROGRESS,
		TEXT
	};

	public enum DragType {
		NONE,
		DRAG,
		DROP
	}

	public GameObject AddonPanel;
	public Slider Progress;
	public Text AddonText;
	public Image Icon;

	[HideInInspector]
	public object Data = null;

	ResourceData _worker = null;
	public ResourceData Worker {
		get { return _worker; }
		set {
			_worker = value;
			Icon.sprite = _worker == null ? ResourceManager.ErrorSprite : _worker.Image;
		}
	}

	SlotType _type = SlotType.NORMAL;
	public SlotType Type {
		get { return _type; }
		set {
			_type = value;
			LayoutElement element = gameObject.GetComponent<LayoutElement>();
			switch (_type) {
				case SlotType.NORMAL:
					AddonPanel.SetActive(false);
					element.preferredHeight = 45f;
					break;
				case SlotType.PROGRESS:
					AddonPanel.SetActive(true);
					Progress.gameObject.SetActive(true);
					AddonText.gameObject.SetActive(false);
					element.preferredHeight = 60;
					break;
				case SlotType.TEXT:
					AddonPanel.SetActive(true);
					Progress.gameObject.SetActive(false);
					AddonText.gameObject.SetActive(true);
					element.preferredHeight = 60;
					break;
			}
		}
	}

	[HideInInspector]
	public DragType Drag = DragType.NONE;

	public delegate void SlotClick(guiSlot slot);
	public SlotClick OnSlotClick;
	public SlotClick OnSlotEndDrag;


	public static guiSlot Create(Transform parent, ResourceData worker, SlotType type, DragType drag, object data = null) {

		GameObject obj =  Instantiate<GameObject>(Resources.Load<GameObject>("Slot"));
		obj.transform.SetParent(parent);
		obj.transform.localScale = Vector3.one;

		guiSlot slot = obj.GetComponent<guiSlot>();

		slot.Worker = worker;
		slot.Type = type;
		slot.Drag = drag;
		slot.Data = data;

		return slot;
	}

	void Awake() {
		Type = _type;
		Icon.sprite = _worker == null ? ResourceManager.ErrorSprite : _worker.Image;
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void BeginDrag() {
		if (Drag != DragType.DRAG) return;
		GUImain.DragObject = new DragClass(Worker, Data);
	}

	public void OnMouseClick() {
		if (OnSlotClick != null) OnSlotClick(this);
	}
	public void OnEndDrag() {
		if (OnSlotEndDrag != null) OnSlotEndDrag(this);
	}
	public override string ToString() {
		return "Slot id:" + _worker.id;
	}

	public void Refresh() {
		Icon.sprite = _worker == null ? ResourceManager.ErrorSprite : _worker.Image;

	}
}
