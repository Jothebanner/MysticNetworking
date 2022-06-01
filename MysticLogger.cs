using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MysticLogger
{
	public delegate void CustomLogger(string log);
	public delegate void CustomExceptionLogger(Exception exception);

	static CustomLogger logger;
	static CustomExceptionLogger eLogger;
	public static void SetLoggingFunction(CustomLogger _logger)
	{
		logger = _logger;
	}
	
	public static void SetExceptionLoggingFunction(CustomExceptionLogger _logger)
	{
		eLogger = _logger;
	}

	public static void Log(string message)
	{
		logger(message);
	}
	
	public static void Log(int message)
	{
		logger(message.ToString());
	}

	public static void Log(Exception exception)
	{
		eLogger(exception);
	}

}
