# Repro

This simple project reproduces the problem that we are seeing within ensek as far as I understand it.

## Dependencies

This is a .net 4.8 framework web api.
It makes calls to aws and relies on a local installation of aws localstack (so it has somewhere to send the messages to)
The latest newrelic agent is also installed locally

## Repro steps
To reproduce the issue you need to run the project locally and hit the endpoint https://localhost:44337/api/default several times.
The forced garbage collection in the endpoint is not necessary but means that you see the problem more quickly.

This creates the following output in the newrelic agent log file (C:\ProgramData\New Relic\.NET Agent\Logs for me)
```
TransactionName{"IsWeb":true,"Category":"WebAPI","Name":"Default/HelloWorld","UnprefixedName":"WebAPI/Default/HelloWorld"} (priority 6, FrameworkHigh) from TransactionName{"IsWeb":true,"Category":"ASP","Name":"api/{controller}/{id}","UnprefixedName":"ASP/api/{controller}/{id}"} (priority 4, Route)
2024-10-16 11:16:30,017 NewRelic FINEST: [pid: 49484, tid: 9] Trx 54fee61f0abbfe47: Segment start {Id=7,ParentId=6,Name=DotNet/Default/HelloWorld,IsLeaf=False,Combinable=False,MethodCallData=System.Web.Http.Controllers.ApiControllerActionInvoker.InvokeActionAsync}
2024-10-16 11:16:30,017 NewRelic FINEST: [pid: 49484, tid: 9] Trx 54fee61f0abbfe47: Retrieved from NewRelic.Providers.Storage.HttpContext.HttpContextStorage`1[NewRelic.Agent.Core.Transactions.IInternalTransaction]
2024-10-16 11:16:30,017 NewRelic FINEST: [pid: 49484, tid: 9] Trx 54fee61f0abbfe47: Attempting to execute NewRelic.Agent.Core.Wrapper.AttachToAsyncWrapper found from InstrumentedMethodInfo: Method: System.Web.Http.Controllers.ApiControllerActionInvoker, System.Web.Http, Version=5.2.9.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35:InvokeActionAsyncCore(System.Web.Http.Controllers.HttpActionContext,System.Threading.CancellationToken), RequestedWrapperName: AttachToAsyncWrapper, IsAsync: True, RequestedMetricName: , RequestedTransactionNamePriority: , StartWebTransaction: False
2024-10-16 11:16:30,017 NewRelic FINEST: [pid: 49484, tid: 9] Trx 54fee61f0abbfe47: Attached to NewRelic.Providers.Storage.AsyncLocal.AsyncLocalStorage`1[NewRelic.Agent.Core.Transactions.IInternalTransaction]
2024-10-16 11:16:30,033 NewRelic FINEST: [pid: 49484, tid: 2] Transaction 4a181e47da8e59ac (WebTransaction/WebAPI/Default/HelloWorld) transform completed.
2024-10-16 11:16:30,033 NewRelic  DEBUG: [pid: 49484, tid: 2] Transaction was garbage collected without ever ending.
```

I have been told the the message "Transaction was garbage collected without ever ending." is the problem that we are seeing and is associated with it not logging the http status codes
(I have not check that this is actually the case but I have no reason to doubt it)

##Things to note

1) If the customer action filter is replace with a no-op (Task.Delay(1)) then the problem goes away
2) I have not check if any other form of IO causes the same problem, so it may or may not be something to do with the sending of the SNS event