using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteamGameFinder : MonoBehaviour
{
	public static SteamGameFinder inst;

	public bool SearchInProgress() { return searchingForGames; }

	private bool searchingForGames = false;
	private List<SteamGame> games = new List<SteamGame>();

	public void Host()
	{

	}

	public void Search()
	{

	}

	public List<SteamGame> GetGames()
	{
		return games;
	}

	public void Join(SteamGame game)
	{
		Debug.Assert(games.Contains(game), "Invites not supported, where did you get this from?");

	}


	void Awake()
    {
		inst = this;   
    }

    void Update()
    {
        
    }
}
