using UnityEngine;
using System.Collections.Generic;
using System.Net;

public class IntroGUI : MonoBehaviour {
	
	public string gameTypeName = "BunnytronGame";
	
	List<HostData> hostsFromMasterServer = new List<HostData>();
	
	public GUIStyle titleStyle;
	public GUIStyle buttonStyle;
	enum GUIState {
		MainMenu,
		JoinServer,
		None
	};
	
	GUIState state = GUIState.MainMenu;
	bool isServer = false;
	string hostName = "";
	
	void Start() {
		MasterServer.ipAddress = "ec2-23-21-20-134.compute-1.amazonaws.com";
		MasterServer.port = 23466;
		
		Network.natFacilitatorIP = "ec2-23-21-20-134.compute-1.amazonaws.com";
		Network.natFacilitatorPort = 50005;
		
		MasterServer.ClearHostList();
		hostName = Dns.GetHostName();
		
		MasterServer.RequestHostList(gameTypeName);
	}
	

    void Update() {
        if (MasterServer.PollHostList().Length != 0) {
            HostData[] hostData = MasterServer.PollHostList();
            int i = 0;
            while (i < hostData.Length) {
				hostsFromMasterServer.Add( hostData[i] );
                Debug.Log("Game name: " + hostData[i].gameName);
                i++;
            }
            MasterServer.ClearHostList();
        }
	}
	
	void OnDestroy() {
		if (isServer) {
			MasterServer.UnregisterHost();
		}
	}
	
	void OnGUI() {
		
			float areaWidth = 300;
			GUILayout.BeginArea(new Rect((Screen.width - areaWidth) / 2, 20,
									areaWidth, 300));
			GUILayout.Label("Bunny-tron", titleStyle,  GUILayout.ExpandWidth(true));
		
			GUILayout.FlexibleSpace();
		
			if (state == GUIState.MainMenu) {
				GUILayout.BeginHorizontal();
				GUILayout.Label("Host Name", buttonStyle);
				hostName = GUILayout.TextField(hostName, GUILayout.ExpandWidth(true));
				GUILayout.EndHorizontal();
			
				if (GUILayout.Button("Single Player", GUILayout.ExpandWidth(true))) {
					Debug.Log ("Start sinle player");
				}
				if (GUILayout.Button("Start Server", GUILayout.Width(areaWidth))) {
					Debug.Log ("Start server");
					StartServer();
				}
			
				if (hostsFromMasterServer.Count == 0) {
					GUILayout.Button("No Server To Join..", GUILayout.ExpandWidth(true));
				} else {
					GUILayout.Button("Join Servers", GUILayout.ExpandWidth(true));
				}
			
				foreach (HostData hostData in hostsFromMasterServer) {
					if (GUILayout.Button("  Server:" + hostData.gameName, GUILayout.ExpandWidth(true))) {
						ConnectToServer(hostData);
					}
				}
				
			}
		
		
			GUILayout.EndArea();
	}
	
	void StartServer() {
        var useNat = !Network.HavePublicAddress();
        var err = Network.InitializeServer(32, 25002, useNat);
		if (err != NetworkConnectionError.NoError) {
			Debug.Log(string.Format("error initializing server {0} useNat {1}",
				err, useNat));
			return;
		}
		
		this.isServer = true;
		Debug.Log ("started server..");
	}
	
	void OnServerInitialized() {
		MasterServer.RegisterHost(gameTypeName, hostName + " game", "game for all");
		Debug.Log ("registered host..");
    }
	
	void ConnectToServer(HostData hostData) {
		var err = Network.Connect(hostData);
		if (err != NetworkConnectionError.NoError) {
			Debug.Log(string.Format("error initializing server {0}", err));
			return;
		}
	}
	
    void OnConnectedToServer() {
        Debug.Log("Connected to server");
    }
	
    void OnMasterServerEvent(MasterServerEvent msEvent) 
	{
        if (msEvent == MasterServerEvent.RegistrationSucceeded) {
            Debug.Log("Server registered");
		}
		else {
			Debug.Log(string.Format ("master server event {0}", msEvent));
		}
    }
}
