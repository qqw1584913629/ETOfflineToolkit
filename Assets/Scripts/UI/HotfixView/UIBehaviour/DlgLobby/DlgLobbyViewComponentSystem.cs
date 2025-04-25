
using UnityEngine;
using UnityEngine.UI;
namespace MH
{
	[EntitySystem]
	public class DlgLobbyViewComponentAwakeSystem : AwakeSystem<DlgLobbyViewComponent>
	{
		protected override void Awake(DlgLobbyViewComponent self)
		{
			self.uiTransform = self.Parent.GetParent<UIBaseWindow>().uiTransform;
		}
	}
	[EntitySystem]
	public class DlgLobbyViewComponentDestroySystem : DestroySystem<DlgLobbyViewComponent>
	{
		protected override void Destroy(DlgLobbyViewComponent self)
		{
			self.DestroyWidget();
		}
	}
	public static class DlgLobbyViewComponentSystem
	{


	}


}
