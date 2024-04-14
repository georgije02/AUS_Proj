using Common;
using System;
using System.Collections.Generic;
using System.Threading;

namespace ProcessingModule
{
    /// <summary>
    /// Class containing logic for automated work.
    /// </summary>
    public class AutomationManager : IAutomationManager, IDisposable
	{

		private Thread automationWorker1;
		private Thread automationWorker2;
		private AutoResetEvent automationTrigger;
        private IStorage storage;
		private IProcessingManager processingManager;
		private int delayBetweenCommands;
        private IConfiguration configuration;

        /// <summary>
        /// Initializes a new instance of the <see cref="AutomationManager"/> class.
        /// </summary>
        /// <param name="storage">The storage.</param>
        /// <param name="processingManager">The processing manager.</param>
        /// <param name="automationTrigger">The automation trigger.</param>
        /// <param name="configuration">The configuration.</param>
        public AutomationManager(IStorage storage, IProcessingManager processingManager, AutoResetEvent automationTrigger, IConfiguration configuration)
		{
			this.storage = storage;
			this.processingManager = processingManager;
            this.configuration = configuration;
            this.automationTrigger = automationTrigger;
        }

        /// <summary>
        /// Initializes and starts the threads.
        /// </summary>
		private void InitializeAndStartThreads()
		{
			InitializeAutomationWorkerThread();
			StartAutomationWorkerThread();
		}

        /// <summary>
        /// Initializes the automation worker thread.
        /// </summary>
		private void InitializeAutomationWorkerThread()
		{
			automationWorker1 = new Thread(AutomationWorker_DoWork1);
			automationWorker1.Name = "Aumation Thread";

			automationWorker2 = new Thread(AutomationWorker_DoWork2);
			automationWorker2.Name = "Aumation Thread";
		}

        /// <summary>
        /// Starts the automation worker thread.
        /// </summary>
		private void StartAutomationWorkerThread()
		{
			automationWorker1.Start();
			automationWorker2.Start();
		}


		private void AutomationWorker_DoWork1()
		{
			PointIdentifier d1 = new PointIdentifier(PointType.DIGITAL_OUTPUT, 2400);
			
			List<PointIdentifier> list = new List<PointIdentifier> { d1 };
			while (!disposedValue)
			{
				List<IPoint> points = storage.GetPoints(list);

				Thread.Sleep(2000);
				processingManager.ExecuteReadCommand(points[0].ConfigItem, configuration.GetTransactionId(), configuration.UnitAddress, points[0].ConfigItem.StartAddress, 1);
				
			}
		}

		private void AutomationWorker_DoWork2()
		{
			PointIdentifier m1 = new PointIdentifier(PointType.ANALOG_OUTPUT, 2200);
			PointIdentifier m2 = new PointIdentifier(PointType.ANALOG_OUTPUT, 2201);
			PointIdentifier m3 = new PointIdentifier(PointType.ANALOG_OUTPUT, 2202);
			PointIdentifier m4 = new PointIdentifier(PointType.ANALOG_OUTPUT, 2202);
			PointIdentifier b1 = new PointIdentifier(PointType.ANALOG_INPUT, 3400);
			PointIdentifier s1 = new PointIdentifier(PointType.ANALOG_INPUT, 3401);
			PointIdentifier s2 = new PointIdentifier(PointType.ANALOG_INPUT, 3402);
			List<PointIdentifier> list = new List<PointIdentifier> { m1, m2, m3, m4, b1, s1, s2 };
			while (!disposedValue)
			{
				List<IPoint> points = storage.GetPoints(list);

				Thread.Sleep(4000);
				processingManager.ExecuteReadCommand(points[0].ConfigItem, configuration.GetTransactionId(), configuration.UnitAddress, points[0].ConfigItem.StartAddress, 1);
				processingManager.ExecuteReadCommand(points[1].ConfigItem, configuration.GetTransactionId(), configuration.UnitAddress, points[1].ConfigItem.StartAddress, 1);
				processingManager.ExecuteReadCommand(points[2].ConfigItem, configuration.GetTransactionId(), configuration.UnitAddress, points[2].ConfigItem.StartAddress, 1);
				processingManager.ExecuteReadCommand(points[3].ConfigItem, configuration.GetTransactionId(), configuration.UnitAddress, points[3].ConfigItem.StartAddress, 1);
				processingManager.ExecuteReadCommand(points[4].ConfigItem, configuration.GetTransactionId(), configuration.UnitAddress, points[4].ConfigItem.StartAddress, 1);
				processingManager.ExecuteReadCommand(points[5].ConfigItem, configuration.GetTransactionId(), configuration.UnitAddress, points[5].ConfigItem.StartAddress, 1);
				processingManager.ExecuteReadCommand(points[6].ConfigItem, configuration.GetTransactionId(), configuration.UnitAddress, points[6].ConfigItem.StartAddress, 1);
			}
		}

		#region IDisposable Support
		private bool disposedValue = false; // To detect redundant calls


        /// <summary>
        /// Disposes the object.
        /// </summary>
        /// <param name="disposing">Indication if managed objects should be disposed.</param>
		protected virtual void Dispose(bool disposing)
		{
			if (!disposedValue)
			{
				if (disposing)
				{
				}
				disposedValue = true;
			}
		}


		// This code added to correctly implement the disposable pattern.
		public void Dispose()
		{
			// Do not change this code. Put cleanup code in Dispose(bool disposing) above.
			Dispose(true);
			// GC.SuppressFinalize(this);
		}

        /// <inheritdoc />
        public void Start(int delayBetweenCommands)
		{
			this.delayBetweenCommands = delayBetweenCommands*1000;
            InitializeAndStartThreads();
		}

        /// <inheritdoc />
        public void Stop()
		{
			Dispose();
		}
		#endregion
	}
}
