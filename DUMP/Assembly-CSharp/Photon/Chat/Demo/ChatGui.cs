using System;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

namespace Photon.Chat.Demo
{
	// Token: 0x020003A9 RID: 937
	public class ChatGui : MonoBehaviour, IChatClientListener
	{
		// Token: 0x1700017A RID: 378
		// (get) Token: 0x06001904 RID: 6404 RVA: 0x0007E40F File Offset: 0x0007C60F
		// (set) Token: 0x06001905 RID: 6405 RVA: 0x0007E417 File Offset: 0x0007C617
		public string UserName { get; set; }

		// Token: 0x06001906 RID: 6406 RVA: 0x0007E420 File Offset: 0x0007C620
		public void Start()
		{
			Object.DontDestroyOnLoad(base.gameObject);
			this.UserIdText.text = "";
			this.StateText.text = "";
			this.StateText.gameObject.SetActive(true);
			this.UserIdText.gameObject.SetActive(true);
			this.Title.SetActive(true);
			this.ChatPanel.gameObject.SetActive(false);
			this.ConnectingLabel.SetActive(false);
			if (string.IsNullOrEmpty(this.UserName))
			{
				this.UserName = "user" + (Environment.TickCount % 99).ToString();
			}
			this.chatAppSettings = PhotonNetwork.PhotonServerSettings.AppSettings.GetChatSettings();
			bool flag = !string.IsNullOrEmpty(this.chatAppSettings.AppIdChat);
			this.missingAppIdErrorPanel.SetActive(!flag);
			this.UserIdFormPanel.gameObject.SetActive(flag);
			if (!flag)
			{
				Debug.LogError("You need to set the chat app ID in the PhotonServerSettings file in order to continue.");
			}
		}

		// Token: 0x06001907 RID: 6407 RVA: 0x0007E528 File Offset: 0x0007C728
		public void Connect()
		{
			this.UserIdFormPanel.gameObject.SetActive(false);
			this.chatClient = new ChatClient(this, ConnectionProtocol.Udp);
			this.chatClient.UseBackgroundWorkerForSending = true;
			this.chatClient.AuthValues = new AuthenticationValues(this.UserName);
			this.chatClient.ConnectUsingSettings(this.chatAppSettings);
			this.ChannelToggleToInstantiate.gameObject.SetActive(false);
			Debug.Log("Connecting as: " + this.UserName);
			this.ConnectingLabel.SetActive(true);
		}

		// Token: 0x06001908 RID: 6408 RVA: 0x0007E5B9 File Offset: 0x0007C7B9
		public void OnDestroy()
		{
			if (this.chatClient != null)
			{
				this.chatClient.Disconnect(ChatDisconnectCause.DisconnectByClientLogic);
			}
		}

		// Token: 0x06001909 RID: 6409 RVA: 0x0007E5D0 File Offset: 0x0007C7D0
		public void OnApplicationQuit()
		{
			if (this.chatClient != null)
			{
				this.chatClient.Disconnect(ChatDisconnectCause.DisconnectByClientLogic);
			}
		}

		// Token: 0x0600190A RID: 6410 RVA: 0x0007E5E8 File Offset: 0x0007C7E8
		public void Update()
		{
			if (this.chatClient != null)
			{
				this.chatClient.Service();
			}
			if (this.StateText == null)
			{
				Object.Destroy(base.gameObject);
				return;
			}
			this.StateText.gameObject.SetActive(this.ShowState);
		}

		// Token: 0x0600190B RID: 6411 RVA: 0x0007E638 File Offset: 0x0007C838
		public void OnEnterSend()
		{
			if (Input.GetKey(KeyCode.Return) || Input.GetKey(KeyCode.KeypadEnter))
			{
				this.SendChatMessage(this.InputFieldChat.text);
				this.InputFieldChat.text = "";
			}
		}

		// Token: 0x0600190C RID: 6412 RVA: 0x0007E670 File Offset: 0x0007C870
		public void OnClickSend()
		{
			if (this.InputFieldChat != null)
			{
				this.SendChatMessage(this.InputFieldChat.text);
				this.InputFieldChat.text = "";
			}
		}

