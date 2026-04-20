using System;
using System.Collections.Generic;
using UnityEngine;

namespace Peak
{
	// Token: 0x020003C2 RID: 962
	[Serializable]
	public abstract class DictionaryContainer<TKey, TValue> : ISerializationCallbackReceiver
	{
		// Token: 0x17000180 RID: 384
		// (get) Token: 0x06001977 RID: 6519 RVA: 0x000811E4 File Offset: 0x0007F3E4
		public Dictionary<TKey, TValue> Dict
		{
			get
			{
				if (this._dictionary.Count == 0 && this._keys.Count != 0)
				{
					int num = 0;
					while (num < this._keys.Count && num < this._values.Count)
					{
						this._dictionary[this._keys[num]] = this._values[num];
						num++;
					}
				}
				return this._dictionary;
			}
		}

		// Token: 0x06001978 RID: 6520 RVA: 0x00081258 File Offset: 0x0007F458
		void ISerializationCallbackReceiver.OnBeforeSerialize()
		{
			this._keys.Clear();
			this._values.Clear();
			foreach (KeyValuePair<TKey, TValue> keyValuePair in this._dictionary)
			{
				TKey tkey;
				TValue tvalue;
				keyValuePair.Deconstruct(out tkey, out tvalue);
				TKey item = tkey;
				TValue item2 = tvalue;
				this._keys.Add(item);
				this._values.Add(item2);
			}
		}

		// Token: 0x06001979 RID: 6521 RVA: 0x000812E4 File Offset: 0x0007F4E4
		public void OnAfterDeserialize()
		{
		}

		// Token: 0x04001737 RID: 5943
		[SerializeField]
		[HideInInspector]
		private List<TKey> _keys = new List<TKey>();

		// Token: 0x04001738 RID: 5944
		[SerializeField]
		[HideInInspector]
		private List<TValue> _values = new List<TValue>();

		// Token: 0x04001739 RID: 5945
		private Dictionary<TKey, TValue> _dictionary = new Dictionary<TKey, TValue>();
	}
}
