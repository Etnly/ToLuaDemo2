using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class AppFacade
{
	private static AppFacade _instance;

	public static AppFacade Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = new AppFacade();
			}

			return _instance;
		}
	}
}