using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;


public enum FSState
{
	idle,
	pre,
	active,
	post
}

//FloatingScore can move itself on screen following a Bezier curve
public class FloatingScore : MonoBehaviour
{
	public FSState state = FSState.idle;

	[SerializeField]
	private int _score = 0; //The score field
	public string scoreString;

	//The score property also sets scoreString when set
	public int score
	{
		get
		{
			return(_score);
		}
		set
		{
			_score = value;
			scoreString = Utils.AddCommasToNumber(_score);
			GetComponent<Text>().text = scoreString;
		}
	}

	public List<Vector3> bezierPts; //Bezier points for movement
	public List<float> fontSizes; //Bezier points for font scaling
	public float timeStart = -1f;
	public float timeDuration = 1f;
	public string easingCurve = Easing.InOut; //Uses Easing in Utils.cs

	//The GameObject that will receive the sendMessage when this is done moving
	public GameObject reportFinishTo = null;

	public void Init(List<Vector3> ePts, float eTimeS = 0, float eTimeD = 1)
	{
		bezierPts = new List<Vector3>(ePts);

		if (ePts.Count == 1) //If there's only one point
		{
			//...then just go there
			transform.position = ePts[0];
			return;
		}

		if (eTimeS == 0)
		{
			eTimeS = Time.time;
		}

		timeStart = eTimeS;
		timeDuration = eTimeD;

		state = FSState.pre; //Set it to the pre state, ready to start moving
	}

	public void FSCallback (FloatingScore fs)
	{
	
		score += fs.score;
	}
	
	// Update is called once per frame
	void Update()
	{
		//If this is not moving, just return
		if (state == FSState.idle)
		{
			return;
		}

		
		float u = (Time.time - timeStart)/timeDuration;

		//Use easing class from Utils to curve the u value
		float uC = Easing.Ease (u, easingCurve);

		if (u < 0) //If u < 0, then we shouldn't move yet
		{
			state = FSState.pre;

			//Move to the initial point
			transform.position = bezierPts[0];
		}
		else
		{
			if (u >= 1) //If u >= 1, we're done moving
			{
				uC = 1; //Set uC = 1 so we don't overshoot
				state = FSState.post;
				if (reportFinishTo != null) //If there's a callback GameObject
				{
					reportFinishTo.SendMessage("FSCallback", this);
					Destroy(gameObject);
				}
				else //If there is nothing to callback
				{
					//then don't destroy this. Just let it stay still
					state = FSState.idle;
				}
			}
			else
			{
				
				state = FSState.active;
			}
			//Use bezier curve to move this to the right point
			Vector3 pos = Utils.Bezier(uC, bezierPts);
			transform.position = pos;
			if (fontSizes != null && fontSizes.Count > 0)
			{
				
				int size = Mathf.RoundToInt(Utils.Bezier(uC, fontSizes));
				GetComponent<Text>().fontSize = size;
			}
		}
	}
}
