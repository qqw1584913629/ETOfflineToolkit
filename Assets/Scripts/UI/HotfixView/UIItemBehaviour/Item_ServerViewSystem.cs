
using UnityEngine;
using UnityEngine.UI;
namespace MH
{
	[EntitySystem]
	public class Scroll_Item_ServerAwakeSystem : AwakeSystem<Scroll_Item_Server>
	{
		protected override void Awake(Scroll_Item_Server self)
		{
		}
	}
	[EntitySystem]
	public class Scroll_Item_ServerDestroySystem : DestroySystem<Scroll_Item_Server>
	{
		protected override void Destroy(Scroll_Item_Server self)
		{
			self.DestroyWidget();
		}
	}
	public static partial class Scroll_Item_ServerViewSystem 
	{
		public static void Init(this Scroll_Item_Server self, int index)
		{
			self.E_SelectButton.AddListener(self.OnServerItemSelectHandler);
			self.Index = index;
			self.Refresh();
		}

		public static void Refresh(this Scroll_Item_Server self)
		{
			self.E_ServerTextText.text = $"{self.Index + 1}服";
		}

		public static void OnServerItemSelectHandler(this Scroll_Item_Server self)
		{
			Debug.Log($"选择了服务器：{self.Index + 1}服");
		}
	}
}
