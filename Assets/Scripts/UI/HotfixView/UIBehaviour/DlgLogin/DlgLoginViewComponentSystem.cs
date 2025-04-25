
using UnityEngine;
using UnityEngine.UI;
namespace MH
{
	[EntitySystem]
	public class DlgLoginViewComponentAwakeSystem : AwakeSystem<DlgLoginViewComponent>
	{
		protected override void Awake(DlgLoginViewComponent self)
		{
			self.uiTransform = self.Parent.GetParent<UIBaseWindow>().uiTransform;
		}
	}
	[EntitySystem]
	public class DlgLoginViewComponentDestroySystem : DestroySystem<DlgLoginViewComponent>
	{
		protected override void Destroy(DlgLoginViewComponent self)
		{
			self.DestroyWidget();
		}
	}
	public static class DlgLoginViewComponentSystem
	{


	}


}
