using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class ExecutionQueueManager : MonoBehaviour
{
    public List<QueueDictionary> taskQueues = new();

    public ExecutionQueue CreateTaskQueue(string name)
    {
        ExecutionQueue queue = taskQueues.Find(x => x.queueName == name).executionQueue;
        if (queue != null)
            return queue;

        ExecutionQueue taskQueue = new();
        taskQueues.Add(new(name, taskQueue));
        return taskQueue;
    }

    public ExecutionQueue GetTaskQueue(string name)
    {
        ExecutionQueue queue = taskQueues.Find(x => x.queueName == name).executionQueue;
        if (queue != null)
            return queue;
        Debug.LogError("TaskQueue don't Exist");
        return null;
    }

    public async Task LoadScrenOnTask(Func<Task, Task> func, Task[] tasks)
    {
        for (int i = 0; i < tasks.Length; i++)
        {
            await func(tasks[i]);
        }
    }

    public async Task LoadScrenOnTask(Func<Task, Task> func, Task task)
    {
        await func(task);
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

    public void Complete() => _queue.CompleteAdding();

    private async Task ProcessQueueAsync()
    {
        foreach (var value in _queue.GetConsumingEnumerable())
            await value();
    }

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
