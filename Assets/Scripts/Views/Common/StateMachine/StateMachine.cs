// Converted from UnityScript to C# at http://www.M2H.nl/files/js_to_c.php - by Mike Hergaarden
// Do test the code! You usually need to change a few small bits.

using UnityEngine;
using System.Collections;

public class EventContext : System.Object {
	public GameObject sender = null;
	public bool  shouldThrowEventToParent = true;
}

public class EvStateDidChangeContext : EventContext {
	public string statePath;
}

public class StateMachine : MonoBehaviour {

	// StateMachine script component attached by Unity Editor should be true. Any subsequent substates created by the script should be false.
	private bool  isRoot = true;
	// current active substate of this state machine.
	private StateMachine currentSubState = null;
	// substates which belongs to this state machine.
	private Hashtable subStates = null;
	
	private StateMachine cachedRootStateMachine = null;
	
	// state machine will send "EvStateDidChange" with state path to listeners. and this flag is intended to use for ROOT state machine only!!
	public bool  isStateChangeNotificationRequired = false;

	public bool  isShowChangeStateLog = false;

	private void  LogError ( string message  ){
		StateMachine rootStateMachine = this.GetRootStateMachine();
		Debug.LogError(string.Format("[StateMachine:{0}] {1}", rootStateMachine.gameObject.name, message));
	}
	
	public void  Awake (){
		Transform parent = transform.parent;
		if( parent ) {
			StateMachine sm = parent.GetComponent< StateMachine >();
			if( sm ) {
				this.gameObject.SetActive(false);
			}
		}else{
			gameObject.SendMessage( "EvStateEnter", null, SendMessageOptions.DontRequireReceiver);
		}
	}
	
	public static string lastStateName = "";
	public StateMachine ChangeSubState ( string stateName ,   EventContext context  ){
	
		if( currentSubState && currentSubState.name == stateName ) return null;
		if(GetActiveState() != null)
			lastStateName = GetActiveState().name;
		StateMachine newState;
		
		if( subStates == null ) {
			subStates = new Hashtable();
			foreach(Transform child in transform) {
				StateMachine sm = child.GetComponent< StateMachine >();
				if( sm ) {
					sm.isRoot = false;
					subStates[child.name] = sm;
					child.gameObject.SetActive(false);
				}
			}
		}
		
		if( !subStates.Contains( stateName ) ) {
			newState = CreateSubState( stateName, gameObject.transform );
			subStates[stateName] = newState;
		}else{
			newState = subStates[stateName] as StateMachine;
		}
		
		StateMachine tail = currentSubState;

		if( tail ) {
			// Recursively exits nested states
			// - Invoke EvStateExit event on tails and deactivate it
			// - Cut currentSubStates off from the parents of tails
			while( tail.currentSubState ) {
				tail = tail.currentSubState;
			}
			while( tail && tail != this ) {
				tail.SendMessage( "EvStateExit", null, SendMessageOptions.DontRequireReceiver );
				tail.gameObject.SetActive(false);
				tail = tail.GetParentStateMachine();
				if (tail) {
					tail.currentSubState = null;
				}
			}
		}
		
		currentSubState = newState;
		if(currentSubState != null){
			currentSubState.gameObject.SetActive(true);
			currentSubState.gameObject.SendMessage( "EvStateEnter", context, SendMessageOptions.DontRequireReceiver);
		}
		
		if( GetRootStateMachine().isStateChangeNotificationRequired ) {
			EvStateDidChangeContext changedContext= new EvStateDidChangeContext();
			changedContext.statePath = GetStatePath();
			EventCaster.CastEvent( GetRootStateMachine().gameObject, "EvStateDidChange", changedContext );
		}
		
		return currentSubState;
	}
	
	public StateMachine ChangeSubState<State>() where State : MonoBehaviour{
		return ChangeSubState(typeof(State).Name,null);
	}
	public StateMachine ChangeSubState ( string stateName  ){
		if (isShowChangeStateLog) {
			Debug.Log ("ChangeSubState stateName:" + stateName);
		}
		return ChangeSubState( stateName, null );
	}
	
	public StateMachine ChangeState <State>()where State : MonoBehaviour{
		return ChangeState(typeof(State).Name);	
	}
	
	public StateMachine ChangeState ( string stateName   ){
		if (isShowChangeStateLog) {
			Debug.Log ("ChangeState stateName:" + stateName + " currentStateName:" + this.gameObject.name);
		}
		StateMachine parentStateMachine = GetParentStateMachine();
		if (parentStateMachine) {
			return parentStateMachine.ChangeSubState(stateName);
		}
		return null;
	}
	
