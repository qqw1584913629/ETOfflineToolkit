using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace MH
{
	public static  class DlgLobbySystem
	{

		public static void RegisterUIEvent(this DlgLobby self)
		{
			self.View.ES_Close.CloseWindow(self.Root, WindowID.WindowID_Lobby);
			self.View.ELoopScrollList_ServerLoopHorizontalScrollRect.AddItemRefreshListener(self.OnServerItemLoopRefreshHandler);
			self.View.E_EnterGameButton.AddListenerAsync(self.EnterGame);
		}

		public static async ETTask EnterGame(this DlgLobby self)
		{
			await SceneChangeHelper.SceneChangeTo(self.Root, "Game");
			self.Root.GetComponent<UIComponent>().CloseWindow(WindowID.WindowID_Lobby);
		}

		public static void ShowWindow(this DlgLobby self, Entity contextData = null)
		{
			self.Refresh();
		}

		public static void Refresh(this DlgLobby self)
		{
			self.AddUIScrollItems(ref self.ScrollItemServers, 100);
			self.View.ELoopScrollList_ServerLoopHorizontalScrollRect.SetVisible(true, 100);


			using (ListComponent<int> list = ListComponent<int>.Create())
			{
				list.Add(1);
				list.Add(2);
				list.Add(3);
				list.Add(4);
				list.Add(5);
				list.Add(6);
				foreach (var item in list)
				{
					Debug.Log(item);
				}
			}

		}

		public static void OnServerItemLoopRefreshHandler(this DlgLobby self, Transform transform, int index)
		{
			var scrollItemServer = self.ScrollItemServers[index].BindTrans(transform);
			scrollItemServer.Init(index);
		}
	}
}
