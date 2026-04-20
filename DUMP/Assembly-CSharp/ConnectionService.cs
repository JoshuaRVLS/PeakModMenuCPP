using System;
using Zorro.Core;

// Token: 0x0200007A RID: 122
public class ConnectionService : GameService
{
	// Token: 0x06000575 RID: 1397 RVA: 0x00020388 File Offset: 0x0001E588
	public ConnectionService()
	{
		this.StateMachine = new ConnectionService.ConnectionServiceStateMachine();
		this.StateMachine.RegisterState(new DefaultConnectionState());
		this.StateMachine.RegisterState(new JoinSpecificRoomState());
		this.StateMachine.RegisterState(new InRoomState());
		this.StateMachine.RegisterState(new HostState());
		this.StateMachine.RegisterState(new KickedState());
		this.StateMachine.SwitchState<DefaultConnectionState>(false);
	}

	// Token: 0x040005AB RID: 1451
	public ConnectionService.ConnectionServiceStateMachine StateMachine;

	// Token: 0x02000438 RID: 1080
	public class ConnectionServiceStateMachine : StateMachine<ConnectionState>
	{
	}
}
