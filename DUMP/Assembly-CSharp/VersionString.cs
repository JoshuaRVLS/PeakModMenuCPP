using System;
using Photon.Pun;
using TMPro;
using UnityEngine;
using Zorro.Core;

// Token: 0x020001F8 RID: 504
public class VersionString : MonoBehaviour
{
	// Token: 0x06000FDC RID: 4060 RVA: 0x0004DB26 File Offset: 0x0004BD26
	private void Start()
	{
		this.m_text = base.GetComponent<TextMeshProUGUI>();
	}

	// Token: 0x06000FDD RID: 4061 RVA: 0x0004DB34 File Offset: 0x0004BD34
	private void Update()
	{
		BuildVersion buildVersion = new BuildVersion(Application.version, "???");
		this.m_text.text = buildVersion.ToString();
		if (string.IsNullOrEmpty(buildVersion.BuildName))
		{
			this.m_text.text = "v" + buildVersion.ToString();
		}
		if (PhotonNetwork.InRoom)
		{
			ConnectionService service = GameHandler.GetService<ConnectionService>();
			if (service != null)
			{
				InRoomState inRoomState = service.StateMachine.CurrentState as InRoomState;
				if (inRoomState != null && !string.IsNullOrEmpty(inRoomState.verifiedLobby))
				{
					TextMeshProUGUI text = this.m_text;
					text.text = string.Concat(new string[]
					{
						text.text,
						" - ",
						PhotonNetwork.CloudRegion,
						" - ",
						inRoomState.verifiedLobby
					});
				}
			}
		}
	}

	// Token: 0x04000D75 RID: 3445
	private TextMeshProUGUI m_text;
}
