using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {

	public Text name, age, location, gender, company;

	private Name _name;
	private AGL _agl;
	private Company _company;

	private void Start() {
		_name = GetComponent<Name>();
		_agl = GameObject.Find("Main Camera").GetComponent<AGL>();
		_company = GameObject.FindGameObjectWithTag("Company") .GetComponent<Company>();

		name.text = "Name: " + _name.name;
		age.text = "Age: " + _agl.age;
		location.text = "Location: " + _agl.location;
		gender.text = "Gender: " + _agl.gender;
		company.text = "Company: " + _company.favoriteCompany;
		
	}
}
