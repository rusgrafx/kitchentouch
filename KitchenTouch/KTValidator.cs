using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

public class KTValidator
{
	/// <summary>
	/// Validates the US Zip code (XXXXX or XXXXX-XXXX)
	/// </summary>
	/// <param name="value">value</param>
	/// <returns>True if Zip code is valid, false otherwise</returns>
	public bool IsZIP(object value)
	{
		if (Regex.IsMatch((String)value, @"^\s*(\d{5}|(\d{5}-\d{4}))\s*$"))
		{
			return true;
		}
		else
		{
			return false;
		}
	}

	/// <summary>
	/// Validates Boolean value
	/// </summary>
	/// <param name="value">value</param>
	/// <returns>True if value is of type bool, false otherwise</returns>
	public bool IsBool(object value)
	{
		if (value.GetType() == typeof(bool))
		{
			return true;
		}
		else
		{
			return false;
		}
	}

	/// <summary>
	/// Validates path to local folder
	/// </summary>
	/// <param name="value">value</param>
	/// <returns>True if folder exists, false otherwise</returns>
	public bool IsLocalPath(object value)
	{
		if (Directory.Exists((String)value))
		{
			return true;
		}
		else
		{
			return false;
		}
	}

	/// <summary>
	/// Validates HTTP or HTTPS URLs
	/// </summary>
	/// <param name="value">value</param>
	/// <returns>True if valid URL, false otherwise</returns>
	public bool IsHttpUrl(object value)
	{
		if ( Regex.IsMatch((String)value, @"http(s)?://([\w-]+\.)+[\w-]+(/[\w- ./?%&=]*)?", RegexOptions.Compiled))
		{
			return true;
		}
		else
		{
			return false;
		}
	}

	public bool IsEmail(object value)
	{
		if ( Regex.IsMatch((String)value, @"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$", RegexOptions.Compiled | RegexOptions.Singleline) )
		{
			return true;
		}
		else
		{
			return false;
		}
	}

	/// <summary>
	/// Validates that a number falls within a specific range
	/// </summary>
	/// <param name="value">value</param>
	/// <returns>True if value is within the range, false otherwise</returns>
	public bool IsIntRange(object value, int min, int max)
	{
		if (value.GetType() != typeof(int))
		{
			return false;
		}
		if ((int)value > max || (int)value < min)
		{
			return false;
		}
		return true;
	}
}
