// Converted from UnityScript to C# at http://www.M2H.nl/files/js_to_c.php - by Mike Hergaarden
// Do test the code! You usually need to change a few small bits.

using UnityEngine;
using System.Collections;

public class EventCaster : MonoBehaviour {

	ArrayList listeners = new ArrayList();
	
	
	public void  AddListener (  GameObject listener   ){
		if( !listeners.Contains( listener ) ) {
			listeners.Add( listener );
		}
	}
	
	public void  RemoveListener (  GameObject listener   ){
	
		if( listeners.Contains( listener ) ) {
			listeners.Remove( listener );
		}
	
	}
	
	// intended for StateMachines
	
	public void  CastEvent ( string msg ,   EventContext context  ){
		if(context != null) Debug.LogError("context required");
		if(!context.sender) Debug.LogError("context.sender required");
		foreach(GameObject p in listeners){
			if (p != null) {
				StateMachine sm = p.GetComponent< StateMachine >();	
				if( sm ) {
					sm.CallEvent( context.sender, msg, context );
				}
			}
		}
	}
	
	static public void  CastEvent ( GameObject sender ,   string msg ,   EventContext context  ){
		EventCaster ec = sender.GetComponent< EventCaster >();
		if( ec ) {
			if( context != null ) {
				context = new EventContext();
			}
			context.sender = sender;
			ec.CastEvent( msg, context );
		}
	}
	
	public static void  CastEvent ( GameObject sender ,   string msg  ){
		EventCaster ec = sender.GetComponent< EventCaster >();
		if( ec ) {
			CastEvent( sender, msg, null );
		}
	}
	
	// intended for GameObject
	
	public void  SendMessageToListeners ( string msg ,   Object context  ){
		foreach(GameObject p in listeners){
			if(!p){
				Debug.LogWarning("Event lister object already destroyed. You should call [RemoveListener] for destroyed object");
				continue;
			}
			StateMachine sm = p.GetComponent< StateMachine >();	
			if( sm ) {
				sm.SendMessage( msg, context, SendMessageOptions.DontRequireReceiver );
			}
		}
	}
	
	public static void  SendMessageToListener ( GameObject obj ,   string msg ,   Object context  ){
		EventCaster ec = obj.GetComponent< EventCaster >();
		if( ec ) {
			ec.SendMessageToListeners( msg, context );
		}
	}
	
	public static void  SendMessageToListener ( GameObject obj ,   string msg  ){
		EventCaster ec = obj.GetComponent< EventCaster >();
		if( ec ) {
			ec.SendMessageToListeners( msg, null );
		}
	}

}