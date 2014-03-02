using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Unit : MonoBehaviour
{
    public int maxmoves, movesleft;
    public int hp, range, damage;
    public int price;
    public bool skip, sleep, fortified;
    public Texture2D icon;
    public List<Action> actions = new List<Action>();

    public void Start()
    {
        actions.Add(new Action(Resources.Load("Images/movement") as Texture2D, Action.actionType.move));
        actions.Add(new Action(Resources.Load("Images/settling") as Texture2D, Action.actionType.settle));
    }

    public void newTurn()
    {
        movesleft = maxmoves;
    }

    public void Settle(Manager manager)
    {
        GameObject go = PhotonNetwork.Instantiate("City", transform.position, Quaternion.identity, 0) as GameObject;
        manager.cities.Add(go.GetComponent<City>());
        manager.units.Remove(this);
        foreach (Task task in manager.tasks)
        {
            if (task.type == Task.taskType.movement)
            {
                if (task.unitTarget == this)
                {
                    manager.tasks.Remove(task);
                    break;
                }
            }
        }
        PhotonNetwork.Destroy(gameObject);
    }

    public void Move()
    {

    }
}
