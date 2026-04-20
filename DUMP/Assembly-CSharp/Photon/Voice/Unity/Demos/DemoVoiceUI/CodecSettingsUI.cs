using System;
using System.Collections.Generic;
using POpusCodec.Enums;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Photon.Voice.Unity.Demos.DemoVoiceUI
{
	// Token: 0x0200039E RID: 926
	public class CodecSettingsUI : MonoBehaviour
	{
		// Token: 0x0600187A RID: 6266 RVA: 0x0007C18C File Offset: 0x0007A38C
		private void Awake()
		{
			this.frameDurationDropdown.ClearOptions();
			this.frameDurationDropdown.AddOptions(CodecSettingsUI.frameDurationOptions);
			this.InitFrameDuration();
			this.frameDurationDropdown.SetSingleOnValueChangedCallback(new UnityAction<int>(this.OnFrameDurationChanged));
			this.samplingRateDropdown.ClearOptions();
			this.samplingRateDropdown.AddOptions(CodecSettingsUI.samplingRateOptions);
			this.InitSamplingRate();
			this.samplingRateDropdown.SetSingleOnValueChangedCallback(new UnityAction<int>(this.OnSamplingRateChanged));
			this.bitrateInputField.SetSingleOnValueChangedCallback(new UnityAction<string>(this.OnBitrateChanged));
			this.InitBitrate();
		}

		// Token: 0x0600187B RID: 6267 RVA: 0x0007C226 File Offset: 0x0007A426
		private void Update()
		{
			this.InitFrameDuration();
			this.InitSamplingRate();
			this.InitBitrate();
		}

		// Token: 0x0600187C RID: 6268 RVA: 0x0007C23C File Offset: 0x0007A43C
		private void OnBitrateChanged(string newBitrateString)
		{
			int bitrate;
			if (int.TryParse(newBitrateString, out bitrate))
			{
				this.recorder.Bitrate = bitrate;
			}
		}

		// Token: 0x0600187D RID: 6269 RVA: 0x0007C260 File Offset: 0x0007A460
		private void OnFrameDurationChanged(int index)
		{
			OpusCodec.FrameDuration frameDuration = this.recorder.FrameDuration;
			switch (index)
			{
			case 0:
				frameDuration = OpusCodec.FrameDuration.Frame2dot5ms;
				break;
			case 1:
				frameDuration = OpusCodec.FrameDuration.Frame5ms;
				break;
			case 2:
				frameDuration = OpusCodec.FrameDuration.Frame10ms;
				break;
			case 3:
				frameDuration = OpusCodec.FrameDuration.Frame20ms;
				break;
			case 4:
				frameDuration = OpusCodec.FrameDuration.Frame40ms;
				break;
			case 5:
				frameDuration = OpusCodec.FrameDuration.Frame60ms;
				break;
			}
			this.recorder.FrameDuration = frameDuration;
		}

		// Token: 0x0600187E RID: 6270 RVA: 0x0007C2D4 File Offset: 0x0007A4D4
		private void OnSamplingRateChanged(int index)
		{
			SamplingRate samplingRate = this.recorder.SamplingRate;
			switch (index)
			{
			case 0:
				samplingRate = SamplingRate.Sampling08000;
				break;
			case 1:
				samplingRate = SamplingRate.Sampling12000;
				break;
			case 2:
				samplingRate = SamplingRate.Sampling16000;
				break;
			case 3:
				samplingRate = SamplingRate.Sampling24000;
				break;
			case 4:
				samplingRate = SamplingRate.Sampling48000;
				break;
			}
			this.recorder.SamplingRate = samplingRate;
		}

		// Token: 0x0600187F RID: 6271 RVA: 0x0007C33C File Offset: 0x0007A53C
		private void InitFrameDuration()
		{
			int value = 0;
			OpusCodec.FrameDuration frameDuration = this.recorder.FrameDuration;
			if (frameDuration <= OpusCodec.FrameDuration.Frame10ms)
			{
				if (frameDuration != OpusCodec.FrameDuration.Frame5ms)
				{
					if (frameDuration == OpusCodec.FrameDuration.Frame10ms)
					{
						value = 2;
					}
				}
				else
				{
					value = 1;
				}
			}
			else if (frameDuration != OpusCodec.FrameDuration.Frame20ms)
			{
				if (frameDuration != OpusCodec.FrameDuration.Frame40ms)
				{
					if (frameDuration == OpusCodec.FrameDuration.Frame60ms)
					{
						value = 5;
					}
				}
				else
				{
					value = 4;
				}
			}
			else
			{
				value = 3;
			}
			this.frameDurationDropdown.value = value;
		}

		// Token: 0x06001880 RID: 6272 RVA: 0x0007C3AC File Offset: 0x0007A5AC
		private void InitSamplingRate()
		{
			int value = 0;
			SamplingRate samplingRate = this.recorder.SamplingRate;
			if (samplingRate <= SamplingRate.Sampling16000)
			{
				if (samplingRate != SamplingRate.Sampling12000)
				{
					if (samplingRate == SamplingRate.Sampling16000)
					{
						value = 2;
					}
				}
				else
				{
					value = 1;
				}
			}
			else if (samplingRate != SamplingRate.Sampling24000)
			{
				if (samplingRate == SamplingRate.Sampling48000)
				{
					value = 4;
				}
			}
			else
			{
				value = 3;
			}
			this.samplingRateDropdown.value = value;
		}

		// Token: 0x06001881 RID: 6273 RVA: 0x0007C410 File Offset: 0x0007A610
		private void InitBitrate()
		{
			this.bitrateInputField.text = this.recorder.Bitrate.ToString();
		}

		// Token: 0x04001675 RID: 5749
		[SerializeField]
		private Dropdown frameDurationDropdown;

		// Token: 0x04001676 RID: 5750
		[SerializeField]
		private Dropdown samplingRateDropdown;

		// Token: 0x04001677 RID: 5751
		[SerializeField]
		private InputField bitrateInputField;

		// Token: 0x04001678 RID: 5752
		[SerializeField]
		private Recorder recorder;

		// Token: 0x04001679 RID: 5753
		private static readonly List<string> frameDurationOptions = new List<string>
		{
			"2.5ms",
			"5ms",
			"10ms",
			"20ms",
			"40ms",
			"60ms"
		};

		// Token: 0x0400167A RID: 5754
		private static readonly List<string> samplingRateOptions = new List<string>
		{
			"8kHz",
			"12kHz",
			"16kHz",
			"24kHz",
			"48kHz"
		};
	}
}