		// Token: 0x0600190D RID: 6413 RVA: 0x0007E6A4 File Offset: 0x0007C8A4
		private void SendChatMessage(string inputLine)
		{
			if (string.IsNullOrEmpty(inputLine))
			{
				return;
			}
			if ("test".Equals(inputLine))
			{
				if (this.TestLength != this.testBytes.Length)
				{
					this.testBytes = new byte[this.TestLength];
				}
				this.chatClient.SendPrivateMessage(this.chatClient.AuthValues.UserId, this.testBytes, true);
			}
			bool flag = this.chatClient.PrivateChannels.ContainsKey(this.selectedChannelName);
			string target = string.Empty;
			if (flag)
			{
				target = this.selectedChannelName.Split(new char[]
				{
					':'
				})[1];
			}
			if (inputLine[0].Equals('\\'))
			{
				string[] array = inputLine.Split(new char[]
				{
					' '
				}, 2);
				if (array[0].Equals("\\help"))
				{
					this.PostHelpToCurrentChannel();
				}
				if (array[0].Equals("\\state"))
				{
					int status = 0;
					List<string> list = new List<string>();
					list.Add("i am state " + status.ToString());
					string[] array2 = array[1].Split(new char[]
					{
						' ',
						','
					});
					if (array2.Length != 0)
					{
						status = int.Parse(array2[0]);
					}
					if (array2.Length > 1)
					{
						list.Add(array2[1]);
					}
					this.chatClient.SetOnlineStatus(status, list.ToArray());
					return;
				}
				if ((array[0].Equals("\\subscribe") || array[0].Equals("\\s")) && !string.IsNullOrEmpty(array[1]))
				{
					this.chatClient.Subscribe(array[1].Split(new char[]
					{
						' ',
						','
					}));
					return;
				}
				if ((array[0].Equals("\\unsubscribe") || array[0].Equals("\\u")) && !string.IsNullOrEmpty(array[1]))
				{
					this.chatClient.Unsubscribe(array[1].Split(new char[]
					{
						' ',
						','
					}));
					return;
				}
				if (array[0].Equals("\\clear"))
				{
					if (flag)
					{
						this.chatClient.PrivateChannels.Remove(this.selectedChannelName);
						return;
					}
					ChatChannel chatChannel;
					if (this.chatClient.TryGetChannel(this.selectedChannelName, flag, out chatChannel))
					{
						chatChannel.ClearMessages();
						return;
					}
				}
				else if (array[0].Equals("\\msg") && !string.IsNullOrEmpty(array[1]))
				{
					string[] array3 = array[1].Split(new char[]
					{
						' ',
						','
					}, 2);
					if (array3.Length < 2)
					{
						return;
					}
					string target2 = array3[0];
					string message = array3[1];
					this.chatClient.SendPrivateMessage(target2, message, false);
					return;
				}
				else
				{
					if ((!array[0].Equals("\\join") && !array[0].Equals("\\j")) || string.IsNullOrEmpty(array[1]))
					{
						Debug.Log("The command '" + array[0] + "' is invalid.");
						return;
					}
					string[] array4 = array[1].Split(new char[]
					{
						' ',
						','
					}, 2);
					if (this.channelToggles.ContainsKey(array4[0]))
					{
						this.ShowChannel(array4[0]);
						return;
					}
					this.chatClient.Subscribe(new string[]
					{
						array4[0]
					});
					return;
				}
			}
			else
			{
				if (flag)
				{
					this.chatClient.SendPrivateMessage(target, inputLine, false);
					return;
				}
				this.chatClient.PublishMessage(this.selectedChannelName, inputLine, false);
			}
		}

		// Token: 0x0600190E RID: 6414 RVA: 0x0007E9FC File Offset: 0x0007CBFC
		public void PostHelpToCurrentChannel()
		{
			Text currentChannelText = this.CurrentChannelText;
			currentChannelText.text += ChatGui.HelpText;
		}

		// Token: 0x0600190F RID: 6415 RVA: 0x0007EA19 File Offset: 0x0007CC19
		public void DebugReturn(DebugLevel level, string message)
		{
			if (level == DebugLevel.ERROR)
			{
				Debug.LogError(message);
				return;
			}
			if (level == DebugLevel.WARNING)
			{
				Debug.LogWarning(message);
				return;
			}
			Debug.Log(message);
		}

