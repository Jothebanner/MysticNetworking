using System;
using System.Threading;

public static class RandomIdGenerator
{
	// multithreaded psuedo random id generation for secure private id's for new connections // thread safe
	// probably super overkill for a self-hosted lobby, but dope as frick
	public static string GenerateId(int idLength = 6, string availableChars = "abcdefghijklmnopqurstuvwxyz0123456789ABCDEFGHIJKLMNOPQURSTUVWXYZ")
	{
		string id = "";

		for (int i = 0; i < idLength; i++)
		{
			// based off of guid to get a random number
			var random = new ThreadLocal<System.Random>(() => new System.Random(Guid.NewGuid().GetHashCode()));
			id += availableChars[random.Value.Next(0, availableChars.Length)];
		}

		return id;
	}

}
