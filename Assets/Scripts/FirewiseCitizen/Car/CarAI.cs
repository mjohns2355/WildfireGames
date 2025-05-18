using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class CarAI : MonoBehaviour
{
    [SerializeField]
    private List<Vector3> path = null;
    [SerializeField]
    private List<Vector3> stops = null;
    [SerializeField]
    private float arriveDistance = .3f, lastPointArriveDistance = .1f;
    [SerializeField]
    private float turningAngleOffset = 5;
    [SerializeField]
    private Vector3 currentTargetPosition;

    [SerializeField]
    private GameObject raycastStartingPoint = null;
    [SerializeField]
    private float collisionRaycastLength = 0.5f;

    private float jamTimer = 0;

    private bool sawFire = false;

    private float drivingTimer = 0;

    float changeDirChance = 1f;


    internal bool IsThisLastPathIndex()
    {
        return index >= path.Count-1;
    }

    private int index = 0;
    private int stopIndex = 0;

    private bool stop;
    public bool collisionStop = false;
    private bool noStops = false;
    Combustible combustible;
    public bool Stop
    {
        get { return stop || collisionStop; }
        set { stop = value; }
    }

    [field: SerializeField]
    public UnityEvent<Vector2> OnDrive { get; set; }

    private void Start()
    {
        if (path == null || path.Count == 0)
        {
            Stop = true;
        }
        else
        {
            currentTargetPosition = path[index];
        }

        combustible = GetComponentInChildren<Combustible>();
        if (combustible)
        {
            combustible.OnIgnite.AddListener(() =>
            {
                if (Stop) return;
                //Debug.Log("Car on fire");
                Stop = true;
            });

            combustible.OnStopIgniting.AddListener(() =>
            {
                if (!Stop) return;
                Stop = false;
            });
        }
    }

    public void SetPath(List<Vector3> path)
    {
        if(path.Count == 0)
        {
            Destroy(gameObject);
            return;
        }
        this.path = path;
        index = 0;
        currentTargetPosition = this.path[index];

        Vector3 relativepoint = transform.InverseTransformPoint(this.path[index + 1]);

        float angle = Mathf.Atan2(relativepoint.x, relativepoint.z) * Mathf.Rad2Deg;

        transform.rotation = Quaternion.Euler(0, angle, 0);
        Stop = false;
    }

    public void SetStops(List<Vector3> stops)
    {
        // remove the final destination from stops
        for(int i = 0; i < stops.Count -1; i++)
        {
            this.stops.Add(stops[i]);
        }

        sawFire = true;
    }
    private void Update()
    {
        CheckIfArrived();
        CheckIfNearToStop();
        Drive();
        CheckForCollisions();
        if (collisionStop)
        {
            jamTimer += Time.deltaTime;
            if(jamTimer > 3)
            {
                var car = GetComponent<CarController>();
                ATC_AIDirector.Instance.RespawnACar(car.start, car.ends, car.carSpeed);
                Destroy(gameObject);
            }
        }

        if (Input.GetKeyDown(KeyCode.K))
        {
            TestRespawnCar();
        }
    }

    public void TestRespawnCar()
    {
        var car = GetComponent<CarController>();
        ATC_AIDirector.Instance.RespawnACar(car.start, car.ends, car.carSpeed);
        Destroy(gameObject);
    }
    private void CheckForCollisions()
    {
        if (Physics.Raycast(raycastStartingPoint.transform.position, transform.forward, collisionRaycastLength, 1<<gameObject.layer))
        {
            collisionStop = true;
        }
        else
        {
            collisionStop = false;
            jamTimer = 0;
        }
        //Debug.DrawRay(raycastStartingPoint.transform.position, transform.forward* collisionRaycastLength, Color.red);

    }

    private void Drive()
    {
        if (Stop)
        {
            OnDrive?.Invoke(Vector2.zero);
        }
        else
        {
            drivingTimer += Time.deltaTime;
            Vector3 relativepoint = transform.InverseTransformPoint(currentTargetPosition);
            float angle = Mathf.Atan2(relativepoint.x, relativepoint.z) * Mathf.Rad2Deg;
            var rotateCar = 0;
            if(angle > turningAngleOffset)
            {
                rotateCar = 1;
            }else if(angle < -turningAngleOffset)
            {
                rotateCar = -1;
            }
            OnDrive?.Invoke(new Vector2(rotateCar, 1));
        }
    }

    private void CheckIfArrived()
    {
        if(Stop == false)
        {
            var distanceToCheck = arriveDistance;
            if(index == path.Count - 1)
            {
                distanceToCheck = lastPointArriveDistance;
            }
            if(Vector3.Distance(currentTargetPosition,transform.position) < distanceToCheck)
            {
                SetNextTargetIndex();
            }
            // Avoid changing direction when car is close to destination
            if (Vector3.Distance(path[^1],transform.position) < 3f)
            {
                //Debug.Log("Close to destination");
                changeDirChance = 0;
            }
        }
    }

    private void CheckIfNearToStop()
    {
        if (stops.Count == 0 || noStops) return;
        if (Stop == false)
        {
            var distanceToCheck = arriveDistance;

            if (Vector3.Distance(stops[stopIndex], transform.position) < distanceToCheck)
            {

                StartCoroutine(CarReachStopRoutine());
            }
        }
    }

    IEnumerator CarReachStopRoutine()
    {
        //Debug.Log("Close to stop");
        Stop = true;
        stopIndex++;

        if(stopIndex >= stops.Count)
        {
            noStops = true;
        }
        yield return new WaitForSeconds(3f);
        Stop = false;
    }
    private void SetNextTargetIndex()
    {
        var carsEvacuated = GameManager.Instance.carsEvacuated;
        index++;
        if(index >= path.Count)
        {
            Stop = true;
            if(carsEvacuated  == 0)
            {
                GameManager.Instance.firstEvacCarTimeStamp = GameManager.Instance.SimTimer;
            }

            GameManager.Instance.carsEvacuated++;
            ATC_AIDirector.Instance.currentCarNum--;
            if(ATC_AIDirector.Instance.currentCarNum == 0)
            {
                GameManager.Instance.lastEvacCarTimeStamp = GameManager.Instance.SimTimer;
            }
            Destroy(gameObject);
        }
        else
        {
            currentTargetPosition = path[index];
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Fire") && other.GetComponent<FireMovementController>().onCombustible && !sawFire && drivingTimer > 10f)
        {
            //Debug.Log("See fire");
            if (changeDirChance <= 0.25f) return;
            sawFire = true;
            HandleFireDetection();
        }

    }

    private void HandleFireDetection()
    {
        //changeDirChance -= 0.25f;
        if(changeDirChance >= 1f)
        {
            changeDirChance -= 0.1f;
        }
        else
        {
            changeDirChance -= 0.25f;
        }
        
        if (UnityEngine.Random.Range(0, 1f) > changeDirChance) return;
        
        //Debug.Log("Change Direction");
        // Get nearest house or road and respawn car
        var pos = Vector3Int.RoundToInt(transform.position);
        var roadPos = ATC_AIDirector.Instance.placementManager.GetNearestRoad(pos, 1, 1).Value;

        var newStart = ATC_AIDirector.Instance.placementManager.GetStructureAt(roadPos);
        var car = GetComponent<CarController>();

        var newPath = ATC_AIDirector.Instance.FindPath(newStart, car.ends[0]);
        if (newPath == null) return;
        car.GetComponent<CarController>().start = newStart;

        car.GetComponent<CarAI>().SetPath(newPath);
        drivingTimer = 0f;
        sawFire = false;
        //Debug.Log($"New Path: {newPath.Count}");
        //ATC_AIDirector.Instance.RespawnACar(newStart, car.ends, car.carSpeed);
        //Destroy(gameObject);
    }
}
