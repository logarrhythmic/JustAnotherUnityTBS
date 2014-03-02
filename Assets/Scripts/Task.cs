using UnityEngine;
using System.Collections;

public class Task
{
    public enum taskType {movement, research, culture, religion, citystate, building };
    public taskType type;
    public Unit unitTarget;
    public City cityTarget;
    public CityState citystateTarget;
    public Texture2D image;

    public Task(taskType tasktype, Unit target, Texture2D unitImage)
    {
        unitTarget = target;
        type = tasktype;
        image = unitImage;
    }

    public Task(taskType tasktype, City target)
    {
        cityTarget = target;
        type = tasktype;
        image = Resources.Load("Images/building") as Texture2D;
    }
}
