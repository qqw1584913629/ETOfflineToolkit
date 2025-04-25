using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace MH
{
	public static  class DlgLoginSystem
	{

		public static void RegisterUIEvent(this DlgLogin self)
		{
			self.View.ES_Close.HideWindow(self.Scene, WindowID.WindowID_Login);
			self.View.E_LoginButton.AddListener(self.Login);
		}

		public static void Login(this DlgLogin self)
		{
			var account = self.View.E_AccountInputField.text;
			var password = self.View.E_PasswordInputField.text;
			Debug.Log($"账号：{account}，密码：{password}");
			self.Scene.GetComponent<UIComponent>().HideWindow(WindowID.WindowID_Login);//注意区分Scene 和 Root
			self.Root.GetComponent<UIComponent>().ShowWindow(WindowID.WindowID_Lobby);
		}
		public static void ShowWindow(this DlgLogin self, Entity contextData = null)
		{
			self.Refresh();
		}

		public static void Refresh(this DlgLogin self)
		{
			
		}
		 

	}
}
