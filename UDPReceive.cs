using UnityEngine;
using System.Collections;

using System;
using System.IO;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

public class UDPReceive : MonoBehaviour {
	private volatile bool _stopThread;
	private Thread receiveThread;

	// udpclient object
	UdpClient client;

	//GUI display and data storage vars
	private Int32 frame;
	private Vector3 Tran;
	private Vector3 Rot;

	// public input port
	public int port = 51001; // define > init

	// start from unity3d
	public void Start()
	{
		_stopThread = false;
		//ReceiveData ();
		receiveThread = new Thread(new ThreadStart(ReceiveData));
		receiveThread.Start();
	}

	//set object transform every frame
	public void LateUpdate()
	{
		//convert tracker mm to unity m and Zup to Yup
		transform.position = new Vector3(Tran.x / 1000, Tran.z / 1000, Tran.y / 1000);

    //convert tracker Zup RH system to unity Yup LH system

    transform.rotation = Quaternion.AngleAxis(-Rot.x, Vector3.right);
    transform.rotation = Quaternion.AngleAxis(-Rot.y, transform.forward) * transform.rotation;
    transform.rotation = Quaternion.AngleAxis(-Rot.z, transform.up) * transform.rotation;
  }

    /*//OnGUI show data values
    void OnGUI()
	{
		//show the port number to the user
		GUI.Label(new Rect(10,10,400,40),"Trans: " + Tran.ToString() + " Rot: " + Rot.ToString());
		//show the frame number to the user
		GUI.Label(new Rect(10,40,100,40),"Frame: " + frame);

	}*/

    public void OnDestroy()
    {
        _stopThread = true;
    }

    // receive thread
    private void ReceiveData()
	{
		IPEndPoint localpt = new IPEndPoint(IPAddress.Any, port);
		// create a new upd client on the specified port
		client = new UdpClient (localpt);

		// if no data is recieve for one second the socket will throw an excption
		client.Client.ReceiveTimeout = 1000;

		client.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
		//_stopThread is a vloatile bool that can be set by the main thread to teminate gracefully
		while (!_stopThread)
		{
			//the recieve call is enclose in a try catch to handle the timeout exception
			try
			{
				// create an ip to recieve with
				IPEndPoint anyIP = new IPEndPoint(IPAddress.Any, 0);
				//atually receive some data in the a bype array
				byte[] data = client.Receive(ref anyIP);
				//use a memmory stream to read the data out in to usefull vaiables
				using (MemoryStream memory = new MemoryStream(data))
				{
					// Use the memory stream in a binary reader.
					using (BinaryReader reader = new BinaryReader(memory))
					{
						frame = reader.ReadInt32();
						byte items = reader.ReadByte();
						byte id = reader.ReadByte();
						short size = reader.ReadInt16();
						char[] name = reader.ReadChars(24);
						Tran.x = (float) reader.ReadDouble();
						Tran.y = (float) reader.ReadDouble();
						Tran.z = (float) reader.ReadDouble();
						//convert rotation radians to degrees
						Rot.x = (float) (reader.ReadDouble()/Math.PI)*180;
						Rot.y = (float) (reader.ReadDouble()/Math.PI)*180;
						Rot.z = (float) (reader.ReadDouble()/Math.PI)*180;

						//Debug.Log(String.Format(" {1}, {2} ", Tran, Rot));

					}
				}
			}
			catch (Exception err)
			{
				print(err.ToString());
			}
		}
		client.Close();
		Debug.Log ("Finished recieve");
	}
}

