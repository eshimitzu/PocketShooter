﻿using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace AssetUsageDetectorNamespace.Extras
{
	public static class Utilities
	{
		// A set of commonly used Unity types
		private static readonly HashSet<Type> primitiveUnityTypes = new HashSet<Type>() {
				typeof( string ), typeof( Vector4 ), typeof( Vector3 ), typeof( Vector2 ), typeof( Rect ),
				typeof( Quaternion ), typeof( Color ), typeof( Color32 ), typeof( LayerMask ),
				typeof( Matrix4x4 ), typeof( AnimationCurve ), typeof( Gradient ), typeof( RectOffset ),
				typeof( Assembly ) // Searching assembly variables for reference throws InvalidCastException on .NET 4.0 runtime
		};

		private static readonly HashSet<string> folderContentsSet = new HashSet<string>();

		public static readonly GUILayoutOption GL_EXPAND_WIDTH = GUILayout.ExpandWidth( true );
		public static readonly GUILayoutOption GL_EXPAND_HEIGHT = GUILayout.ExpandHeight( true );
		public static readonly GUILayoutOption GL_WIDTH_25 = GUILayout.Width( 25 );
		public static readonly GUILayoutOption GL_WIDTH_100 = GUILayout.Width( 100 );
		public static readonly GUILayoutOption GL_WIDTH_250 = GUILayout.Width( 250 );
		public static readonly GUILayoutOption GL_HEIGHT_30 = GUILayout.Height( 30 );
		public static readonly GUILayoutOption GL_HEIGHT_35 = GUILayout.Height( 35 );
		public static readonly GUILayoutOption GL_HEIGHT_40 = GUILayout.Height( 40 );

		private static GUIStyle m_boxGUIStyle; // GUIStyle used to draw the results of the search
		public static GUIStyle BoxGUIStyle
		{
			get
			{
				if( m_boxGUIStyle == null )
				{
					m_boxGUIStyle = new GUIStyle( EditorStyles.helpBox )
					{
						alignment = TextAnchor.MiddleCenter,
						font = EditorStyles.label.font
					};
				}

				return m_boxGUIStyle;
			}
		}

		private static GUIStyle m_tooltipGUIStyle; // GUIStyle used to draw the tooltip
		public static GUIStyle TooltipGUIStyle
		{
			get
			{
				GUIStyleState normalState;

				if( m_tooltipGUIStyle != null )
					normalState = m_tooltipGUIStyle.normal;
				else
				{
					m_tooltipGUIStyle = new GUIStyle( EditorStyles.helpBox )
					{
						alignment = TextAnchor.MiddleCenter,
						font = EditorStyles.label.font
					};

					normalState = m_tooltipGUIStyle.normal;

					normalState.background = null;
					normalState.textColor = Color.black;
				}

				if( normalState.background == null || normalState.background.Equals( null ) )
				{
					Texture2D backgroundTexture = new Texture2D( 1, 1 );
					backgroundTexture.SetPixel( 0, 0, new Color( 0.88f, 0.88f, 0.88f, 0.85f ) );
					backgroundTexture.Apply();

					normalState.background = backgroundTexture;
				}

				return m_tooltipGUIStyle;
			}
		}

		// Get a unique-ish string hash code for an object
		public static string Hash( this object obj )
		{
			if( obj is Object )
				return obj.GetHashCode().ToString();

			return obj.GetHashCode() + obj.GetType().Name;
		}

		// Check if object is an asset or a Scene object
		public static bool IsAsset( this object obj )
		{
			return obj is Object && AssetDatabase.Contains( (Object) obj );
		}

		// Check if object is a folder asset
		public static bool IsFolder( this Object obj )
		{
			return obj is DefaultAsset && AssetDatabase.IsValidFolder( AssetDatabase.GetAssetPath( obj ) );
		}

		// Returns an enumerator to iterate through all asset paths in the folder
		public static IEnumerable<string> EnumerateFolderContents( Object folderAsset )
		{
			string[] folderContents = AssetDatabase.FindAssets( "", new string[] { AssetDatabase.GetAssetPath( folderAsset ) } );
			if( folderContents == null )
				return new EmptyEnumerator<string>();

			folderContentsSet.Clear();
			for( int i = 0; i < folderContents.Length; i++ )
			{
				string filePath = AssetDatabase.GUIDToAssetPath( folderContents[i] );
				if( !string.IsNullOrEmpty( filePath ) && !AssetDatabase.IsValidFolder( filePath ) )
					folderContentsSet.Add( filePath );
			}

			return folderContentsSet;
		}

		// Select an object in the editor
		public static void SelectInEditor( this Object obj )
		{
			if( obj == null )
				return;

			Event e = Event.current;

			// If CTRL key is pressed, do a multi-select;
			// otherwise select only the clicked object and ping it in editor
			if( !e.control )
				Selection.activeObject = obj.PingInEditor();
			else
			{
				Component objAsComp = obj as Component;
				GameObject objAsGO = obj as GameObject;
				int selectionIndex = -1;

				Object[] selection = Selection.objects;
				for( int i = 0; i < selection.Length; i++ )
				{
					Object selected = selection[i];

					// Don't allow including both a gameobject and one of its components in the selection
					if( selected == obj || ( objAsComp != null && selected == objAsComp.gameObject ) ||
						( objAsGO != null && selected is Component && ( (Component) selected ).gameObject == objAsGO ) )
					{
						selectionIndex = i;
						break;
					}
				}

				Object[] newSelection;
				if( selectionIndex == -1 )
				{
					// Include object in selection
					newSelection = new Object[selection.Length + 1];
					selection.CopyTo( newSelection, 0 );
					newSelection[selection.Length] = obj;
				}
				else
				{
					// Remove object from selection
					newSelection = new Object[selection.Length - 1];
					int j = 0;
					for( int i = 0; i < selectionIndex; i++, j++ )
						newSelection[j] = selection[i];
					for( int i = selectionIndex + 1; i < selection.Length; i++, j++ )
						newSelection[j] = selection[i];
				}

				Selection.objects = newSelection;
			}
		}

		// Ping an object in either Project view or Hierarchy view
		public static Object PingInEditor( this Object obj )
		{
			if( obj is Component )
				obj = ( (Component) obj ).gameObject;

			Object selection = obj;

			// Pinging a prefab only works if the pinged object is the root of the prefab
			// or a direct child of it. Pinging any grandchildren of the prefab
			// does not work; in which case, traverse the parent hierarchy until
			// a pingable parent is reached
			if( obj.IsAsset() && obj is GameObject )
			{
#if UNITY_2018_3_OR_NEWER
				Transform objTR = ( (GameObject) obj ).transform.root;

				PrefabAssetType prefabAssetType = PrefabUtility.GetPrefabAssetType( objTR.gameObject );
				if( prefabAssetType == PrefabAssetType.Regular || prefabAssetType == PrefabAssetType.Variant )
				{
					string assetPath = AssetDatabase.GetAssetPath( objTR.gameObject );
					var openPrefabStage = UnityEditor.Experimental.SceneManagement.PrefabStageUtility.GetCurrentPrefabStage();
					if( openPrefabStage != null && openPrefabStage.stageHandle.IsValid() && assetPath == openPrefabStage.prefabAssetPath )
					{
						// Ping the object in prefab stage
						Transform targetParent = objTR;
						Transform selectedObject = ( (GameObject) obj ).transform;
						objTR = openPrefabStage.prefabContentsRoot.transform;
						while( targetParent != selectedObject )
						{
							Transform temp = selectedObject;
							while( temp.parent != targetParent )
								temp = temp.parent;

							objTR = objTR.GetChild( temp.GetSiblingIndex() );
							targetParent = temp;
						}

						selection = objTR.gameObject;
					}
#if UNITY_2019_1_OR_NEWER
					else if( obj != objTR.gameObject )
					{
						Debug.Log( "Open " + assetPath + " in prefab mode to select and edit " + obj.name );
						selection = objTR.gameObject;
					}
#else
					else
						Debug.Log( "Open " + assetPath + " in prefab mode to select and edit " + obj.name );
#endif
				}
#else
				Transform objTR = ( (GameObject) obj ).transform;
				while( objTR.parent != null && objTR.parent.parent != null )
					objTR = objTR.parent;
#endif

				obj = objTR.gameObject;
			}

			EditorGUIUtility.PingObject( obj );
			return selection;
		}

		// Check if the field is serializable
		public static bool IsSerializable( this FieldInfo fieldInfo )
		{
			// see Serialization Rules: https://docs.unity3d.com/Manual/script-Serialization.html
			if( fieldInfo.IsInitOnly || ( ( !fieldInfo.IsPublic || fieldInfo.IsNotSerialized ) &&
			   !Attribute.IsDefined( fieldInfo, typeof( SerializeField ) ) ) )
				return false;

			return IsTypeSerializable( fieldInfo.FieldType );
		}

		// Check if the property is serializable
		public static bool IsSerializable( this PropertyInfo propertyInfo )
		{
			return IsTypeSerializable( propertyInfo.PropertyType );
		}

		// Check if type is serializable
		private static bool IsTypeSerializable( Type type )
		{
			// see Serialization Rules: https://docs.unity3d.com/Manual/script-Serialization.html
			if( typeof( Object ).IsAssignableFrom( type ) )
				return true;

			if( type.IsArray )
			{
				if( type.GetArrayRank() != 1 )
					return false;

				type = type.GetElementType();

				if( typeof( Object ).IsAssignableFrom( type ) )
					return true;
			}
			else if( type.IsGenericType )
			{
				if( type.GetGenericTypeDefinition() != typeof( List<> ) )
					return false;

				type = type.GetGenericArguments()[0];

				if( typeof( Object ).IsAssignableFrom( type ) )
					return true;
			}

			if( type.IsGenericType )
				return false;

			return Attribute.IsDefined( type, typeof( SerializableAttribute ), false );
		}

		// Check if the type is a common Unity type (let's call them primitives)
		public static bool IsPrimitiveUnityType( this Type type )
		{
			return type.IsPrimitive || primitiveUnityTypes.Contains( type );
		}

		// Get <get> function for a field
		public static VariableGetVal CreateGetter( this FieldInfo fieldInfo, Type type )
		{
			// Commented the IL generator code below because it might actually be slower than simply using reflection
			// Credit: https://www.codeproject.com/Articles/14560/Fast-Dynamic-Property-Field-Accessors
			//DynamicMethod dm = new DynamicMethod( "Get" + fieldInfo.Name, fieldInfo.FieldType, new Type[] { typeof( object ) }, type );
			//ILGenerator il = dm.GetILGenerator();
			//// Load the instance of the object (argument 0) onto the stack
			//il.Emit( OpCodes.Ldarg_0 );
			//// Load the value of the object's field (fi) onto the stack
			//il.Emit( OpCodes.Ldfld, fieldInfo );
			//// return the value on the top of the stack
			//il.Emit( OpCodes.Ret );

			//return (VariableGetVal) dm.CreateDelegate( typeof( VariableGetVal ) );

			return fieldInfo.GetValue;
		}

		// Get <get> function for a property
		public static VariableGetVal CreateGetter( this PropertyInfo propertyInfo )
		{
			// Ignore indexer properties
			if( propertyInfo.GetIndexParameters().Length > 0 )
				return null;

			// Can't use PropertyWrapper (which uses CreateDelegate) for property getters of structs
			if( propertyInfo.DeclaringType.IsValueType )
				return propertyInfo.CanRead ? ( ( obj ) => propertyInfo.GetValue( obj, null ) ) : (VariableGetVal) null;

			MethodInfo mi = propertyInfo.GetGetMethod( true );
			if( mi != null )
			{
				Type GenType = typeof( PropertyWrapper<,> ).MakeGenericType( propertyInfo.DeclaringType, propertyInfo.PropertyType );
				return ( (IPropertyAccessor) Activator.CreateInstance( GenType, mi ) ).GetValue;
			}

			return null;
		}

		public static bool IsEmpty( this List<ObjectToSearch> objectsToSearch )
		{
			if( objectsToSearch == null )
				return true;

			for( int i = 0; i < objectsToSearch.Count; i++ )
			{
				if( objectsToSearch[i].obj != null && !objectsToSearch[i].obj.Equals( null ) )
					return false;
			}

			return true;
		}

		public static bool IsEmpty( this List<Object> searchInAssetsSubset )
		{
			if( searchInAssetsSubset == null )
				return true;

			for( int i = 0; i < searchInAssetsSubset.Count; i++ )
			{
				if( searchInAssetsSubset[i] != null && !searchInAssetsSubset[i].Equals( null ) )
					return false;
			}

			return true;
		}
	}
}