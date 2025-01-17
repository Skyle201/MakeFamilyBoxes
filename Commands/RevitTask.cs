using Autodesk.Revit.UI;

namespace MakeFamilyBoxes.Commands;

public class RevitTask : IDisposable
{
    private readonly EventHandler _handler;
    private TaskCompletionSource<object> _tcs;
    private readonly ExternalEvent _externalEvent;

    public RevitTask()
    {
        _handler = new EventHandler();
        _handler.EventCompleted += OnEventCompleted;
        _externalEvent = ExternalEvent.Create(_handler);
    }

    public void Dispose()
    {
        _externalEvent.Dispose();
    }

    public Task Run(Action<UIApplication> act)
    {
        _tcs = new TaskCompletionSource<object>();
        _handler.Func = (app) =>
        {
            act(app);
            return new object();
        };
        _externalEvent.Raise();
        return _tcs.Task;
    }

    public Task<TResult> Run<TResult>(Func<UIApplication, TResult> func)
    {
        _tcs = new TaskCompletionSource<object>();
        var task = Task.Run(async () => (TResult)await _tcs.Task);
        _handler.Func = (app) => func(app);
        _externalEvent?.Raise();
        return task;
    }

    private void OnEventCompleted(object sender, object? e)
    {
        if (_handler.Exception is null && e is not null)
        {
            _tcs?.TrySetResult(e);
        }
        else
        {
            _tcs?.TrySetException(_handler.Exception);
        }
    }

    private class EventHandler : IExternalEventHandler
    {
        private Func<UIApplication, object?>? _func;
        public event EventHandler<object?>? EventCompleted;
        public Exception? Exception { get; private set; }
        public Func<UIApplication, object?>? Func
        {
            get => _func;
            set => _func = value ?? throw new ArgumentNullException();
        }

        public void Execute(UIApplication app)
        {
            object? result = null;
            Exception = null;
            try
            {
                result = Func?.Invoke(app);
            }
            catch (Exception ex)
            {
                Exception = ex;
            }
            EventCompleted?.Invoke(this, result);
        }

        public string GetName() => "RevitTask";
    }
}
