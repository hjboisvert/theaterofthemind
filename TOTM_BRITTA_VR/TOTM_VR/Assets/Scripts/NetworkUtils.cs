using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

public class NetworkUtils : MonoBehaviour
{
    public string GetBroadcastIP()     // Figure out LAN broadcast address (for RTCP sync messages back to BROADCAST)
	{
		string broadcastIP = "";
		// Sets the last octet of ethernet gateway address to 255. 
		Debug.Log("Determining LAN broadcast address");
		NetworkInterface[] interfaces = NetworkInterface.GetAllNetworkInterfaces();
		foreach (NetworkInterface adapter in interfaces)
		{
			if (adapter.OperationalStatus == OperationalStatus.Up && adapter.NetworkInterfaceType.ToString().Contains("Ethernet"))
			{
				if (adapter.Supports(NetworkInterfaceComponent.IPv4) == false) // skip if no ip4 
				{
					continue;
				}
				IPInterfaceProperties properties = adapter.GetIPProperties();
				IPv4InterfaceProperties ip4Properties = properties.GetIPv4Properties();
				Debug.Log("Gateway Addresses");
				foreach (GatewayIPAddressInformation entry in adapter.GetIPProperties().GatewayAddresses)
				{
					Debug.Log(entry.Address.ToString());
					string[] octets = entry.Address.ToString().Split('.');
					octets[3] = "255";
					broadcastIP = string.Join(".", octets);
					string ipMsg = "Setting Broadcast IP: " + broadcastIP;
					Debug.Log(ipMsg);
					//break;
				}
			}
		}
		if (broadcastIP.Length == 0)
		{
			Debug.Log("ERR NO BROADCAST IP");
		}
		return broadcastIP;
	}
}
