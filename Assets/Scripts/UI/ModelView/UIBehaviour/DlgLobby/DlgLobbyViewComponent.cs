
using UnityEngine;
using UnityEngine.UI;
namespace MH
{
	public class DlgLobbyViewComponent : Entity,IAwake,IDestroy 
	{
		public UnityEngine.RectTransform EG_CenterRectTransform
     	{
     		get
     		{
     			if (this.uiTransform == null)
     			{
     				Debug.LogError("uiTransform is null.");
     				return null;
     			}
     			if( this.m_EG_CenterRectTransform == null )
     			{
		    		this.m_EG_CenterRectTransform = UIFindHelper.FindDeepChild<UnityEngine.RectTransform>(this.uiTransform.gameObject,"EG_Center");
     			}
     			return this.m_EG_CenterRectTransform;
     		}
     	}

		public UnityEngine.UI.Button E_EnterGameButton
     	{
     		get
     		{
     			if (this.uiTransform == null)
     			{
     				Debug.LogError("uiTransform is null.");
     				return null;
     			}
     			if( this.m_E_EnterGameButton == null )
     			{
		    		this.m_E_EnterGameButton = UIFindHelper.FindDeepChild<UnityEngine.UI.Button>(this.uiTransform.gameObject,"EG_Center/E_EnterGame");
     			}
     			return this.m_E_EnterGameButton;
     		}
     	}

		public UnityEngine.UI.Image E_EnterGameImage
     	{
     		get
     		{
     			if (this.uiTransform == null)
     			{
     				Debug.LogError("uiTransform is null.");
     				return null;
     			}
     			if( this.m_E_EnterGameImage == null )
     			{
		    		this.m_E_EnterGameImage = UIFindHelper.FindDeepChild<UnityEngine.UI.Image>(this.uiTransform.gameObject,"EG_Center/E_EnterGame");
     			}
     			return this.m_E_EnterGameImage;
     		}
     	}

		public UnityEngine.UI.LoopHorizontalScrollRect ELoopScrollList_ServerLoopHorizontalScrollRect
     	{
     		get
     		{
     			if (this.uiTransform == null)
     			{
     				Debug.LogError("uiTransform is null.");
     				return null;
     			}
     			if( this.m_ELoopScrollList_ServerLoopHorizontalScrollRect == null )
     			{
		    		this.m_ELoopScrollList_ServerLoopHorizontalScrollRect = UIFindHelper.FindDeepChild<UnityEngine.UI.LoopHorizontalScrollRect>(this.uiTransform.gameObject,"EG_Center/ELoopScrollList_Server");
     			}
     			return this.m_ELoopScrollList_ServerLoopHorizontalScrollRect;
     		}
     	}

		public ES_Close ES_Close
     	{
     		get
     		{
     			if (this.uiTransform == null)
     			{
     				Debug.LogError("uiTransform is null.");
     				return null;
     			}
     			ES_Close value = this.m_es_close;
     			if( value == null )
     			{
		    	   Transform subTrans = UIFindHelper.FindDeepChild<Transform>(this.uiTransform.gameObject,"ES_Close");
		    	   this.m_es_close = this.AddChild<ES_Close,Transform>(subTrans);
     			}
     			return this.m_es_close;
     		}
     	}

		public void DestroyWidget()
		{
			this.m_EG_CenterRectTransform = null;
			this.m_E_EnterGameButton = null;
			this.m_E_EnterGameImage = null;
			this.m_ELoopScrollList_ServerLoopHorizontalScrollRect = null;
			this.m_es_close = null;
			this.uiTransform = null;
		}

		private UnityEngine.RectTransform m_EG_CenterRectTransform = null;
		private UnityEngine.UI.Button m_E_EnterGameButton = null;
		private UnityEngine.UI.Image m_E_EnterGameImage = null;
		private UnityEngine.UI.LoopHorizontalScrollRect m_ELoopScrollList_ServerLoopHorizontalScrollRect = null;
		private ES_Close m_es_close = null;
		public Transform uiTransform = null;
	}
}
