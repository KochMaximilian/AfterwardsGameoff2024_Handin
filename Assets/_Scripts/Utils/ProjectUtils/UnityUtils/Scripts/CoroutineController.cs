using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityUtils
{
    public class CoroutineController : MonoBehaviour
    {
        private static CoroutineController Singleton
        {
            set => _singleton = value;
            get
            {
                if( _singleton == null ) InitializeType();
                return _singleton;
            }
        }
        
        private static CoroutineController _singleton;
        private static readonly Dictionary<string,IEnumerator> _routines = new Dictionary<string,IEnumerator>(100);

        private static void InitializeType ()
        {
            Singleton = new GameObject($"#{nameof(CoroutineController)}").AddComponent<CoroutineController>();
            DontDestroyOnLoad( Singleton );
        }

        public static Coroutine Start ( IEnumerator routine ) => Singleton.StartCoroutine( routine );
        public static Coroutine Start ( IEnumerator routine , string id )
        {
            var coroutine = Singleton.StartCoroutine( routine );
            if( !_routines.ContainsKey(id) ) _routines.Add( id , routine );
            else
            {
                Singleton.StopCoroutine( _routines[id] );
                _routines[id] = routine;
            }
            return coroutine;
        }
        public static void Stop ( IEnumerator routine ) => Singleton.StopCoroutine( routine );
        public static void Stop ( string id )
        {
            if( _routines.TryGetValue(id,out var routine) )
            {
                Singleton.StopCoroutine( routine );
                _routines.Remove( id );
            }
            else Debug.LogWarning($"coroutine '{id}' not found");
        }
        public static void StopAll () => Singleton.StopAllCoroutines();
    
    }
}
