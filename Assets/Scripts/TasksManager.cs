using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class TasksManager : MonoBehaviour
{
    #region SimpleQueue
    private Dictionary<string, List<Task>> tasksQueue = new();
    public Dictionary<string, List<Task>> TasksQueue => tasksQueue;

    private List<Task> defaultTasksQueue = new();

    private void Awake()
    {
        tasksQueue.Clear();
        tasksQueue.Add("default", defaultTasksQueue);
    }

    public List<Task> CreateTaskList(string name)
    {
        if (tasksQueue.ContainsKey(name))
            return tasksQueue[name];
        List<Task> list = new List<Task>();
        tasksQueue.Add(name, list);
        return list;
    }

    public List<Task> GetTaskList(string name)
    {
        if (tasksQueue.ContainsKey(name))
            return tasksQueue[name];

        Debug.LogError("Key not found : " + name);
        return null;
    }

    public List<Task> GetTaskList()
    {
        return tasksQueue["default"];
    }

    public async Task AddTaskToList(string name, Task task)
    {
        if (tasksQueue.ContainsKey(name))
        {
            tasksQueue[name].Add(task);
            await task;
            tasksQueue[name].Remove(task);
            return;
        }
        Debug.LogError("Key not found : " + name);
    }

    public async Task AddTaskToList(Task task)
    {
        tasksQueue["default"].Add(task);
        await task;
        tasksQueue["default"].Remove(task);
    }

    public bool AllTasksFinish(string name)
    {
        if (tasksQueue.ContainsKey(name))
            return tasksQueue[name].Count <= 0;

        Debug.LogError("Key not found : " + name);
        return false;
    }

    public bool AllTasksFinish() => tasksQueue["default"].Count <= 0;

    #endregion
    #region ComplexQueue

    public List<QueueDictionary> complexTasksQueue = new();

    public ExecutionQueue CreateComplexTaskQueue(string name)
    {
        ExecutionQueue queue = complexTasksQueue.Find(x => x.queueName == name).executionQueue;
        if (queue != null)
            return queue;

        ExecutionQueue taskQueue = new();
        complexTasksQueue.Add(new(name, taskQueue));
        return taskQueue;
    }

    public ExecutionQueue GetComplexTaskQueue(string name)
    {
        ExecutionQueue queue = complexTasksQueue.Find(x => x.queueName == name).executionQueue;
        if (queue != null)
            return queue;

        return CreateComplexTaskQueue(name);
    }
}

[System.Serializable]
public struct QueueDictionary
{
    public string queueName;
    public ExecutionQueue executionQueue;
    public QueueDictionary(string queueName, ExecutionQueue executionQueue)
    {
        this.queueName = queueName;
        this.executionQueue = executionQueue;
    }
}

public sealed class ExecutionQueue
{
    private readonly BlockingCollection<Func<Task>> _queue = new BlockingCollection<Func<Task>>();

    public ExecutionQueue() => Completion = Task.Run(() => ProcessQueueAsync());

    public Task Completion { get; }

    private int MaxTask = int.MaxValue;

    public void Complete()
    {
        _queue.CompleteAdding();
        MaxTask = _queue.Count;
    }

    private async Task ProcessQueueAsync()
    {
        foreach (var value in _queue.GetConsumingEnumerable())
            await value();
    }

    public float PercentComplete() => 1f - ((float)_queue.Count / MaxTask);

    public Task Run(Func<Task> lambda)
    {
        var tcs = new TaskCompletionSource<object>();
        _queue.Add(async () =>
        {
            // Execute the lambda and propagate the results to the Task returned from Run
            try
            {
                await lambda();
                tcs.TrySetResult(null);
            }
            catch (OperationCanceledException ex)
            {
                tcs.TrySetCanceled(ex.CancellationToken);
            }
            catch (Exception ex)
            {
                tcs.TrySetException(ex);
            }
        });
        return tcs.Task;
    }

    public Task Run(IEnumerator enumerator)
    {
        var tcs = new TaskCompletionSource<object>();
        _queue.Add(async () =>
        {
            // Execute the lambda and propagate the results to the Task returned from Run
            try
            {
                await Task.Run(() => enumerator);
                tcs.TrySetResult(null);
            }
            catch (OperationCanceledException ex)
            {
                tcs.TrySetCanceled(ex.CancellationToken);
            }
            catch (Exception ex)
            {
                tcs.TrySetException(ex);
            }
        });
        return tcs.Task;
    }
}
#endregion