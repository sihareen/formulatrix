using System;
using System.Collections.Concurrent;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace Formulatrix.Intern.GrabTheFrame
{
    // Interfaces from original problem
    public interface IFrameCallback
    {
        void FrameReceived(IntPtr pFrame, int pixelWidth, int pixelHeight);
    }

    public interface IValueReporter
    {
        void Report(double value);
    }

    // FrameAverageStreamer - Queue-based implementation
    public class FrameAverageStreamer : IFrameCallback, IDisposable
    {
        private readonly IValueReporter _reporter;
        private readonly BlockingCollection<byte[]> _frameQueue;
        private readonly CancellationTokenSource _cts;
        private readonly Task _processingTask;
        private bool _disposed;

        public FrameAverageStreamer(FrameGrabber frameGrabber, IValueReporter reporter, int maxQueueSize = 5)
        {
            _reporter = reporter ?? throw new ArgumentNullException(nameof(reporter));
            _frameQueue = new BlockingCollection<byte[]>(maxQueueSize);
            _cts = new CancellationTokenSource();

            frameGrabber.OnFrameUpdated += HandleFrameReceived;

            // Start processing on dedicated thread
            _processingTask = Task.Run(() => ProcessFramesAsync(_cts.Token));
        }

        public void FrameReceived(IntPtr pFrame, int pixelWidth, int pixelHeight)
        {
            if (_disposed) return;

            // Copy frame data immediately to break dependency on native buffer reuse
            int frameSize = pixelWidth * pixelHeight;
            byte[] frameCopy = new byte[frameSize];
            Marshal.Copy(pFrame, frameCopy, 0, frameSize);

            // Drop frame if queue is full (backpressure)
            _frameQueue.TryAdd(frameCopy);
        }

        private void ProcessFramesAsync(CancellationToken ct)
        {
            try
            {
                foreach (byte[] frameData in _frameQueue.GetConsumingEnumerable(ct))
                {
                    double average = CalculateAverage(frameData);
                    _reporter.Report(average);
                }
            }
            catch (OperationCanceledException) { }
        }

        private double CalculateAverage(byte[] frameData)
        {
            if (frameData == null || frameData.Length == 0)
                return 0.0;

            // Use long accumulator to prevent overflow, double for precision
            long sum = 0;
            for (int i = 0; i < frameData.Length; i++)
                sum += frameData[i];

            return (double)sum / frameData.Length;
        }

        public void StartStreaming() { }

        public void Dispose()
        {
            if (_disposed) return;

            _cts.Cancel();
            _frameQueue.CompleteAdding();

            try { _processingTask.Wait(); }
            catch (AggregateException) { }

            _cts.Dispose();
            _frameQueue.Dispose();
            _disposed = true;
        }

        private void HandleFrameReceived(IntPtr pFrame, int width, int height)
        {
            FrameReceived(pFrame, width, height);
        }
    }

    // Supporting stub class for testing
    public class FrameGrabber
    {
        public delegate void FrameUpdateHandler(IntPtr pFrame, int width, int height);
        public event FrameUpdateHandler OnFrameUpdated;

        public void SimulateFrame(byte[] data, int width, int height)
        {
            IntPtr ptr = Marshal.AllocHGlobal(data.Length);
            try
            {
                Marshal.Copy(data, 0, ptr, data.Length);
                OnFrameUpdated?.Invoke(ptr, width, height);
            }
            finally
            {
                Marshal.FreeHGlobal(ptr);
            }
        }
    }

    // FrameAverageStreamerLatest - Single-frame "latest" implementation
    public class FrameAverageStreamerLatest : IFrameCallback, IDisposable
    {
        private readonly IValueReporter _reporter;
        private readonly object _lock = new object();
        private readonly CancellationTokenSource _cts;
        private readonly Task _processingTask;

        private byte[] _latestFrame;
        private bool _hasNewFrame;
        private bool _disposed;

        public FrameAverageStreamerLatest(FrameGrabber frameGrabber, IValueReporter reporter)
        {
            _reporter = reporter ?? throw new ArgumentNullException(nameof(reporter));
            _cts = new CancellationTokenSource();

            frameGrabber.OnFrameUpdated += HandleFrameReceived;

            _processingTask = Task.Run(() => ProcessLoop(_cts.Token));
        }

        public void FrameReceived(IntPtr pFrame, int pixelWidth, int pixelHeight)
        {
            if (_disposed) return;

            int frameSize = pixelWidth * pixelHeight;
            byte[] frameCopy = new byte[frameSize];
            Marshal.Copy(pFrame, frameCopy, 0, frameSize);

            lock (_lock)
            {
                _latestFrame = frameCopy;
                _hasNewFrame = true;
                Monitor.Pulse(_lock);
            }
        }

        private void ProcessLoop(CancellationToken ct)
        {
            while (!ct.IsCancellationRequested)
            {
                byte[] frameToProcess = null;

                lock (_lock)
                {
                    while (!_hasNewFrame && !ct.IsCancellationRequested)
                        Monitor.Wait(_lock, 100);

                    if (_hasNewFrame)
                    {
                        frameToProcess = _latestFrame;
                        _hasNewFrame = false;
                    }
                }

                if (frameToProcess != null)
                {
                    double average = CalculateAverage(frameToProcess);
                    _reporter.Report(average);
                }
            }
        }

        private double CalculateAverage(byte[] frameData)
        {
            if (frameData == null || frameData.Length == 0)
                return 0.0;

            long sum = 0;
            for (int i = 0; i < frameData.Length; i++)
                sum += frameData[i];

            return (double)sum / frameData.Length;
        }

        public void Dispose()
        {
            if (_disposed) return;

            _cts.Cancel();
            try { _processingTask.Wait(); }
            catch (AggregateException) { }
            _cts.Dispose();
            _disposed = true;
        }

        private void HandleFrameReceived(IntPtr pFrame, int width, int height)
        {
            FrameReceived(pFrame, width, height);
        }
    }
}