	public StateMachine ChangeState ( string stateName ,   EventContext context   ){
//		Debug.Log ("ChangeState stateName:" + stateName + " currentStateName:" + this.gameObject.name);
		StateMachine parentStateMachine = GetParentStateMachine();
		if (parentStateMachine) {
			return parentStateMachine.ChangeSubState(stateName, context);
		}
		return null;
	}
	
	static private StateMachine CreateSubState ( string stateName ,   Transform parent  ){
		GameObject stateObject = new GameObject(stateName);
		stateObject.transform.parent = parent;
//		StateMachine subStateMachine = UnityEngineInternal.APIUpdaterRuntimeServices.AddComponent(stateObject, "Assets/Scripts/StateMachine/StateMachine.cs (139,34)", stateName) as StateMachine;
		StateMachine subStateMachine = stateObject.AddComponent(System.Type.GetType(stateName)) as StateMachine;
		if (subStateMachine) {
			subStateMachine.isRoot = false;
		}
		return subStateMachine;
	}
	
	// Returns the target game object of this state machine.
	// (the game object which is under control of this state machine)
	public GameObject GetTarget (){
		return GetRootStateMachine().gameObject;
	}
	
	public StateMachine GetParentStateMachine (){
		if (this.isRoot) {
			return null;
		}
		if (this.gameObject.transform.parent) {
			if (this.gameObject.transform.parent.GetComponents(typeof(StateMachine)).Length > 1) {
				this.LogError("GameObjects must not have multiple State Machine Components!" + this.gameObject.transform.parent.GetComponents(typeof(StateMachine)));
			}
			return this.gameObject.transform.parent.GetComponent<StateMachine>() as StateMachine;
		}
		return null;
	}
	
	public StateMachine GetSubState ( string stateName  ){
		return subStates[stateName] as StateMachine;
	}
	
	public StateMachine GetRootStateMachine (){
		if( cachedRootStateMachine != null ) {
			return cachedRootStateMachine;
		}else{
			cachedRootStateMachine = this;
			while (cachedRootStateMachine.GetParentStateMachine() != null) {
				cachedRootStateMachine = cachedRootStateMachine.GetParentStateMachine();
			}
			return cachedRootStateMachine;
		}
	}
	
	public StateMachine GetCurrentSubState (){
		return currentSubState;
	}
	
	public StateMachine GetActiveState (){
		StateMachine tail = currentSubState;
		while (tail != null && tail.GetCurrentSubState() ) {
			tail = tail.GetCurrentSubState();
		}
		return tail;
	}
	
	public bool IsActiveState ( StateMachine stateMachine  ){
		StateMachine s = GetActiveState();
		if (s) {
			if (stateMachine) {
				return s.GetType() == stateMachine.GetType();
			}
			return false;
		} else {
			return false;
		}
	}
	
	public bool IsActiveState ( System.Type stateMachineType  ){
		StateMachine s= GetActiveState();
		if (s) {
			return s.GetType() == stateMachineType;
		} else {
			return false;
		}
	}
	
	public bool IsActiveState ( string stateName  ){
		StateMachine s = GetActiveState();
		if (s) {
			return s.GetType().ToString().Equals(stateName);
		} else {
			return false;
		}
	}
	
	// Returns the full path, each segment separated by ".", from root state to end state.
	public string GetStatePath (){
		ArrayList names = new ArrayList();
		StateMachine tail = GetActiveState();
		
		while (tail != null) {
			names.Add(tail.gameObject.name);
			tail = tail.GetParentStateMachine();
		}
		names.Reverse();
		string[] namesArray = null;
		if (names.Count > 0)
		{
			namesArray = names.ToArray()as string[];
		}

		if (namesArray == null)
		{
			return ".";
		}

		return string.Join(".", namesArray);
	}
	
	public void  CallEvent ( GameObject sender ,   string msg ,   EventContext context  ){
		if( context == null ) {
			context = new EventContext();
		}
		context.sender = sender;
		StateMachine tail = currentSubState;
		if( tail ) {
			while( tail.currentSubState ) {
				tail = tail.currentSubState;
			}

			tail.SendMessage( msg, context, SendMessageOptions.DontRequireReceiver );
			while( context.shouldThrowEventToParent && tail.GetParentStateMachine() ) {
				tail = tail.GetParentStateMachine();
				tail.SendMessage( msg, context, SendMessageOptions.DontRequireReceiver );
			}
		} else if( context.shouldThrowEventToParent ) {
			this.SendMessage( msg, context, SendMessageOptions.DontRequireReceiver );
		}
	}
	
	public void  CallEvent ( GameObject sender ,   string msg  ){
		CallEvent( sender, msg, null );
	}
	
	// Protected methods (Override in subclasses)
	
	protected virtual void  EvStateEnter (){
	}
	protected virtual void  EvStateExit (){
	}
	protected virtual void  EvStateDidChange ( EvStateDidChangeContext context  ){
	}

}