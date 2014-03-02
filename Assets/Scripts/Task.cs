using UnityEngine;
using System.Collections;

public class Task
{
    public enum taskType {movement, research, culture, religion, citystate };
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
}
