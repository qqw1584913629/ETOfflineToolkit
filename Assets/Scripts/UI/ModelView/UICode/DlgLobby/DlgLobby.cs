using System.Collections.Generic;

namespace MH
{
	public  class DlgLobby :Entity,IAwake,IUILogic
	{

		public DlgLobbyViewComponent View { get => this.GetComponent<DlgLobbyViewComponent>();}


		public Dictionary<int, Scroll_Item_Server> ScrollItemServers;
	}
}
