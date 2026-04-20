using System;
using Photon.Pun;
using UnityEngine;

namespace Peak.Network
{
	// Token: 0x020003DE RID: 990
	public abstract class PhotonCallbackTarget : IDisposable
	{
		// Token: 0x06001A4F RID: 6735 RVA: 0x00082AA9 File Offset: 0x00080CA9
		protected PhotonCallbackTarget()
		{
			Application.quitting += this.Dispose;
			PhotonNetwork.AddCallbackTarget(this);
		}

		// Token: 0x06001A50 RID: 6736 RVA: 0x00082AC9 File Offset: 0x00080CC9
		public virtual void Dispose()
		{
			PhotonNetwork.RemoveCallbackTarget(this);
			Application.quitting -= this.Dispose;
		}
	}
}