		// Token: 0x06001910 RID: 6416 RVA: 0x0007EA38 File Offset: 0x0007CC38
		public void OnConnected()
		{
			if (this.ChannelsToJoinOnConnect != null && this.ChannelsToJoinOnConnect.Length != 0)
			{
				this.chatClient.Subscribe(this.ChannelsToJoinOnConnect, this.HistoryLengthToFetch);
			}
			this.ConnectingLabel.SetActive(false);
			this.UserIdText.text = "Connected as " + this.UserName;
			this.ChatPanel.gameObject.SetActive(true);
			if (this.FriendsList != null && this.FriendsList.Length != 0)
			{
				this.chatClient.AddFriends(this.FriendsList);
				foreach (string text in this.FriendsList)
				{
					if (this.FriendListUiItemtoInstantiate != null && text != this.UserName)
					{
						this.InstantiateFriendButton(text);
					}
				}
			}
			if (this.FriendListUiItemtoInstantiate != null)
			{
				this.FriendListUiItemtoInstantiate.SetActive(false);
			}
			this.chatClient.SetOnlineStatus(2);
		}

		// Token: 0x06001911 RID: 6417 RVA: 0x0007EB2C File Offset: 0x0007CD2C
		public void OnDisconnected()
		{
			Debug.Log("OnDisconnected()");
			this.ConnectingLabel.SetActive(false);
		}

		// Token: 0x06001912 RID: 6418 RVA: 0x0007EB44 File Offset: 0x0007CD44
		public void OnChatStateChange(ChatState state)
		{
			this.StateText.text = state.ToString();
		}

		// Token: 0x06001913 RID: 6419 RVA: 0x0007EB60 File Offset: 0x0007CD60
		public void OnSubscribed(string[] channels, bool[] results)
		{
			foreach (string channelName in channels)
			{
				this.chatClient.PublishMessage(channelName, "says 'hi'.", false);
				if (this.ChannelToggleToInstantiate != null)
				{
					this.InstantiateChannelButton(channelName);
				}
			}
			Debug.Log("OnSubscribed: " + string.Join(", ", channels));
			this.ShowChannel(channels[0]);
		}

		// Token: 0x06001914 RID: 6420 RVA: 0x0007EBCC File Offset: 0x0007CDCC
		public void OnSubscribed(string channel, string[] users, Dictionary<object, object> properties)
		{
			Debug.LogFormat("OnSubscribed: {0}, users.Count: {1} Channel-props: {2}.", new object[]
			{
				channel,
				users.Length,
				properties.ToStringFull()
			});
		}

		// Token: 0x06001915 RID: 6421 RVA: 0x0007EBF8 File Offset: 0x0007CDF8
		private void InstantiateChannelButton(string channelName)
		{
			if (this.channelToggles.ContainsKey(channelName))
			{
				Debug.Log("Skipping creation for an existing channel toggle.");
				return;
			}
			Toggle toggle = Object.Instantiate<Toggle>(this.ChannelToggleToInstantiate);
			toggle.gameObject.SetActive(true);
			toggle.GetComponentInChildren<ChannelSelector>().SetChannel(channelName);
			toggle.transform.SetParent(this.ChannelToggleToInstantiate.transform.parent, false);
			this.channelToggles.Add(channelName, toggle);
		}

		// Token: 0x06001916 RID: 6422 RVA: 0x0007EC6C File Offset: 0x0007CE6C
		private void InstantiateFriendButton(string friendId)
		{
			GameObject gameObject = Object.Instantiate<GameObject>(this.FriendListUiItemtoInstantiate);
			gameObject.gameObject.SetActive(true);
			FriendItem component = gameObject.GetComponent<FriendItem>();
			component.FriendId = friendId;
			gameObject.transform.SetParent(this.FriendListUiItemtoInstantiate.transform.parent, false);
			this.friendListItemLUT[friendId] = component;
		}

		// Token: 0x06001917 RID: 6423 RVA: 0x0007ECC8 File Offset: 0x0007CEC8
		public void OnUnsubscribed(string[] channels)
		{
			foreach (string text in channels)
			{
				if (this.channelToggles.ContainsKey(text))
				{
					Object.Destroy(this.channelToggles[text].gameObject);
					this.channelToggles.Remove(text);
					Debug.Log("Unsubscribed from channel '" + text + "'.");
					if (text == this.selectedChannelName && this.channelToggles.Count > 0)
					{
						IEnumerator<KeyValuePair<string, Toggle>> enumerator = this.channelToggles.GetEnumerator();
						enumerator.MoveNext();
						KeyValuePair<string, Toggle> keyValuePair = enumerator.Current;
						this.ShowChannel(keyValuePair.Key);
						keyValuePair = enumerator.Current;
						keyValuePair.Value.isOn = true;
					}
				}
				else
				{
					Debug.Log("Can't unsubscribe from channel '" + text + "' because you are currently not subscribed to it.");
				}
			}
		}

