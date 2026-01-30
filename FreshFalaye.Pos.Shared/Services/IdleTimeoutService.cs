using Microsoft.JSInterop;

namespace FreshFalaye.Pos.Shared.Services
{
    public class IdleTimeoutService : IDisposable
    {
        private readonly IJSRuntime _js;
        private DotNetObjectReference<IdleTimeoutService>? _objRef;
        private Timer? _timer;

        public bool IsLocked { get; private set; }

        // 🔧 CONFIG (minutes)
        private const int IDLE_MINUTES = 15;

        public event Action? OnLocked;

        public IdleTimeoutService(IJSRuntime js)
        {
            _js = js;
        }

        public async Task StartAsync()
        {
            _objRef = DotNetObjectReference.Create(this);
            await _js.InvokeVoidAsync("posIdle.start", _objRef);
            ResetTimer();
        }

        [JSInvokable]
        public void OnUserActivity()
        {
            if (IsLocked) return;
            ResetTimer();
        }

        private void ResetTimer()
        {
            _timer?.Dispose();

            _timer = new Timer(_ =>
            {
                IsLocked = true;
                OnLocked?.Invoke();
            }, null, TimeSpan.FromMinutes(IDLE_MINUTES), Timeout.InfiniteTimeSpan);
        }

        public void Unlock()
        {
            IsLocked = false;
            ResetTimer();
        }

        public void Dispose()
        {
            _timer?.Dispose();
            _objRef?.Dispose();
        }
    }
}
