using UnityEngine;
using System.Collections;

public abstract class Singleton<T> : MonoBehaviour where T : Singleton<T> {

	private static T m_Instance;

	public static T I {
		get {
			if (m_Instance == null) {
				m_Instance = (T)(FindObjectOfType (typeof(T)));

				if (m_Instance == null) {
					Debug.LogError (typeof(T) + " is nothing");
				} else {
					m_Instance.Init ();
				}
			}
			return m_Instance;
		}
	}

	protected virtual void Awake () {
		CheckInstance ();
	}

	protected abstract void Init ();

	protected bool CheckInstance () {
		if (this == I) {
			return true;
		}
		Destroy (this);
		return false;
	}

	static public bool IsValid () {
		return m_Instance != null;
	}

	protected virtual void OnDestroy () {
		if (this == m_Instance) {
			m_Instance = null;
		}
	}

}