		// Token: 0x06001918 RID: 6424 RVA: 0x0007EDAD File Offset: 0x0007CFAD
		public void OnGetMessages(string channelName, string[] senders, object[] messages)
		{
			if (channelName.Equals(this.selectedChannelName))
			{
				this.ShowChannel(this.selectedChannelName);
			}
		}

		// Token: 0x06001919 RID: 6425 RVA: 0x0007EDCC File Offset: 0x0007CFCC
		public void OnPrivateMessage(string sender, object message, string channelName)
		{
			this.InstantiateChannelButton(channelName);
			byte[] array = message as byte[];
			if (array != null)
			{
				Debug.Log("Message with byte[].Length: " + array.Length.ToString());
			}
			if (this.selectedChannelName.Equals(channelName))
			{
				this.ShowChannel(channelName);
			}
		}

		// Token: 0x0600191A RID: 6426 RVA: 0x0007EE1C File Offset: 0x0007D01C
		public void OnStatusUpdate(string user, int status, bool gotMessage, object message)
		{
			Debug.LogWarning("status: " + string.Format("{0} is {1}. Msg:{2}", user, status, message));
			if (this.friendListItemLUT.ContainsKey(user))
			{
				FriendItem friendItem = this.friendListItemLUT[user];
				if (friendItem != null)
				{
					friendItem.OnFriendStatusUpdate(status, gotMessage, message);
				}
			}
		}

		// Token: 0x0600191B RID: 6427 RVA: 0x0007EE79 File Offset: 0x0007D079
		public void OnUserSubscribed(string channel, string user)
		{
			Debug.LogFormat("OnUserSubscribed: channel=\"{0}\" userId=\"{1}\"", new object[]
			{
				channel,
				user
			});
		}

		// Token: 0x0600191C RID: 6428 RVA: 0x0007EE93 File Offset: 0x0007D093
		public void OnUserUnsubscribed(string channel, string user)
		{
			Debug.LogFormat("OnUserUnsubscribed: channel=\"{0}\" userId=\"{1}\"", new object[]
			{
				channel,
				user
			});
		}

		// Token: 0x0600191D RID: 6429 RVA: 0x0007EEAD File Offset: 0x0007D0AD
		public void OnChannelPropertiesChanged(string channel, string userId, Dictionary<object, object> properties)
		{
			Debug.LogFormat("OnChannelPropertiesChanged: {0} by {1}. Props: {2}.", new object[]
			{
				channel,
				userId,
				properties.ToStringFull()
			});
		}

		// Token: 0x0600191E RID: 6430 RVA: 0x0007EED0 File Offset: 0x0007D0D0
		public void OnUserPropertiesChanged(string channel, string targetUserId, string senderUserId, Dictionary<object, object> properties)
		{
			Debug.LogFormat("OnUserPropertiesChanged: (channel:{0} user:{1}) by {2}. Props: {3}.", new object[]
			{
				channel,
				targetUserId,
				senderUserId,
				properties.ToStringFull()
			});
		}

		// Token: 0x0600191F RID: 6431 RVA: 0x0007EEF8 File Offset: 0x0007D0F8
		public void OnErrorInfo(string channel, string error, object data)
		{
			Debug.LogFormat("OnErrorInfo for channel {0}. Error: {1} Data: {2}", new object[]
			{
				channel,
				error,
				data
			});
		}

		// Token: 0x06001920 RID: 6432 RVA: 0x0007EF18 File Offset: 0x0007D118
		public void AddMessageToSelectedChannel(string msg)
		{
			ChatChannel chatChannel = null;
			if (!this.chatClient.TryGetChannel(this.selectedChannelName, out chatChannel))
			{
				Debug.Log("AddMessageToSelectedChannel failed to find channel: " + this.selectedChannelName);
				return;
			}
			if (chatChannel != null)
			{
				chatChannel.Add("Bot", msg, 0);
			}
		}

