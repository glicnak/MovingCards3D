using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "Cards")]
public class Card : ScriptableObject
{
	public new string name;
	public string villain;
	public int attack;
	public int health;
	public int cost;
	public string type;
	public string description;

	public string artworkName;
	
	public Abilities abilities;
}
