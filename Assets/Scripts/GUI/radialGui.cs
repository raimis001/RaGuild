using UnityEngine;
using System.Collections;

public class radialGui : MonoBehaviour {

	public GameObject BuildMenu;
	public GameObject FarmMenu;
	public GameObject WoodMenu;
	public GameObject CarMenu;
	public GameObject RoadMenu;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void SwitchBuildMenu() {
		BuildMenu.SetActive(!BuildMenu.activeSelf);
		if (BuildMenu.activeSelf) return;

		FarmMenu.SetActive(false);
		WoodMenu.SetActive(false);
		CarMenu.SetActive(false);
		RoadMenu.SetActive(false);
	}

	public void SwitchFarmMenu() {
		WoodMenu.SetActive(false);
		CarMenu.SetActive(false);
		RoadMenu.SetActive(false);
		FarmMenu.SetActive(!FarmMenu.activeSelf);
	}

	public void SwitchWoodMenu() {
		FarmMenu.SetActive(false);
		CarMenu.SetActive(false);
		RoadMenu.SetActive(false);
		WoodMenu.SetActive(!WoodMenu.activeSelf);
	}

	public void SwitchCarMenu() {
		FarmMenu.SetActive(false);
		WoodMenu.SetActive(false);
		RoadMenu.SetActive(false);
		CarMenu.SetActive(!CarMenu.activeSelf);
	}

	public void SwitchRoadMenu() {
		FarmMenu.SetActive(false);
		WoodMenu.SetActive(false);
		CarMenu.SetActive(false);
		RoadMenu.SetActive(!RoadMenu.activeSelf);
	}

}
