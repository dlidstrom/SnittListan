﻿#nullable enable

using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Reflection;
using System.Web.Http;
using Newtonsoft.Json;
using NLog;
using Snittlistan.Queue.Messages;
using Snittlistan.Web.Areas.V2.Tasks;
using Snittlistan.Web.Controllers;
using Snittlistan.Web.Infrastructure;
using Snittlistan.Web.Infrastructure.Attributes;
using Snittlistan.Web.Infrastructure.Database;

namespace Snittlistan.Web.Areas.V2.Controllers.Api;

[OnlyLocalAllowed]
public class TaskController : AbstractApiController
{
    private static readonly Logger Log = LogManager.GetCurrentClassLogger();
    private readonly JsonSerializerSettings settings = new()
    {
        TypeNameHandling = TypeNameHandling.All,
        MetadataPropertyHandling = MetadataPropertyHandling.ReadAhead
    };

    public async Task<IHttpActionResult> Post(TaskRequest request)
    {
        Log.Info($"Received task {request.TaskJson}");

        TaskBase? taskObject = JsonConvert.DeserializeObject<TaskBase>(
            request.TaskJson,
            settings);
        if (taskObject is null)
        {
            return BadRequest("could not deserialize task json");
        }

        // check for published task
        PublishedTask? publishedTask = await CompositionRoot.Databases.Snittlistan.PublishedTasks.SingleOrDefaultAsync(x => x.MessageId == request.MessageId);
        if (publishedTask is null)
        {
            return BadRequest($"No published task found with message id {request.MessageId}");
        }

        if (publishedTask.HandledDate.HasValue)
        {
            return Ok($"task with message id {publishedTask.MessageId} already handled");
        }

        Type handlerType = typeof(ITaskHandler<>).MakeGenericType(taskObject.GetType());

        MethodInfo handleMethod = handlerType.GetMethod("Handle");
        using IDisposable scope = NestedDiagnosticsLogicalContext.Push(taskObject.BusinessKey);
        Log.Info("Begin");
        Tenant tenant = await CompositionRoot.GetCurrentTenant();
        Guid correlationId = request.CorrelationId ?? default;
        Guid causationId = request.MessageId ?? default;
        TaskPublisher taskPublisher = new(tenant, CompositionRoot.Databases, correlationId, causationId);
        IPublishContext publishContext = (IPublishContext)Activator.CreateInstance(
            typeof(MessageContext<>).MakeGenericType(taskObject.GetType()),
            CompositionRoot,
            taskObject,
            tenant,
            correlationId,
            causationId);
        publishContext.PublishMessage = task =>
            taskPublisher.PublishTask(task, "system");

        object handler = CompositionRoot.Kernel.Resolve(handlerType);
        Task task = (Task)handleMethod.Invoke(handler, new[] { publishContext });
        await task;
        Log.Info("End");
        publishedTask.MarkHandled(DateTime.Now);

        return Ok();
    }
}

public class TaskRequest
{
    public TaskRequest(string taskJson, Guid? correlationId, Guid? messageId)
    {
        TaskJson = taskJson;
        CorrelationId = correlationId;
        MessageId = messageId;
    }

    [Required]
    public string TaskJson { get; }

    [Required]
    public Guid? CorrelationId { get; }

    [Required]
    public Guid? MessageId { get; }
}
