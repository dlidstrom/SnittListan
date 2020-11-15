module Infrastructure

open System.Diagnostics.Tracing

// https://developers.redhat.com/blog/2019/12/23/tracing-net-core-applications/
type LoggingEventListener() =
    inherit EventListener() with
        override this.OnEventSourceCreated(eventSource) =
            if eventSource.Name <> "System.Threading.Tasks.TplEventSource"
                && eventSource.Name <> "System.Buffers.ArrayPoolEventSource" then
                this.EnableEvents(eventSource, EventLevel.LogAlways, EventKeywords.All)
        override _.OnEventWritten(eventData) =
            printfn
                "%s - %s : %s"
                eventData.EventSource.Name
                eventData.EventName
                (eventData.Payload
                    |> Seq.zip eventData.PayloadNames
                    |> Seq.map string
                    |> String.concat ",")
