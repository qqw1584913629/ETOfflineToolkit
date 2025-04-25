
using UnityEngine;
using UnityEngine.UI;
namespace MH
{
	[EntitySystem]
	public class ES_CloseAwakeSystem : AwakeSystem<ES_Close, Transform>
	{
		protected override void Awake(ES_Close self, Transform transform)
		{
			self.uiTransform = transform;
		}
	}
	[EntitySystem]
	public class ES_CloseDestroySystem : DestroySystem<ES_Close>
	{
		protected override void Destroy(ES_Close self)
		{
			self.DestroyWidget();
		}
	}
	public static partial class ES_CloseViewSystem 
	{
		public static void CloseWindow(this ES_Close self, Scene scene, WindowID windowID)
		{
			self.E_CloseButton.AddListener(() =>
			{
				var uiComponent = scene.GetComponent<UIComponent>();
				uiComponent.CloseWindow(windowID);
			});
		}
		public static void HideWindow(this ES_Close self, Scene scene, WindowID windowID)
		{
			self.E_CloseButton.AddListener(() =>
			{
				var uiComponent = scene.GetComponent<UIComponent>();
				uiComponent.HideWindow(windowID);
			});
		}
	}


}
