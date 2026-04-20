using System;
using Photon.Pun;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using Zorro.Core;
using Zorro.Core.Serizalization;

// Token: 0x0200014E RID: 334
public abstract class PhotonBinaryStreamSerializer<T> : MonoBehaviourPunCallbacks, IPunObservable where T : struct, IBinarySerializable
{
	// Token: 0x06000B1F RID: 2847
	public abstract T GetDataToWrite();

	// Token: 0x06000B20 RID: 2848 RVA: 0x0003BEE4 File Offset: 0x0003A0E4
	protected virtual void Awake()
	{
		this.photonView = base.GetComponent<PhotonView>();
	}

	// Token: 0x06000B21 RID: 2849 RVA: 0x0003BEF4 File Offset: 0x0003A0F4
	public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (stream.IsWriting)
		{
			if (this.ShouldSendData())
			{
				if (IBinarySerializable.shouldLog)
				{
					Debug.Log(base.gameObject.name + " sending data in type " + base.GetType().Name);
				}
				T dataToWrite = this.GetDataToWrite();
				BinarySerializer binarySerializer = new BinarySerializer(UnsafeUtility.SizeOf<T>(), Allocator.Temp);
				dataToWrite.Serialize(binarySerializer);
				byte[] array = binarySerializer.buffer.ToByteArray();
				NetworkStats.RegisterBytesSent<T>((ulong)((long)array.Length));
				stream.SendNext(array);
				binarySerializer.Dispose();
				return;
			}
		}
		else
		{
			if (IBinarySerializable.shouldLog)
			{
				Debug.Log(base.gameObject.name + " received data in type " + base.GetType().Name);
			}
			BinaryDeserializer binaryDeserializer = new BinaryDeserializer((byte[])stream.ReceiveNext(), Allocator.Temp);
			T t = Activator.CreateInstance<T>();
			t.Deserialize(binaryDeserializer);
			binaryDeserializer.Dispose();
			this.RemoteValue = Optionable<T>.Some(t);
			this.OnDataReceived(t);
		}
	}

	// Token: 0x06000B22 RID: 2850 RVA: 0x0003BFF2 File Offset: 0x0003A1F2
	public virtual void OnDataReceived(T data)
	{
		this.sinceLastPackage = 0f;
	}

	// Token: 0x06000B23 RID: 2851 RVA: 0x0003BFFF File Offset: 0x0003A1FF
	public virtual bool ShouldSendData()
	{
		return true;
	}

	// Token: 0x04000A3F RID: 2623
	protected Optionable<T> RemoteValue;

	// Token: 0x04000A40 RID: 2624
	protected float sinceLastPackage;

	// Token: 0x04000A41 RID: 2625
	protected new PhotonView photonView;
}
