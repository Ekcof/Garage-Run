using System.Threading;

public static class CancellationTokenSourceExtensions
{
    public static void CancelAndDispose(this CancellationTokenSource cts)
    {
        if (cts != null)
        {
            if (!cts.IsCancellationRequested && cts.Token.CanBeCanceled)
                cts.Cancel();
            cts.Dispose();
        }
    }
}
