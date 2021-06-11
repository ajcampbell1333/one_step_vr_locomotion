using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// When user's head moves beyond a threshold (activationRadius) away from an established center point of the play space,
/// locomote the user's avatar in that direction with velocity relative to that radius, i.e. user walks faster by leaning further from center.
/// To begin moving, user takes one step out of center (if threshold is properly calibrated).
/// To stop moving, user returns to center.
/// It is recommended to have a small circular mat to act as tactile feedback for returning to center.
/// </summary>
public class OneStepController : MonoBehaviour
{
    [SerializeField] public Transform trackingSpace;
    [SerializeField] public Transform headTransform;
    [SerializeField] public Transform activationRadiusVisualizer;
    [SerializeField] public float activationRadius;
    [SerializeField] public float maxRadius;
    [SerializeField] public float speed;
    [SerializeField] public bool showVisualizers;
    [SerializeField] public float visualizerArrowheadHeight;
    [SerializeField] public float visualizerArrowheadWidth;

    private LineRenderer _directionVisualizer;
    private Vector3 _headAtFloor;
    private Vector3 _head2Center;
    private float _absMagnitude;

    private void Awake()
    {
        _directionVisualizer = GetComponent<LineRenderer>();
    }

    private void Update()
    {
        _headAtFloor = new Vector3(headTransform.position.x, 0, headTransform.position.z);
        _head2Center = _headAtFloor - transform.position;
        _absMagnitude = Mathf.Abs(_head2Center.magnitude);
        if (_absMagnitude > activationRadius)
        {
            trackingSpace.position +=
                    ((_absMagnitude < maxRadius) ? _head2Center : _head2Center.normalized * maxRadius) * speed * Time.deltaTime;
            UpdateArrowPositions();
        }
        else if (_directionVisualizer.positionCount > 0)
            ResetArrow();

        // Enables user to recalibrate their desired "center of play space" on the fly
        if (OVRInput.GetDown(OVRInput.Button.One))
            Recalibrate();
    }

    /// <summary>
    /// This is only for visualizing the direction of motion during debugging via a line renderer.
    /// </summary>
    private void UpdateArrowPositions()
    {
        if (!showVisualizers && _directionVisualizer.positionCount > 0)
        {
            ResetArrow();
            return;
        }

        if (_absMagnitude > activationRadius && showVisualizers)
        {
            _directionVisualizer.SetPositions(new Vector3[] { 
                transform.position,
                transform.position + _head2Center,
                transform.position + _head2Center - _head2Center.normalized * visualizerArrowheadHeight + Vector3.Cross(Vector3.up,_head2Center).normalized*0.5f*visualizerArrowheadWidth,
                transform.position + _head2Center - _head2Center.normalized * visualizerArrowheadHeight + Vector3.Cross(_head2Center,Vector3.up).normalized*0.5f*visualizerArrowheadWidth,
                transform.position + _head2Center
            });
        }
    }

    /// <summary>
    /// Reset the visualizer to cause it to disappear.
    /// </summary>
    private void ResetArrow()
    {
        _directionVisualizer.SetPositions(new Vector3[] { Vector3.zero, Vector3.zero, Vector3.zero, Vector3.zero, Vector3.zero });
    }

    private void Recalibrate()
    {
        transform.position = _headAtFloor;
    }
}