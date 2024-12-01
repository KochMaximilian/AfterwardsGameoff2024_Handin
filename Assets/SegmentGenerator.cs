using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class SegmentGenerator : MonoBehaviour
{
    [SerializeField] private GameObject nubi;
    [SerializeField] private GameObject nubiPlaceHolderPrefab;
    [SerializeField] private GameObject segmentPrefab;
    [SerializeField] private Transform segmentsParent;
    [SerializeField] private float segmentLength = 45;
    [SerializeField] private int segmentsBetweenNubi = 2;
    [SerializeField] private GameObject player;
   
    private ObjectPool<GameObject> _segmentPool;
    private List<GameObject> _segments = new List<GameObject>();
    private int _currentSegment;
    
    private int _globalIndex;
    private int _nubiGlobalIndex;
    
    private void Start()
    {
        _segmentPool = new ObjectPool<GameObject>(
            () => Instantiate(segmentPrefab, segmentsParent), 
            x => x.gameObject.SetActive(true), 
            x => x.gameObject.SetActive(false), 
            x => Destroy(x.gameObject));
        
        Initialize();
    }

    private void Update()
    {
        int previousSegment = _currentSegment;
        _currentSegment = GetCurrentSegment();
        
        if(previousSegment != _currentSegment)
        {
            if (_currentSegment > previousSegment)
            {
                _globalIndex++;
                GenerateForward();
            }
            else if (_currentSegment < previousSegment)
            {
                _globalIndex--;
                GenerateBackward();
            }
            
            _currentSegment = GetCurrentSegment();
        }
    }
    
    private int GetCurrentSegment()
    {
        float minDistance = float.MaxValue;
        for(int i = 0; i < _segments.Count; i++)
        {
            float distance = Vector3.Distance(player.transform.position, _segments[i].transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                _currentSegment = i;
            }
        }
        
        return _currentSegment;
    }
    
    private void Initialize()
    {
        List<GameObject> segments = new List<GameObject>();
        for (var i = 0; i < 13; i++)
        {
            var segment = _segmentPool.Get();
            segments.Add(segment);
        }

        foreach (var segment in segments)
        {
            _segmentPool.Release(segment);
        }

        Vector3 initialPosition = player.transform.position + segmentLength * Vector3.forward * 6;
        for (var i = 0; i < 13; i++)
        {
            var segment = _segmentPool.Get();
            segment.transform.position = initialPosition;
            initialPosition -= segmentLength * Vector3.forward;
            _segments.Add(segment);
        }
        
        _currentSegment = 6;
        nubi.transform.position = _segments[_currentSegment].transform.position - Vector3.forward * (segmentLength * segmentsBetweenNubi);
        
        _globalIndex = 6;
        _nubiGlobalIndex = _globalIndex + segmentsBetweenNubi;
    }
    
    public void GenerateForward()
    {
        var segment = _segmentPool.Get();
        segment.transform.position = _segments[_segments.Count - 1].transform.position - segmentLength * Vector3.forward;
        _segments.Add(segment);
        if (_segments.Count > segmentsBetweenNubi)
        {
            _segmentPool.Release(_segments[0]);
            _segments.RemoveAt(0);
        }
    }
    
    public void GenerateBackward()
    {
        var segment = _segmentPool.Get();
        segment.transform.position = _segments[0].transform.position + segmentLength * Vector3.forward;
        _segments.Insert(0, segment);
        if (_segments.Count > segmentsBetweenNubi)
        {
            _segmentPool.Release(_segments[_segments.Count - 1]);
            _segments.RemoveAt(_segments.Count - 1);
        }
    }
    
    private int DistanceToNubi()
    {
        return Mathf.Abs(Mathf.Abs(_globalIndex) - Mathf.Abs(_nubiGlobalIndex));
    }
}
