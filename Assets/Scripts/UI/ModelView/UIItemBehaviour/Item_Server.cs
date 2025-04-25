
using UnityEngine;
using UnityEngine.UI;
namespace MH
{
	public partial class Scroll_Item_Server : Entity,IAwake,IDestroy,IUIScrollItem<Scroll_Item_Server>,IUILogic 
	{
		public long DataId {get;set;}
		private bool isCacheNode = false;
		public void SetCacheMode(bool isCache)
		{
			this.isCacheNode = isCache;
		}

		public Scroll_Item_Server BindTrans(Transform trans)
		{
			this.uiTransform = trans;
			return this;
		}

		public UnityEngine.UI.Button E_SelectButton
     	{
     		get
     		{
     			if (this.uiTransform == null)
     			{
     				Debug.LogError("uiTransform is null.");
     				return null;
     			}
     			if (this.isCacheNode)
     			{
     				if( this.m_E_SelectButton == null )
     				{
		    			this.m_E_SelectButton = UIFindHelper.FindDeepChild<UnityEngine.UI.Button>(this.uiTransform.gameObject,"E_Select");
     				}
     				return this.m_E_SelectButton;
     			}
     			else
     			{
		    		return UIFindHelper.FindDeepChild<UnityEngine.UI.Button>(this.uiTransform.gameObject,"E_Select");
     			}
     		}
     	}

		public UnityEngine.UI.Image E_SelectImage
     	{
     		get
     		{
     			if (this.uiTransform == null)
     			{
     				Debug.LogError("uiTransform is null.");
     				return null;
     			}
     			if (this.isCacheNode)
     			{
     				if( this.m_E_SelectImage == null )
     				{
		    			this.m_E_SelectImage = UIFindHelper.FindDeepChild<UnityEngine.UI.Image>(this.uiTransform.gameObject,"E_Select");
     				}
     				return this.m_E_SelectImage;
     			}
     			else
     			{
		    		return UIFindHelper.FindDeepChild<UnityEngine.UI.Image>(this.uiTransform.gameObject,"E_Select");
     			}
     		}
     	}

		public UnityEngine.UI.Text E_ServerTextText
     	{
     		get
     		{
     			if (this.uiTransform == null)
     			{
     				Debug.LogError("uiTransform is null.");
     				return null;
     			}
     			if (this.isCacheNode)
     			{
     				if( this.m_E_ServerTextText == null )
     				{
		    			this.m_E_ServerTextText = UIFindHelper.FindDeepChild<UnityEngine.UI.Text>(this.uiTransform.gameObject,"E_Select/E_ServerText");
     				}
     				return this.m_E_ServerTextText;
     			}
     			else
     			{
		    		return UIFindHelper.FindDeepChild<UnityEngine.UI.Text>(this.uiTransform.gameObject,"E_Select/E_ServerText");
     			}
     		}
     	}

		public void DestroyWidget()
		{
			this.m_E_SelectButton = null;
			this.m_E_SelectImage = null;
			this.m_E_ServerTextText = null;
			this.uiTransform = null;
			this.DataId = 0;
		}

		private UnityEngine.UI.Button m_E_SelectButton = null;
		private UnityEngine.UI.Image m_E_SelectImage = null;
		private UnityEngine.UI.Text m_E_ServerTextText = null;
		public Transform uiTransform = null;
	}
}
