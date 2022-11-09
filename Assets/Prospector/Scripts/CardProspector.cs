using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//The CardState cariable type has one of four values: drawpile, tableau, target, & discard
public enum CardState
{
	drawpile,
	tableau,
	target,
	discard
}

public class CardProspector : Card { 
	public CardState state = CardState.drawpile;

	
	public List<CardProspector> hiddenBy = new List<CardProspector>();

	//LayoutID matches this card to a Layout XML id if it's a tableau card
	public int layoutID;

	
	public SlotDef slotDef;

	//This allows the card to react to being clicked
	override public void OnMouseUpAsButton()
	{
		
		Prospector.S.CardClicked(this);

		
		base.OnMouseUpAsButton();
	}
}