		// Token: 0x06001921 RID: 6433 RVA: 0x0007EF64 File Offset: 0x0007D164
		public void ShowChannel(string channelName)
		{
			if (string.IsNullOrEmpty(channelName))
			{
				return;
			}
			ChatChannel chatChannel = null;
			if (!this.chatClient.TryGetChannel(channelName, out chatChannel))
			{
				Debug.Log("ShowChannel failed to find channel: " + channelName);
				return;
			}
			this.selectedChannelName = channelName;
			this.CurrentChannelText.text = chatChannel.ToStringMessages();
			Debug.Log("ShowChannel: " + this.selectedChannelName);
			foreach (KeyValuePair<string, Toggle> keyValuePair in this.channelToggles)
			{
				keyValuePair.Value.isOn = (keyValuePair.Key == channelName);
			}
		}

		// Token: 0x06001922 RID: 6434 RVA: 0x0007F028 File Offset: 0x0007D228
		public void OpenDashboard()
		{
			Application.OpenURL("https://dashboard.photonengine.com");
		}

		// Token: 0x040016DD RID: 5853
		public string[] ChannelsToJoinOnConnect;

		// Token: 0x040016DE RID: 5854
		public string[] FriendsList;

		// Token: 0x040016DF RID: 5855
		public int HistoryLengthToFetch;

		// Token: 0x040016E1 RID: 5857
		private string selectedChannelName;

		// Token: 0x040016E2 RID: 5858
		public ChatClient chatClient;

		// Token: 0x040016E3 RID: 5859
		protected internal ChatAppSettings chatAppSettings;

		// Token: 0x040016E4 RID: 5860
		public GameObject missingAppIdErrorPanel;

		// Token: 0x040016E5 RID: 5861
		public GameObject ConnectingLabel;

		// Token: 0x040016E6 RID: 5862
		public RectTransform ChatPanel;

		// Token: 0x040016E7 RID: 5863
		public GameObject UserIdFormPanel;

		// Token: 0x040016E8 RID: 5864
		public InputField InputFieldChat;

		// Token: 0x040016E9 RID: 5865
		public Text CurrentChannelText;

		// Token: 0x040016EA RID: 5866
		public Toggle ChannelToggleToInstantiate;

		// Token: 0x040016EB RID: 5867
		public GameObject FriendListUiItemtoInstantiate;

		// Token: 0x040016EC RID: 5868
		private readonly Dictionary<string, Toggle> channelToggles = new Dictionary<string, Toggle>();

		// Token: 0x040016ED RID: 5869
		private readonly Dictionary<string, FriendItem> friendListItemLUT = new Dictionary<string, FriendItem>();

		// Token: 0x040016EE RID: 5870
		public bool ShowState = true;

		// Token: 0x040016EF RID: 5871
		public GameObject Title;

		// Token: 0x040016F0 RID: 5872
		public Text StateText;

		// Token: 0x040016F1 RID: 5873
		public Text UserIdText;

		// Token: 0x040016F2 RID: 5874
		private static string HelpText = "\n    -- HELP --\nTo subscribe to channel(s) (channelnames are case sensitive) :  \n\t<color=#E07B00>\\subscribe</color> <color=green><list of channelnames></color>\n\tor\n\t<color=#E07B00>\\s</color> <color=green><list of channelnames></color>\n\nTo leave channel(s):\n\t<color=#E07B00>\\unsubscribe</color> <color=green><list of channelnames></color>\n\tor\n\t<color=#E07B00>\\u</color> <color=green><list of channelnames></color>\n\nTo switch the active channel\n\t<color=#E07B00>\\join</color> <color=green><channelname></color>\n\tor\n\t<color=#E07B00>\\j</color> <color=green><channelname></color>\n\nTo send a private message: (username are case sensitive)\n\t\\<color=#E07B00>msg</color> <color=green><username></color> <color=green><message></color>\n\nTo change status:\n\t\\<color=#E07B00>state</color> <color=green><stateIndex></color> <color=green><message></color>\n<color=green>0</color> = Offline <color=green>1</color> = Invisible <color=green>2</color> = Online <color=green>3</color> = Away \n<color=green>4</color> = Do not disturb <color=green>5</color> = Looking For Group <color=green>6</color> = Playing\n\nTo clear the current chat tab (private chats get closed):\n\t<color=#E07B00>\\clear</color>";

		// Token: 0x040016F3 RID: 5875
		public int TestLength = 2048;

		// Token: 0x040016F4 RID: 5876
		private byte[] testBytes = new byte[2048];
	}
}
